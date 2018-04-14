using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

public class UIAB_DanceProcess : LoadingProcess
{
    private int process = 0;
    private string dance_content;
    private string m_sourcePath;
    private string m_artPath;
    //private string m_xlsPath;
    public UIAB_DanceProcess(string sourcePath, string artPath)
        : base("ab局内动作一致性")
    {
        m_sourcePath = sourcePath;
        m_artPath = artPath;
        //m_xlsPath = xlsPath;


        max = 1;
        Do = DoSomeThing;
        Name = GetCurrentProcessName;
        Finish = OnFinish;
    }
    public bool OnFinish()
    {
        //List<string> android_unuse;
        //List<string> ios_unuse;
        //List<string> android_need_add;
        //List<string> ios_need_add;
        List<string> android_un = new List<string>();
        for (int i = 0; i < android_unuse.Count; i++)
        {
            for (int j = 0; j < android_unuse[i].pathList.Count; j++)
            {
                android_un.Add(android_unuse[i].pathList[j]);
            }
        }
        ResourceMoveHelper.WriteOnly(GetMyType() + "/安卓ab删除", android_un);

        List<string> ios_un = new List<string>();
        for (int i = 0; i < ios_unuse.Count; i++)
        {
            for (int j = 0; j < ios_unuse[i].pathList.Count; j++)
            {
                ios_un.Add(ios_unuse[i].pathList[j]);
            }
        }
        ResourceMoveHelper.WriteOnly(GetMyType() + "/ios_ab删除", ios_un);
        ResourceMoveHelper.WriteOnly(GetMyType() + "/安卓ab需添加", android_need_add);
        ResourceMoveHelper.WriteOnly(GetMyType() + "/ios_ab需添加", ios_need_add);


        //List<string> xlsList = new List<string>();
        //string[] allLine = File.ReadAllLines(m_xlsPath);
        //for (int i = 0; i < allLine.Length; i++)
        //{
        //    xlsList.Add(allLine[i].ToLower());
        //}

        //List<string> xls_add = dance_xml_actions.FindAll(x => !xlsList.Contains(x));
        //List<string> xml_add = xlsList.FindAll(x => !dance_xml_actions.Contains(x));
        //ResourceUsageCheck.WriteOnly(GetMyType() + "/xls需要补充", xls_add, WriteType.TEXT);
        //ResourceUsageCheck.WriteOnly(GetMyType() + "/xml需要补充", xml_add, WriteType.TEXT);



        List<string> actionmap_add = danceIds.FindAll(x => !actionmapIds.Contains(x));
        List<string> dance_xml_add = actionmapIds.FindAll(x => !danceIds.Contains(x));
        ResourceMoveHelper.WriteOnly(GetMyType() + "/actionmap有,dancexml没有", dance_xml_add, WriteType.TEXT);
        ResourceMoveHelper.WriteOnly(GetMyType() + "/dancexml有,actionmap没有", actionmap_add, WriteType.TEXT);
        return true;

    }
    public string GetCurrentProcessName()
    {
        return "比对ab和dance.xml中...";
        //return sectionName[GetCurrentProcess()];
    }
    List<ABBean> android_unuse;
    List<ABBean> ios_unuse;
    List<string> android_need_add;
    List<string> ios_need_add;
    List<string> dance_xml_actions;

    List<string> danceIds;
    List<string> actionmapIds;


    //比对 dance.xml 和actionmap.xml

    public void DoSomeThing()
    {
        //string[] android_ab = new string[] { @"F:\p4_workspace\DGM\x5_mobile\mobile_dancer_resource\Resources\ResourcePublish\CDN\SourceFiles\android\assetbundles\art\role\actions\ingame_dance_actions_anim\sd_123bpm_lovesong_43_2" };
        string[] android_ab = Directory.GetFiles(m_sourcePath + @"\android\assetbundles\art\role\actions\ingame_dance_actions_anim", "*", SearchOption.AllDirectories);
        string[] ios_ab = Directory.GetFiles(m_sourcePath + @"\iOS\assetbundles\art\role\actions\ingame_dance_actions_anim", "*", SearchOption.AllDirectories);
        List<ABBean> android_list = new List<ABBean>();
        for (int i = 0; i < android_ab.Length; i++)
        {
            string simpleName = System.IO.Path.GetFileNameWithoutExtension(android_ab[i]).ToLower();
            ABBean tempBean = android_list.Find(x => x.simpleName.Equals(simpleName));
            if (tempBean == null)
            {
                tempBean = new ABBean();
                tempBean.simpleName = simpleName;
                android_list.Add(tempBean);
            }
            tempBean.AddPath(android_ab[i]);
        }
        List<ABBean> ios_list = new List<ABBean>();
        for (int i = 0; i < ios_ab.Length; i++)
        {
            string simpleName = System.IO.Path.GetFileNameWithoutExtension(ios_ab[i]).ToLower();
            ABBean tempBean = ios_list.Find(x => x.simpleName.Equals(simpleName));
            if (tempBean == null)
            {
                tempBean = new ABBean();
                tempBean.simpleName = simpleName;
                ios_list.Add(tempBean);
            }
            tempBean.AddPath(ios_ab[i]);
        }


        string dance_xml = m_sourcePath + @"\crossplatform\config\action_config\dance.xml";
        dance_content = File.ReadAllText(dance_xml).ToLower();

        //这是在ab上该删除的

        android_unuse = android_list.FindAll(x => !dance_content.Contains(x.simpleName));
        ios_unuse = ios_list.FindAll(x => !dance_content.Contains(x.simpleName));

        //这是在ab上该添加的
        MatchCollection mc = Regex.Matches(dance_content, "(?<=<item id=\").+(?=\")");
        dance_xml_actions = new List<string>();
        for (int i = 0; i < mc.Count; i++)
        {
            string value = mc[i].Value;
            dance_xml_actions.Add(value);
        }
        android_need_add = dance_xml_actions.FindAll(x => android_list.Find(y => y.simpleName.Equals(x)) == null);
        ios_need_add = dance_xml_actions.FindAll(x => ios_list.Find(y => y.simpleName.Equals(x)) == null);



        //比对actionmap.xml
        //dance_content
        var searchPattern = new Regex("id=\"\\S+\"", RegexOptions.IgnoreCase);
        MatchCollection dcmc = searchPattern.Matches(dance_content);
        danceIds = new List<string>();
        foreach (Match item in dcmc)
        {
            danceIds.Add(item.Value.Replace("\"", "").Replace("id=", ""));
        }

        string actionmap_xml = m_sourcePath + @"\crossplatform\config\role\actionmap.xml";
        string actionmap_content = File.ReadAllText(actionmap_xml).ToLower();
        searchPattern = new Regex("name=\"sd_\\S+\"", RegexOptions.IgnoreCase);
        dcmc = searchPattern.Matches(actionmap_content);
        actionmapIds = new List<string>();
        foreach (Match item in dcmc)
        {
            actionmapIds.Add(item.Value.Replace("\"", "").Replace("name=", ""));
        }



    }

    private class ABBean
    {
        public string simpleName;
        public List<string> pathList = new List<string>();
        public void AddPath(string fullpath)
        {
            pathList.Add(fullpath);
        }
    }
}