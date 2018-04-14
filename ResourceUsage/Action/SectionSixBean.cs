using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Reflection;
class ActionSixBean
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
        simpleName = System.IO.Path.GetFileNameWithoutExtension(fullName).ToLower();
        Match match = Regex.Match(simpleName, @"(?<=\d{2,11}_)[\S]*$");
        simpleName = match.Value;
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
#region actions_anim_expression 局外表情:根据表情名,去actions_anim找文件,如果找不到则可以删除
class SectionSixBean : ActionFindInBase
{
    private List<ActionSixBean> useList = new List<ActionSixBean>();
    private List<ActionSixBean> unUse = new List<ActionSixBean>();
    public override void Do(int currentPro)
    {
        useList = beanList.FindAll(x => action_animList.Contains(x.simpleName));//寻找正确的
        unUse = beanList.FindAll(x => !action_animList.Contains(x.simpleName));//寻找没有的

        //action_anim_source.Contains
        //ActionSixBean    beanList[currentPro];
        //读取action-expression的所有表情
        //string path = action_expression[currentPro];
        //string allText = File.ReadAllText(path);
        //string regex = @"(?<=m_expressionName:) ?[\S]+\r\n";
        //Match match = Regex.Match(allText, regex);
        //string value = match.Value;
        //value = value.Replace("\r\n", "").Trim();
        //List<string> pathList;
        //if (action_expressionDic.TryGetValue(value, out pathList))
        //{

        //}
        //else
        //{
        //    pathList = new List<string>(); ;
        //    action_expressionDic.Add(value, pathList);
        //    action_expressionDic_key_str.Add(value);
        //    if (System.IO.Path.GetFileName(path).StartsWith("sd"))
        //    {
        //        action_expression_ingameDance_Dic.Add(value, pathList);
        //    }
        //}
        //pathList.Add(path);
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
        return "actions_anim_expression";
    }

    public override List<string> GetProcessFinishContent(int currentProgress)
    {

        List<string> content = new List<string>();
        for (int i = 0; i < unUse.Count; i++)
        {
            content.Add("," + unUse[i].fullName);
            content.Add("," + unUse[i].meta);

        }
        content.Add("没有在用共:" + unUse.Count);
        content.Add("");

        for (int i = 0; i < useList.Count; i++)
        {
            content.Add("," + useList[i].fullName);

        }
        content.Add("在用共:" + useList.Count);
        return content;
        //    List<ActionThreeBean> unUse = expressionList.FindAll(x => !action_expressionDic_key_str.Contains(x.regexName));//寻找不正确的
        //    for (int i = 0; i < unUse.Count; i++)
        //    {
        //        content.Add("," + unUse[i].fullName);
        //        if (!string.IsNullOrEmpty(unUse[i].meta))
        //        {
        //            content.Add("," + unUse[i].meta);
        //        }
        //    }
        //    content.Add("要删除,共:" + unUse.Count + "个文件");
        //    content.Add("");
        //    List<ActionThreeBean> use = expressionList.FindAll(x => action_expressionDic_key_str.Contains(x.regexName));//寻找正确的
        //    for (int i = 0; i < use.Count; i++)
        //    {
        //        content.Add("," + use[i].fullName);
        //        List<string> pathList = action_expressionDic[use[i].regexName];
        //        for (int j = 0; j < pathList.Count; j++)
        //        {
        //            content.Add(",," + pathList[j]);
        //        }
        //    }
        //    content.Add("有用的,共:" + use.Count + "个文件");
        //    content.Add("");

        //    content.Add("其中局内用到的舞蹈动作表情为");
        //    foreach (var item in action_expression_ingameDance_Dic)
        //    {
        //        content.Add("," + item.Key);
        //        //for (int j = 0; j < item.Value.Count; j++)
        //        //{
        //        //    content.Add(",," + item.Value[j]);
        //        //}
        //    }
        //    return content;
        //}
    }

    private List<ActionSixBean> beanList;
    private List<string> action_animList;
    public override void Init(List<string> configList, object[] nextInfo)
    {
        string action_path = configList[0];
        string[] action_anim_expression = Directory.GetFiles(action_path + "/actions_anim_expression", "*.anim", SearchOption.AllDirectories);
        beanList = new List<ActionSixBean>();
        for (int i = 0; i < action_anim_expression.Length; i++)
        {
            ActionSixBean asb = new ActionSixBean();
            asb.SetFullName(action_anim_expression[i]);
            beanList.Add(asb);
        }
        string[] action_anim_source = Directory.GetFiles(action_path + "/actions_anim", "*.anim", SearchOption.AllDirectories);
        action_animList = new List<string>();
        for (int i = 0; i < action_anim_source.Length; i++)
        {
            string key = System.IO.Path.GetFileNameWithoutExtension(action_anim_source[i]).ToLower();
            action_animList.Add(key);
        }
    }


}
#endregion