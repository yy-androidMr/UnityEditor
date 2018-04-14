using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;


public class OhterCardItemBase //借用这个接口 以后实在不适用,再拿出来
{
    protected OtherInfo serBean;
    protected List<int> idList;
    public OhterCardItemBase()
    {
        serBean = CDNResourceMoveConfig.Instance().otherInfo;
        Init();
    }
    protected void DrawTitle(string titleContent)
    {
        Color old = GUI.color;
        GUI.color = Color.cyan;
        EditorGUILayout.LabelField(titleContent);
        GUI.color = old;

    }
    public virtual void Init()
    {

    }
    public virtual void Update()
    {

    }
    public virtual void Draw()
    {

    }
    public virtual int PathModifyTagCount()
    {
        //注册快速修改路径的数量
        return 0;
    }

    public void ModifyPath(int id, string value)
    {
        for (int i = 0; i < idList.Count; i++)
        {
            if (idList[i] == id)
            {
                //相等就对了
                modifyList[id] = value;
            }
        }
    }
    private Dictionary<int, string> modifyList = new Dictionary<int, string>();
    public void SetPathIdList(List<int> value)
    {
        idList = value;
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
}