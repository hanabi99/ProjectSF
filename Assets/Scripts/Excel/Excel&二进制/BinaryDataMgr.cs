using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Data;
using System;
using System.Reflection;
using System.Text;

/// <summary>
/// 2进制数据管理器
/// </summary>
public class BinaryDataMgr
{
    private static BinaryDataMgr instance = new BinaryDataMgr();
    public static BinaryDataMgr Instance => instance;

    /// <summary>
    /// 管理表
    /// </summary>
    public Dictionary<string, object> tableDic = new Dictionary<string, object>();

    /// <summary>
    /// 数据存储的位置
    /// </summary>
    private static string SAVE_PATH = Application.persistentDataPath + "/Data/";

    /// <summary>
    /// 二进制生成地址
    /// </summary>
    public static string INITBINART_PATH = Application.streamingAssetsPath + "/ExcelBinaryData/";

    private BinaryDataMgr()
    {

    }


    public void InitData()
    {
        LoadTable<PlayerInfoContainer, PlayerInfo>();
        LoadTable<EnemyInfoContainer, EnemyInfo>();
    }

    public T GetTable<T>() where T : class
    {
        string tablename = typeof(T).Name;
        if (tableDic.ContainsKey(tablename))
        {
            return tableDic[tablename] as T;
        }

        return null;
    }
    /// <summary>
    /// 加载Excel表
    /// </summary>
    /// <typeparam name="T">容器类</typeparam>
    /// <typeparam name="K">数据结构类</typeparam>
    public void LoadTable<T, K>()
    {
        using (FileStream fs = File.Open(INITBINART_PATH + typeof(K).Name + ".Binary", FileMode.Open, FileAccess.Read))
        {
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            int index = 0;
            //读取一共有多少行数据
            int RowCount = BitConverter.ToInt32(bytes, index);
            index += 4;
            //读取主键名字
            int KeyNameLength = BitConverter.ToInt32(bytes, index);
            index += 4;
            string KeyName = Encoding.UTF8.GetString(bytes, index, KeyNameLength);
            index += KeyNameLength;

            //创建容器类对象
            Type contanierType = typeof(T);

            object contanierObj = Activator.CreateInstance(contanierType);

            //创建数据结构类
            Type DataType = typeof(K);
            //得到该类的所有变量
            FieldInfo[] infos = DataType.GetFields();

            //读取每一行的信息
            for (int i = 0; i < RowCount; i++)
            {
                object DataObj = Activator.CreateInstance(DataType);
                foreach (var info in infos)
                {
                    if (info.FieldType == typeof(int))
                    {
                        info.SetValue(DataObj, BitConverter.ToInt32(bytes, index));
                        index += 4;
                    }
                    else if (info.FieldType == typeof(float))
                    {
                        info.SetValue(DataObj, BitConverter.ToSingle(bytes, index));
                        index += 4;
                    }
                    else if (info.FieldType == typeof(bool))
                    {
                        info.SetValue(DataObj, BitConverter.ToBoolean(bytes, index));
                        index += 1;
                    }
                    else if (info.FieldType == typeof(string))
                    {
                        int Len = BitConverter.ToInt32(bytes, index);
                        index += 4;
                        info.SetValue(DataObj,Encoding.UTF8.GetString(bytes, index, Len));
                        index += Len;
                    }
                }
                //获取容器中的字典对象
                object dicObject = contanierType.GetField("dataDic").GetValue(contanierObj);
                //字典对象得到ADD方法
                MethodInfo methodInfo =  dicObject.GetType().GetMethod("Add");
                //得到数据结构类 的主键的值
                object KeyValue = DataType.GetField(KeyName).GetValue(DataObj);

                methodInfo.Invoke(dicObject, new object[] { KeyValue ,DataObj});
            }

            tableDic.Add(typeof(T).Name, contanierObj);

            fs.Close();
        }
    }

    /// <summary>
    /// 存储类对象数据
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="fileName"></param>
    public void Save(object obj, string fileName)
    {
        //先判断路径文件夹有没有
        if (!Directory.Exists(SAVE_PATH))
            Directory.CreateDirectory(SAVE_PATH);

        using (FileStream fs = new FileStream(SAVE_PATH + fileName + ".tang", FileMode.OpenOrCreate, FileAccess.Write))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, obj);
            fs.Close();
        }
    }

    /// <summary>
    /// 读取2进制数据转换成对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public T Load<T>(string fileName) where T : class
    {
        //如果不存在这个文件 就直接返回泛型对象的默认值
        if (!File.Exists(SAVE_PATH + fileName + ".tang"))
            return default(T);

        T obj;
        using (FileStream fs = File.Open(SAVE_PATH + fileName + ".tang", FileMode.Open, FileAccess.Read))
        {
            BinaryFormatter bf = new BinaryFormatter();
            obj = bf.Deserialize(fs) as T;
            fs.Close();
        }

        return obj;
    }
}
