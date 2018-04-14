using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;


public class CheckPointCard : ResrouceCardBase<CheckPoint>
{
    private List<string> idList;
    private List<string> moveList;
    private Vector2 m_ScrollPosition;
    public override void Init()
    {
        inCopy = false;
        //pointInfo.src.Sort((x, y) => x.widget - y.widget);//升序
        //pointInfo.desc.Sort((x, y) => x.widget - y.widget);//升序

    }
    private void InitIdc()
    {
        Regex r = new Regex(@"\d{6}"); // 定义一个Regex对象实例
        MatchCollection mc = r.Matches(serBean.idList); // 在字符串中匹配
        idList = new List<string>();
        for (int i = 0; i < mc.Count; i++) //在输入字符串中找到所有匹配
        {
            string temp = mc[i].Value.Remove(0, 1).Insert(0, "1");
            idList.Add(temp);
        }
        idList = idList.Distinct().ToList();
    }
    private static bool IsIgnore(string file, string[] ignore)
    {
        for (int i = 0; i < ignore.Length; i++)
        {
            if (file.Contains(ignore[i]))
            {
                return true;
            }
        }
        return false;
    }

    private static List<string> MoveCheckPoint(string srcRootPath, string[] bgmSrc, List<string> xmlSrc, CheckPoint pointInfo)
    {
        srcRootPath = srcRootPath.Replace("/", "\\").Replace("\\\\", "\\");
        List<string> deleteFile = new List<string>();

        for (int i = 0; i < pointInfo.desc.Count; i++)
        {
            //GetFileName
            PathInfo descPath = pointInfo.desc[i];
            //if (!Directory.Exists(descPath.path))
            //{
            //    Directory.CreateDirectory(descPath.path);
            //}
            for (int j = 0; j < bgmSrc.Length; j++)
            {
                string temp = bgmSrc[j].Replace("/", "\\").Replace("\\\\", "\\");
                string tempFileName = temp.Replace(srcRootPath, "");// GetFileName(bgmSrc[j]);
                string outPath = descPath.path + "\\" + tempFileName;
                string dir = Path.GetDirectoryName(outPath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                if (File.Exists(outPath))
                {
                    System.IO.File.SetAttributes(outPath, System.IO.FileAttributes.Normal);
                    System.IO.File.Delete(outPath);
                }
                deleteFile.Add(bgmSrc[j]);
                System.IO.File.Copy(bgmSrc[j], outPath);
            }


            for (int j = 0; j < xmlSrc.Count; j++)
            {
                string temp = xmlSrc[j].Replace("/", "\\").Replace("\\\\", "\\");
                string tempFileName = temp.Replace(srcRootPath, "");
                string outPath = descPath.path + "\\" + tempFileName;
                string dir = Path.GetDirectoryName(outPath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                if (File.Exists(outPath))
                {
                    System.IO.File.SetAttributes(outPath, System.IO.FileAttributes.Normal);
                    System.IO.File.Delete(outPath);
                }
                deleteFile.Add(xmlSrc[j]);
                System.IO.File.Copy(xmlSrc[j], outPath);
            }

        }
        return deleteFile;
    }
    private bool inCopy = false;
    private int idcCurser = 0;
    private int srcCurser = 0;
    public override void Draw()
    {
        EditorGUI.BeginDisabledGroup(inCopy);
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
            InitIdc();
            moveList = new List<string>();
            idcCurser = 0;
            srcCurser = 0;
            inCopy = true;
        }
        string proc = inCopy ? GetProc() : "";
        EditorGUILayout.LabelField("是否删除原始目录", GUILayout.Width(93));
        serBean.isMove = EditorGUILayout.Toggle(serBean.isMove, GUILayout.Width(50));

        //CDNResourceMoveConfig.Instance()
        EditorGUILayout.LabelField("拷贝进度:" + proc, EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.LabelField("id列表(用任意字符串可以隔开)", EditorStyles.boldLabel);
        //横向排列

        m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition, GUILayout.Width(200));
        serBean.idList = EditorGUILayout.TextArea(serBean.idList);
        EditorGUILayout.EndScrollView();

        EditorGUI.EndDisabledGroup();

    }
    private string GetProc()
    {
        return (srcCurser + 1) + "/" + serBean.src.Count + ":" + idcCurser + "/" + idList.Count;
    }
    public override void Update()
    {
        if (!inCopy)
        {
            return;
        }
        //allClassSrc
        PathInfo pi = serBean.src[srcCurser];

        string[] allClassFiles = Directory.GetFiles(pi.path + serBean.GetLevelPath(), "*.xml", SearchOption.AllDirectories);
        List<string> classSrc = allClassFiles.Where(file => !IsIgnore(file, serBean.GetIgnoreDir())).ToList<string>();
        {
            //获得关卡文件路径, 要移出
            //应该是要5个文件, 如果不是需要记录.
            string[] bgmSrc = Directory.GetFiles(pi.path + serBean.GetBgmPath(), idList[idcCurser], SearchOption.AllDirectories);
            string idAfter = idList[idcCurser].Substring(1, idList[idcCurser].Length - 1);
            var searchPattern = new Regex(@"\S+_\d" + idAfter + ".xml", RegexOptions.IgnoreCase);
            List<string> xmlFiles = classSrc.Where(file => searchPattern.IsMatch(file)).ToList<string>();

            //开始移出
            moveList.AddRange(MoveCheckPoint(pi.path, bgmSrc, xmlFiles, serBean));
        }
        idcCurser++;
        if (idList.Count <= idcCurser)
        {
            srcCurser++;
            if (serBean.src.Count > srcCurser)
            {
                idcCurser = 0;
            }
            else
            {
                DeleteSrc();
                inCopy = false;
            }
        }
        Repaint();
        //return;

    }
    private void DeleteSrc()
    {
        if (serBean.isMove)
        {
            moveList = moveList.Distinct().ToList();
            if (!ResourceMoveHelper.InTest)
            {
                for (int i = 0; i < moveList.Count; i++)
                {
                    System.IO.File.SetAttributes(moveList[i], System.IO.FileAttributes.Normal);
                    System.IO.File.Delete(moveList[i]);
                }
            }
        }
    }
}