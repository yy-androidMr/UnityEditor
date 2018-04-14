using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Reflection;


public class UIActionProcess : LoadingProcess
{
    private int section = 0;
    private List<string> m_configList;

    private List<Type> sectionList;
    private ActionFindInBase currentSectionInstance;
    //private SectionTwoBean sectionTwoInstance;

    public UIActionProcess(string type, List<string> configList)
        : base(type)
    {
        m_configList = configList;
        sectionList = new List<Type>();
        //要做的事情:1. 直接生成excel
        //2.直接挪动要删除的文件.
        //
        sectionList.Add(typeof(SectionOneBean));//ingame_dance_actions_anim 局内舞蹈动作  对应关系[dance_xls:action-expression:assetsbundle] dance_xls不吻合即可删除,action-expression没有,需要找人添加,assetsbundle没有需要找人打包
        sectionList.Add(typeof(SectionTwoBean));//action-expression 局内舞蹈动作对应的k表情文件 [dance_xls]不吻合即可删除
        sectionList.Add(typeof(SectionThreeBean));//expression 局内,舞蹈,非舞蹈的表情.[action-expression]不吻合即刻删除  需要把上述的行为要删除的文件都挪出去再使用该功能
        sectionList.Add(typeof(SectionFourBean));//ingame_nodance_actions_anim 局内非舞蹈动作. [showtime_config配置:其他三个配置]没有即可删除
        sectionList.Add(typeof(SectionFiveBean));//actions_anim 局外动作 [客户端配置:客户端代码] 没有即可删除
        sectionList.Add(typeof(SectionSixBean));//actions_anim_expression 局外表情[actions_anim]不吻合即刻删除
        sectionList.Add(typeof(SectionSevenBean));//这里是做的

        section = 0;
        CreateNextSectionInstance(null);


        max = currentSectionInstance.MaxValue();
        Do = DoSomeThing;
        Name = GetCurrentProcessName;
        Finish = IsProcessFinish;
    }
    private string newMyType;
    public override string GetMyType()
    {
        return newMyType;
    }
    private bool CreateNextSectionInstance(object[] nextInfo)
    {
        if (section >= sectionList.Count)
        {
            currentSectionInstance = null;
            return false;
        }
        //ConstructorInfo[] ci = sectionList[section].GetConstructors();
        ConstructorInfo constructorInfo = sectionList[section].GetConstructor(new Type[0]);
        currentSectionInstance = constructorInfo.Invoke(null) as ActionFindInBase;
        currentSectionInstance.Init(m_configList, nextInfo);
        newMyType = myType + "/" + currentSectionInstance.WriteName() + "/" + currentSectionInstance.WriteName();//修改文件名.
        return true;
    }
    public bool IsProcessFinish()
    {
        bool isFinish = false;
        Dictionary<string, List<string>> newContent = currentSectionInstance.GetProcessFinishNewContent(index);
        if (newContent == null)
        {
            List<string> content = currentSectionInstance.GetProcessFinishContent(index);
            if (content != null)
            {
                ResourceUsageCheck.WriteOnly(GetMyType(), content);
            }
        }
        else
        {
            foreach (var item in newContent)
            {
                ResourceUsageCheck.WriteOnly(myType + "/" + currentSectionInstance.WriteName() + "/" + item.Key, item.Value);
            }
            ResourceUsageCheck.StopTagTime(GetMyType());
        }
        currentSectionInstance.ReleaseMe();
        ResetCount();
        section++;
        isFinish = !CreateNextSectionInstance(currentSectionInstance.GetNextMsg());
        return isFinish;
    }
    public string GetCurrentProcessName()
    {
        return currentSectionInstance.CurrentPrecessName(index);
    }
    public void DoSomeThing()
    {
        currentSectionInstance.Do(index);
    }
}

public abstract class ActionFindInBase
{
    public abstract void Init(List<string> configList, object[] nextInfo);
    public virtual Dictionary<string, List<string>> GetProcessFinishNewContent(int currentProgress)
    {
        return null;
    }
    public virtual List<string> GetProcessFinishContent(int currentProgress)
    {
        return null;
    }
    public abstract int MaxValue();
    public abstract string CurrentPrecessName(int currentProgress);
    public abstract void Do(int pro);
    public abstract void ReleaseMe();
    public virtual object[] GetNextMsg()
    {
        return null;
    }
    public abstract string WriteName();
}

