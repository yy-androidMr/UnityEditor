using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class CopyAction
{
    private int idListCurser = 0;
    private List<string> idList;
    public bool inCopy = false;

    RoleTexture bean;
    public string GetProcess()
    {
        if (idList.Count <= idListCurser)//o了
        {
            return "移动完成";
        }
        return idListCurser + "/" + idList.Count + ",正在移动:" + idList[idListCurser];
    }

    public void init(RoleTexture obj)
    {
        bean = obj;
        idList = new List<string>();
        //组织id列表
        string ids = bean.idcList;
        Regex r = new Regex(@"\d{10}"); // 定义一个Regex对象实例
        MatchCollection mc = r.Matches(ids); // 在字符串中匹配
        for (int i = 0; i < mc.Count; i++) //在输入字符串中找到所有匹配
        {
            if (mc[i].Value.EndsWith("1") || mc[i].Value.EndsWith("2") || mc[i].Value.EndsWith("3"))
            {
                //加入衍生id
                for (int j = 1; j <= 3; j++)
                {
                    string Id2 = mc[i].Value.Substring(0, mc[i].Value.Length - 1) + j;
                    idList.Add(Id2);
                }
            }
            else
            {
                //未有衍生id
                idList.Add(mc[i].Value);
            }
        }
        idList = idList.Distinct().ToList();
        idListCurser = 0;
        inCopy = true;
    }
    public string MoveNext()
    {
        for (int j = 0; j < bean.src.Count; j++)
        {
            if (!bean.src[j].enable)
            {
                continue;
            }
            string[] srcB = Directory.GetFiles(bean.src[j].path, idList[idListCurser] + "*", SearchOption.AllDirectories);
            //srcOutInfo[j] += "\r\n" + String.Join("\r\n", srcB);

            for (int k = 0; k < srcB.Length; k++)
            {
                string item = srcB[k];
                CopyFrom(bean.src[j].path, item);
                if (bean.isMove)
                {
                    System.IO.File.SetAttributes(item, System.IO.FileAttributes.Normal);
                    System.IO.File.Delete(item);
                }
            }

        }
        string id = idList[idListCurser];
        idListCurser++;
        if (idList.Count <= idListCurser)//o了
        {
            inCopy = false;
        }
        return id;
    }

    public void CopyFrom(string src, string file)
    {
        for (int i = 0; i < bean.desc.Count; i++)
        {
            if (!bean.desc[i].enable)
            {
                continue;
            }
            string to = bean.desc[i].path;
            string desc_file = file.Replace(src, to);//有不同输出路径.
            ResourceMoveHelper.CopyFile(file, desc_file);
            //if (File.Exists(desc_file))
            //{
            //    System.IO.File.SetAttributes(desc_file, System.IO.FileAttributes.Normal);
            //    System.IO.File.Delete(desc_file);
            //}
            //string dir = Path.GetDirectoryName(desc_file);
            //if (!Directory.Exists(dir))
            //{
            //    Directory.CreateDirectory(dir);
            //}
            //System.IO.File.Copy(file, desc_file);

        }
        //Debug.Log("invs:" + inv[j] + "     j:" + j + "  desc_file:" + desc_file);

    }

}

public class RoleCard : ResrouceCardBase<RoleTexture>
{
    private Vector2 m_ScrollPosition;
    private Vector2 m_ScrollPosition2;

    private List<Vector2> m_OutputScrollList = new List<Vector2>();

    private CopyAction ca;
    public override void Init()
    {
        if (ca != null)
        {
            ca.inCopy = false;
        }
    }
    public override void Draw()
    {
        if (ca != null)
        {
            EditorGUI.BeginDisabledGroup(ca.inCopy);
        }
        List<PathInfo> src = serBean.src;
        GUIAddFolder(src, "投放文件夹列表");
        for (int i = 0; i < src.Count; i++)
        {
            GUIPath(src, i, Color.green);
        }

        List<PathInfo> desc = serBean.desc;
        GUIAddFolder(desc, "投放文件夹列表");
        for (int i = 0; i < desc.Count; i++)
        {
            GUIPath(desc, i, Color.yellow);
        }
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        if (GUILayout.Button(serBean.isMove ? "开始移动" : "开始复制", GUILayout.Width(70)))
        {
            if (ca == null)
            {
                ca = new CopyAction();
            }
            ca.init(serBean);
        }
        string proc = ca == null ? "" : ca.GetProcess();
        EditorGUILayout.LabelField("是否删除原始目录", GUILayout.Width(93));
        serBean.isMove = EditorGUILayout.Toggle(serBean.isMove, GUILayout.Width(50));

        //CDNResourceMoveConfig.Instance()
        EditorGUILayout.LabelField("拷贝进度:" + proc, EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("id列表(用任意字符串可以隔开)", EditorStyles.boldLabel);
        //横向排列
        EditorGUILayout.BeginHorizontal();

        m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition, GUILayout.Width(200));
        serBean.idcList = EditorGUILayout.TextArea(serBean.idcList);
        EditorGUILayout.EndScrollView();

        EditorGUI.EndDisabledGroup();

        //纵向排列
        EditorGUILayout.BeginVertical();
        if (ca != null)
        {
            m_ScrollPosition2 = EditorGUILayout.BeginScrollView(m_ScrollPosition2);

            EditorGUILayout.LabelField("源文件夹日志", EditorStyles.boldLabel);
            //for (int i = 0; i < ca.srcOutInfo.Count; i++)
            //{
            //    OutPutScroll(i, ca.srcOutInfo[i]);
            //}
            EditorGUILayout.LabelField("", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("投放文件夹日志", EditorStyles.boldLabel);
            //for (int i = 0; i < ca.descInInfo.Count; i++)
            //{
            //    OutPutScroll(i, ca.descInInfo[i]);
            //}
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndVertical();


        EditorGUILayout.EndHorizontal();
    }
    public override void Update()
    {
        if (ca != null)
        {
            if (ca.inCopy)
            {
                ca.MoveNext();
                Repaint();
            }
        }
    }
    public void OutPutScroll(int position, string content)
    {
        for (int i = 0; m_OutputScrollList.Count <= position; i++)
        {
            m_OutputScrollList.Add(new Vector2());
        }
        //m_OutputScrollList[position] = EditorGUILayout.BeginScrollView(m_OutputScrollList[position]);
        EditorGUILayout.TextArea("", GUILayout.Height(90));
        //EditorGUILayout.EndScrollView();
    }


}