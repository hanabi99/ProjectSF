using System;
using UnityEngine;

namespace OnlyNew.CharacterBoom
{
    public enum MaterialType
    {
        fromColorSetter,
        manual,
        originSpriteBlend,
        SpriteBlend,
        ShapeBlend
    }

    public class Materials
    {
        public Material OriginSpriteBlendMat;
        public Material SpriteBlendMat;
        public Material ShapeBlendMat;
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    [Serializable]
    public class SetMatColor : MonoBehaviour
    {
        [SerializeField]
        private Color primaryColor = new Color(1, 1, 1, 1);

        [SerializeField]
        private Color secondColor = new Color(1, 1, 1, 0);

        [SerializeField]
        private Color thirdColor = new Color(1, 1, 1, 0);

        public MaterialType materialType = MaterialType.fromColorSetter;
        public Material material;
        public bool colorOffset = true;
        private Vector4 offset2 = new Vector4(0, 0, 0, 0);
        private Vector4 offset3 = new Vector4(0, 0, 0, 0);
        private Color preSecondColor = new Color(1, 1, 1, 0);
        private Color preThirdColor = new Color(1, 1, 1, 0);
        private Material preMaterial;
        public Materials mats = new Materials();

        public Color PrimaryColor
        {
            get => primaryColor; set
            {
                if (primaryColor != value)
                {
                    primaryColor = value;
                    SetColors();
                }
            }
        }

        public Color SecondColor
        {
            get => secondColor; set
            {
                if (secondColor != value)
                {
                    secondColor = value;
                    SetColors();
                }
            }
        }

        public Color ThirdColor
        {
            get => thirdColor; set
            {
                if (thirdColor != value)
                {
                    thirdColor = value;
                    SetColors();
                }
            }
        }

        public Vector4 Offset2 => offset2;
        public Vector4 Offset3 => offset3;

        private void Start()
        {
            if (Application.isPlaying)
            {
                var renderer = GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.sharedMaterial = material;
                    preMaterial = material;
                }
            }
            SetColors();
        }

        private void OnValidate()
        {
            //Undo.RecordObject(this,"set material color");
            SetMat();
            SetColors();
        }

        private Material tempMaterial;

        private void SetColors()
        {
            //Get the Renderer component from the new cube
            if (material != null)
            {
                var renderer = GetComponent<Renderer>();
                if (renderer == null)
                    return;
                if (preMaterial != material)
                {
                    if (Application.isPlaying)
                    {
                        tempMaterial = material;
                    }
                    else
                    {
                        tempMaterial = new Material(material);
                    }
                    //renderer.material = tempMaterial;
                    tempMaterial.color = Color.red;
                    renderer.sharedMaterial = tempMaterial;
                    preMaterial = material;
                }

                //Call SetColor using the shader property name "_Color" and setting the color to red
                if (colorOffset)
                {
                    if (preSecondColor == secondColor)
                    {
                        secondColor = primaryColor + (Color)offset2;
                    }
                    if (preThirdColor == thirdColor)
                    {
                        thirdColor = primaryColor + (Color)offset3;
                    }
                }
                if (Application.isPlaying)
                {
                    var mat = renderer.material;
                    if (mat != null)
                    {
                        mat.SetColor("_Color", primaryColor);
                        mat.SetColor("_SecondColor", secondColor);
                        mat.SetColor("_ThirdColor", thirdColor);
                    }
                }
                else
                {
                    var mat = renderer.sharedMaterial;
                    if (mat != null)
                    {
                        mat.SetColor("_Color", primaryColor);
                        mat.SetColor("_SecondColor", secondColor);
                        mat.SetColor("_ThirdColor", thirdColor);
                    }
                }
                offset3 = thirdColor - primaryColor;
                offset2 = secondColor - primaryColor;
                preThirdColor = thirdColor;
                preSecondColor = secondColor;
            }
        }

        public void SetMat()
        {
            if (mats != null)
            {
                switch (materialType)
                {
                    case MaterialType.fromColorSetter:
                        break;

                    case MaterialType.manual:
                        break;

                    case MaterialType.originSpriteBlend:
                        if (mats.OriginSpriteBlendMat != null)
                            material = mats.OriginSpriteBlendMat;
                        break;

                    case MaterialType.SpriteBlend:
                        if (mats.SpriteBlendMat != null)
                            material = mats.SpriteBlendMat;
                        break;

                    case MaterialType.ShapeBlend:
                        if (mats.ShapeBlendMat != null)
                            material = mats.ShapeBlendMat;
                        break;

                    default:
                        if (mats.ShapeBlendMat != null)
                            material = mats.ShapeBlendMat;
                        break;
                }
            }
        }
    }

    [Serializable]
    public class ColorSets
    {
        public ColorSets(Color primary, Color second, Color third)
        {
            primaryColor = primary;
            secondColor = second;
            thirdColor = third;
        }

        public Color primaryColor = new Color(1, 1, 1, 1);
        public Color secondColor = new Color(1, 1, 1, 0);
        public Color thirdColor = new Color(1, 1, 1, 0);
    }
}