using UnityEngine;
using UnityEditor;
using System.IO;
using Excel;
using System.Data;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using Aspose.Cells;
using OfficeOpenXml.Style;

public class CDNabLengthExport
{




    private delegate void OnReadSuccess(ExcelPackage package);

    static string filePath = Application.dataPath.Replace("/Assets", "/") + "assetbundles/";//cdn/assetbundles
    static int typeIndex = 0;//类型
    static int pathIndex = 0;//路径
    static int childSizeTypeIndex = 0;//子类型大小(MB)
    static int fileCountIndex = 0;//资源数量
    static int changeIndex = 0;//变化趋势(MB)
    static int oldIndex = 0;//原坐标对应的列
    static List<string> typeList = new List<string>();
    static List<string> pathList = new List<string>();


    [MenuItem("H3D/Tools/导出ab文件夹大小到excel")]
    static void ExprotExcel()
    {
        Init();
        ReadExcel(readAbPath, @"E:\wendang文档\炫舞手游_2017年12月份版本\资源总量分析\资源总量分析_2017_11_16 - 副本.xlsx");
    }
    private static void Init()
    {
        typeIndex = 0;
        pathIndex = 0;
        childSizeTypeIndex = 0;
        fileCountIndex = 0;
        xmlLength = 0;
        xmlPaths.Clear();
        typeList.Clear();
        pathList.Clear();
    }
    public static void readAbPath(ExcelPackage package)
    {

        ExcelWorksheets sheets = package.Workbook.Worksheets;
        ExcelWorksheet sheet = sheets["Sheet1"];

        ReadTypeAndPath(sheet, typeList, pathList);

        WritePIInfo(package, sheet);
        UnityEngine.Debug.Log("typeList:" + typeList.Count + "  pathList:" + pathList.Count);
    }

    static ExcelPackage ReadExcel(OnReadSuccess success, string path)
    {
        string exprotFilePath = path;
        FileInfo newFile = new FileInfo(exprotFilePath);
        //FileStream fs = new FileStream(exprotFilePath, FileMode.Open);
        try
        {
            using (var package = new ExcelPackage(newFile))
            {
                success(package);
                //valuesss = value.Value.ToString();
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log("exception:" + e);
        }
        return null;
    }
    //读取excel

    private static void ReadTypeAndPath(ExcelWorksheet sheet, List<string> typeList, List<string> pathList)
    {
        int rowsCount = 26;
        int colsCount = 100;
        for (int i = 1; i < rowsCount; i++)
        {
            object obj = sheet.Cells[1, i].Value;
            if (obj != null)
            {
                string tempValue = obj.ToString();
                if (tempValue.Equals("类型"))
                {
                    typeIndex = i;
                }
                else if (tempValue.Equals("路径"))
                {
                    pathIndex = i;
                }
                else if (tempValue.Equals("子类型大小(MB)"))
                {
                    childSizeTypeIndex = i;
                }
                else if (tempValue.Equals("资源数量"))
                {
                    fileCountIndex = i;
                }
                else if (tempValue.Equals("变化趋势(MB)"))
                {
                    changeIndex = i;
                }
                else if (tempValue.Equals("原坐标对应列"))
                {
                    oldIndex = i;
                }

            }
        }

        typeList.Add("");//第一个给加上
        for (int j = 2; j < colsCount; j++)
        {
            object obj = sheet.Cells[j, typeIndex].Value;
            if (obj != null)
            {
                typeList.Add(obj.ToString());
            }
            else
            {
                break;
            }
        }

        pathList.Add("");//第一个给加上
        for (int j = 2; j < typeList.Count; j++)
        {
            object obj = sheet.Cells[j, pathIndex].Value;
            if (obj != null)
            {
                pathList.Add(obj.ToString());
            }
            else
            {
                //sheet.Cells[j, pathIndex].Value = "nonono";
                pathList.Add("");
            }
        }
    }

    //计算文件夹
    private static PathInfo GetPathInfo(string path)
    {
        PathInfo pi = new PathInfo();
        pi.relPath = path;
        pi.absPath = filePath + path;
        //filePath
        long length = GetDirectoryLength(pi.absPath, pi);
        if (length > 0)
        {
            pi.size = (double)length / 1024 / 1024;
            pi.canWrite = true;
        }
        else
        {
            pi.canWrite = false;
        }
        return pi;
    }
    private static long xmlLength = 0;
    private static List<string> xmlPaths = new List<string>();
    private static long levelLength = 0;
    private static List<string> levelPaths = new List<string>();
    public static long GetDirectoryLength(string dirPath, PathInfo pi)
    {
        if (CanBreak(dirPath, pi))//该目录不需要计算.
        {
            return 0;
        }
        //判断给定的路径是否存在,如果不存在则退出
        if (!Directory.Exists(dirPath))
            return 0;
        long len = 0;
        //定义一个DirectoryInfo对象
        DirectoryInfo di = new DirectoryInfo(dirPath);
        //通过GetFiles方法,获取di目录中的所有文件的大小
        foreach (FileInfo fi in di.GetFiles())
        {
            if (CanAdd(fi))
            {
                if (fi.FullName.Contains(@"assetbundles\config") && fi.FullName.EndsWith(@".xml"))
                {
                    xmlPaths.Add(fi.FullName);
                    //UnityEngine.Debug.Log("未转二进制:" + fi.FullName.ToString() + "  length:" + fi.Length);
                    xmlLength += fi.Length;
                }
                else if (fi.FullName.Contains(@"assetbundles\level"))
                {
                    levelPaths.Add(fi.FullName);
                    levelLength += fi.Length;
                    //UnityEngine.Debug.Log("未转二进制:" + fi.FullName.ToString() + "  length:" + fi.Length + "  当前测量大小:" + levelLength / 1024.0f / 1024.0f);
                }
                pi.count++;
                len += fi.Length;
            }
        }
        //获取di中所有的文件夹,并存到一个新的对象数组中,以进行递归
        DirectoryInfo[] dis = di.GetDirectories();
        if (dis.Length > 0)
        {
            for (int i = 0; i < dis.Length; i++)
            {
                len += GetDirectoryLength(dis[i].FullName, pi);
            }
        }
        return len;
    }

    private static bool CanAdd(FileInfo info)
    {
        if (!info.Name.EndsWith(".h3dmanifest"))//
        {
            if (info.Name.EndsWith(".xml") && File.Exists(info.FullName + ".bytes"))//配置二进制化.只计算二进制文件
            {

            }
            else
            {
                return true;
            }
        }
        return false;
    }
    public static bool CanBreak(string dirPath, PathInfo pi)
    {

        for (int i = 0; i < pathList.Count; i++)
        {
            if (!string.IsNullOrEmpty(pathList[i]))
            {
                string path = filePath + pathList[i];
                path = path.Replace("/", "\\");
                if (dirPath.Equals(path) && !pi.relPath.Equals(pathList[i]))//判断:当前递归路径,是否和其他路径重合.如果重合.则不递归该路径下的文件夹.
                {
                    UnityEngine.Debug.Log("路径重合:" + pathList[i] + "  pi.absPath:" + pi.relPath);
                    return true;
                }

            }
        }
        return false;
    }


    private static void WritePIInfo(ExcelPackage package, ExcelWorksheet sheet)
    {
        for (int i = 0; i < typeList.Count; i++)
        {
            if (typeList[i].Equals("ab") && !string.IsNullOrEmpty(pathList[i]))
            {
                //进行计算.
                PathInfo pi = GetPathInfo(pathList[i]);
                if (pi.canWrite)
                {
                    if (pi.size > 0)
                    {
                        pi.size = Math.Round(pi.size, 2);
                    }
                    else
                    {
                        pi.size = Math.Round(pi.size, 3);
                    }


                    sheet.Cells[i + 1, childSizeTypeIndex].Value = pi.size;
                    sheet.Cells[i + 1, fileCountIndex].Value = pi.count;
                }
                //UnityEngine.Debug.Log("pi:" + pi.count + "  path:" + pathList[i] + "   size:" + pi.size);
            }
        }
        WriteChange(sheet);
        package.Save();
        xmlPaths.Add("未转二进制文件总共占用大小:" + xmlLength / 1024.0f / 1024.0f);
        levelPaths.Add("未转二进制文件总共占用大小:" + levelLength / 1024.0f / 1024.0f);
        //System.IO.File.WriteAllLines(@"E:\wendang文档\炫舞手游_2017年12月份版本\资源总量分析\未二进制的config_xml.txt", xmlPaths.ToArray());
        //System.IO.File.WriteAllLines(@"E:\wendang文档\炫舞手游_2017年12月份版本\资源总量分析\未二进制的level_xml.txt", levelPaths.ToArray());
        Init();
    }
    private static void WriteChange(ExcelWorksheet newSheet)
    {
        ReadExcel(delegate(ExcelPackage package)
        {
            ExcelWorksheets sheets = package.Workbook.Worksheets;
            ExcelWorksheet sheet = sheets["Sheet1"];
            int rowsCount = 26;
            int m_old_typeIndex = 0;
            for (int i = 1; i < rowsCount; i++)
            {
                object obj = sheet.Cells[1, i].Value;
                if (obj != null)
                {
                    string tempValue = obj.ToString();
                    if (tempValue.Equals("子类型大小(MB)"))
                    {
                        m_old_typeIndex = i;
                    }
                }
            }

            for (int i = 0; i < typeList.Count; i++)
            {
                if ((typeList[i].Equals("apk") || typeList[i].Equals("ab")))
                {
                    if (newSheet.Cells[i + 1, oldIndex].Value != null)
                    {
                        int iso_index = int.Parse(newSheet.Cells[i + 1, oldIndex].Value.ToString());
                        if (sheet.Cells[iso_index, m_old_typeIndex].Value != null)
                        {
                            float old_value = float.Parse(sheet.Cells[iso_index, m_old_typeIndex].Value.ToString());

                            float new_value = float.Parse(newSheet.Cells[i + 1, childSizeTypeIndex].Value.ToString());

                            if (old_value > new_value)
                            {
                                newSheet.Cells[i + 1, changeIndex].Value = "下降" + Math.Abs(new_value - old_value);
                                //ExcelColor ec = new ExcelColor();
                                //newSheet.Cells[i + 1, changeIndex].Style.
                            }
                            else if (old_value < new_value)
                            {
                                newSheet.Cells[i + 1, changeIndex].Value = "上升" + Math.Abs(new_value - old_value);
                            }
                            else
                            {
                                newSheet.Cells[i + 1, changeIndex].Value = "无上升下降";
                            }
                            UnityEngine.Debug.Log("iso_index:" + iso_index + "   old_value:" + old_value + "   new_value:" + new_value);

                        }
                    }
                }
            }

            package.Save();
        }, @"E:\wendang文档\炫舞手游_2017年12月份版本\资源总量分析\资源总量分析_2017_10_23.xlsx");



    }
    public class PathInfo
    {
        public string absPath = "";
        public string relPath = "";
        public double size = 0;//MB
        public int count = 0;
        public bool canWrite = false;
    }
}
