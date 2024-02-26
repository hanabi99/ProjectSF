using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MyGameFrameWork
{
    [CreateAssetMenu(fileName = "ABNameConfig", menuName = "ABNameConfig", order = 0)]
    public class ABNameConfig : ScriptableObject
    {
        private string[] ABRootArr = new string[] { "Game", "Hall", "Window" };
        public List<ABObjcet> ABDataList = new List<ABObjcet>();

        public void GeneratorABConfig()
        {

            foreach (var item in ABRootArr)
            {
                //获取预制体文件夹读取路径
                string floder = Application.dataPath + "/Prefab/" + item;
                //获取文件夹下的所有Prefab文件
                string[] filePathArr = Directory.GetFiles(floder, "*.prefab", SearchOption.AllDirectories);
                foreach (var path in filePathArr)
                {
                    if (path.EndsWith(".meta"))
                    {
                        continue;
                    }

                    //获取预制体名字
                    string fileName = Path.GetFileNameWithoutExtension(path);

                    string assetsPath = path.Substring(path.IndexOf("Assets"));

                    AssetImporter importer = AssetImporter.GetAtPath(assetsPath);

                    ABObjcet aBObjcet = new ABObjcet();
                    if (importer.assetBundleName != null)
                    {
                        aBObjcet.ABName = importer.assetBundleName;
                        aBObjcet.ResName = fileName;
                    }
                    ABDataList.Add(aBObjcet);
                }
            }
        }
        /// <summary>
        /// 通过资源名获得AB包名
        /// </summary>
        /// <param name="resname"></param>
        public string GetABName(string resname)
        {
            for (int i = 0; i < ABDataList.Count; i++)
            {
                if (ABDataList[i].ResName == resname)
                {
                    if (ABDataList[i].ABName != "")
                    {
                        return ABDataList[i].ABName;
                    }
                }
            }

            return null;
        }
    }
    [System.Serializable]
    public class ABObjcet
    {
        public string ABName;
        public string ResName;
    }
}
