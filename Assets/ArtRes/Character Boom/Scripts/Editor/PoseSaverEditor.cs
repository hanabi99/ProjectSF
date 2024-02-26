using OnlyNew.CharacterBoom.Utilities;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OnlyNew.CharacterBoom
{
    [CustomEditor(typeof(PoseSaver))]
    [CanEditMultipleObjects]
    public class PoseSaverEditor : Editor
    {

        SpriteRenderer spriteRenderer;
        List<Transform> transforms = new List<Transform>();
        SerializedProperty presetProperty, rootBoneProperty;
        GUIContent saveButtonContent = new GUIContent("Save Asset", "Save Pose to Asset");
        GUIContent loadButtonContent = new GUIContent("Load Asset", "Load Pose to Asset");
        GUIContent newButtonContent = new GUIContent("New", "Creat pose preset and store it on drive");

        public void OnEnable()
        {
            rootBoneProperty = serializedObject.FindProperty("rootBone");
            presetProperty = serializedObject.FindProperty("preset");
            PoseSaver t = target as PoseSaver;
            t.GetComponentsInChildren<Transform>(true, transforms);
            Undo.RecordObjects(transforms.ToArray(), "PoseSaver");
        }



        public override void OnInspectorGUI()
        {
            Undo.RecordObjects(targets, "PoseSaver");
            serializedObject.Update();
            EditorGUILayout.PropertyField(rootBoneProperty);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(presetProperty);
            if (GUILayout.Button(newButtonContent, GUILayout.Width(50f)))
            {
                foreach (var tar in targets)
                {
                    PoseSaver t = tar as PoseSaver;
                    if (t != null)
                    {
                        try
                        {
                            string dirPath = "Assets/Character Boom/Presets/Poses/";
                            string filename = t.gameObject.name + " pose";
                            PosePreset asset = ScriptableObject.CreateInstance<PosePreset>();

                            AssetCreator.CreateAssetWithCheck<PosePreset>(dirPath, filename, asset);
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


            EditorGUILayout.BeginVertical();

            if (GUILayout.Button(saveButtonContent, GUILayout.Height(30f)))
            {
                foreach (var tar in targets)
                {
                    PoseSaver t = tar as PoseSaver;

                    t.GetComponentsInChildren<Transform>(true, transforms);
                    SaveAsset(t);
                }
            }
            if (GUILayout.Button(loadButtonContent, GUILayout.Height(30f)))
            {


                foreach (var tar in targets)
                {
                    PoseSaver t = tar as PoseSaver;

                    t.GetComponentsInChildren<Transform>(true, transforms);
                    Undo.RecordObjects(transforms.ToArray(), t.gameObject.name);
                    LoadAsset(t);
                }
            }
            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();

        }

        void SaveAsset(PoseSaver t)
        {
            SetterToAsset(t);
        }
        public void SetterToAsset(PoseSaver t)
        {
            if (t.preset != null && t.rootBone != null)
            {
                t.preset.Clear();
#if DEBUG_CHARACTER_BOOM
                var btt = new PoseData()
                {

                    relativeLocalPosition = t.transform.position,
                    relativeLocalRotation = t.transform.rotation,
                    localScale = t.transform.localScale,
                    sortingOrder = 0
                };
                t.preset.TryAdd(t.gameObject.name, btt);
#endif
                for (int i = 1; i < transforms.Count; i++)
                {
                    if (transforms[i] != null)
                    {
                        spriteRenderer = transforms[i].GetComponent<SpriteRenderer>();
                        int sortingOrder = 0;
                        if (spriteRenderer != null)
                        {
                            sortingOrder = spriteRenderer.sortingOrder;
                        }
                        var bt = new PoseData()
                        {

                            relativeLocalPosition = t.rootBone.InverseTransformPoint(transforms[i].position),
                            relativeLocalRotation = Quaternion.Inverse(t.rootBone.rotation) * transforms[i].rotation,
                            localScale = transforms[i].localScale,
                            sortingOrder = sortingOrder
                        };
                        t.preset.TryAdd(transforms[i].gameObject.name, bt);
                    }

                }
                UnityEditor.EditorUtility.SetDirty(t.preset);
            }
        }
        void LoadAsset(PoseSaver t)
        {
            if (t.preset != null && t.rootBone != null)
            {
                PoseData trans;
#if DEBUG_CHARACTER_BOOM
                t.preset.TryGetValue(t.gameObject.name, out PoseData transData);
                t.transform.position = transData.relativeLocalPosition;
                t.transform.rotation = transData.relativeLocalRotation;
                t.transform.localScale = transData.localScale;
#endif
                //not override myself
                for (int i = 1; i < transforms.Count; i++)
                {
                    if (t.preset.TryGetValue(transforms[i].gameObject.name, out trans))
                    {
                        transforms[i].position = t.rootBone.TransformPoint(trans.relativeLocalPosition);
                        transforms[i].rotation = t.rootBone.rotation * trans.relativeLocalRotation;
                        transforms[i].localScale = trans.localScale;
                        spriteRenderer = transforms[i].GetComponent<SpriteRenderer>();

                        if (spriteRenderer != null)
                        {
                            Undo.RecordObject(spriteRenderer, "changeSortingOrder");
#if !DEBUG_CHARACTER_BOOM
                            spriteRenderer.sortingOrder = trans.sortingOrder;
#endif
                        }
                    }
                }
            }
        }
    }
}