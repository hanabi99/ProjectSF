using UnityEditor;
using UnityEngine;

namespace OnlyNew.CharacterBoom
{
    [CustomEditor(typeof(FaceControl))]
    public class FaceControlEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            FaceControl t = target as FaceControl;
            //var s=EditorGUILayout.Slider("Brow Hight", 0f, -1f, 1f);
            t.browOffset = EditorGUILayout.Slider("Brow Hight", t.browOffset, -1f, 1f);
            t.noseOffset = EditorGUILayout.Slider("Nose Hight", t.noseOffset, -1f, 1f);

            t.mouthOffset = EditorGUILayout.Slider("Mouth Hight", t.mouthOffset, -1f, 1f);
            t.eyeSocketOffset = EditorGUILayout.Slider("EyeSocket Hight", t.eyeSocketOffset, -1f, 1f);

            t.eyesOffset = new Vector2(
                EditorGUILayout.Slider("Eyes vertical", t.eyesOffset.x, -1f, 1f)
                , EditorGUILayout.Slider("Eyes horizontal", t.eyesOffset.y, -1f, 1f)
                );

            t.eyesClose = EditorGUILayout.Slider("Eyes Close", t.eyesClose, 0f, 1f);
            t.UpdateFace();
            DrawDefaultInspector();
            EditorGUILayout.HelpBox(
                "The component covers transform of face bone. It is not essential. If you want to control the face through bones, please remove the component",
                MessageType.Info);
            // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties();
        }
    }
}