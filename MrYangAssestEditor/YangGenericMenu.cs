using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
public class YangGenericMenu : EditorWindow
{
    [MenuItem("Assets/拷贝全路径", false, -4)]
    static void CopyPath()
    {
        var assetGuids = Selection.assetGUIDs;
        string choosePath = "";
        foreach (var guid in assetGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            choosePath += YangMenuHelper.helperIns.projAbsPath + path + "\n";
        }

        YangMenuHelper.ControlTextEditor(choosePath);
    }

    [MenuItem("Assets/拷贝p4全路径", false, -3)]
    static void CopyP4Path()
    {
        var assetGuids = Selection.assetGUIDs;
        string choosePath = "";
        foreach (var guid in assetGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            choosePath += YangMenuHelper.helperIns.projAbsPath + path + "\n";
        }
        string[] dirs = choosePath.Split('/');
        choosePath = "/";
        bool isPath = false;
        for (int i = 0; i < dirs.Length; i++)
        {
            if (dirs[i].Equals("x5_mobile"))
            {
                isPath = true;
            }
            if (isPath)
            {
                choosePath += "/" + dirs[i];
            }
        }
        YangMenuHelper.ControlTextEditor(choosePath);
    }

    [MenuItem("Assets/打开atlas对应的两个文件夹", false, -2)]
    static void OpenAtlasDirs()
    {

        var assetGuids = Selection.assetGUIDs;
        foreach (var guid in assetGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.Contains("/StaticResources/art/UIAtlas/NewUI"))
            {
                string filePath = YangMenuHelper.helperIns.projAbsPath + path;

                if (YangMenuHelper.IsDirectory(filePath))
                {
                    System.Diagnostics.Process.Start(filePath);
                }
                else
                {
                    filePath = YangMenuHelper.helperIns.GetDirectoryPath(filePath);

                }
                System.Diagnostics.Process.Start(filePath);
                string[] filess = filePath.Split('/');
                bool started = false;
                string targetName = "";
                for (int i = 0; i < filess.Length; i++)
                {
                    if (filess[i].Equals("NewUI"))
                    {
                        started = true;
                    }
                    else
                    {
                        if (started && filess[i] != "" && filess[i] != null)
                        {
                            targetName += filess[i] + "/";
                        }
                    }
                }
                string target = YangMenuHelper.helperIns.projResourcePath + YangMenuHelper.helperIns.Get美术资源Path() + "UI/新界面程序用目录/";
                DirectoryInfo mydir = new DirectoryInfo(target + targetName);
                if (mydir.Exists)
                {
                    target += targetName;
                }
                System.Diagnostics.Process.Start(target);
            }
            else
            {
            }
        }
    }
    [MenuItem("Assets/查找其引用", false, -1)]
    static void FindUseage()
    {
        var assetGuids = Selection.assetGUIDs;
        string[] prefab_files = Directory.GetFiles(YangMenuHelper.helperIns.projAbsPath + "Assets/resources/Art", "*.prefab", SearchOption.AllDirectories);
        foreach (var guid in assetGuids)
        {
            int file_index = 0;
            EditorApplication.update = delegate()
       {
           string prefab_path = prefab_files[file_index];
           bool isCancel = EditorUtility.DisplayCancelableProgressBar("正在查找" + guid + "引用...", prefab_path, (float)file_index / (float)(prefab_files.Length));
           string allText = File.ReadAllText(prefab_path);
           if (allText.Contains(guid))
           {
               UnityEngine.Debug.Log("prefab_path :" + prefab_path);
           }

           file_index++;
           if (isCancel || file_index >= prefab_files.Length)
           {
               EditorUtility.ClearProgressBar();
               file_index = 0;
               EditorApplication.update = null;
           }

       };
            return;
        }
    }
    //[MenuItem("GameObject/设置为家具", false, -100)]
    //[ContextMenu("Reset Name")]
    //private static void ResetName()
    //{
    //}
    //[MenuItem("Game/Open Window")]
    //    static void Init()
    //    {
    //     var window = GetWindow (this);
    //        window.position = Rect (50, 50, 250, 60);
    //        window.Show ();
    //    }
    //    @MenuItem("Game/Open Window")
    //    static function Init () {
    //        var window = GetWindow (MyGenericMenu);
    //        window.position = Rect (50, 50, 250, 60);
    //        window.Show ();
    //    }

    //    function Callback (obj:Object) {
    //        Debug.Log ("Selected: " + obj);
    //    }

    //    function OnGUI() {
    //        var evt : Event = Event.current;
    //        var contextRect : Rect = new Rect (10, 10, 100, 100);

    //        if (evt.type == EventType.ContextClick)
    //        {
    //            var mousePos : Vector2 = evt.mousePosition;
    //            if (contextRect.Contains (mousePos))
    //            {
    //                // Now create the menu, add items and show it
    //                var menu : GenericMenu = new GenericMenu ();

    //                menu.AddItem (new GUIContent ("MenuItem1"), false, Callback, "item 1");
    //                menu.AddItem (new GUIContent ("MenuItem2"), false, Callback, "item 2");
    //                menu.AddSeparator ("");
    //                menu.AddItem (new GUIContent ("SubMenu/MenuItem3"), false, Callback, "item 3");

    //                menu.ShowAsContext ();

    //                evt.Use();
    //            }
    //        }
    //    }

}
