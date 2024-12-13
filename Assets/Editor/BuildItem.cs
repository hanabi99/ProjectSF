using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

namespace SFBuild
{
    /// <summary>
    /// TODO: 之后优化IOS Android 打包Editor代码 现在仅作测试
    /// </summary>
    public static class BuildItem
    {
        public static string m_AndroidPath => Application.dataPath + "/../BuildTarget/Android/SF.apk";
        private static string xCodeOutPutPath => Application.dataPath + "/../BuildTarget/IOS/";
    
        [MenuItem("Build/Android")]
        public static void Build()
        {
            CreateWorkFolder(m_AndroidPath);
            
            Debug.Log(m_AndroidPath);
            //版本
            PlayerSettings.bundleVersion = GetJenkinsParameter("version") == "1" ? "0.1.0": GetJenkinsParameter("version");
            //打包次数
            PlayerSettings.iOS.buildNumber = GetJenkinsParameter("buildNum");
            //包名
            PlayerSettings.applicationIdentifier = GetJenkinsParameter("bundleName") == "1" ? "com.hanabi.ProjectSF" : GetJenkinsParameter("bundleName");
            
            BuildPipeline.BuildPlayer(FindEnableEditorrScenes(), m_AndroidPath, BuildTarget.Android, BuildOptions.None);
        }
        /// <summary>
        ///解释jekins 传输的参数
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static string GetJenkinsParameter(string name)
        {
            foreach(var arg in Environment.GetCommandLineArgs())
            {
                if (arg.StartsWith(name))
                {
                    return arg.Split("-"[0])[1];
                }
            }
            return "1";
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
