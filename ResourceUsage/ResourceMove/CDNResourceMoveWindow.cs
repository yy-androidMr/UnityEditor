using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Linq;

public enum ResourceActionType
{
    [Description("服装资源和图标")]
    ROLE,
    [Description("音乐关卡文件")]
    CHECK_POINT,
    [Description("卡牌资源")]
    CDN_TEXTURE,
    [Description("其他功能")]
    OTHER,
}

public class CDNResourceMoveWindow : EditorWindow
{

    public Dictionary<int, ResrouceCardInterface> cardList = new Dictionary<int, ResrouceCardInterface>();
    public string[] selectionMenu;
    private Vector2 m_ScrollPosition;

    public void Awake()
    {
        curSelectCard = null;
        List<string> menuList = new List<string>();
        foreach (ResourceActionType suit in Enum.GetValues(typeof(ResourceActionType)))
        {
            menuList.Add(DescriptionContent(suit));
        }
        selectionMenu = menuList.ToArray();

        ResourceMoveHelper.ReFreshUI = Repaint;
        cardList.Add((int)ResourceActionType.ROLE, new RoleCard());
        cardList.Add((int)ResourceActionType.CHECK_POINT, new CheckPointCard());
        //cardList.Add((int)ResourceActionType.CDN_TEXTURE, new RoleCard());
        cardList.Add((int)ResourceActionType.OTHER, new OhterCard());
        //CDNResourceMoveConfig.Instance().descList.Add(Application.dataPath);
    }
    private ResrouceCardInterface curSelectCard;
    void OnGUI()
    {
        if (curSelectCard != null)
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(CDNResourceMoveConfig.Instance().pathListInHide ? "展开路径" : "收起路径", GUILayout.Width(70)))
            {
                CDNResourceMoveConfig.Instance().pathListInHide = !CDNResourceMoveConfig.Instance().pathListInHide;
            }
            if (GUILayout.Button("添加路径", GUILayout.Width(70)))
            {
                string folderPath = EditorUtility.OpenFolderPanel("选择文件夹", "", "");
                if (folderPath != string.Empty)
                {
                    if (!CDNResourceMoveConfig.Instance().pathList.Contains(folderPath))
                    {
                        CDNResourceMoveConfig.Instance().pathList.Add(folderPath);
                    }
                }
            }
            //绘制输出路径.
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            if (GUILayout.Button("日志输出路径", GUILayout.Width(100)))
            {
                string folderPath = EditorUtility.OpenFolderPanel("选择文件夹", "", "");
                if (folderPath != string.Empty)
                {
                    CDNResourceMoveConfig.Instance().logOutPath = folderPath;
                }
            }
            EditorGUILayout.LabelField(CDNResourceMoveConfig.Instance().logOutPath);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            if (!CDNResourceMoveConfig.Instance().pathListInHide)
            {
                m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition, GUILayout.Height(90));
                if (curSelectCard.PathList() == null || curSelectCard.PathList().Count == 0)
                {
                    DrawAddPath(curSelectCard.PathDic());
                }
                if (curSelectCard.PathDic() == null || curSelectCard.PathDic().Count == 0)
                {
                    DrawModifyPath();
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        //EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(true));
        //(ResourceActionType)
        CDNResourceMoveConfig.Instance().cardIndex = GUILayout.SelectionGrid(CDNResourceMoveConfig.Instance().cardIndex, selectionMenu, selectionMenu.Length, EditorStyles.toolbarButton);
        ResrouceCardInterface inter;
        if (cardList.TryGetValue(CDNResourceMoveConfig.Instance().cardIndex, out inter))
        {
            curSelectCard = inter;
            inter.Draw();
        }
    }
    private void DrawModifyPath()
    {
        List<int> modifyList = curSelectCard.PathList();
        if (modifyList == null)
        {
            return;
        }
        //EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(true));
        for (int i = 0; i < CDNResourceMoveConfig.Instance().pathList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            foreach (var item in modifyList)
            {
                if (GUILayout.Button(item + "", GUILayout.Width(50)))
                {
                    var path = CDNResourceMoveConfig.Instance().pathList[i];
                    curSelectCard.OnPathClick(item, path);
                }
            }
            EditorGUILayout.LabelField(CDNResourceMoveConfig.Instance().pathList[i], EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
        }
    }
    protected void DrawAddPath(Dictionary<List<PathInfo>, int> pathDic)
    {
        if (pathDic == null)
        {
            return;
        }
        //EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(true));
        for (int i = 0; i < CDNResourceMoveConfig.Instance().pathList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            foreach (var item in pathDic)
            {
                if (GUILayout.Button(item.Value + "", GUILayout.Width(50)))
                {
                    var path = CDNResourceMoveConfig.Instance().pathList[i];
                    List<PathInfo> digout = item.Key.Where(o => o.path.Equals(path)).ToList();
                    if (digout.Count > 0)
                    {
                        continue;
                    }
                    PathInfo pi = new PathInfo();
                    pi.path = path;
                    item.Key.Add(pi);
                }
            }
            EditorGUILayout.LabelField(CDNResourceMoveConfig.Instance().pathList[i], EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
        }

    }
    public string DescriptionContent(Enum en)
    {
        Type type = en.GetType();
        MemberInfo[] memInfo = type.GetMember(en.ToString());
        if (memInfo != null && memInfo.Length > 0)
        {
            object[] attrs = memInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (attrs != null && attrs.Length > 0)
                return ((DescriptionAttribute)attrs[0]).Description;
        }
        return en.ToString();
    }

    void Update()
    {
        ResrouceCardInterface inter;
        if (cardList.TryGetValue(CDNResourceMoveConfig.Instance().cardIndex, out inter))
        {
            inter.Update();
        }
    }


    void OnDisable()
    {
    }
    void OnDestroy()
    {
        ResourceMoveHelper.ReFreshUI = null;
    }

}
