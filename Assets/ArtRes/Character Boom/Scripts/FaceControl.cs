#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace OnlyNew.CharacterBoom
{
    /// <summary>
    /// This component corrects the “eye_socket”bone position according to the pivot "eyes_pivot" when zooming.
    /// If you want to set eye_socket mannually , please remove this componnet.
    /// </summary>
    [ExecuteAlways]
    public class FaceControl : MonoBehaviour
    {
        [HideInInspector]
        public Transform eye_socket, eyes, mouth, nose, brow, eyelashes, head;
        public Transform eyesPivot;

        //these are all localPosition
        public readonly Vector3 eyeSocketPosition = new Vector3(1.42251f, -0.7019163f, 0f);
        public Vector3 eyesPivotPosition = new Vector3(0.95f, -0.973f, 0f);

        public readonly Vector3 eyesPosition = new Vector3(1.271355f, -1.029734f, 0f);
        public readonly Vector3 mouthPosition = new Vector3(0.2239994f, -1.455769f, 0f);
        public readonly Vector3 nosePosition = new Vector3(0.6319644f, -1.542712f, 0f);
        public readonly Vector3 browPosition = new Vector3(2.280807f, -0.7154566f, 0f);
        Vector3 eyeSocketAndPivotDistance;
        Dictionary<string, Transform> dict = new Dictionary<string, Transform>();
        [HideInInspector]
        public float browOffset, noseOffset, mouthOffset, eyesClose = 1f, eyeSocketOffset;
        [HideInInspector]
        public Vector2 eyesOffset;
        private void Awake()
        {
            var children = GetComponentsInChildren<Transform>();
            dict.Clear();
            foreach (var child in children)
            {
                dict.Add(child.gameObject.name, child);
            }

            dict.TryGetValue("head", out head);
            dict.TryGetValue("eye_socket", out eye_socket);
            dict.TryGetValue("eyes", out eyes);
            dict.TryGetValue("nose", out nose);
            dict.TryGetValue("mouth", out mouth);
            dict.TryGetValue("brow", out brow);
            dict.TryGetValue("Eyelashes", out eyelashes);
            if (head != null)
            {
                if (!dict.ContainsKey("eyes_pivot"))
                {
                    var s = new GameObject("eyes_pivot");
                    s.transform.parent = dict["head"].transform;
                    s.transform.position = head.TransformPoint(eyesPivotPosition);
                    if (eye_socket != null)
                        s.transform.localRotation = eye_socket.localRotation;
                    s.transform.localScale = Vector3.one;
                    eyesPivot = s.transform;
                }
                else
                {
                    eyesPivot = dict["eyes_pivot"];
                    if (eye_socket != null)
                        eyesPivot.localRotation = eye_socket.localRotation;
                }
            }

        }
        // Start is called before the first frame update
        private void OnValidate()
        {
        }

        // Update is called once per frame
        void LateUpdate()
        {
            UpdateFace();
        }
        public void UpdateFace()
        {
            if (head != null)
            {
                var offset = head.position;
                if (brow != null)
                {
                    brow.position = head.TransformPoint(new Vector3(browPosition.x + browOffset, browPosition.y, browPosition.z));
                }
                if (nose != null)
                {
                    nose.position = head.TransformPoint(new Vector3(nosePosition.x + noseOffset, nosePosition.y, nosePosition.z));
                }

                if (mouth != null)
                {
                    mouth.position = head.TransformPoint(new Vector3(mouthPosition.x + mouthOffset, mouthPosition.y, mouthPosition.z));
                }
                if (eye_socket != null)
                {
                    eye_socket.position = head.TransformPoint(new Vector3(eyeSocketPosition.x + eyeSocketOffset, eyeSocketPosition.y, eyeSocketPosition.z));
                }
                if (eyes != null)
                {
                    eyes.position = head.TransformPoint(new Vector3(eyesPosition.x + eyesOffset.x + eyeSocketOffset, eyesPosition.y + eyesOffset.y, mouthPosition.z));
                }
                if (eye_socket != null)
                {
                    eye_socket.localScale = new Vector3(eyesClose, eye_socket.localScale.y, eye_socket.localScale.z);
                }
                if (eyesPivot != null && eye_socket != null)
                {

                    eyesPivot.position = head.TransformPoint(new Vector3(eyesPivotPosition.x + eyeSocketOffset, eyesPivotPosition.y, eyesPivotPosition.z));


                    var eyesPivotLocalPosition = head.InverseTransformPoint(eyesPivot.position);
                    eyeSocketAndPivotDistance = eyeSocketPosition + new Vector3(eyeSocketOffset, 0, 0) - eyesPivotLocalPosition;
                    eye_socket.position = head.TransformPoint(new Vector3(eye_socket.localScale.x * eyeSocketAndPivotDistance.x + eyesPivotLocalPosition.x, eye_socket.localScale.y * eyeSocketAndPivotDistance.y + eyesPivotLocalPosition.y, eyeSocketPosition.z));
                }
                else if (eye_socket != null)
                {
                    eye_socket.position = head.TransformPoint(new Vector3(eyeSocketPosition.x + eyeSocketOffset, eyeSocketPosition.y, eyeSocketPosition.z));
                }
            }
        }
    }
}
#endif