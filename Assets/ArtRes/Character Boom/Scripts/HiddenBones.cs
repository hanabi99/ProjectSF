using System.Collections.Generic;
using UnityEngine;

namespace OnlyNew.CharacterBoom
{
    [ExecuteAlways]
    public class HiddenBones : MonoBehaviour
    {
        public bool hiddenHairBones = false;
        public bool hiddenSkirtBones = false;
        public bool hiddenFaceBones = false;

        [HideInInspector]
        [Header("May cause eye socket disappear")]
        public bool hiddenAllBones = false;

        private string[] hairBones = { "hair_root" };
        private string[] skirtBones = { "skirt_1", "skirt_2", "skirt_3", "skirt_4", "skirt_5" };
        private string[] faceBones = { "eyes", "brow", "nose", "mouth" };
        private string[] allBones = { "root" };
        private List<Transform> children = new List<Transform>();

        private Dictionary<string, GameObject> childrenDict = new Dictionary<string, GameObject>();

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (childrenDict.Count == 0)
            {
                gameObject.GetComponentsInChildren<Transform>(true, children);
                foreach (var item in children)
                {
                    childrenDict.TryAdd(item.gameObject.name, item.gameObject);
                }
            }
        }

        // Start is called before the first frame update
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
        }

        private void Reset()
        {
            Awake();
        }

        private void OnValidate()
        {
            if (childrenDict.Count == 0)
            {
                Initialize();
            }
            GameObject bone;
            foreach (var name in hairBones)
            {
                if (childrenDict.TryGetValue(name, out bone))
                {
                    bone.SetActive(!hiddenHairBones);
                    //ToggleVisibility(bone,!hiddenHairBones);
                }
                else
                {
                    Debug.LogWarning("Can't find bone: " + name + ". Please put HiddenBones component in parent GameObject");
                }
            }
            foreach (var name in skirtBones)
            {
                if (childrenDict.TryGetValue(name, out bone))
                {
                    bone.SetActive(!hiddenSkirtBones);
                }
                else
                {
                    Debug.LogWarning("Can't find bone: " + name + ". Please put HiddenBones component in parent GameObject");
                }
            }
            foreach (var name in faceBones)
            {
                if (childrenDict.TryGetValue(name, out bone))
                {
                    bone.SetActive(!hiddenFaceBones);
                }
                else
                {
                    Debug.LogWarning("Can't find bone: " + name + ". Please put HiddenBones component in parent GameObject");
                }
            }
            foreach (var name in allBones)
            {
                if (childrenDict.TryGetValue(name, out bone))
                {
                    bone.SetActive(!hiddenAllBones);
                }
                else
                {
                    Debug.LogWarning("Can't find bone: " + name + ". Please put HiddenBones component in parent GameObject");
                }
            }
        }
    }
}