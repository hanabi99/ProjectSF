using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEngine;
using UnityEngine.Windows;

namespace SFBuild
{
    /// <summary>
    /// TODO: 之后优化IOS Android 打包Editor代码 现在仅作测试
    /// </summary>
    public static class BuildItem
    {
 
        public static string m_AndroidPath = Application.dataPath + "/../BuildTarget/Android/ProjectSF.apk";
        private static string xCodeOutPutPath = Application.dataPath + "/../BuildTarget/IOS/";
    
        [MenuItem("Build/Android")]
        public static void BuildApk()
        {
            CreateWorkFolder(m_AndroidPath);
            Debug.Log(m_AndroidPath);
            SetParameter();
            BuildPipeline.BuildPlayer(FindEnableEditorrScenes(), m_AndroidPath, BuildTarget.Android, BuildOptions.None);
            Debug.Log("Build App Done!");
        }
        
        /// <summary>
        ///解释jekins 传输的参数
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static void SetParameter()
        {
            string[] args = System.Environment.GetCommandLineArgs();
            foreach (var s in args)
            {
                if (s.Contains("--version:"))
                {
                    var version= s.Split(':')[1];
                    // 设置app名字
                    PlayerSettings.bundleVersion = version;
                }

                if (s.Contains("--buildNum:"))
                {
                    var buildNum = s.Split(':')[1];
                    // 设置版本号
                    PlayerSettings.Android.bundleVersionCode = int.Parse(buildNum);
                }
                if (s.Contains("--bundleName:"))
                {
                    var bundleName = s.Split(':')[1];
                    // 设置版本号
                    PlayerSettings.applicationIdentifier = bundleName;
                }
            }
        }
    
        /// <summary>
        /// 查找添加到场景列表中到所有激活到场景
        /// </summary>
        /// <returns></returns>
        private static string[] FindEnableEditorrScenes()
        {
            List<string> editorScenes = new List<string>();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (!scene.enabled) continue;
                editorScenes.Add(scene.path);
            }
            return editorScenes.ToArray();
        }
        
        /// <summary>
        /// 清理工作文件夹
        /// </summary>
        /// <param name="folderPath"></param>
        public static void CreateWorkFolder(string folderPath)
        {
            //绝对储存路径
            if (!Directory.Exists(m_AndroidPath))
            {
                Directory.CreateDirectory(m_AndroidPath);;
            }
        }
    }
}
