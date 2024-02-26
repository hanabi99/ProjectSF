using System;
using System.Collections.Generic;
using UnityEngine;

namespace OnlyNew.CharacterBoom
{
    [DefaultExecutionOrder(1)]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class ColorSetter : MonoBehaviour
    {
        public Material OriginSpriteBlendMat;
        public Material SpriteBlendMat;
        public Material ShapeBlendMat;
        public CharacterBoomColorPreset characterBoomAsset;
        private Material preOriginSpriteBlendMat;
        private Material preSpriteBlendMat, preShapeBlendMat;
        private List<SetMatColor> setters = new List<SetMatColor>();
        public Dictionary<string, ColorSets> colorDict = new Dictionary<string, ColorSets>();
        private bool colorChanged = false;
        public Action OnLoadNewPresetCallback;
        [HideInInspector]
        public bool advance = false;

        [SerializeField]
        [HideInInspector]
        private int assetHash;

        private Materials mats = new Materials();

        private void Awake()
        {
        }

        private void OnEnable()
        {
            SetDefaultMaterials();
            InitialMaterial();
            InitializeAsset();
        }

        private void Update()
        {
            if (colorChanged)
            {
                if (!Application.isPlaying)
                {
                    //SetterToAsset();
                }
                colorChanged = false;
            }
        }

        public bool AssetIsDirty()
        {
            if (characterBoomAsset != null)
            {
                if (assetHash != characterBoomAsset.GetHashCode())
                {
                    assetHash = characterBoomAsset.GetHashCode();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void InitialMaterial()
        {
            GetComponentsInChildren(true, setters);

            if (preOriginSpriteBlendMat != OriginSpriteBlendMat || preSpriteBlendMat != SpriteBlendMat || preShapeBlendMat != ShapeBlendMat)
            {
                if (setters != null)
                {
                    if (setters.Count > 0)
                    {
                        foreach (var setter in setters)
                        {
                            setter.mats = mats;
                            if (setter.materialType == MaterialType.fromColorSetter)
                            {
                                bool isInclude = false;
                                isInclude = CheckInclude(setter.gameObject.name);
                                if (isInclude)
                                {
                                    if (OriginSpriteBlendMat != null)
                                        setter.material = OriginSpriteBlendMat;
                                }
                                else
                                {
                                    //default mat
                                    if (ShapeBlendMat != null)
                                        setter.material = ShapeBlendMat;
                                }
                            }
                            else
                            {
                                setter.SetMat();
                            }
                        }
                        preOriginSpriteBlendMat = OriginSpriteBlendMat;
                        preSpriteBlendMat = SpriteBlendMat;
                        preShapeBlendMat = ShapeBlendMat;
                    }
                }
            }
        }

        public void AssetToSetter(List<SetMatColor> setters)
        {
            if (AssetIsDirty())
            {
                //set colors
                SetColors(this, setters);
                OnLoadNewPresetCallback?.Invoke();
                //Debug.Log("AssetToSetter");
            }
        }

        private void SetColors(ColorSetter t, List<SetMatColor> setters)
        {
            foreach (SetMatColor setter in setters)
            {
                for (int i = 0; i < t.characterBoomAsset.Keys.Count; i++)
                {
                    if (setter.gameObject.name == t.characterBoomAsset.Keys[i])
                    {
                        setter.PrimaryColor = t.characterBoomAsset.Values[i].primaryColor;
                        setter.SecondColor = t.characterBoomAsset.Values[i].secondColor;
                        setter.ThirdColor = t.characterBoomAsset.Values[i].thirdColor;
                        break;
                    }
                }
            }
        }

        private bool CheckInclude(string name)
        {
            foreach (var item in GroupSettings.originSpriteBlendGroup)
            {
                if (name != item)
                {
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        private void OnValidate()
        {
            InitialMaterial();
            InitializeAsset();
        }

        public void InitializeAsset()
        {
            if (characterBoomAsset != null)
            {
                var hashcode = characterBoomAsset.GetHashCode();
                if (assetHash != hashcode)
                {
                    assetHash = hashcode;
                    SetColors(this, setters);
                    OnLoadNewPresetCallback?.Invoke();
                    //Debug.Log("InitializeAsset");
                }
            }
        }

        public void SetDefaultMaterials()
        {
            mats.OriginSpriteBlendMat = OriginSpriteBlendMat;
            mats.SpriteBlendMat = SpriteBlendMat;
            mats.ShapeBlendMat = ShapeBlendMat;
            GetComponentsInChildren(true, setters);

            if (setters.Count > 0)
            {
                foreach (var setter in setters)
                {
                    setter.mats = mats;
                }
            }
        }

        public void ColorChanged()
        {
            colorChanged = true;

            //Debug.Log("ColorChanged");
        }
    }
}