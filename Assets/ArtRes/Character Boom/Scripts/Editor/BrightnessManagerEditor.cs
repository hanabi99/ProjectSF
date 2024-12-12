using UnityEditor;
using UnityEngine;

namespace OnlyNew.CharacterBoom
{
    [CustomEditor(typeof(BrightnessManager))]
    public class BrightnessManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Remember Current Colors"))
            {
                var t = target as BrightnessManager;
                if (t != null)
                {
                    t.RememberCurrentColors();
                }
            }
        }
    }
}