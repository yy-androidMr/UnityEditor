using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ResourceMoveHelper
{
    public static bool InTest = false;
    public static Action ReFreshUI;
    public static void CopyFile(string from, string to)
    {
        if (File.Exists(from))
        {
            //文件要是存在,删除
            if (File.Exists(to))
            {
                System.IO.File.SetAttributes(to, System.IO.FileAttributes.Normal);
                System.IO.File.Delete(to);
            }
            else
            {
                //文件不存在,创建该文件夹目录.
                string dir = Path.GetDirectoryName(to);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
            //拷贝.
            System.IO.File.Copy(from, to);
        }
    }

    public static void StartProcess(LoadingProcess lp)
    {
        EditorApplication.update = delegate()
        {
            bool isCancel = false;
            try
            {

                isCancel = EditorUtility.DisplayCancelableProgressBar("正在查找" + lp.GetMyType() + "引用...", lp.Name == null ? "" : lp.Name(), lp.CurentPercent());
                if (isCancel || lp.DoAdd())
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                }
            }
            catch (Exception e)
            {
                isCancel = true;
                Debug.Log(e);
            }

        };
    }
    public static void WriteOnly(string outFileName, List<string> content, WriteType type = WriteType.TEXT)
    {
        if (string.IsNullOrEmpty(CDNResourceMoveConfig.Instance().logOutPath) || !Directory.Exists(CDNResourceMoveConfig.Instance().logOutPath))
        {
            Debug.LogError("write error !:" + CDNResourceMoveConfig.Instance().logOutPath);
            return;
        }
        string outputPath = CDNResourceMoveConfig.Instance().logOutPath + "/" + outFileName;

        string dir = Path.GetDirectoryName(outputPath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        if (type == WriteType.EXCEL)
        {
            ExcelIns ei = ExcelManager.GetExcel(outputPath + ".xls");
            ei.WriteSimpleList(content);
            //ei.GetWorkSheet();
        }
        else
        {
            outputPath = outputPath + ".txt";
            if (!File.Exists(outputPath))
            {
                FileStream fs1 = new FileStream(outputPath, FileMode.Create, FileAccess.Write);//创建写入文件 
                fs1.Close();
            }
            System.IO.File.WriteAllLines(outputPath, content.ToArray());
        }
    }
}
