//using System.Collections;
//using System.Collections.Generic;
//using Battlehub.RTCommon;
//using UnityEngine;
//using UnityEngine.Events;

//[RequireComponent(typeof(MeshCollider))]
//[RequireComponent(typeof(Rigidbody))]
//public class BoxSelector : MonoBehaviour
//{

//    [SerializeField]
//    public UnityEvent<GameObject, bool> OnBoxSelect;
//    protected Vector3 _startMousePos;

//    protected Vector3 _endMousePos;

//    private Ray _ray;

//    [SerializeField]
//    private float _maxDistance = 10000;

//    private Mesh _currentSelectionMesh;

//    private MeshCollider _selectionCollider;

//    private LayerMask _layerMask;

//    private bool _mouseClicked;

//    private float _dragThreshold = .005f;

//    [SerializeField]
//    private bool _mouseDragging;
//    private void Start()
//    {
//        transform.position = Vector3.zero;
//        GetComponent<Rigidbody>().isKinematic = true;
//        GetComponent<Rigidbody>().useGravity = false;
//        _selectionCollider = GetComponent<MeshCollider>();
//        _selectionCollider.convex = true;
//        _selectionCollider.isTrigger = true;
//    }
//    void Update()
//    {
//        //if (!Input.GetKey(KeyCode.LeftControl)) return;
//        if (Input.GetMouseButtonDown(0))
//        {
//            _startMousePos = Input.mousePosition;
//        }
       
//        if (Input.GetMouseButton(0))
//        {
//            _endMousePos = Input.mousePosition;
//            if (Vector3.Magnitude(_startMousePos - _endMousePos) > _dragThreshold) _mouseDragging = true;
//        }

//        if (Input.GetMouseButtonUp(0) && _mouseDragging)
//        {
//            StartBoxSelection();
//            _mouseDragging=false;
//        }
//    }
//    private void StartBoxSelection()
//    {
//        _selectionCollider.enabled = true;
//        Vector3[] vertices = new Vector3[8];
//        Vector2[] corners = CreateCorners(_startMousePos, _endMousePos);

//        int index = 0;
//        for (int i = 0; i < corners.Length; ++i)
//        {
//            _ray = Camera.main.ScreenPointToRay(corners[i]);
//            Vector3 hitPoint = _ray.GetPoint(_maxDistance);

//            vertices[index] = hitPoint;
//            vertices[index + 4] = _ray.origin - hitPoint;

//            //Debug.DrawLine(Camera.main.ScreenToWorldPoint(corners[i]), hitPoint, Color.yellow, 3f);
//            index++;
//        }

//        _currentSelectionMesh = CreateSelectionMesh(vertices);
//        _selectionCollider.sharedMesh = _currentSelectionMesh;

//        StartCoroutine(ProcessTriggerHits());
//        _startMousePos = Vector3.zero;
//    }


//    private static int[] cubeTriangles =
//    {
//        0, 1, 2,
//        2, 1, 3,
//        4, 6, 0,
//        0, 6, 2,
//        6, 7, 2,
//        2, 7, 3,
//        7, 5, 3,
//        3, 5, 1,
//        5, 0, 1,
//        1, 4, 0,
//        4, 5, 6,
//        6, 5, 7
//    };
//    private Mesh CreateSelectionMesh(Vector3[] verts)
//    {
//        Vector3[] meshVerts = new Vector3[8];

//        for (int i = 0; i < 4; ++i)
//        {
//            meshVerts[i] = verts[i];
//            meshVerts[i + 4] = verts[i] + verts[i + 4];
//        }
//        Mesh mesh = new Mesh();
//        mesh.name = "BoxSelectionMesh";
//        mesh.vertices = meshVerts;
//        mesh.triangles = cubeTriangles;
//        return mesh;
//    }
//    private Vector2[] CreateCorners(Vector2 p1, Vector2 p2)
//    {
//        var bottomLeft = Vector3.Min(p1, p2);
//        var topRight = Vector3.Max(p1, p2);

//        Vector2[] corners = new Vector2[]
//        {
//            new Vector2(bottomLeft.x, topRight.y),
//            new Vector2(topRight.x, topRight.y),
//            new Vector2(bottomLeft.x, bottomLeft.y),
//            new Vector2(topRight.x, bottomLeft.y)
//        };
//        return corners;
//    }

//    private IEnumerator ProcessTriggerHits()
//    {
//        yield return new WaitForFixedUpdate();
//        _selectionCollider.enabled = false;
//    }

//    void OnTriggerEnter(Collider col)
//    {
//        if (!col.gameObject.GetComponent<ExposeToEditor>()) return;
//        if (col.gameObject.layer == 10)
//        {
//            Debug.Log("collision with: " + col.gameObject.name);
//            OnBoxSelect?.Invoke(col.gameObject, true);

//        }
//    }

//}
