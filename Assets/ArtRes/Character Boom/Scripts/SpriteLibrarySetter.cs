using System.Collections.Generic;
using UnityEngine;

namespace OnlyNew.CharacterBoom
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UnityEngine.U2D.Animation.SpriteLibrary))]
    [RequireComponent(typeof(ColorSetter))]
    [ExecuteAlways]
    public class SpriteLibrarySetter : MonoBehaviour
    {
        public string label;
        private List<UnityEngine.U2D.Animation.SpriteResolver> spriteResolvers = new List<UnityEngine.U2D.Animation.SpriteResolver>();
        private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
        private ColorSetter colorSetter;

        private void Awake()
        {
            GetComponentsInChildren(true, spriteRenderers);
            colorSetter = GetComponent<ColorSetter>();
            foreach (var spriteRenderer in spriteRenderers)
            {
                if (spriteRenderer.gameObject.GetComponent<UnityEngine.U2D.Animation.SpriteResolver>() == null)
                {
                    spriteRenderer.gameObject.AddComponent<UnityEngine.U2D.Animation.SpriteResolver>();
                }
            }
            SetSkin();
        }

        private void SetSkin()
        {
            GetComponentsInChildren(true, spriteResolvers);
            if (spriteResolvers.Count > 0)
            {
                foreach (var item in spriteResolvers)
                {
                    setLabel(item, label);
                }
            }
            else
            {
                Debug.LogError("cannot find SpriteResolver");
            }
        }

        private static void setLabel(UnityEngine.U2D.Animation.SpriteResolver item, string label)
        {
            foreach (var name in GroupSettings.skin)
            {
                if (item.gameObject.name == name)
                {
                    item.SetCategoryAndLabel(item.gameObject.name, "basic skin");
                    return;
                }
                else if (item.gameObject.name == "Shadow")
                {
                    item.SetCategoryAndLabel(item.gameObject.name, "basic skin");
                    return;
                }
            }
            foreach (var name in GroupSettings.face)
            {
                if (item.gameObject.name == name)
                {
                    item.SetCategoryAndLabel(item.gameObject.name, "basic skin");
                    return;
                }
            }

            item.SetCategoryAndLabel(item.gameObject.name, label);
        }

        private void OnValidate()
        {
            SetSkin();
        }
    }
}