using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using OC;
using OC.Interactions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace OC.UI.Interactions
{
    public enum LabelVisibilityMode
    {
        AlwaysVisible,
        ShowOnHover
    }

    [Serializable]
    public sealed class LabelProperty
    {
        public string propertyName;
        public string format;
        public string unit;
    }

    [AddComponentMenu("Open Commissioning/UI/Label")]
    public class Label : MonoBehaviour
    {
        private static readonly Regex TokenPattern = new Regex(@"\{([^{}]+)\}");

        [FormerlySerializedAs("_sourceComponent")]
        [SerializeField]
        private MonoBehaviour _input;
        [SerializeField]
        private List<LabelProperty> _selectedProperties = new List<LabelProperty>();
        [FormerlySerializedAs("_textTemplate")]
        [SerializeField]
        private string _caption;
        [SerializeField]
        private LabelVisibilityMode _visibilityMode;
        [SerializeField]
        private bool _faceCamera = true;
        [SerializeField]
        private bool _reverseDirection = false;
        [SerializeField]
        private float _fontSize = 0.2f;
        [SerializeField]
        private FontStyles _fontStyle = FontStyles.Normal;
        [SerializeField]
        private Color _color = Color.white;
        [SerializeField]
        private Vector3 _textOffset = new Vector3(0f, 0.2f, 0f);

        private readonly RaycastHit[] _hoverHits = new RaycastHit[32];
        private TextMeshPro _text;
        private bool _isHovered;
        // Master switch from LabelManager / Label Tool. True when no manager is present.
        private bool _masterVisible = true;

        /// <summary>Global on/off from <see cref="LabelManager"/>. Does not disable the GameObject (Label may share it with a device).</summary>
        public void SetMasterVisible(bool visible)
        {
            _masterVisible = visible;
            ApplyVisibility();
        }

        // Public readable properties plus serialized fields of simple / IProperty types (not UnityEngine.Object refs).
        public static IEnumerable<string> GetDisplayMemberNames(Type type)
        {
            var names = new SortedSet<string>(StringComparer.Ordinal);
            for (; type != null && type != typeof(Component) && type != typeof(Behaviour) && type != typeof(MonoBehaviour);
                 type = type.BaseType)
            {
                foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
                {
                    if (property.CanRead
                        && property.GetIndexParameters().Length == 0
                        && IsDisplayType(property.PropertyType))
                    {
                        names.Add(property.Name);
                    }
                }

                foreach (var field in type.GetFields(
                             BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                {
                    if (!IsSerializedField(field) || !IsDisplayType(field.FieldType)) continue;
                    names.Add(field.Name);
                }
            }

            return names;
        }

        private static bool IsSerializedField(FieldInfo field)
        {
            return field.IsPublic || field.IsDefined(typeof(SerializeField), inherit: true);
        }

        // Scalars, strings, vectors, colors, enums, and OC IProperty — skip Object references and arrays.
        private static bool IsDisplayType(Type type)
        {
            if (type == null) return false;
            if (typeof(IProperty).IsAssignableFrom(type)) return true;
            if (type.IsEnum) return true;
            if (type == typeof(bool) || type == typeof(string)) return true;
            if (type == typeof(int) || type == typeof(uint) || type == typeof(long) || type == typeof(ulong)) return true;
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return true;
            if (type == typeof(Vector2) || type == typeof(Vector3) || type == typeof(Vector4)) return true;
            if (type == typeof(Color) || type == typeof(Color32)) return true;
            return false;
        }

        private void Reset()
        {
            _reverseDirection = false;
            EnsureText();
        }

        private void Awake()
        {
            // If the Label Tool is in the scene, start hidden until it is toggled on.
            if (LabelManager.Singleton != null) _masterVisible = false;
            EnsureText();
            ExcludeTextFromOutline();
            RefreshText();
            ApplyVisibility();
        }

        private void Update()
        {
            UpdateHover();
            // Content first, then visibility: Show On Hover must enable a mesh that already has the Caption.
            RefreshText();
            ApplyVisibility();
        }

        private void LateUpdate()
        {
            var text = TmpText;
            if (text == null) return;

            // Keep the mesh above the object so it does not clip through machine geometry.
            text.transform.localPosition = _textOffset;

            var camera = Camera.main;
            var flip = _reverseDirection ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
            if (_faceCamera && camera != null)
            {
                text.transform.rotation = camera.transform.rotation * flip;
                // Pull slightly toward the camera to avoid z-fighting with nearby meshes.
                text.transform.position -= camera.transform.forward * 0.02f;
            }
            else
            {
                text.transform.localRotation = flip;
            }

            // Outline reapplies hover/selection masks on Interaction renderers; keep text clear.
            ExcludeTextFromOutline();
        }

        private void OnValidate()
        {
            _fontSize = Mathf.Max(0.01f, _fontSize);
            RefreshText();
            ApplyVisibility();
        }

        // Prefer Interaction.Hovered (same path as the white outline). Fall back to a physics raycast
        // when there is no Interaction component or the Interaction Tool is off.
        private void UpdateHover()
        {
            _isHovered = _visibilityMode == LabelVisibilityMode.ShowOnHover && IsPointerOverSource();
        }

        private bool IsPointerOverSource()
        {
            var interaction = FindHoverInteraction();
            if (interaction != null && interaction.State.Value.HasFlag(InteractionState.Hovered))
            {
                return true;
            }

            // Interaction Tool off leaves State idle even when an Interaction exists — raycast instead.
            return IsPointerOverSourceByRaycast();
        }

        private Interaction FindHoverInteraction()
        {
            if (_input != null)
            {
                var onSource = _input.GetComponent<Interaction>()
                    ?? _input.GetComponentInParent<Interaction>()
                    ?? _input.GetComponentInChildren<Interaction>();
                if (onSource != null) return onSource;
            }

            return GetComponent<Interaction>()
                ?? GetComponentInParent<Interaction>()
                ?? GetComponentInChildren<Interaction>();
        }

        private bool IsPointerOverSourceByRaycast()
        {
            var camera = Camera.main;
            if (camera == null || !TryGetPointerPosition(out var pointer)) return false;

            var root = _input != null ? _input.transform : transform;
            var ray = camera.ScreenPointToRay(pointer);
            var hitCount = Physics.RaycastNonAlloc(
                ray,
                _hoverHits,
                500f,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Collide);

            for (var index = 0; index < hitCount; index++)
            {
                if (IsRelatedTransform(_hoverHits[index].transform, root)) return true;
            }

            return false;
        }

        // Collider may be on a parent or child of the Label / input script.
        private static bool IsRelatedTransform(Transform hit, Transform root)
        {
            return hit == root || hit.IsChildOf(root) || root.IsChildOf(hit);
        }

        private static bool TryGetPointerPosition(out Vector2 position)
        {
            if (Mouse.current != null)
            {
                position = Mouse.current.position.ReadValue();
                return true;
            }

            if (UnityEngine.InputSystem.Pointer.current != null)
            {
                position = UnityEngine.InputSystem.Pointer.current.position.ReadValue();
                return true;
            }

            position = default;
            return false;
        }

        private TextMeshPro TmpText => _text != null ? _text : (_text = GetComponentInChildren<TextMeshPro>(true));

        // Adds the text child so adding the component is enough; an existing TextMeshPro child is reused.
        private void EnsureText()
        {
            if (TmpText != null)
            {
                ConfigureText(_text);
                return;
            }

            var child = new GameObject("Text");
            child.transform.SetParent(transform, false);
            child.transform.localPosition = _textOffset;
            _text = child.AddComponent<TextMeshPro>();
            ConfigureText(_text);
        }

        private static void ConfigureText(TextMeshPro text)
        {
            text.alignment = TextAlignmentOptions.Center;
            text.enableWordWrapping = false;
            text.overflowMode = TextOverflowModes.Overflow;
            text.extraPadding = true;
            text.outlineWidth = 0f;
            text.rectTransform.sizeDelta = new Vector2(20f, 5f);

            var meshRenderer = text.renderer;
            if (meshRenderer != null)
            {
                meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                meshRenderer.receiveShadows = false;
            }
        }

        // Interaction.Outline paints every child Renderer on hover; TMP glyph quads become white boxes.
        private void ExcludeTextFromOutline()
        {
            var text = TmpText;
            if (text?.renderer == null) return;

            text.renderer.renderingLayerMask = 1u;
            foreach (var interaction in GetComponentsInParent<Interaction>(true))
            {
                interaction.Renderers?.Remove(text.renderer);
            }
        }

        private void RefreshText()
        {
            var text = TmpText;
            if (text == null) return;

            ConfigureText(text);
            text.fontSize = _fontSize;
            text.fontStyle = _fontStyle;
            text.color = _color;

            var content = string.IsNullOrEmpty(_caption) ? BuildAutoText() : FormatTemplate(_caption);
            if (text.text != content)
            {
                text.text = content;
                text.ForceMeshUpdate(true);
            }
        }

        private void ApplyVisibility()
        {
            var text = TmpText;
            if (text == null) return;

            var visible = _masterVisible
                && (_visibilityMode == LabelVisibilityMode.AlwaysVisible || _isHovered);
            // Keep the TMP component enabled so Caption mesh updates while hidden; only hide the renderer.
            if (!text.enabled) text.enabled = true;
            if (text.renderer != null && text.renderer.enabled != visible)
            {
                text.renderer.enabled = visible;
                if (visible) text.ForceMeshUpdate(true);
            }
        }

        private string BuildAutoText()
        {
            var lines = new List<string>();
            foreach (var property in _selectedProperties)
            {
                var key = property.propertyName;
                if (!string.IsNullOrEmpty(key) && TryGetValue(key, out var value, out var format, out var unit))
                {
                    lines.Add(key + ": " + FormatValue(value, format ?? property.format, unit ?? property.unit));
                }
            }

            return lines.Count == 0 ? gameObject.name : string.Join("\n", lines);
        }

        private string FormatTemplate(string template)
        {
            return TokenPattern.Replace(template, match =>
                TryResolveToken(match.Groups[1].Value, out var text) ? text : match.Value);
        }

        // Token syntax: {Name}, {MemberName}, optionally with a format after a colon: {Value:N2}.
        private bool TryResolveToken(string token, out string text)
        {
            var separator = token.IndexOf(':');
            var key = separator < 0 ? token : token.Substring(0, separator);
            var format = separator < 0 ? null : token.Substring(separator + 1);

            if (key == "Name")
            {
                text = gameObject.name;
                return true;
            }

            text = null;
            if (!TryGetValue(key, out var value, out var defaultFormat, out var unit)) return false;
            text = FormatValue(value, format ?? defaultFormat, unit);
            return true;
        }

        private bool TryGetValue(string key, out object value, out string format, out string unit)
        {
            format = null;
            unit = null;
            foreach (var property in _selectedProperties)
            {
                if (property.propertyName != key) continue;
                format = property.format;
                unit = property.unit;
                return TryReadMember(_input, key, out value);
            }

            // Caption tokens can name a member without it being selected for the auto list.
            if (TryReadMember(_input, key, out value)) return true;

            value = null;
            return false;
        }

        private static bool TryReadMember(Component source, string memberPath, out object value)
        {
            value = null;
            if (source == null || string.IsNullOrEmpty(memberPath)) return false;

            object current = source;
            foreach (var name in memberPath.Split('.'))
            {
                if (current == null) return false;
                var type = current.GetType();
                var property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
                if (property != null && property.CanRead && property.GetIndexParameters().Length == 0)
                {
                    current = property.GetValue(current);
                }
                else
                {
                    var field = type.GetField(
                        name,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (field == null) return false;
                    current = field.GetValue(current);
                }

                // OC reactive properties (IProperty<T>) expose their data through Value.
                if (current is IProperty)
                {
                    current = current.GetType().GetProperty("Value")?.GetValue(current);
                }
            }

            value = current;
            return true;
        }

        private static string FormatValue(object value, string format, string unit = null)
        {
            string text;
            if (value is IFormattable formattable && !string.IsNullOrEmpty(format))
            {
                try
                {
                    text = formattable.ToString(format, CultureInfo.CurrentCulture);
                }
                catch (FormatException)
                {
                    // Format is typed by the user (e.g. "2" instead of "N2"); show the unformatted value.
                    text = Convert.ToString(value, CultureInfo.CurrentCulture);
                }
            }
            else
            {
                text = Convert.ToString(value, CultureInfo.CurrentCulture);
            }

            return string.IsNullOrEmpty(unit) ? text : text + " " + unit;
        }
    }
}
