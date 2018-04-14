using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Reflection;
class ActionSevenBean
{
    public string simpleName;
    public List<string> fullNameList = new List<string>();
    public void SetFullName(string fullPath)
    {
        fullNameList.Add(fullPath.ToLower());
    }
}

#region actions_anim_expression 局外表情:根据表情名,去actions_anim找文件,如果找不到则可以删除
class SectionSevenBean : ActionFindInBase
{
    private List<ActionSevenBean> use = new List<ActionSevenBean>();
    private List<ActionSevenBean> needAdd = new List<ActionSevenBean>();
    private List<ActionSevenBean> unUse = new List<ActionSevenBean>();
    public override void Do(int currentPro)
    {
        string[] ab_source = Directory.GetFiles(sourceFilePath + @"\android\assetbundles\art\role\actions", "*", SearchOption.AllDirectories);
        //string[] ab_source = Directory.GetFiles(@"assetbundles\cdn\assetbundles\art\role\actions", "*", SearchOption.AllDirectories);
        string pre_path = Environment.CurrentDirectory;
        abList = new List<ActionSevenBean>();
        for (int i = 0; i < ab_source.Length; i++)
        {
            string item = ab_source[i];
            //string simpleName = Regex.Replace(System.IO.Path.GetFileNameWithoutExtension(item), @"(?<=\d)[^\d]*$", "").ToLower();
            string simpleName = System.IO.Path.GetFileNameWithoutExtension(item).Split('.')[0].ToLower();

            ActionSevenBean asb = abList.Find(x => x.simpleName == simpleName);
            if (asb == null)
            {
                asb = new ActionSevenBean();
                asb.simpleName = simpleName;
                abList.Add(asb);
            }
            asb.SetFullName(item);
        }
        string[] art_root_source = Directory.GetFiles(action_path, "*", SearchOption.AllDirectories);
        //art_root_source = new string[] { @"f:\p4_workspace\dgm\x5_mobile\mobile_dancer_resource\resources\美术资源\动作资源\动作\ingame_dance_actions_anim\sd_100bpm_callmebaby_02_2_nrot.anim" };
        //0000000101_shequ_luodi_zhengzha_02_m
        art_root_List = new List<ActionSevenBean>();
        for (int i = 0; i < art_root_source.Length; i++)
        {
            string item = art_root_source[i];
            if (item.Contains("action-expression"))
            {
                continue;
            }
            string fileName = System.IO.Path.GetFileName(item).Split('.')[0].ToLower();
            string simpleName;// Regex.Replace(System.IO.Path.GetFileNameWithoutExtension(item), @"(?<=\d)[^\d]*$", "").ToLower();
            if (fileName.EndsWith("_root"))
            {
                simpleName = fileName.Replace("_root", "");
            }
            else if (fileName.EndsWith("_nrot"))
            {
                simpleName = fileName.Replace("_nrot", "");
            }
            else if (fileName.EndsWith("_high_m"))
            {
                simpleName = fileName.Replace("_high_m", "");
            }
            else if (fileName.EndsWith("_high_f"))
            {
                simpleName = fileName.Replace("_high_f", "");
            }
            else if (fileName.EndsWith("_high"))
            {
                simpleName = fileName.Replace("_high", "");
            }
            else
            {
                simpleName = fileName;
            }
            //Debug.Log("simpleName:" + simpleName);
            ActionSevenBean asb = art_root_List.Find(x => x.simpleName == simpleName);
            if (asb == null)
            {
                asb = new ActionSevenBean();
                asb.simpleName = simpleName;
                art_root_List.Add(asb);
            }
            asb.SetFullName(item);
        }


        use = abList.FindAll(x => art_root_List.Exists(i => i.simpleName.Equals(x.simpleName)));
        unUse = abList.FindAll(x => !art_root_List.Exists(i => i.simpleName.Equals(x.simpleName)));
        needAdd = art_root_List.FindAll(x => !use.Exists(i => i.simpleName.Equals(x.simpleName)));
    }

    public override void ReleaseMe()
    {
    }

    public override string CurrentPrecessName(int currentProgress)
    {
        return "比对" + WriteName() + "中...";
    }

    public override int MaxValue()
    {
        //return action_expression.Count;
        return 1;
    }
    public override string WriteName()
    {
        return "action_ab";
    }

    public override Dictionary<string, List<string>> GetProcessFinishNewContent(int currentProgress)
    {
        Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();
        List<string> content = new List<string>();

        content.Add("要删除共:" + unUse.Count);
        List<string> mapValue = new List<string>();
        dic.Add("要删除(局内舞蹈动作).xls", mapValue);
        for (int i = 0; i < unUse.Count; i++)
        {
            content.Add(unUse[i].simpleName);
            for (int j = 0; j < unUse[i].fullNameList.Count; j++)
            {
                if (unUse[i].fullNameList[j].Contains("ingame_dance_actions_anim"))
                {
                    mapValue.Add(unUse[i].fullNameList[j]);
                    mapValue.Add(unUse[i].fullNameList[j].Replace("android", "iOS"));
                }
                content.Add(unUse[i].fullNameList[j]);
            }
        }
        mapValue = new List<string>();
        dic.Add("要删除(非局内舞蹈动作).xls", mapValue);
        for (int i = 0; i < unUse.Count; i++)
        {
            for (int j = 0; j < unUse[i].fullNameList.Count; j++)
            {
                if (!unUse[i].fullNameList[j].Contains("ingame_dance_actions_anim"))
                {
                    mapValue.Add(unUse[i].fullNameList[j]);
                    mapValue.Add(unUse[i].fullNameList[j].Replace("android", "iOS"));
                }
            }
        }

        content.Add("需要生成ab文件:" + needAdd.Count);
        mapValue = new List<string>();
        dic.Add("需要生成ab.xls", mapValue);
        for (int i = 0; i < needAdd.Count; i++)
        {
            content.Add(needAdd[i].simpleName);
            for (int j = 0; j < needAdd[i].fullNameList.Count; j++)
            {
                mapValue.Add(needAdd[i].fullNameList[j]);
                content.Add(needAdd[i].fullNameList[j]);
            }
        }

        content.Add("使用共:" + use.Count);
        mapValue = new List<string>();
        dic.Add("在使用的.xls", mapValue);
        for (int i = 0; i < use.Count; i++)
        {
            content.Add(use[i].simpleName);
            for (int j = 0; j < use[i].fullNameList.Count; j++)
            {
                mapValue.Add(use[i].fullNameList[j]);
                content.Add(use[i].fullNameList[j]);
            }
        }
        dic.Add(WriteName() + ".txt", content);

        return dic;
    }

    private List<ActionSevenBean> abList;
    private List<ActionSevenBean> art_root_List;
    string action_path;
    string sourceFilePath;
    public override void Init(List<string> configList, object[] nextInfo)
    {
        action_path = configList[0];
        sourceFilePath = configList[2];
    }

}
#endregion