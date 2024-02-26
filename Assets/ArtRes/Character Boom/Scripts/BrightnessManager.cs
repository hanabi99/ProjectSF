using System.Collections.Generic;
using UnityEngine;
namespace OnlyNew.CharacterBoom
{
    [DefaultExecutionOrder(10)]
    [ExecuteAlways]
    public class BrightnessManager : MonoBehaviour
    {
        [Range(0, 2f)]
        public float brightness = 0.5f;
        public BrightnessType brightnessType = BrightnessType.Linear;

        private float preBrightness = 0.5f;

        private BrightnessType preBrightnessType = BrightnessType.Linear;

        SetMatColor[] children;
        [SerializeField, HideInInspector]
        ColorSetsDictionary colorDict = new ColorSetsDictionary();
        public enum BrightnessType
        {
            Linear,
            Lerp
        }
        private void Awake()
        {
            Init();

        }
        private void OnEnable()
        {
            var colorsetter = GetComponent<ColorSetter>();
            if (colorsetter != null)
            {
                colorsetter.OnLoadNewPresetCallback += RememberCurrentColors;
            }
        }
        private void OnDisable()
        {
            var colorsetter = GetComponent<ColorSetter>();
            if (colorsetter != null)
            {
                colorsetter.OnLoadNewPresetCallback -= RememberCurrentColors;
            }
        }
        private void Init()
        {
            if (children == null)
            {
                children = GetComponentsInChildren<SetMatColor>();
            }
            foreach (var child in children)
            {
                colorDict.TryAdd(child, new ColorSets(child.PrimaryColor, child.SecondColor, child.ThirdColor));
            }
            UpdateBrightness();
            preBrightness = brightness;
            preBrightnessType= brightnessType;
        }
        public void SetBrightness(float t_brightness)
        {
            brightness = t_brightness;
        }
        public void RememberCurrentColors()
        {
            RefreshColorDict();
        }
        private void Reset()
        {
            brightness = 0.5f;
        }
        private void RefreshColorDict()
        {
            brightness = 0.5f;
            if (children == null)
            {
                children = GetComponentsInChildren<SetMatColor>();
            }
            foreach (var child in children)
            {
                if (!colorDict.TryAdd(child, new ColorSets(child.PrimaryColor, child.SecondColor, child.ThirdColor)))
                {
                    var c = colorDict[child];
                    c.primaryColor = child.PrimaryColor;
                    c.secondColor = child.SecondColor;
                    c.thirdColor = child.ThirdColor;
                }
            }
        }
        private void Update()
        {
            if (preBrightness != brightness || preBrightnessType != brightnessType)
            {
                UpdateBrightness();
                preBrightness = brightness;
                preBrightnessType = brightnessType;
            }
        }


        private void UpdateBrightness()
        {

            if (children.Length > 0)
            {
#if UNITY_EDITOR
                UnityEditor.Undo.RecordObjects(children,"UpdateBrightness");
#endif
                Color c1, c2, c3;
                foreach (var child in children)
                {
                    ColorSets originalColor;
                    if (colorDict.TryGetValue(child, out originalColor))
                    {
                        if (originalColor != null)
                        {
                            if (brightnessType == BrightnessType.Linear)
                            {
                                LinearColor(brightness, out c1, out c2, out c3, originalColor);
                            }
                            else
                            {
                                LerpColor(brightness, out c1, out c2, out c3, originalColor);
                            }


                            child.PrimaryColor = c1;
                            child.SecondColor = c2;
                            child.ThirdColor = c3;
                        }
                    }
                }
            }
            else
            {
                var childrenSpriteRenderer = GetComponentsInChildren<SpriteRenderer>();
            }
        }

        private static void LinearColor(float brightness, out Color c1, out Color c2, out Color c3, ColorSets originalColor)
        {
            var t = brightness * 2f;
            c1 = new Color(
                originalColor.primaryColor.r * t,
                originalColor.primaryColor.g * t,
                originalColor.primaryColor.b * t,
                originalColor.primaryColor.a
                );
            c2 = new Color(
                originalColor.secondColor.r * t,
                originalColor.secondColor.g * t,
                originalColor.secondColor.b * t,
                originalColor.secondColor.a
                );
            c3 = new Color(
                originalColor.thirdColor.r * t,
                originalColor.thirdColor.g * t,
                originalColor.thirdColor.b * t,
                originalColor.thirdColor.a
                );
        }

        private static void LerpColor(float brightness, out Color primaryColor, out Color secondColor, out Color thirdColor, ColorSets originalColor)
        {
            if (brightness >= 0.5f)
            {
                var t = (brightness - 0.5f) * 2f;
                primaryColor = new Color(
                    Mathf.Lerp(originalColor.primaryColor.r, 1f, t),
                    Mathf.Lerp(originalColor.primaryColor.g, 1f, t),
                    Mathf.Lerp(originalColor.primaryColor.b, 1f, t),
                    originalColor.primaryColor.a);
                secondColor = new Color(
                    Mathf.Lerp(originalColor.secondColor.r, 1f, t),
                    Mathf.Lerp(originalColor.secondColor.g, 1f, t),
                    Mathf.Lerp(originalColor.secondColor.b, 1f, t),
                    originalColor.secondColor.a);
                thirdColor = new Color(
                    Mathf.Lerp(originalColor.thirdColor.r, 1f, t),
                    Mathf.Lerp(originalColor.thirdColor.g, 1f, t),
                    Mathf.Lerp(originalColor.thirdColor.b, 1f, t),
                    originalColor.thirdColor.a);
            }
            else
            {
                var t = brightness * 2f;
                primaryColor = new Color(
                    Mathf.Lerp(0f, originalColor.primaryColor.r, t),
                    Mathf.Lerp(0f, originalColor.primaryColor.g, t),
                    Mathf.Lerp(0f, originalColor.primaryColor.b, t),
                    originalColor.primaryColor.a);
                secondColor = new Color(
                    Mathf.Lerp(0f, originalColor.secondColor.r, t),
                    Mathf.Lerp(0f, originalColor.secondColor.g, t),
                    Mathf.Lerp(0f, originalColor.secondColor.b, t),
                    originalColor.secondColor.a);
                thirdColor = new Color(
                    Mathf.Lerp(0f, originalColor.thirdColor.r, t),
                    Mathf.Lerp(0f, originalColor.thirdColor.g, t),
                    Mathf.Lerp(0f, originalColor.thirdColor.b, t),
                    originalColor.thirdColor.a);
            }
        }
    }
}