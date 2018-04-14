using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Reflection;
class ActionThreeBean
{
    public string fullName;//F:\p4_workspace\DGM\x5_mobile\mobile_dancer_resource\Resources\美术资源\动作资源\动作\action-expression\sd_135bpm_heartbreaker_09_1.aem.asset
    public string simpleName;
    public string regexName;
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
        regexName = simpleName.Split('.')[0];
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
#region 根据已有的action-expression删除expression内容,删除完 之后,需要从action-expression读取sd开头的文件,获取表情数量.书写数量.
class SectionThreeBean : ActionFindInBase
{
    public override void Do(int currentPro)
    {
        //读取action-expression的所有表情
        string path = action_expression[currentPro];
        string allText = File.ReadAllText(path);
        string regex = @"(?<=m_expressionName:) ?[\S]+\r\n";
        Match match = Regex.Match(allText, regex);
        string value = match.Value;
        value = value.Replace("\r\n", "").Trim();
        List<string> pathList;
        if (action_expressionDic.TryGetValue(value, out pathList))
        {

        }
        else
        {
            pathList = new List<string>(); ;
            action_expressionDic.Add(value, pathList);
            action_expressionDic_key_str.Add(value);
            if (System.IO.Path.GetFileName(path).StartsWith("sd"))
            {
                action_expression_ingameDance_Dic.Add(value, pathList);
            }
        }
        pathList.Add(path);
    }

    public override void ReleaseMe()
    {
        expressionList.Clear();
        action_expression.Clear();
        action_expressionDic.Clear();
        action_expressionDic_key_str.Clear();
        action_expression_ingameDance_Dic.Clear();
    }

    public override string CurrentPrecessName(int currentProgress)
    {
        return "比对" + WriteName() + "中...";
    }

    public override int MaxValue()
    {
        return action_expression.Count;
    }
    public override string WriteName()
    {
        return "expression";
    }

    public override List<string> GetProcessFinishContent(int currentProgress)
    {
        bool isFinish = currentProgress >= action_expression.Count;
        if (isFinish)
        {
            List<string> content = new List<string>();

            List<ActionThreeBean> unUse = expressionList.FindAll(x => !action_expressionDic_key_str.Contains(x.regexName));//寻找不正确的
            for (int i = 0; i < unUse.Count; i++)
            {
                content.Add("," + unUse[i].fullName);
                if (!string.IsNullOrEmpty(unUse[i].meta))
                {
                    content.Add("," + unUse[i].meta);
                }
            }
            content.Add("要删除,共:" + unUse.Count + "个文件");
            content.Add("");
            List<ActionThreeBean> use = expressionList.FindAll(x => action_expressionDic_key_str.Contains(x.regexName));//寻找正确的
            for (int i = 0; i < use.Count; i++)
            {
                content.Add("," + use[i].fullName);
                List<string> pathList = action_expressionDic[use[i].regexName];
                for (int j = 0; j < pathList.Count; j++)
                {
                    content.Add(",," + pathList[j]);
                }
            }
            content.Add("有用的,共:" + use.Count + "个文件");
            content.Add("");

            content.Add("其中局内用到的舞蹈动作表情为");
            foreach (var item in action_expression_ingameDance_Dic)
            {
                content.Add("," + item.Key);
                //for (int j = 0; j < item.Value.Count; j++)
                //{
                //    content.Add(",," + item.Value[j]);
                //}
            }
            return content;
        }
        return null;
    }
    private List<ActionThreeBean> expressionList;
    private List<string> action_expression;
    private Dictionary<string, List<string>> action_expressionDic;//所有的
    private List<string> action_expressionDic_key_str;
    private Dictionary<string, List<string>> action_expression_ingameDance_Dic;//局内的
    public override void Init(List<string> configList, object[] nextInfo)
    {
        string action_path = configList[0];
        string[] expressionSource = Directory.GetFiles(action_path + "/expression", "*.anim", SearchOption.AllDirectories);
        string[] action_expression_source = Directory.GetFiles(action_path + "/action-expression", "*.asset", SearchOption.AllDirectories);
        action_expressionDic = new Dictionary<string, List<string>>();
        action_expressionDic_key_str = new List<string>();
        action_expression_ingameDance_Dic = new Dictionary<string, List<string>>();
        action_expression = new List<string>();
        for (int i = 0; i < action_expression_source.Length; i++)
        {
            action_expression.Add(action_expression_source[i].ToLower());

        }
        //string[] action_source_txt = Directory.GetFiles(action_path + "/ingame_dance_actions_anim", "*.txt", SearchOption.AllDirectories);
        expressionList = new List<ActionThreeBean>();
        for (int i = 0; i < expressionSource.Length; i++)
        {
            ActionThreeBean atb = new ActionThreeBean();
            atb.SetFullName(expressionSource[i].ToLower());
            expressionList.Add(atb);
        }
        ////组织 assetsbundle
        //string[] bundleFile = Directory.GetFiles(@"assetbundles\cdn\assetbundles\art\role\actions\ingame_dance_actions_anim", "*", SearchOption.AllDirectories);

    }
}
#endregion