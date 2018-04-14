using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;


public class OhterCard : ResrouceCardBase<CheckPoint>
{
    private List<OhterCardItemBase> childItem = new List<OhterCardItemBase>();
    public override void Init()
    {
        childItem.Clear();
        childItem.Add(new ABActionUniformityItem());
        childItem.Add(new ABActionUniformityItem());

        RegistPathModifyBtn();
    }

    private void RegistPathModifyBtn()
    {
        int count = 0;
        for (int i = 0; i < childItem.Count; i++)
        {
            count += childItem[i].PathModifyTagCount();
        }
        RegistPathListModifyId(count);

        count = 0;
        for (int i = 0; i < childItem.Count; i++)
        {
            List<int> idList = pathListCache.GetRange(count, childItem[i].PathModifyTagCount());
            count += childItem[i].PathModifyTagCount();
            childItem[i].SetPathIdList(idList);
        }
    }
    public override void OnPathClick(int pathId, string value)
    {
        int index = pathListCache.IndexOf(pathId);

        for (int i = 0; i < childItem.Count; i++)
        {
            childItem[i].ModifyPath(pathId, value);
        }

    }
    public override void Update()
    {
        for (int i = 0; i < childItem.Count; i++)
        {
            childItem[i].Update();
        }
    }
    private Vector2 m_ScrollPosition;

    public override void Draw()
    {
        EditorGUILayout.BeginScrollView(m_ScrollPosition);

        for (int i = 0; i < childItem.Count; i++)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            childItem[i].Draw();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        EditorGUILayout.EndScrollView();

    }
}