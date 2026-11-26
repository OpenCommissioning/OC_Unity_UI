using System.Linq;
using OC.UI.Interactions;
using UnityEditor;
using UnityEngine;

namespace OC.UI.Editor
{
    [CustomEditor(typeof(Label))]
    public class LabelEditor : UnityEditor.Editor
    {
        private SerializedProperty _input;
        private SerializedProperty _selectedProperties;
        private SerializedProperty _caption;
        private SerializedProperty _visibilityMode;
        private SerializedProperty _faceCamera;
        private SerializedProperty _reverseDirection;
        private SerializedProperty _fontSize;
        private SerializedProperty _fontStyle;
        private SerializedProperty _color;
        private SerializedProperty _textOffset;

        private void OnEnable()
        {
            _input = serializedObject.FindProperty("_input");
            _selectedProperties = serializedObject.FindProperty("_selectedProperties");
            _caption = serializedObject.FindProperty("_caption");
            _visibilityMode = serializedObject.FindProperty("_visibilityMode");
            _faceCamera = serializedObject.FindProperty("_faceCamera");
            _reverseDirection = serializedObject.FindProperty("_reverseDirection");
            _fontSize = serializedObject.FindProperty("_fontSize");
            _fontStyle = serializedObject.FindProperty("_fontStyle");
            _color = serializedObject.FindProperty("_color");
            _textOffset = serializedObject.FindProperty("_textOffset");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawInput();
            DrawPropertySelections();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_visibilityMode);
            EditorGUILayout.PropertyField(_faceCamera);
            EditorGUILayout.PropertyField(_reverseDirection);
            EditorGUILayout.PropertyField(_textOffset);
            EditorGUILayout.PropertyField(_fontSize);
            EditorGUILayout.PropertyField(_fontStyle);
            EditorGUILayout.PropertyField(_color);

            DrawCaption();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawInput()
        {
            if (_input.objectReferenceValue != null && _input.objectReferenceValue is not MonoBehaviour)
            {
                _input.objectReferenceValue = null;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            var current = _input.objectReferenceValue as MonoBehaviour;
            var next = (MonoBehaviour)EditorGUILayout.ObjectField(
                new GUIContent("Input", "Any MonoBehaviour script. Lists simple serialized fields and public properties."),
                current,
                typeof(MonoBehaviour),
                true);
            if (EditorGUI.EndChangeCheck())
            {
                _input.objectReferenceValue = next;
            }

            if (GUILayout.Button("Find Parent", GUILayout.Width(90)))
            {
                FindParentInput();
            }
            EditorGUILayout.EndHorizontal();
        }

        // Closest parent/self script that has displayable members.
        private void FindParentInput()
        {
            var label = (Label)target;
            var candidates = label.GetComponentsInParent<MonoBehaviour>(true)
                .Where(c => c != null && c != label && Label.GetDisplayMemberNames(c.GetType()).Any())
                .ToArray();

            if (candidates.Length == 0)
            {
                EditorUtility.DisplayDialog(
                    "Find Parent",
                    "No script with displayable members was found on this object or its parents.",
                    "OK");
                return;
            }

            _input.objectReferenceValue = candidates[0];
        }

        private void DrawPropertySelections()
        {
            var source = _input.objectReferenceValue as MonoBehaviour;
            if (source == null)
            {
                EditorGUILayout.HelpBox("Assign an Input script, or click Find Parent.", MessageType.Info);
                return;
            }

            var memberNames = Label.GetDisplayMemberNames(source.GetType()).ToList();
            if (memberNames.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    $"{source.GetType().Name} has no simple serialized fields or public properties to display.",
                    MessageType.Warning);
                return;
            }

            var captionActive = !string.IsNullOrWhiteSpace(_caption.stringValue);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Displayed Properties", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            using (new EditorGUI.DisabledScope(captionActive))
            {
                if (GUILayout.Button("Select All", GUILayout.Width(80)))
                {
                    ClearCaption();
                    foreach (var memberName in memberNames)
                    {
                        if (FindSelection(memberName) < 0) AddSelection(memberName);
                    }
                }

                if (GUILayout.Button("Deselect All", GUILayout.Width(90)))
                {
                    _selectedProperties.ClearArray();
                }
            }
            EditorGUILayout.EndHorizontal();

            if (captionActive)
            {
                EditorGUILayout.HelpBox("Clear the Caption to choose displayed properties.", MessageType.Info);
            }

            using (new EditorGUI.DisabledScope(captionActive))
            {
                foreach (var memberName in memberNames)
                {
                    var index = FindSelection(memberName);

                    EditorGUILayout.BeginHorizontal();
                    var enabled = EditorGUILayout.ToggleLeft(memberName, index >= 0);
                    if (enabled != (index >= 0))
                    {
                        if (enabled)
                        {
                            ClearCaption();
                            AddSelection(memberName);
                            index = FindSelection(memberName);
                        }
                        else
                        {
                            _selectedProperties.DeleteArrayElementAtIndex(index);
                            index = -1;
                        }
                    }

                    if (index >= 0)
                    {
                        EditorGUILayout.PropertyField(
                            _selectedProperties.GetArrayElementAtIndex(index).FindPropertyRelative("unit"),
                            GUIContent.none,
                            GUILayout.Width(70));
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private void DrawCaption()
        {
            var propertiesActive = _selectedProperties.arraySize > 0;
            using (new EditorGUI.DisabledScope(propertiesActive))
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_caption, new GUIContent("Caption"));
                if (EditorGUI.EndChangeCheck() && !string.IsNullOrWhiteSpace(_caption.stringValue))
                {
                    _selectedProperties.ClearArray();
                }
            }

            if (propertiesActive)
            {
                EditorGUILayout.HelpBox(
                    "Deselect displayed properties to edit the Caption.",
                    MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "Tokens: {Name}, {MemberName}. Add a format after a colon, e.g. {Phase} or {Value:N2}. " +
                    "An empty Caption lists the checked properties.",
                    MessageType.Info);
            }
        }

        private void ClearCaption()
        {
            _caption.stringValue = string.Empty;
        }

        private void AddSelection(string memberName)
        {
            _selectedProperties.InsertArrayElementAtIndex(_selectedProperties.arraySize);
            var selection = _selectedProperties.GetArrayElementAtIndex(_selectedProperties.arraySize - 1);
            selection.FindPropertyRelative("propertyName").stringValue = memberName;
            selection.FindPropertyRelative("format").stringValue = string.Empty;
            selection.FindPropertyRelative("unit").stringValue = string.Empty;
        }

        private int FindSelection(string memberName)
        {
            for (var index = 0; index < _selectedProperties.arraySize; index++)
            {
                var selection = _selectedProperties.GetArrayElementAtIndex(index);
                if (selection.FindPropertyRelative("propertyName").stringValue == memberName)
                {
                    return index;
                }
            }

            return -1;
        }
    }
}
