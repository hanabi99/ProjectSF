using UnityEditor;
using UnityEngine;
using static OnlyNew.CharacterBoom.BrightnessManager;

namespace OnlyNew.CharacterBoom
{
    [CustomEditor(typeof(BrightnessManager))]
    public class BrightnessManagerEditor : Editor
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