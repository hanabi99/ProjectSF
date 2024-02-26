using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
[CreateAssetMenu(fileName = "LoadPrefabPathConfig", menuName = "LoadPrefabPathConfig", order = 0)]
public class LoadPrefabPathConfig : ScriptableObject
{
    private string[] panelRootArr = new string[] { "Game","Hall","Window" };
    public List<PanelData> panelDataList = new List<PanelData>();

    public void GeneratorPanelConfig()
    {
        //检测预制体有没有新增，如果没有就不需要生成配置
        int count = 0;
        foreach (var item in panelRootArr)
        {
            string[] filePathArr =  Directory.GetFiles(Application.dataPath + "/Prefab/" + item, "*.prefab", SearchOption.AllDirectories);
            foreach (var path in filePathArr)
            {
                if (path.EndsWith(".meta"))
                {
                    continue;
                }
                count += 1;
            }
        }
        if (count== panelDataList.Count)
        {
            Debug.Log("预制体个数没有发生改变，不生成窗口配置");
            return;
        }

        panelDataList.Clear();
        foreach (var item in panelRootArr)
        {
            //获取预制体文件夹读取路径
            string floder = Application.dataPath + "/Prefab/" + item;
            //获取文件夹下的所有Prefab文件
            string[] filePathArr = Directory.GetFiles(floder,"*.prefab",SearchOption.AllDirectories);
            foreach (var path in filePathArr)
            {
                if (path.EndsWith(".meta"))
                {
                    continue;
                }
                //获取预制体名字
                string fileName = Path.GetFileNameWithoutExtension(path);
                //计算文件读取路径 
                string filePath = item + "/" + fileName;
                PanelData data = new PanelData { name = fileName, path = filePath };
                panelDataList.Add(data);
            }
        }
    }
    public string GetPanelPath(string wndName)
    {
        foreach (var item in panelDataList)
        {
            if (string.Equals(item.name,wndName))
            {
                return item.path;
            }
        }
        Debug.LogError(wndName+"不存在在配置文件中，请检查预制体存放位置，或配置文件");
        return "";
    }
}
[System.Serializable]
public class PanelData
{
    public string name;
    public string path;
}