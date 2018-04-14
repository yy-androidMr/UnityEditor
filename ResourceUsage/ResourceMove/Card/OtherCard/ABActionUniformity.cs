using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;


//ab动作一致性检查按钮
public class ABActionUniformityItem : OhterCardItemBase
{
    //ShowNotification(new GUIContent("没有配置输出路径"));  

    public override void Draw()
    {
        DrawTitle("ab局内动作一致性");
        EditorGUILayout.BeginHorizontal();
        PathTag(idList[0], ref serBean.cdnSource);
        if (GUILayout.Button("修改SourceFile", GUILayout.Width(100)))
        {
            string folderPath = EditorUtility.OpenFolderPanel("选择文件夹", "", "");
            if (folderPath != string.Empty)
            {
                serBean.cdnSource = folderPath;
            }
        }
        EditorGUILayout.LabelField(serBean.cdnSource);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        PathTag(idList[1], ref serBean.art);
        if (GUILayout.Button("修改art目录", GUILayout.Width(100)))
        {
            string folderPath = EditorUtility.OpenFolderPanel("选择文件夹", "", "");
            if (folderPath != string.Empty)
            {
                serBean.art = folderPath;
            }
        }
        EditorGUILayout.LabelField(serBean.art);
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("输出", GUILayout.Width(100)))
        {
            ResourceMoveHelper.StartProcess(new UIAB_DanceProcess(serBean.cdnSource, serBean.art));
        }
    }
    public override int PathModifyTagCount()
    {
        return 2;
    }

    //public void ModifyPath(int index, string value)
    //{
    //    //idList
    //    if (index == 0)
    //    {
    //        serBean.cdnSource = value;
    //    }
    //    else if (index == 1)
    //    {
    //        serBean.art = value;
    //    }
    //}
}