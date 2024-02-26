using System.Collections.Generic;
using UnityEngine;

namespace OnlyNew.CharacterBoom
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class PoseSaver : MonoBehaviour
    {
        public Transform rootBone;
        public PosePreset preset;

        private void Awake()
        {
            if (rootBone == null)
            {
                List<Transform> transforms = new List<Transform>();
                Dictionary<string, Transform> dict = new Dictionary<string, Transform>();
                GetComponentsInChildren<Transform>(true, transforms);
                for (int i = 1; i < transforms.Count; i++)
                {
                    dict.TryAdd(transforms[i].gameObject.name, transforms[i]);
                }
                if (dict.Count > 0)
                {
                    dict.TryGetValue("root", out rootBone);
                }
            }
        }
    }
}