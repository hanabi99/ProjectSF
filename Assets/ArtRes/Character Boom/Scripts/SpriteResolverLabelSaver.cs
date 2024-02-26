using UnityEngine;

#if UNITY_EDITOR
#endif
namespace OnlyNew.CharacterBoom
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class SpriteResolverLabelSaver : MonoBehaviour
    {
        public SpriteResolverLabelPreset preset;

        public bool foldout = false;
        public bool skin = true;
        public bool hair = true;
        public bool face = true;
        public bool cloth = true;
        #region AutoLoad    //block it for a issue related to SpriteSkin. Sprite will be placed in the wrong location.
        //        List<SpriteResolver> spriteResolvers;
        //        bool isDirty = false;
        //        private void OnEnable()
        //        {
        //            GetComponentsInChildren<SpriteResolver>(true, spriteResolvers);
        //            LoadAsset(this);
        //        }
        //        private void Update()
        //        {
        //            if (isDirty)
        //            {
        //                LoadAsset(this);
        //                isDirty = false;
        //            }
        //        }
        //        private void OnValidate()
        //        {
        //            if (preset != null)
        //            {
        //                if (AssetIsDirty(preset))
        //                {
        //                    isDirty = true;
        //                    Clean();
        //                }
        //            }
        //        }
        //        void LoadAsset(SpriteResolverLabelSaver t)
        //        {
        //            if (t.preset != null)
        //            {
        //                string label;
        //                GetComponentsInChildren<SpriteResolver>(true, spriteResolvers);
        //                if (spriteResolvers == null)
        //                    return;
        //                if (spriteResolvers.Count == 0)
        //                    return;
        //#if UNITY_EDITOR
        //                Undo.RecordObjects(spriteResolvers.ToArray(), "Change Asset");
        //#endif
        //                foreach (var spriteResolver in spriteResolvers)
        //                {
        //                    if (t.preset.TryGetValue(spriteResolver.gameObject.name, out label))
        //                        spriteResolver.SetCategoryAndLabel(spriteResolver.gameObject.name, label);

        //                }
        //            }
        //        }
        #endregion
    }
}