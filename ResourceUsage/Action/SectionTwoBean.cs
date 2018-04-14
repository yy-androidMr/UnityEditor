using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Reflection;
class ActionTwoBean
{
    public string fullName;//F:\p4_workspace\DGM\x5_mobile\mobile_dancer_resource\Resources\美术资源\动作资源\动作\action-expression\sd_135bpm_heartbreaker_09_1.aem.asset
    public string simpleName;
    public string meta;
    public void SetFullName(string path)
    {
        fullName = path.ToLower();
        meta = fullName + ".meta";
        if (!File.Exists(meta))
        {
            meta = "";
        }
        simpleName = System.IO.Path.GetFileName(fullName);
    }
}
//class ActionBean
//{
//    public string simpleName;
//    public string fullName;
//    public string actionConfigName;
//    public void SetFullName(string path)
//    {
//        fullName = path.ToLower();
//        //获取了文件名
//        simpleName = System.IO.Path.GetFileNameWithoutExtension(fullName);
//        //var maches = Regex.Match(simpleName, ".+\\d");
//        //int iLastNumberIndex = maches.Length - 1;
//        actionConfigName = Regex.Replace(simpleName, @"(?<=\d)[^\d]*$", "");
//    }
//}
#region 根据已有局内动作删除action-expression内容
class SectionTwoBean : ActionFindInBase
{
    private Dictionary<string, List<ActionTwoBean>> action_expression_idr;
    private List<string> action_expression_str = new List<string>();
    //dance.xls
    private List<string> dance_xls;
    List<string> dance_xls_use = new List<string>();//
    List<string> dance_xls_not_use = new List<string>();//
    List<string> dance_xls_use_not_file = new List<string>();//
    public override void Do(int currentPro)
    {
        //ActionTwoBean atb = action_expression[currentPro];
        ////ingame_dance_actions_anim 文件夹里面找expression对应.
        //if (ingame_dance_actions_anim.Contains(atb.regexMatch))
        //{
        //    ingame_dance_actions_anim_use.Add(atb.fullName);
        //}

        //查找配置表. dances.xls
        //if (dance_xls.Contains(atb.regexMatch))
        //{
        //    dance_xls_use.Add(atb.fullName);
        //}
        //else
        //{
        //    dance_xls_not_use.Add(atb.fullName);
        //}
        dance_xls_use = action_expression_str.FindAll(x => dance_xls.Contains(x));//寻找正确的

        dance_xls_not_use = action_expression_str.FindAll(x => !dance_xls.Contains(x));//寻找该删除的

        dance_xls_use_not_file = dance_xls.FindAll(x => !action_expression_str.Contains(x));//寻找. 表里面有的,但是文件没有
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
        return 1;
    }
    public override string WriteName()
    {
        return "action-expression";
    }

    public override List<string> GetProcessFinishContent(int currentProgress)
    {
        List<string> content = new List<string>();
        int count = 0;
        for (int i = 0; i < dance_xls_not_use.Count; i++)
        {
            List<ActionTwoBean> beanList = action_expression_idr[dance_xls_not_use[i]];
            for (int j = 0; j < beanList.Count; j++)
            {
                content.Add("," + beanList[j].fullName);
                //if (beanList[j].fullName != "")
                //{
                //    content.Add("," + beanList[j].meta);
                //}
            }
            count += beanList.Count;
        }
        content.Add("配置表没有,共:" + count + "个文件");
        content.Add("");
        for (int i = 0; i < dance_xls_use_not_file.Count; i++)
        {
            content.Add("," + dance_xls_use_not_file[i]);
        }
        content.Add("配置表有,action-expression没有,共:" + dance_xls_use_not_file.Count + "个文件");
        content.Add("");

        count = 0;
        for (int i = 0; i < dance_xls_use.Count; i++)
        {
            List<ActionTwoBean> beanList = action_expression_idr[dance_xls_use[i]];
            for (int j = 0; j < beanList.Count; j++)
            {
                content.Add("," + beanList[j].fullName);
            }
            count += beanList.Count;
        }
        content.Add("对应正确,共:" + count + "个文件");
        return content;
    }
    public override void Init(List<string> configList, object[] nextInfo)
    {
        string action_path = configList[0];
        string[] action_expression_source = Directory.GetFiles(action_path + "/action-expression", "sd*.asset", SearchOption.AllDirectories);
        //string[] action_source_txt = Directory.GetFiles(action_path + "/ingame_dance_actions_anim", "*.txt", SearchOption.AllDirectories);

        //组织原始action文件列表
        action_expression_idr = new Dictionary<string, List<ActionTwoBean>>();
        for (int i = 0; i < action_expression_source.Length; i++)
        {
            //ActionBean ab = new ActionBean();
            //ab.SetFullName(action_expression_source[i]);
            string regexMatch = System.IO.Path.GetFileNameWithoutExtension(action_expression_source[i]).Split('.')[0].ToLower();
            List<ActionTwoBean> actionList;
            if (action_expression_idr.TryGetValue(regexMatch, out actionList))
            {

            }
            else
            {
                actionList = new List<ActionTwoBean>();
                action_expression_idr.Add(regexMatch, actionList);
                action_expression_str.Add(regexMatch);
            }
            ActionTwoBean atb = new ActionTwoBean();
            atb.SetFullName(action_expression_source[i]);
            actionList.Add(atb);
        }
        //
        action_expression_str = action_expression_str.Distinct().ToList();

        //1 :配置表
        dance_xls = new List<string>();
        string ingame_dance_action_config_path = configList[1];
        string[] allLine = File.ReadAllLines(ingame_dance_action_config_path);
        for (int i = 0; i < allLine.Length; i++)
        {
            dance_xls.Add(allLine[i].ToLower());
        }

        ////组织 assetsbundle
        //string[] bundleFile = Directory.GetFiles(@"assetbundles\cdn\assetbundles\art\role\actions\ingame_dance_actions_anim", "*", SearchOption.AllDirectories);

    }
}
#endregion