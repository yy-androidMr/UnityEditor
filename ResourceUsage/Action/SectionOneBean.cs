using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Reflection;
public class ActionOneBean
{
    public string simpleName;
    public string fullName;
    public string actionConfigName;
    public void SetFullName(string path)
    {
        fullName = path.ToLower();
        //获取了文件名
        simpleName = System.IO.Path.GetFileNameWithoutExtension(fullName);
        //var maches = Regex.Match(simpleName, ".+\\d");
        //int iLastNumberIndex = maches.Length - 1;
    }
}
#region 获取局内动作引用. ingame_dance_actions_anim
class SectionOneBean : ActionFindInBase
{
    List<string> allConfigAction;
    public Dictionary<string, List<ActionOneBean>> m_ingameActrionDir = new Dictionary<string, List<ActionOneBean>>();
    public List<string> m_ingameActionList_DirImg = new List<string>();
    public List<string> m_bundleFileList = new List<string>();

    //section0 的list
    public Dictionary<string, List<ActionOneBean>> useDir_0 = new Dictionary<string, List<ActionOneBean>>();
    public List<string> useDir = new List<string>();
    public List<string> deleteDir = new List<string>();
    public List<string> assestbundlesUse = new List<string>();
    public List<string> assestbundlesNotUse = new List<string>();


    public List<string> configNotFile_0 = new List<string>();
    public List<string> assestbundlesUse_0 = new List<string>();

    public override void Do(int currentPro)
    {



        useDir = m_ingameActionList_DirImg.FindAll(x => allConfigAction.Contains(x));//寻找正确的
        deleteDir = m_ingameActionList_DirImg.FindAll(x => !allConfigAction.Contains(x));//寻找要删除的


        assestbundlesUse = allConfigAction.FindAll(x => (!m_ingameActionList_DirImg.Contains(x) && m_bundleFileList.Contains(x)));//寻找 配置上有,但是在原始文件找不到,assetsbundle上有的
        assestbundlesNotUse = allConfigAction.FindAll(x => (!m_ingameActionList_DirImg.Contains(x) && !m_bundleFileList.Contains(x)));//寻找 配置上有,但是在原始文件找不到,assetsbundle上没有

        ////      private List<string> useList_0 = new List<string>();
        ////private List<string> configNotFile_0 = new List<string>();
        ////private List<string> notUse_0 = new List<string>();
        //string processName = CurrentPrecessName(currentPro);
        //bool digout = false;
        //for (int i = 0; i < m_ingameActionList_DirImg.Count; i++)
        //{
        //    if (processName.Equals(m_ingameActionList_DirImg[i]))
        //    {
        //        useDir_0.Add(m_ingameActionList_DirImg[i], m_ingameActrionDir[m_ingameActionList_DirImg[i]]);
        //        digout = true;
        //        break;//相等
        //    }
        //}
        ////foreach (var item in m_ingameActrionDir)
        ////{
        ////    if (processName.Equals(item.Key))
        ////    {
        ////        //配置表找动作文件列表 找到了 放到useDir_0里
        ////        useDir_0.Add(item.Key, item.Value);
        ////        digout = true;
        ////        break;//相等
        ////    }
        ////    Console.WriteLine(item.Key + item.Value);
        ////}

        ////配置表找动作文件列表,找不到???放到 configNotFile_0 里面
        //if (!digout)
        //{
        //    if (m_bundleFileList.Contains(processName))
        //    {
        //        assestbundlesUse_0.Add(processName);
        //    }
        //    else
        //    {
        //        configNotFile_0.Add(processName);
        //    }

        //    //这里继续去 assetsbundle 里面搜索
        //}
    }


    public override void ReleaseMe()
    {
        m_ingameActrionDir.Clear();
        //m_ingameActionList_DirImg.Clear();
        m_bundleFileList.Clear();
        useDir_0.Clear();
        configNotFile_0.Clear();
        assestbundlesUse_0.Clear();
    }

    public override string CurrentPrecessName(int currentProgress)
    {
        return "比对" + WriteName() + "中...";
    }
    public override string WriteName()
    {
        return "ingame_dance_actions_anim";
    }
    public override int MaxValue()
    {
        return 1;
    }

    public override List<string> GetProcessFinishContent(int currentProgress)
    {
        ////进行反向查找.ActionBean[] notUseAction =
        //List<KeyValuePair<string, List<ActionOneBean>>> notUseAction = m_ingameActrionDir.Where(t => !allConfigAction.Contains(t.Key)).ToList();
        //List<string> content = new List<string>();
        //content.Add("配置上没有,action原始文件有,共:" + notUseAction.Count + "个文件");
        //for (int i = 0; i < notUseAction.Count; i++)
        //{
        //    content.Add("," + notUseAction[i].Key);
        //    for (int j = 0; j < notUseAction[i].Value.Count; j++)
        //    {
        //        content.Add(",," + notUseAction[i].Value[j].fullName);
        //    }
        //}
        //content.Add("配置上有,action原始文件没有,assestbundles有,共:" + assestbundlesUse_0.Count + "个文件");
        //for (int i = 0; i < assestbundlesUse_0.Count; i++)
        //{
        //    content.Add("," + assestbundlesUse_0[i]);
        //}
        //if (configNotFile_0.Count > 0)
        //{
        //    content.Add("配置上有,action原始文件没有,assestbundles没有,共:" + configNotFile_0.Count + "个文件");
        //    for (int i = 0; i < configNotFile_0.Count; i++)
        //    {
        //        content.Add("," + configNotFile_0[i]);
        //    }
        //}
        //content.Add("关联正确,共:" + useDir_0.Count + "个文件");
        //foreach (var item in useDir_0)
        //{
        //    content.Add("," + item.Key);
        //    for (int j = 0; j < item.Value.Count; j++)
        //    {
        //        content.Add(",," + item.Value[j].fullName);
        //    }
        //}
        ////UIActionProcess.WirteUsageFile(content);

        List<string> content = new List<string>();

        //  useDir = m_ingameActionList_DirImg.FindAll(x => allConfigAction.Contains(x));//寻找正确的
        //deleteDir = m_ingameActionList_DirImg.FindAll(x => !allConfigAction.Contains(x));//寻找要删除的

        //要删除
        int count = 0;
        for (int i = 0; i < deleteDir.Count; i++)
        {
            List<ActionOneBean> aobList = m_ingameActrionDir[deleteDir[i]];
            for (int j = 0; j < aobList.Count; j++)
            {
                content.Add("," + aobList[j].fullName);
            }
            count += aobList.Count;
        }
        content.Add("配置表没有,共:" + count + "个文件");
        content.Add("");

        //配置表 assetbundle
        for (int i = 0; i < assestbundlesUse.Count; i++)
        {
            content.Add("," + assestbundlesUse[i]);
        }
        content.Add("配置表有,原始文件没有,assetsbundle有,共:" + assestbundlesUse.Count + "个文件");
        content.Add("");

        //配置表 其他都没有
        if (assestbundlesNotUse.Count > 0)
        {
            for (int i = 0; i < assestbundlesNotUse.Count; i++)
            {
                content.Add("," + assestbundlesNotUse[i]);
            }
            content.Add("配置表有,原始文件没有,assetsbundle没有,共:" + assestbundlesNotUse.Count + "个文件");
            content.Add("");
        }

        //正确的
        count = 0;
        for (int i = 0; i < useDir.Count; i++)
        {
            List<ActionOneBean> aobList = m_ingameActrionDir[useDir[i]];

            if (ResourceUsageCheck.isXULIANG_LOOK)
            {
                for (int j = 0; j < aobList.Count; j++)
                {
                    if (aobList[j].fullName.EndsWith(".act.txt"))
                    {
                        content.Add(System.IO.Path.GetFileNameWithoutExtension(aobList[j].fullName.Replace(".act.txt", "")));
                        break;
                    }
                }
                count++;
            }
            else
            {
                for (int j = 0; j < aobList.Count; j++)
                {
                    content.Add("," + aobList[j].fullName);
                }
                count += aobList.Count;
            }
        }
        content.Add("正确,共:" + count + "个文件");


        //assestbundlesUse = allConfigAction.FindAll(x => (!m_ingameActionList_DirImg.Contains(x) && m_bundleFileList.Contains(x)));//寻找 配置上有,但是在原始文件找不到,assetsbundle上有的
        //assestbundlesNotUse = allC

        return content;
    }

    public override void Init(List<string> configList, object[] nextInfo)
    {
        string action_path = configList[0];
        string[] action_source = Directory.GetFiles(action_path + "/ingame_dance_actions_anim", "*.anim", SearchOption.AllDirectories);
        //string[] action_source = new string[] { @"f:\p4_workspace\dgm\x5_mobile\mobile_dancer_resource\resources\美术资源\动作资源\动作/ingame_dance_actions_anim\sd_130bpm_i'll be back_76_2_root.anim" };
        //string[] action_source_txt = new string[] { };
        string[] action_source_txt = Directory.GetFiles(action_path + "/ingame_dance_actions_anim", "*.txt", SearchOption.AllDirectories);

        //组织原始action文件列表
        for (int i = 0; i < action_source.Length; i++)
        {
            string regexName = Regex.Replace(System.IO.Path.GetFileNameWithoutExtension(action_source[i]), @"(?<=\d)[^\d]*$", "");
            regexName = regexName.ToLower();
            List<ActionOneBean> beanList;
            if (m_ingameActrionDir.TryGetValue(regexName, out beanList))
            {

            }
            else
            {
                beanList = new List<ActionOneBean>();
                m_ingameActrionDir.Add(regexName, beanList);
                m_ingameActionList_DirImg.Add(regexName);
            }
            ActionOneBean ab = new ActionOneBean();
            ab.SetFullName(action_source[i]);
            beanList.Add(ab);
        }


        for (int i = 0; i < action_source_txt.Length; i++)
        {
            string regexName = Regex.Replace(System.IO.Path.GetFileNameWithoutExtension(action_source_txt[i]), @"(?<=\d)[^\d]*$", "");
            regexName = regexName.ToLower();
            List<ActionOneBean> beanList;
            if (m_ingameActrionDir.TryGetValue(regexName, out beanList))
            {

            }
            else
            {
                beanList = new List<ActionOneBean>();
                m_ingameActrionDir.Add(regexName, beanList);
                m_ingameActionList_DirImg.Add(regexName);
            }
            ActionOneBean ab = new ActionOneBean();
            ab.SetFullName(action_source_txt[i]);
            beanList.Add(ab);
        }
        //m_ingame_actions = m_ingame_actions.Where((x, i) => m_ingame_actions.FindIndex(z => z.actionConfigName == x.actionConfigName) == i).ToList();

        //组织配置列表.
        string ingame_dance_action_config_path = configList[1];
        allConfigAction = new List<string>();
        string dance_content = File.ReadAllText(ingame_dance_action_config_path).ToLower();
        MatchCollection mc = Regex.Matches(dance_content, "(?<=<item id=\").+(?=\")");
        for (int i = 0; i < mc.Count; i++)
        {
            string value = mc[i].Value;
            allConfigAction.Add(value);
        }
        allConfigAction.Sort();
        string[] bundleFile = Directory.GetFiles(@"assetbundles\cdn\assetbundles\art\role\actions\ingame_dance_actions_anim", "*", SearchOption.AllDirectories);
        for (int i = 0; i < bundleFile.Length; i++)
        {
            if (!bundleFile[i].Contains('.'))
            {
                m_bundleFileList.Add(System.IO.Path.GetFileNameWithoutExtension(bundleFile[i]).ToLower());
            }
        }
    }

}
//public delegate void OnReadSuccess(ExcelPackage success);
//static ExcelPackage ReadExcel(OnReadSuccess success, string path)
//{
//    string exprotFilePath = path;
//    FileInfo newFile = new FileInfo(exprotFilePath);
//    //FileStream fs = new FileStream(exprotFilePath, FileMode.Open);
//    try
//    {
//        Excel.Application app = new Excel.Application();
//        using (var package = new ExcelPackage(newFile))
//        {
//            success(package);
//            //valuesss = value.Value.ToString();
//        }
//    }
//    catch (Exception e)
//    {
//        UnityEngine.Debug.Log("exception:" + e);
//    }
//    return null;
//}
#endregion