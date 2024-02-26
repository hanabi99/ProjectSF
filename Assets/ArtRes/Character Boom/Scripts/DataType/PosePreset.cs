using System;
using UnityEditor;
using UnityEngine;
using ScriptableDictionaries;
namespace OnlyNew.CharacterBoom
{
    [CreateAssetMenu(fileName = "PosePreset", menuName = "PosePreset", order = 1)]
    public class PosePreset : ScriptableDictionary<string, PoseData>
    {

    }


    [Serializable]
    public struct PoseData
    {
        public PoseData(Vector3 pos, Quaternion rot, Vector3 scl,int sortingOrder)
        {
            this.relativeLocalPosition = pos;
            this.relativeLocalRotation = rot;
            this.localScale = scl;
            this.sortingOrder = sortingOrder;
        }
        public Vector3 relativeLocalPosition;
        public Quaternion relativeLocalRotation;
        public Vector3 localScale;
        public int sortingOrder;
    }
}