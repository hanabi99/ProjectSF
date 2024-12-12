using OnlyNew.CharacterBoom.Utilities;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OnlyNew.CharacterBoom
{
    [CustomEditor(typeof(ColorSetter))]
    [CanEditMultipleObjects]
    public class ColorSetterEditor : UnityEditor.Editor
    {
        private List<SetMatColor> setters = new List<SetMatColor>();
        private Dictionary<string, SetMatColorGUI> settersGUIDict = new Dictionary<string, SetMatColorGUI>();

        private List<SetMatColorGUI> settersSkin = new List<SetMatColorGUI>();
        private List<SetMatColorGUI> settersHair = new List<SetMatColorGUI>();
        private List<SetMatColorGUI> settersFace = new List<SetMatColorGUI>();
        private List<SetMatColorGUI> settersCloth = new List<SetMatColorGUI>();
        private List<SetMatColorGUI> settersOthers = new List<SetMatColorGUI>();
        private int hash = 0;
        private static bool showSkin = true;
        private static bool showHair = true;
        private static bool showCloth = true;
        private static bool showFace = true;
        private static bool showOthers = true;

        private List<bool> setterFoldouts = new List<bool>();
        private int setterCount = 0;
        SerializedProperty originalSpriteBlendMatProperty;
        SerializedProperty spriteBlendMatProperty;
        SerializedProperty shapeBlendMatProperty;
        SerializedProperty characterBoomAssetProperty;
        GUIContent newButtonContent = new GUIContent("New", "Creat color preset and store it on drive");
        GUIContent saveButtonContent = new GUIContent("Save Asset", "Save Colors to Asset");
        //static bool advance = false;

        private class SetMatColorGUI
        {
            public SetMatColorGUI(SetMatColor t_setter)
            {
                setter = t_setter;
                foldout = false;
            }

            public SetMatColor setter;
            public bool foldout;
        }

        public void OnEnable()
        {
            ColorSetter t = target as ColorSetter;
            var renderers = t.GetComponentsInChildren<SpriteRenderer>();
            originalSpriteBlendMatProperty = serializedObject.FindProperty("OriginSpriteBlendMat");
            spriteBlendMatProperty = serializedObject.FindProperty("SpriteBlendMat");
            shapeBlendMatProperty = serializedObject.FindProperty("ShapeBlendMat");
            characterBoomAssetProperty = serializedObject.FindProperty("characterBoomAsset");
            foreach (var renderer in renderers)
            {
                if (renderer.GetComponent<SetMatColor>() == null)
                {
                    renderer.gameObject.AddComponent<SetMatColor>();
                }
            }
            t.GetComponentsInChildren(true, setters);
            //foreach (var setter in setters)
            //{
            //    t.colorDict.Clear();
            //    t.colorDict.Add(setter.gameObject.name,new ColorSets(setter.PrimaryColor,setter.SecondColor,setter.ThirdColor));
            //}
            t.InitialMaterial();
            //t.InitializeAsset();

            Undo.RecordObjects(setters.ToArray(), "ColorSetters");
        }

        public override void OnInspectorGUI()
        {
            Undo.RecordObjects(targets, "ColorSetter");
            serializedObject.Update();
            EditorGUILayout.PropertyField(originalSpriteBlendMatProperty);
            EditorGUILayout.PropertyField(spriteBlendMatProperty);
            EditorGUILayout.PropertyField(shapeBlendMatProperty);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(characterBoomAssetProperty);
            if (GUILayout.Button(newButtonContent, GUILayout.Width(50f)))
            {
                foreach (var tar in targets)
                {
                    ColorSetter t1 = tar as ColorSetter;
                    if (t1 != null)
                    {
                        try
                        {
                            string dirPath = "Assets/Character Boom/Presets/Colors/";
                            string filename = t1.gameObject.name + " Colors";
                            CharacterBoomColorPreset asset = ScriptableObject.CreateInstance<CharacterBoomColorPreset>();

                            AssetCreator.CreateAssetWithCheck<CharacterBoomColorPreset>(dirPath, filename, asset);
                            if (asset != null)
                            {
                                t1.characterBoomAsset = asset;
                            }
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }
                }
            }

            EditorGUILayout.EndHorizontal();
            ColorSetter t = target as ColorSetter;

            Undo.RecordObjects(setters.ToArray(), "ColorSetters");
            settersGUIDict.Clear();

            foreach (var setter in setters)
            {
                var gui = new SetMatColorGUI(setter);
                if (!settersGUIDict.ContainsKey(gui.setter.gameObject.name))
                {
                    settersGUIDict.Add(gui.setter.gameObject.name, gui);
                }
            }
            int currentHash = target.GetHashCode();
            if (hash != currentHash)
            {
                hash = currentHash;
                ClearGroup();
                PickSettersToGroups(t);
            }
            t.advance = EditorGUILayout.ToggleLeft("Advance", t.advance);

            if (t.advance)
            {
                DrawAdvanceOptions();
            }
            else
            {
                DrawEasyOptions();
            }
            EditorGUILayout.BeginVertical();

            if (GUILayout.Button(saveButtonContent, GUILayout.Height(30f)))
            {
                foreach (var tar in targets)
                {
                    ColorSetter tget = tar as ColorSetter;
                    tget.GetComponentsInChildren(true, setters);
                    SaveAsset(tget);
                }
            }
            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawEasyOptions()
        {
            setterCount = 0;
            showSkin = EditorGUILayout.Foldout(showSkin, "Skin", true, EditorStyles.foldoutPreDrop);
            if (showSkin)
            {
                if (settersSkin.Count > 0)
                {
                    if (settersSkin[0] != null)
                    {
                        settersSkin[0].setter.PrimaryColor = EditorGUILayout.ColorField("Skin Basic Color", settersSkin[0].setter.PrimaryColor);
                        settersSkin[0].setter.SecondColor = EditorGUILayout.ColorField("Skin Shadow Color 1", settersSkin[0].setter.SecondColor);
                        settersSkin[0].setter.ThirdColor = EditorGUILayout.ColorField("Skin Shadow Color 2", settersSkin[0].setter.ThirdColor);
                        foreach (SetMatColorGUI setterGUI in settersSkin)
                        {
                            if (setterGUI == null)
                            {
                                Debug.LogError("error, setter == null");
                            }
                            else
                            {
                                setterGUI.setter.PrimaryColor = settersSkin[0].setter.PrimaryColor;
                                setterGUI.setter.SecondColor = settersSkin[0].setter.SecondColor;
                                setterGUI.setter.ThirdColor = settersSkin[0].setter.ThirdColor;
                            }
                        }
                    }
                }
            }
            EditorGUILayout.Space();

            showHair = EditorGUILayout.Foldout(showHair, "Hair", true, EditorStyles.foldoutPreDrop);
            if (showHair)
            {
                if (settersHair.Count > 0)
                {
                    if (settersHair[0] != null)
                    {
                        //use Front hair color replace all color of hair  when advance option is false
                        settersHair[0].setter.PrimaryColor = EditorGUILayout.ColorField("Hair Basic Color", settersHair[0].setter.PrimaryColor);
                        settersHair[0].setter.SecondColor = EditorGUILayout.ColorField("Hair Shadow Color 1", settersHair[0].setter.SecondColor);
                        settersHair[0].setter.ThirdColor = EditorGUILayout.ColorField("Hair Shadow Color 2", settersHair[0].setter.ThirdColor);
                        foreach (SetMatColorGUI setterGUI in settersHair)
                        {
                            if (setterGUI == null)
                            {
                                Debug.LogError("error, setter == null");
                            }
                            else
                            {
                                setterGUI.setter.PrimaryColor = settersHair[0].setter.PrimaryColor;
                                setterGUI.setter.SecondColor = settersHair[0].setter.SecondColor;
                                setterGUI.setter.ThirdColor = settersHair[0].setter.ThirdColor;
                            }
                        }
                    }
                }
            }
            EditorGUILayout.Space();

            showFace = EditorGUILayout.Foldout(showFace, "Face", true, EditorStyles.foldoutPreDrop);
            if (showFace)
            {
                DrawSetters(settersFace);
            }
            EditorGUILayout.Space();

            showCloth = EditorGUILayout.Foldout(showCloth, "Cloth", true, EditorStyles.foldoutPreDrop);
            if (showCloth)
            {
                DrawSetters(settersCloth);
            }
            EditorGUILayout.Space();

            showOthers = EditorGUILayout.Foldout(showOthers, "Others", true, EditorStyles.foldoutPreDrop);
            if (showOthers)
            {
                DrawSetters(settersOthers);
            }
            EditorGUILayout.Space();
        }

        private void DrawAdvanceOptions()
        {
            setterCount = 0;
            showSkin = EditorGUILayout.Foldout(showSkin, "Skin", true, EditorStyles.foldoutPreDrop);
            if (showSkin)
            {
                DrawSetters(settersSkin);
            }
            EditorGUILayout.Space();

            showHair = EditorGUILayout.Foldout(showHair, "Hair", true, EditorStyles.foldoutPreDrop);
            if (showHair)
            {
                DrawSetters(settersHair);
            }
            EditorGUILayout.Space();

            showFace = EditorGUILayout.Foldout(showFace, "Face", true, EditorStyles.foldoutPreDrop);
            if (showFace)
            {
                DrawSetters(settersFace);
            }
            EditorGUILayout.Space();

            showCloth = EditorGUILayout.Foldout(showCloth, "Cloth", true, EditorStyles.foldoutPreDrop);
            if (showCloth)
            {
                DrawSetters(settersCloth);
            }
            EditorGUILayout.Space();

            showOthers = EditorGUILayout.Foldout(showOthers, "Others", true, EditorStyles.foldoutPreDrop);
            if (showOthers)
            {
                DrawSetters(settersOthers);
            }
            EditorGUILayout.Space();
        }

        private void DrawSetters(List<SetMatColorGUI> settersGUI)
        {
            for (int i = 0; i < settersGUI.Count; i++)
            {
                var setter = settersGUI[i].setter;
                if (setter == null)
                {
                    Debug.LogError("error, setter == null");
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    setterFoldouts[setterCount] = EditorGUILayout.Foldout(setterFoldouts[setterCount], setter.gameObject.name, true);
                    if (setterFoldouts[setterCount])
                    {
                        setter.PrimaryColor = EditorGUILayout.ColorField(setter.PrimaryColor);
                        EditorGUILayout.EndHorizontal();

                        setter.SecondColor = EditorGUILayout.ColorField("SecondColor", setter.SecondColor);
                        setter.ThirdColor = EditorGUILayout.ColorField("ThirdColor", setter.ThirdColor);
                        EditorGUILayout.Space();
                    }
                    else
                    {
                        setter.PrimaryColor = EditorGUILayout.ColorField(setter.PrimaryColor);
                        EditorGUILayout.EndHorizontal();
                    }
                }
                setterCount++;
            }
        }

        private void ClearGroup()
        {
            settersSkin.Clear();
            settersHair.Clear();
            settersFace.Clear();
            settersCloth.Clear();
            settersOthers.Clear();
            setterFoldouts.Clear();
        }

        private void PickSettersToGroups(ColorSetter t)
        {
            PickSettersToGroup(GroupSettings.skin, settersSkin);
            PickSettersToGroup(GroupSettings.hair, settersHair);
            PickSettersToGroup(GroupSettings.face, settersFace);
            PickSettersToGroup(GroupSettings.cloth, settersCloth);
            settersOthers.AddRange(settersGUIDict.Values);
            for (int i = 0; i < settersGUIDict.Count; i++)
            {
                setterFoldouts.Add(false);
            }
        }

        private void PickSettersToGroup(string[] category, List<SetMatColorGUI> groupedSetters)
        {
            foreach (var str in category)
            {
                //var setterGUI = settersGUI.Find(x => x.setter.gameObject.name.EndsWith(str));
                SetMatColorGUI setterGUI;
                if (settersGUIDict.TryGetValue(str, out setterGUI))
                {
                    groupedSetters.Add(setterGUI);
                    settersGUIDict.Remove(str);
                    //settersGUI.Remove(setterGUI);
                    setterFoldouts.Add(false);
                }
                else
                {
                    Debug.LogWarning("not found " + str);
                }
            }
        }

        private void SaveAsset(ColorSetter t)
        {
            SetterToAsset(t);
        }

        public void SetterToAsset(ColorSetter t)
        {
            if (t.characterBoomAsset != null)
            {
                t.characterBoomAsset.Clear();
                foreach (var setter in setters)
                {
                    t.characterBoomAsset.Add(setter.gameObject.name,
                        new ColorSets(setter.PrimaryColor, setter.SecondColor, setter.ThirdColor));
                }
                UnityEditor.EditorUtility.SetDirty(t.characterBoomAsset);
            }
        }
    }
}