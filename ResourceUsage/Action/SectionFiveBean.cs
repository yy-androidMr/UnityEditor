using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Reflection;
class ActionFiveBean
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
#region actions_anim
class SectionFiveBean : ActionFindInBase
{
    private Dictionary<string, List<string>> configuse = new Dictionary<string, List<string>>();
    private Dictionary<string, List<string>> codeuse = new Dictionary<string, List<string>>();
    private List<string> config_notuse = new List<string>();
    private List<string> code_notuse = new List<string>();
    bool inconfig = true;
    public override void Do(int currentPro)
    {
        if (currentPro < config_list.Count)
        {
            inconfig = true;
            string path = config_list[currentPro].FullName;
            string allText = File.ReadAllText(path).ToLower();
            for (int i = 0; i < no_dance_action_animDic_keys.Count; i++)
            {
                string key = no_dance_action_animDic_keys[i];
                if (allText.Contains(key))
                {
                    List<string> configList;
                    if (configuse.TryGetValue(key, out configList))
                    {

                    }
                    else
                    {
                        configList = new List<string>();
                        configuse.Add(key, configList);
                    }
                    configList.Add(path);
                }
            }
        }
        else
        {
            //cs_source
            if (inconfig)
            {
                inconfig = false;
                for (int i = 0; i < no_dance_action_animDic_keys.Count; i++)
                {
                    if (!configuse.ContainsKey(no_dance_action_animDic_keys[i]))
                    {
                        config_notuse.Add(no_dance_action_animDic_keys[i]);
                    }
                }
            }

            string path = cs_source[currentPro - config_list.Count];
            string allText = File.ReadAllText(path).ToLower();
            for (int i = 0; i < config_notuse.Count; i++)
            {
                string key = config_notuse[i];
                if (allText.Contains("\"" + key))
                {
                    List<string> codeList;
                    if (codeuse.TryGetValue(key, out codeList))
                    {

                    }
                    else
                    {
                        codeList = new List<string>();
                        codeuse.Add(key, codeList);
                    }
                    codeList.Add(path);
                }
                else
                {
                    //if (!code_notuse.Contains(key))
                    //{
                    //    code_notuse.Add(key);
                    //}
                }
            }
        }
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
        return config_list.Count + cs_source.Length;
    }
    public override string WriteName()
    {
        return "actions_anim";
    }

    public override List<string> GetProcessFinishContent(int currentProgress)
    {
        bool isFinish = currentProgress >= config_list.Count + cs_source.Length;
        if (isFinish)
        {

            for (int i = 0; i < config_notuse.Count; i++)
            {
                if (!codeuse.ContainsKey(config_notuse[i]))
                {
                    code_notuse.Add(config_notuse[i]);
                }
            }


            List<string> content = new List<string>();
            for (int i = 0; i < code_notuse.Count; i++)
            {
                content.Add("," + code_notuse[i]);

                List<string> files = action_animDic[code_notuse[i]];
                for (int j = 0; j < files.Count; j++)
                {
                    content.Add(",," + files[j]);
                    if (File.Exists(files[j] + ".meta"))
                    {
                        content.Add(",," + files[j] + ".meta");
                    }
                }
            }
            content.Add("没有在用共:" + code_notuse.Count);
            content.Add("");
            string[] no_dance_action_expression_source = null;
            if (ResourceUsageCheck.isXULIANG_LOOK)
            {
                no_dance_action_expression_source = Directory.GetFiles(action_path + "/actions_anim_expression", "*.anim", SearchOption.AllDirectories);
            }

            foreach (var item in codeuse)
            {

                if (ResourceUsageCheck.isXULIANG_LOOK)
                {
                    List<string> files = action_animDic[item.Key];
                    for (int j = 0; j < files.Count; j++)
                    {
                        string fileName = System.IO.Path.GetFileNameWithoutExtension(files[j]);

                        string expression = "";
                        for (int i = 0; i < no_dance_action_expression_source.Length; i++)
                        {
                            if (no_dance_action_expression_source[i].Contains(fileName))
                            {
                                expression = "," + System.IO.Path.GetFileNameWithoutExtension(no_dance_action_expression_source[i]);
                                break;
                            }
                        }


                        content.Add(fileName + expression);
                    }
                }
                else
                {
                    content.Add("," + item.Key);
                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        content.Add(",," + item.Value[i]);
                    }
                }
            }
            content.Add("代码在用共:" + codeuse.Count);

            content.Add("");
            foreach (var item in configuse)
            {
                if (ResourceUsageCheck.isXULIANG_LOOK)
                {
                    List<string> files = action_animDic[item.Key];
                    for (int j = 0; j < files.Count; j++)
                    {
                        string fileName = System.IO.Path.GetFileNameWithoutExtension(files[j]);

                        string expression = "";
                        for (int i = 0; i < no_dance_action_expression_source.Length; i++)
                        {
                            if (no_dance_action_expression_source[i].Contains(fileName))
                            {
                                expression = "," + System.IO.Path.GetFileNameWithoutExtension(no_dance_action_expression_source[i]);
                                break;
                            }
                        }

                        content.Add(fileName + expression);
                    }
                }
                else
                {
                    content.Add("," + item.Key);
                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        content.Add(",," + item.Value[i]);
                    }
                }
            }
            content.Add("配置在用共:" + configuse.Count);
            return content;
        }
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
        return null;
    }
    private Dictionary<string, List<string>> action_animDic;//局外所有的
    List<string> no_dance_action_animDic_keys;
    private List<FileInfo> config_list;
    string[] cs_source;
    private List<string> notInRule;
    string action_path;
    public override void Init(List<string> configList, object[] nextInfo)
    {
        action_path = configList[0];
        string[] action_anim_source = Directory.GetFiles(action_path + "/actions_anim", "*.anim", SearchOption.AllDirectories);
        action_animDic = new Dictionary<string, List<string>>();
        no_dance_action_animDic_keys = new List<string>();
        notInRule = new List<string>();
        Dictionary<string, int> reChangeDic = new Dictionary<string, int>();
        for (int i = 0; i < configList.Count; i++)
        {
            if (configList[i].StartsWith("regex5:"))
            {
                //证明需要重组
                string[] splitValue = configList[i].Split(':');
                reChangeDic.Add(splitValue[2], int.Parse(splitValue[1]));
            }
        }

        string _f = "_f.anim";
        string _m = "_m.anim";
        for (int i = 0; i < action_anim_source.Length; i++)
        {
            string key = System.IO.Path.GetFileNameWithoutExtension(action_anim_source[i]).ToLower();

            if (key.EndsWith("_f"))
            {
                key = key.Substring(0, key.Length - 2);
            }
            if (key.EndsWith("_m"))
            {
                key = key.Substring(0, key.Length - 2);
            }

            key = ReChangeKey(reChangeDic, key);
            Add(key, action_anim_source[i]);
            //if (key.EndsWith(_f))
            //{
            //    key = key.Replace(_f, "");//2v2_jiesuan_l
            //    //这里需要对key 做重组. <regex delete="2">2v2_shwotime_wudao01_</regex> 配置上有的.过滤
            //}
            //else if (key.EndsWith(_m))
            //{
            //    key = key.Replace(_m, "");//2v2_jiesuan_l
            //    key = ReChangeKey(reChangeDic, key);
            //    Add(key, action_anim_source[i]);
            //}
            //else
            //{
            //    notInRule.Add(key);
            //}
        }
        config_list = new List<FileInfo>();
        GetAllFile(@"assetbundles\cdn\assetbundles\config", config_list);

        //F:\p4_workspace\DGM\x5_mobile\mobile_dancer\trunk\client\Assets\Scripts\CoreGame\Render\ModeSpecial\ModeTeamArena\CoreGameActionMngTeamArena.cs
        cs_source = Directory.GetFiles(Application.dataPath + "/Scripts", "*.cs", SearchOption.AllDirectories);
        //cs_source = new string[] { @"F:\p4_workspace\DGM\x5_mobile\mobile_dancer\trunk\client\Assets\Scripts\CoreGame\Render\ModeSpecial\ModeTeamArena\CoreGameActionMngTeamArena.cs" };
    }

    private string ReChangeKey(Dictionary<string, int> rechange, string key)
    {

        //2v2_kaichang_01_1  key
        //<regex delete="1">2v2_kaichang_01_</regex> rechange
        //2v2_kaichang_01  end
        foreach (var item in rechange)
        {
            if (key.StartsWith(item.Key))
            {
                if (item.Value > 50)
                {
                    return null;
                }
                if (item.Value == 20)
                {
                    return item.Key;
                }
                for (int i = 0; i < item.Value; i++)
                {
                    key = key.Substring(0, key.LastIndexOf('_'));
                }
                break;
            }
        }
        return key;
    }
    private void Add(string key, string value)
    {
        if (string.IsNullOrEmpty(key))
        {
            return;
        }
        List<string> values;
        if (action_animDic.TryGetValue(key, out values))
        {

        }
        else
        {
            values = new List<string>();
            action_animDic.Add(key, values);
            no_dance_action_animDic_keys.Add(key);
        }

        values.Add(value);
    }



    private static void GetAllFile(string dir, List<FileInfo> fileList)
    {
        DirectoryInfo theFolder = new DirectoryInfo(dir);
        FileInfo[] fileInfo = theFolder.GetFiles();
        foreach (FileInfo NextFile in fileInfo)  //遍历文件  
        {
            if (NextFile.Name.EndsWith(".xml"))
            {
                fileList.Add(NextFile);
            }
        }
        DirectoryInfo[] dirInfo = theFolder.GetDirectories();
        for (int i = 0; i < dirInfo.Length; i++)
        {
            if (!dirInfo[i].Name.EndsWith("showtime_config"))
            {
                GetAllFile(dirInfo[i].FullName, fileList);
            }
        }
    }
}
#endregion