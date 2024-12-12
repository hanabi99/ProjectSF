using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OnlyNew.CharacterBoom
{
    [CustomEditor(typeof(RandomAssetSetter))]
    public class RandomAssetSetterEditor : UnityEditor.Editor
    {
        private Texture tex;
        private Texture2D background;

        private void OnEnable()
        {
            tex = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture>("Assets/Plugins/Character Boom/Character Boom Icon.png");
        }

        // Start is called before the first frame update
        private int index;

        public void ImportAssets(RandomAssetSetter t)
        {
            List<CharacterBoomColorPreset> m_assets = new List<CharacterBoomColorPreset>();
            List<CharacterBoomColorPreset> m_assets_backup = new List<CharacterBoomColorPreset>();
            m_assets.Clear();
            var setters = t.GetComponentsInChildren<ColorSetter>();
            if (setters != null && setters.Length == 0)
            {
                Debug.LogWarning("RandomAssetSetter should nest in parent-object of ColorSetter");
                return;
            }

            var guids = UnityEditor.AssetDatabase.FindAssets("t:CharacterBoomColorPreset");
            if (guids != null && guids.Length > 0)
            {
                foreach (var guid in guids)
                {
                    var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                    var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<CharacterBoomColorPreset>(path);
                    if (asset.name != "basic")
                    {
                        m_assets.Add(asset);
                    }
                }
            }
            List<SetMatColor> setMatColors = new List<SetMatColor>();
            if (m_assets != null && m_assets.Count > 0)
            {
                foreach (var setter in setters)
                {
                    if (setter != null)
                    {
                        index = Random.Range(0, m_assets.Count);
                        setter.characterBoomAsset = m_assets[index];
                        m_assets_backup.Add(m_assets[index]);
                        m_assets.RemoveAt(index);
                        if (m_assets.Count == 0)
                        {
                            m_assets.AddRange(m_assets_backup);
                            m_assets_backup.Clear();
                        }
                        setMatColors.Clear();
                        setter.GetComponentsInChildren(true, setMatColors);
                        setter.AssetToSetter(setMatColors);
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var t = target as RandomAssetSetter;
            GUIStyle style = new GUIStyle("Button");

            var button = GUILayout.Button(new GUIContent("Character Boom", tex, "Reference assets from path"), style, GUILayout.Height(64f));
            if (button)
            {
                ImportAssets(t);
            }
        }

        private Texture2D MakeBackgroundTexture(int width, int height, Color color)
        {
            Color[] pixels = new Color[width * height];

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            Texture2D backgroundTexture = new Texture2D(width, height);

            backgroundTexture.SetPixels(pixels);
            backgroundTexture.Apply();

            return backgroundTexture;
        }
    }
}