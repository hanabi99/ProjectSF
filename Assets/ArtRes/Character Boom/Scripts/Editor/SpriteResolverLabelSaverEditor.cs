using OnlyNew.CharacterBoom.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D.Animation;
namespace OnlyNew.CharacterBoom
{
    [CustomEditor(typeof(SpriteResolverLabelSaver))]
    [CanEditMultipleObjects]
    public class SpriteResolverLabelSaverEditor : Editor
    {

        Dictionary<string, SpriteResolver> resolverDict = new Dictionary<string, SpriteResolver>();
        List<SpriteResolver> spriteResolvers = new List<SpriteResolver>();
        GUIContent presetContent = new GUIContent("Label Preset", "The path of new clip.");


        GUIContent newButtonContent = new GUIContent("New", "Create new SpriteResolverLabelPreset and store it on drive");
        GUIContent saveButtonContent = new GUIContent("Save Asset", "Save label to Asset");
        GUIContent loadButtonContent = new GUIContent("Load Asset", "Load label to Asset");
        SerializedProperty presetProperty, skinProperty, faceProperty, hairProperty, clothProperty;

        public void OnEnable()
        {
            presetProperty = serializedObject.FindProperty("preset");
            skinProperty = serializedObject.FindProperty("skin");
            faceProperty = serializedObject.FindProperty("face");
            hairProperty = serializedObject.FindProperty("hair");
            clothProperty = serializedObject.FindProperty("cloth");
            SpriteResolverLabelSaver t = target as SpriteResolverLabelSaver;

            t.GetComponentsInChildren<SpriteResolver>(true, spriteResolvers);
            foreach (var item in spriteResolvers)
            {
                resolverDict.Add(item.gameObject.name, item);
            }
            Undo.RecordObjects(spriteResolvers.ToArray(), "SpriteResolverlabelSaver");
        }



        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            Undo.RecordObjects(targets, "SpriteResolverLabelSaver");
            serializedObject.Update();
            var tg = target as SpriteResolverLabelSaver;
            EditorGUILayout.BeginHorizontal();
            //tg.preset = EditorGUILayout.ObjectField(presetContent, tg.preset, typeof(SpriteResolverLabelPreset), false) as SpriteResolverLabelPreset;
            EditorGUILayout.PropertyField(presetProperty, presetContent);

            if (GUILayout.Button(newButtonContent, GUILayout.Width(50f)))
            {
                foreach (var tar in targets)
                {
                    SpriteResolverLabelSaver t = tar as SpriteResolverLabelSaver;
                    if (t != null)
                    {
                        try
                        {
                            string dirPath = "Assets/Character Boom/Presets/Sprite Resolver Labels/";
                            string filename = t.gameObject.name + " labels";
                            SpriteResolverLabelPreset asset = ScriptableObject.CreateInstance<SpriteResolverLabelPreset>();

                            AssetCreator.CreateAssetWithCheck<SpriteResolverLabelPreset>(dirPath, filename, asset);
                            if (asset != null)
                            {
                                t.preset = asset;
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

            tg.foldout = EditorGUILayout.BeginFoldoutHeaderGroup(tg.foldout, "Filter");
            if (tg.foldout)
            {
                EditorGUILayout.PropertyField(skinProperty);
                EditorGUILayout.PropertyField(hairProperty);
                EditorGUILayout.PropertyField(faceProperty);
                EditorGUILayout.PropertyField(clothProperty);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.BeginVertical();

            if (GUILayout.Button(saveButtonContent, GUILayout.Height(30f)))
            {
                foreach (var tar in targets)
                {
                    SpriteResolverLabelSaver t = tar as SpriteResolverLabelSaver;

                    t.GetComponentsInChildren<SpriteResolver>(true, spriteResolvers);
                    SaveAsset(t);
                }
            }
            if (GUILayout.Button(loadButtonContent, GUILayout.Height(30f)))
            {


                foreach (var tar in targets)
                {
                    SpriteResolverLabelSaver t = tar as SpriteResolverLabelSaver;

                    t.GetComponentsInChildren<SpriteResolver>(true, spriteResolvers);
                    Undo.RecordObjects(spriteResolvers.ToArray(), t.gameObject.name);
                    LoadAsset(t);
                }
            }
            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }





        void SaveAsset(SpriteResolverLabelSaver t)
        {
            SetterToAsset(t);
        }
        public void SetterToAsset(SpriteResolverLabelSaver t)
        {
            if (t.preset != null)
            {
                t.preset.Clear();
                foreach (var spriteResolver in spriteResolvers)
                {
                    if (spriteResolver.GetLabel() != null)
                    {
                        t.preset.Add(spriteResolver.gameObject.name, spriteResolver.GetLabel());
                    }
                    else
                    {
                        var filename = GetNameFromAssetName(spriteResolver.gameObject);
                        if (filename != null)
                        {
                            t.preset.Add(spriteResolver.gameObject.name, filename);
                        }
                        else
                        {
                            Debug.LogError("Could't get label.");
                        }
                    }
                }
                UnityEditor.EditorUtility.SetDirty(t.preset);
            }
        }
        string GetNameFromAssetName(GameObject t)
        {
            var spriteRenderer = t.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                var sprite = spriteRenderer.sprite;
                if (sprite != null)
                {
                    var path = AssetDatabase.GetAssetOrScenePath(sprite);
                    var filename = System.IO.Path.GetFileNameWithoutExtension(path);
                    return filename;

                }
            }
            return null;
        }
        void LoadAsset(SpriteResolverLabelSaver t)
        {
            if (t.preset != null)
            {
                string label;
                Undo.RecordObjects(spriteResolvers.ToArray(), "Load labels");
                foreach (var spriteResolver in spriteResolvers)
                {
                    var name = spriteResolver.gameObject.name;
                    if (!t.skin)
                    {
                        if (GroupSettings.skin.Contains(name))
                            continue;
                    }
                    if (!t.face)
                    {
                        if (GroupSettings.face.Contains(name))
                            continue;
                    }
                    if (!t.cloth)
                    {
                        if (GroupSettings.cloth.Contains(name))
                            continue;
                    }
                    if (!t.hair)
                    {
                        if (GroupSettings.hair.Contains(name))
                            continue;
                    }
                    if (t.preset.TryGetValue(name, out label))
                        spriteResolver.SetCategoryAndLabel(name, label);

                }
            }
        }
    }
}