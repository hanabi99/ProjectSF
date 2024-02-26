using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Excel;
using System.Data;
using System;
using System.Text;

public class ExcelTool
{
    /// <summary>
    /// excel 存放路径
    /// </summary>
    public static string EXCEL_PATH = Application.dataPath + "/ExcelTable/";

    /// <summary>
    /// 数据结构类生成地址
    /// </summary>
    public static string INITDATA_PATH = Application.dataPath + "/Scripts/Excel/ExcelData/";
    /// <summary>
    /// 容器类生成地址
    /// </summary>
    public static string INITCONTAINER_PATH = Application.dataPath + "/Scripts/Excel/ExcelData/";

 

    public static int BEGIN_INDEX = 4;


    [MenuItem("GameTool/Excel/GenerateExcel")]
    private static void GenerateExcelInfo()
    {
        DirectoryInfo dInfo = Directory.CreateDirectory(EXCEL_PATH);

        FileInfo[] fileInfos =  dInfo.GetFiles();

        DataTableCollection tableCollection;

        for (int i = 0; i < fileInfos.Length; i++)
        {
            if(fileInfos[i].Extension != ".xlsx" && fileInfos[i].Extension != ".xls")
            {
                continue;
            }

            using (FileStream fs = fileInfos[i].Open(FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                tableCollection = excelDataReader.AsDataSet().Tables ;
                fs.Close();
            }

            foreach (DataTable item in tableCollection)
            {
                Debug.Log(item.TableName);
                //生成数据结构类
                GenerateExcelTableClass(item);
                //生成容器类
                GenerateExcelContainer(item);
                //生成2进制数据
                GenerateExcelBinary(item);
            }

            AssetDatabase.Refresh();
        }
    }
    public static void GenerateExcelBinary(DataTable table)
    {
        if (!Directory.Exists(BinaryDataMgr.INITBINART_PATH))
        {
            Directory.CreateDirectory(BinaryDataMgr.INITBINART_PATH);
        }
        using (FileStream fs = File.Open(BinaryDataMgr.INITBINART_PATH + table.TableName +".Binary", FileMode.OpenOrCreate, FileAccess.Write))
        {
            //存储一共多少行数据
            fs.Write(BitConverter.GetBytes(table.Rows.Count - 4), 0, 4);
            string keyname = GetVarNameRow(table)[GetKeyIndex(table)].ToString();
            //存储主键变量名
            byte[] bytes = Encoding.UTF8.GetBytes(keyname);
            //存储变量名字符串长度
            fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
            fs.Write(bytes, 0, bytes.Length);

            //遍历所有内容的行 进行2进制写入
            DataRow row;
            DataRow rowtype = GetTypeNameRow(table);
            for (int i = BEGIN_INDEX; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    switch (rowtype[j].ToString())
                    {
                        case "int":
                            fs.Write(BitConverter.GetBytes(int.Parse(row[j].ToString())), 0, 4);
                            break;
                        case "float":
                            fs.Write(BitConverter.GetBytes(float.Parse(row[j].ToString())), 0, 4);
                            break;
                        case "bool":
                            fs.Write(BitConverter.GetBytes(bool.Parse(row[j].ToString())), 0, 1);
                            break;
                        case "string":
                            bytes = Encoding.UTF8.GetBytes(row[j].ToString());
                            fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
                            fs.Write(bytes, 0, bytes.Length);
                            break;
                    }
                }
            }
            fs.Close();
        }
    }
   
    /// <summary>
    /// 生成数据结构类
    /// </summary>
    /// <param name="table"></param>
    private static void GenerateExcelTableClass(DataTable table)
    {
        DataRow rowName = GetVarNameRow(table);
        DataRow typeName = GetTypeNameRow(table);

        if (!Directory.Exists(INITDATA_PATH))
        {
            Directory.CreateDirectory(INITDATA_PATH);
        }

        string str = "public class " + table.TableName + "\n{\n";

        //字符串拼接
        for (int i = 0; i < table.Columns.Count; i++)
        {
            str += "    public" + " "+ typeName[i].ToString() + " " + rowName[i].ToString() + ";\n";
        }

        str += "}";

        File.WriteAllText(INITDATA_PATH + table.TableName + ".cs", str);
    }

    public static void  GenerateExcelContainer(DataTable table)
    {
        //得到主键的索引
        int keyIndex = GetKeyIndex(table);
        //得到字段类型行
        DataRow rowType = GetTypeNameRow(table);

        if (!Directory.Exists(INITCONTAINER_PATH))
        {
            Directory.CreateDirectory(INITCONTAINER_PATH);
        }

        string str = "using System.Collections.Generic;\n";

        str += "public class " + table.TableName + "Container" + "\n{\n";

        str += "      " + "public Dictionary<" + rowType[keyIndex].ToString() + "," + table.TableName + ">"+"dataDic= new " + "Dictionary<" + rowType[keyIndex].ToString() + "," + table.TableName + ">();\n";

        str += "}";

        File.WriteAllText(INITCONTAINER_PATH + table.TableName + "Container"+".cs", str);
    }

    /// <summary>
    /// 获取变量名
    /// </summary>
    public static DataRow GetVarNameRow(DataTable table)
    {
       return table.Rows[0];
    }
    /// <summary>
    /// 获取变量的类型
    /// </summary>
    public static DataRow GetTypeNameRow(DataTable table)
    {
        return table.Rows[1];
    }

    public static int GetKeyIndex(DataTable dataTable)
    {

        DataRow row = dataTable.Rows[2];
        for (int i = 0; i < dataTable.Columns.Count; i++)
        {
            if(row[i].ToString() == "key")
            {
                return i;
            }
        }
        return 0;
    }



    [MenuItem("GameTool/Excel/打开Excel表")]
    private static void OpenExcel()
    {
        using (FileStream fs = File.Open(Application.dataPath + "/ExcelTable/PlayerInfo.xlsx", FileMode.Open, FileAccess.Read))
        {
            //通过我们的文件流获取Excel数据
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
            //将excel表中的数据转换为DataSet数据类型 方便我们 获取其中的内容
            DataSet result = excelReader.AsDataSet();
            //得到Excel文件中的所有表信息
            for (int i = 0; i < result.Tables.Count; i++)
            {
                Debug.Log("表名：" + result.Tables[i].TableName);
                Debug.Log("行数：" + result.Tables[i].Rows.Count);
                Debug.Log("列数：" + result.Tables[i].Columns.Count);
            }
            fs.Close();
        }
    }

    [MenuItem("GameTool/Excel/读取Excel里的具体信息")]
    private static void ReadExcel()
    {
        using (FileStream fs = File.Open(Application.dataPath + "/ExcelTable/PlayerInfo.xlsx", FileMode.Open, FileAccess.Read))
        {
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
            DataSet result = excelReader.AsDataSet();

            for (int i = 0; i < result.Tables.Count; i++)
            {
                //得到其中一张表的具体数据
                DataTable table = result.Tables[i];
                //得到其中一行的数据
                //DataRow row = table.Rows[0];
                //得到行中某一列的信息
                //Debug.Log(row[1].ToString());
                DataRow row;
                for (int j = 0; j < table.Rows.Count; j++)
                {
                    //得到每一行的信息
                    row = table.Rows[j];
                    Debug.Log("*********新的一行************");
                    for (int k = 0; k < table.Columns.Count; k++)
                    {
                        Debug.Log(row[k].ToString());
                    }
                }
            }

            fs.Close();
        }
    }

}
