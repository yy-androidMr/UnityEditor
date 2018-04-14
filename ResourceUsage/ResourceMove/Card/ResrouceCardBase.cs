using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.IO;

public abstract class ResrouceCardBase<K> : ResrouceCardInterface
{
    protected K serBean;
    protected Dictionary<List<PathInfo>, int> infoListCache = new Dictionary<List<PathInfo>, int>();
    protected List<int> pathListCache = new List<int>();
    private int curKeyIndex = 0;
    public ResrouceCardBase()
    {
        serBean = GetSerializableBean<K>();
        Init();
    }
    public Dictionary<List<PathInfo>, int> PathDic()
    {
        return infoListCache;
    }

    public List<int> PathList()
    {
        return pathListCache;
    }
    public virtual void OnPathClick(int pathId, string value)
    {
        ModifyPath(pathId, value);
    }

    public abstract void Init();
    public abstract void Draw();
    public abstract void Update();

    public T GetSerializableBean<T>()
    {
        Type t = CDNResourceMoveConfig.Instance().GetType();
        var mems = t.GetFields(); //拿所有成员
        Type mT = typeof(T);
        for (int i = 0; i < mems.Length; i++)
        {
            if (mems[i].FieldType == mT)// mems[i].FieldType.IsAssignableFrom(T)
            {
                FieldInfo mi = mems[i];
                return (T)mi.GetValue(CDNResourceMoveConfig.Instance());
            }
        }
        EditorUtility.DisplayDialog("错误",
                                    "找不到对应的类型:" + mT,
                                    "确认");
        return (T)new object();
    }
    protected void Repaint()
    {
        if (ResourceMoveHelper.ReFreshUI != null)
        {
            ResourceMoveHelper.ReFreshUI();
        }
    }
    public void RegistPathListModifyId(int count)
    {
        if (pathListCache.Count == count)
        {
            return;
        }
        pathListCache.Clear();
        for (int i = 0; i < count; i++)
        {
            while (pathListCache.Contains(curKeyIndex))
            {
                curKeyIndex++;
            }
            pathListCache.Add(curKeyIndex);
            curKeyIndex++;
        }
    }

    protected void GUIAddFolder(List<PathInfo> pathList, string desc, string btnName = "添加路径")
    {
        if (!infoListCache.ContainsKey(pathList))
        {
            while (infoListCache.ContainsValue(curKeyIndex))
            {
                curKeyIndex++;
            }
            infoListCache.Add(pathList, curKeyIndex);
            curKeyIndex++;
        }
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(desc, EditorStyles.boldLabel, GUILayout.Width(70));
        if (GUILayout.Button(btnName, GUILayout.Width(70)))
        {
            string folderPath = EditorUtility.OpenFolderPanel("选择文件夹", "", "");
            if (folderPath != string.Empty)
            {
                PathInfo pi = new PathInfo();
                pi.path = folderPath;
                pi.enable = true;
                pathList.Add(pi);
            }
        }
        int tag = -1;
        infoListCache.TryGetValue(pathList, out tag);
        EditorGUILayout.LabelField("标记:" + tag, EditorStyles.boldLabel, GUILayout.Width(70));

        EditorGUILayout.EndHorizontal();
    }
    protected void GUIPath(List<PathInfo> pathList, int index, Color color)
    {
        PathInfo pathInfo = pathList[index];
        Color old = GUI.color;
        GUI.color = color;
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(pathInfo.path, EditorStyles.helpBox))
        {
            string folderPath = EditorUtility.OpenFolderPanel("选择文件夹", System.IO.Path.GetFullPath(pathInfo.path), "");
            if (folderPath != string.Empty)
            {
                pathInfo.path = folderPath;
            }
        }
        pathInfo.enable = EditorGUILayout.Toggle(pathInfo.enable, GUILayout.Width(20));
        GUI.color = Color.red;
        if (GUILayout.Button("x", GUILayout.Width(15), GUILayout.Height(14)))
        {
            //删除本条.
            pathList.RemoveAt(index);
        }
        GUI.color = color;
        //CDNResourceMoveConfig.Instance().ModifyPath(isSrc, index, pathInfo,));

        EditorGUILayout.EndHorizontal();
        GUI.color = old;
    }


    #region 文件路径快速替换用
    private Dictionary<int, string> modifyList = new Dictionary<int, string>();
    public void ModifyPath(int id, string value)
    {
        for (int i = 0; i < pathListCache.Count; i++)
        {
            if (pathListCache[i] == id)
            {
                //相等就对了
                modifyList[id] = value;
            }
        }
    }
    public void PathTag(int id, ref string path)
    {
        string cachePath;
        if (modifyList.TryGetValue(id, out cachePath))
        {
            if (!cachePath.Equals(path))
            {
                //不相等.证明改了值了.
                path = cachePath;
            }
        }
        else
        {
            modifyList.Add(id, path);
        }
        EditorGUILayout.LabelField("标记:" + id, EditorStyles.boldLabel, GUILayout.Width(70));
    }
    #endregion

}