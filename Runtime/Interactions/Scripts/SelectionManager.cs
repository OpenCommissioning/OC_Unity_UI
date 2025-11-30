using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OC.MaterialFlow;
using OC.UI.Toolbar;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OC.UI.Interactions
{
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(Rigidbody))]
    [DefaultExecutionOrder(-1000)]
    public class SelectionManager : MonoBehaviourSingleton<SelectionManager>
    {
        public IEnumerable<Interaction> SelectedInteractions => _selectedInteractions;
        public List<GameObject> HitGameObjects => _hitGameobjects;
        public Ray Pointer => _pointer;
        
        public event Action<List<Interaction>> OnSelectionChanged;

        public bool Enable
        {
            get => _enable;
            set
            {
                if (_enable == value) return;
                _enable = value;
                if (!_enable)
                {
                    ResetHit();
                    ClearSelection();
                }
            }
        }

        [Header("State")] 
        [SerializeField] 
        private bool _enable;

        [Header("Settings")]
        [SerializeField]
        private KeyCode _multiSelectKey = KeyCode.LeftControl;
        [SerializeField]
        private LayerMask _layermask;

        [SerializeField]
        private bool _drawDebugGizmos;

        [Header("Debug")] 
        [SerializeField] 
        private List<GameObject> _hitGameobjects = new ();
        [SerializeField]
        private List<Interaction> _selectedInteractions;

        private const float MAX_DISTANCE = 500;
        private const float DRAG_TRESHOLD = 0.3f;

        private Interaction _hoveredInteraction;
        private Interaction _lastHoveredInteraction;
        private readonly RaycastHit[] _hits = new RaycastHit[10];
        private int _hitsCount;
        private Vector3 _startMousePos;
        private Vector3 _endMousePos;
        private Ray _boxSelectionRay;
        private Ray _pointer;
        private Mesh _currentSelectionMesh;
        [SerializeField]
        private MeshCollider _selectionCollider;
        private bool _mouseDragging;
        [SerializeField]
        public bool _isDrawing;
        private bool _mouseClickStartedInEmptySpace;
        private BoxDrawer _boxDrawer;

        private Camera _camera;

        private GameObject _closestHitGameobject;
        private bool _hitHandle;
        

        private void Start()
        {
            _camera = Camera.main;
            
            GameObject boxDrawerGameobject = new("BoxDrawer")
            {
                transform =
                {
                    parent = gameObject.transform
                }
            };
            _boxDrawer = boxDrawerGameobject.AddComponent<BoxDrawer>();
            transform.position = Vector3.zero;
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Rigidbody>().useGravity = false;
            _selectionCollider = GetComponent<MeshCollider>();
            _selectionCollider.convex = true;
            _selectionCollider.isTrigger = true;
            
            UserInteractionManager.Instance.OnInteractionEnableChanged += SetEnable;

            SetEnable(false);
        }
        
        private void OnDestroy()
        {
            UserInteractionManager.Instance.OnInteractionEnableChanged -= SetEnable;
        }
        
        private void SetEnable(bool value)
        {
            _enable = value;
            if (!_enable)
            {
                ResetHit();
                ClearSelection();
            }
        }
        
        public void Raycast()
        {
            if (!_enable) return;
            
            Array.Clear(_hits, 0, _hitsCount);
            _hitGameobjects.Clear();
            _hitHandle = false;

            if (Input.mousePosition.sqrMagnitude > 1e16) return;
            
            _pointer = _camera.ScreenPointToRay(Input.mousePosition);
            _hitsCount = Physics.RaycastNonAlloc(_pointer.origin, _pointer.direction, _hits, MAX_DISTANCE, _layermask);

            if (_hitsCount == 0)
            {
                ResetHit();
                return;
            }

            var hits = _hits.OrderBy(hit => hit.distance);
            foreach (var raycast in hits)
            {
                if (raycast.distance < OC.Utils.TOLERANCE) continue;
                if (raycast.collider.gameObject.CompareTag($"Handles"))
                {
                    _hitHandle = true;
                    continue;
                }
                _hitGameobjects.Add(raycast.collider.gameObject);
            }

            if (_hitGameobjects.Count < 1)
            {
                ResetHit();
                return; 
            }
            
            if (_hitGameobjects.First() == _closestHitGameobject) return;
            if (_closestHitGameobject != null) PointerExitEvent(_closestHitGameobject);

            _closestHitGameobject = _hitGameobjects.First();
            PointerEnterEvent(_closestHitGameobject);
        }

        public void ResetHit()
        {
            PointerUpEvent(_closestHitGameobject);
            PointerExitEvent(_closestHitGameobject);
            _hitGameobjects.Clear();
            _closestHitGameobject = null;
        }

        public void ProcessPointerEvents(int pointerId)
        {
            if (!_enable) return;
            
            if (Input.GetMouseButtonDown(pointerId))
            {
                if (_hitHandle) return;

                if (_hitGameobjects.Count > 0)
                {
                    if (_selectedInteractions.Count > 0)
                    {
                        var index = (_hitGameobjects.IndexOf(_selectedInteractions.First().gameObject) + 1) % _hitGameobjects.Count;
                        var nextHitGameobject = _hitGameobjects[index];
                        Select(nextHitGameobject, Input.GetKey(_multiSelectKey));
                        PointerDownEvent(nextHitGameobject);
                    }
                    else
                    {
                        Select(_closestHitGameobject, Input.GetKey(_multiSelectKey));
                        PointerDownEvent(_closestHitGameobject);
                    }
                }
                else
                {
                    ResetHit();
                    ClearSelection();
                }
            }

            if (Input.GetMouseButtonUp(pointerId))
            {
                if (_hitGameobjects.Count > 0)
                {
                    PointerClickEvent(_closestHitGameobject);
                    PointerUpEvent(_closestHitGameobject);
                }
            }
        }

        private void BoxSelection()
        {
            if (_mouseClickStartedInEmptySpace)
            {
                if (_isDrawing) _boxDrawer.DrawBox();

                _endMousePos = Input.mousePosition;
                if (Vector3.Magnitude(_startMousePos - _endMousePos) > DRAG_TRESHOLD)
                {
                    _mouseDragging = true;
                    if (!_isDrawing)
                    {
                        _boxDrawer.StartDrawing();
                        _isDrawing = true;
                    }
                }
            }
            if (Input.GetMouseButtonUp(0) && _mouseDragging && _mouseClickStartedInEmptySpace)
            {
                DetectObjectsInBox();
                _mouseDragging = false;
                _boxDrawer.StopDrawing();
                _isDrawing = false;
                _mouseClickStartedInEmptySpace = false;
            }
        }

        public void Deselect(Interaction interaction) => RemoveSelection(interaction);

        private void Select(GameObject go, bool multiple)
        {
            if (!go.TryGetComponent<Interaction>(out var interaction)) return;
            if (interaction.IsDisabled) return;
            if (!interaction.Mode.HasFlag(Interaction.InteractionMode.Selection)) return;

            if (multiple)
            {
                TrySelection(interaction);
            }
            else
            {
                ClearSelection();
                TrySelection(interaction);
            }
        }
        
        private void TrySelection(Interaction interaction)
        {
            if (_selectedInteractions.Contains(interaction))
            {
                RemoveSelection(interaction);
            }
            else
            {
                AddSelection(interaction);
            }
        }

        private void AddSelection(Interaction interaction)
        {
            if (_selectedInteractions.Contains(interaction)) return;
            _selectedInteractions.Add(interaction);
            OnSelect(interaction.gameObject);
            OnSelectionChanged?.Invoke(_selectedInteractions);
        }
        
        private void RemoveSelection(Interaction interaction)
        {
            if (!_selectedInteractions.Contains(interaction)) return;
            _selectedInteractions.Remove(interaction);
            OnDeselect(interaction.gameObject);
            OnSelectionChanged?.Invoke(_selectedInteractions);
        }
        
        private void ClearSelection()
        {
            foreach (var selection in _selectedInteractions)
            {
                OnDeselect(selection.gameObject);
            }
            _selectedInteractions.Clear();
            OnSelectionChanged?.Invoke(_selectedInteractions);
        }

        private void DetectObjectsInBox()
        {
            _selectionCollider.enabled = true;
            var vertices = new Vector3[8];
            var corners = CreateCorners(_startMousePos, _endMousePos);
            var index = 0;

            if (Camera.main == null)
            {
                Logging.Logger.Log(LogType.Error, "Camera Main isn't defined");
                return;
            }

            for (var i = 0; i < corners.Length; ++i)
            {
                _boxSelectionRay = Camera.main.ScreenPointToRay(corners[i]);

                var endPoint = _boxSelectionRay.GetPoint(MAX_DISTANCE);
                vertices[index] = endPoint;
                vertices[index + 4] = Camera.main.ScreenToWorldPoint(corners[i]) - endPoint;
                if (_drawDebugGizmos) Debug.DrawLine(Camera.main.ScreenToWorldPoint(corners[i]), endPoint, Color.yellow, 10f);

                index++;
            }

            _currentSelectionMesh = CreateSelectionMesh(vertices);
            _selectionCollider.sharedMesh = _currentSelectionMesh;

            StartCoroutine(ProcessTriggerHits());
            _startMousePos = Vector3.zero;


        }
        
        private static readonly int[] CubeTriangles = {
                0, 1, 2,
                2, 1, 3,
                4, 6, 0,
                0, 6, 2,
                6, 7, 2,
                2, 7, 3,
                7, 5, 3,
                3, 5, 1,
                5, 0, 1,
                1, 4, 0,
                4, 5, 6,
                6, 5, 7
            };

        private Mesh CreateSelectionMesh(Vector3[] verts)
        {
            var meshVerts = new Vector3[8];

            for (var i = 0; i < 4; ++i)
            {
                meshVerts[i] = verts[i];
                meshVerts[i + 4] = verts[i] + verts[i + 4];
            }

            var mesh = new Mesh
            {
                name = "SelectionMesh",
                vertices = meshVerts,
                triangles = CubeTriangles
            };

            return mesh;
        }
        private Vector2[] CreateCorners(Vector2 p1, Vector2 p2)
        {
            var bottomLeft = Vector3.Min(p1, p2);
            var topRight = Vector3.Max(p1, p2);

            var corners = new[]
            {
                new Vector2(bottomLeft.x, topRight.y),
                new Vector2(topRight.x, topRight.y),
                new Vector2(bottomLeft.x, bottomLeft.y),
                new Vector2(topRight.x, bottomLeft.y)
            };

            var width = (corners[0] - corners[1]).magnitude;
            var height = (corners[0] - corners[2]).magnitude;

            if (width < 2)
            {
                var diff = 2 - width;
                corners[0].x -= diff * 0.5f;
                corners[1].x += diff * 0.5f;
                corners[2].x -= diff * 0.5f;
                corners[3].x += diff * 0.5f;
            }

            if (height < 2)
            {
                var diff = 2 - height;
                corners[0].y += diff * 0.5f;
                corners[1].y += diff * 0.5f;
                corners[2].y -= diff * 0.5f;
                corners[3].y -= diff * 0.5f;
            }
            return corners;
        }
        
        private void OnTriggerEnter(Collider col)
        {
            if (!col.gameObject.GetComponent<Interaction>()) return;
            if (((1 << col.gameObject.layer) & _layermask.value) > 0)
            {
                Select(col.gameObject, true);
            }
        }

        private IEnumerator ProcessTriggerHits()
        {
            yield return new WaitForFixedUpdate();
            _selectionCollider.enabled = false;
        }

        public void FindUniqueID(ulong id)
        {
            var selectedGameObject = Pool.Instance.PoolManager.FindPayload(id).gameObject;
            if (selectedGameObject == null)
            {
                Logging.Logger.Log(LogType.Log, $"Entity with UniqueID \"{id}\" not found");
            }
            else
            {
                Logging.Logger.Log(LogType.Log, $"Entity with UniqueID \"{id}\" found");
                //RuntimeSelectionComponent.Instance.Focus(selectedGameObject.transform.position, 0.5f);
                Select(selectedGameObject, false);
            }
        }

        public void FindDevice(string deviceName)
        {
            //TODO
            // var device = ProjectManager.Singleton.Devices.FindLast(x => x.Link.Name == deviceName);
            // if (device == null)
            // {
            //     Logging.Logger.Log(LogType.Log, $"Device with Name \"{deviceName}\" not found");
            // }
            // else
            // {
            //     Logging.Logger.Log(LogType.Log, $"Device with Name \"{deviceName}\" found");
            //     // RuntimeSelectionComponent.Instance.Focus(device.Link.GameObject.transform.position, 0.5f);
            //     Select(device.Link.GameObject, false);
            // }
        }
        
        private void PointerEnterEvent(GameObject target)
        {
            ExecuteEvents.Execute(target, new PointerEventData(EventSystem.current), ExecuteEvents.pointerEnterHandler);
        }
        
        private void PointerExitEvent(GameObject target)
        {
            ExecuteEvents.Execute(target, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
        }

        private void OnSelect(GameObject target)
        {
            ExecuteEvents.Execute(target, new PointerEventData(EventSystem.current), ExecuteEvents.selectHandler);
        }
        
        private void OnDeselect(GameObject target)
        {
            ExecuteEvents.Execute(target, new PointerEventData(EventSystem.current), ExecuteEvents.deselectHandler);
        }

        private void PointerClickEvent(GameObject target)
        {
            ExecuteEvents.Execute(target, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
        }
        
        private void PointerDownEvent(GameObject target)
        {
            ExecuteEvents.Execute(target, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
        }
        
        private void PointerUpEvent(GameObject target)
        {
            ExecuteEvents.Execute(target, new PointerEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
        }
    }
}
