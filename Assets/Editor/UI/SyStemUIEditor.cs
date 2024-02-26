using System.Collections;
using System.Collections.Generic;
using U3DExtends;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class SyStemUIEditor : Editor
{
    /// <summary>
    /// 智能禁用RayCastTarget
    /// </summary>
    [InitializeOnLoadMethod]
    private static void InitEditor()
    {
        //监听hierarchy发生改变的委托
        EditorApplication.hierarchyChanged += HanderTextOrImageRaycast;
        EditorApplication.hierarchyChanged += LoadWindowCamera;
    }
    private static void HanderTextOrImageRaycast()
    {
        GameObject obj = Selection.activeGameObject;
        if (obj != null)
        {
            if (obj.name.Contains("Text"))
            {
                Text text = obj.GetComponent<Text>();
                if (text != null)
                {
                    text.raycastTarget = false;
                }
            }
            else if (obj.name.Contains("Image"))
            {
                Image image = obj.GetComponent<Image>();
                if (image != null)
                {
                    image.raycastTarget = false;
                }
                else
                {
                    RawImage rawImage = obj.GetComponent<RawImage>();
                    if (rawImage != null)
                    {
                        rawImage.raycastTarget = false;
                    }
                }
            }

        }
    }
    /// <summary>
    ///智能添加相机
    /// </summary>
    private static void LoadWindowCamera()
    {
        if (Selection.activeGameObject != null)
        {
            GameObject uiCameraObj = Camera.main.gameObject;
            if (uiCameraObj != null)
            {
                Camera camera = uiCameraObj.GetComponent<Camera>();
                if (Selection.activeGameObject.name.Contains("Window"))
                {
                    Canvas canvas = Selection.activeGameObject.GetComponent<Canvas>();
                    if (canvas != null)
                    {
                        canvas.worldCamera = camera;
                    }
                }
            }
        }
    }
    /// <summary>
    /// 可视化修改添加图集标签
    /// </summary>
    public class SetSpriteAltasTag : EditorWindow
    {
        public static string spritePackingTag = "";
        public static System.Type assetType;
        [MenuItem("Assets/SetSpriteTag")]
        public static void OpenWindow()
        {
            SetSpriteAltasTag window = GetWindowWithRect<SetSpriteAltasTag>(new Rect(0, 0, 600, 300));

            window.Show();

        }
        public static void SetSpriteTag()
        {
            // 获取选中的资源路径
            string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            Debug.Log(assetPath);

            // 获取资源导入器
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);

            if (importer != null)
            {
                // 获取资源类型
                assetType = importer.GetType();
                Debug.Log("类型为:" + assetType);
                if (assetType == typeof(TextureImporter))
                {
                    TextureImporter texture = importer as TextureImporter;
                    texture.spritePackingTag = spritePackingTag;
                }
                else
                {
                    Debug.LogError("不是图片类型");
                }
            }
        }

        private void OnGUI()
        {
            spritePackingTag = GUI.TextField(new Rect(10, 50, 300, 20), spritePackingTag);
            if (GUI.Button(new Rect(10, 110, 150, 30), "确认更改标签"))
            {
                SetSpriteTag();
            }
        }

    }

    [MenuItem("GameObject/优化Batch", false, -2)]
    public static void OptimizationUIBatch()
    {
        UILayoutTool.OptimizeBatchForMenu();
    }
}
