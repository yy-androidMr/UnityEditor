using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Xml;
using OfficeOpenXml;
using Aspose.Cells;

public class CDNResourceMove
{
    //public static LoadConfig()
    //{

    //}



    [MenuItem("H3D/使用查找/资源移动", false, -50)]
    public static void OpenWindow()
    {
        //Rect re = new Rect(0, 0, 1200, 600);
        //EditorWindow.GetWindowWithRect(typeof(CDNResourceMoveWindow), re);//规定大小窗口
        //CDNResourceMoveWindow window = EditorWindow.GetWindow<CDNResourceMoveWindow>(true, "资源移动", true);

        CDNResourceMoveWindow instance = (CDNResourceMoveWindow)EditorWindow.GetWindow<CDNResourceMoveWindow>("资源处理", typeof(CDNResourceMoveWindow));
        instance.minSize = new Vector2(600, 500);
        instance.Show();
    }

}