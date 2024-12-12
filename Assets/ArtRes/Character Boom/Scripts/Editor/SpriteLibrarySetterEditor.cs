using UnityEditor;

namespace OnlyNew.CharacterBoom
{
    [CustomEditor(typeof(SpriteLibrarySetter))]
    public class SpriteLibrarySetterEditor : UnityEditor.Editor
    {
        private void OnEnable()
        {
            // TODO: find properties we want to work with
            //serializedObject.FindProperty();
        }

        public override void OnInspectorGUI()
        {
            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update();

            // TODO: Draw UI here
            //EditorGUILayout.PropertyField();
            DrawDefaultInspector();
            EditorGUILayout.HelpBox(
                "This component overrides the SpriteResolver settings for each child-object. If you need to configure individual objects, please remove this component"
                , MessageType.Info);

            // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties();
        }
    }
}