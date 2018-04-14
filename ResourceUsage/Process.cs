using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

public enum WriteType
{
    TEXT,
    EXCEL
}
public delegate T PDelegate<T>();
public abstract class LoadingProcess
{
    public string myType;
    protected int index = 0;
    protected int max = 0;

    public LoadingProcess(string type)
    {
        myType = type;
    }
    public PDelegate<string> Name;

    public Action Do;
    public PDelegate<bool> Finish;
    public bool DoAdd()
    {
        if (Do != null)
        {
            Do();
        }
        index++;
        if (index >= max)
        {
            if (Finish != null)
            {
                return Finish();
            }
            return true;
        }
        return false;
    }
    public float CurentPercent()
    {
        return (float)index / (float)max;
    }
    public void ResetCount()
    {
        index = 0;
    }
    public virtual string GetMyType()
    {
        return myType;
    }

    public void Write(List<string> content, WriteType type = WriteType.TEXT, string childFile = "")
    {
        ResourceUsageCheck.WriteOnly(GetMyType() + childFile, content, type);
    }
}

public class IconProcess : LoadingProcess
{
    List<ResourceUsageCheck.IconFileInfo> m_fileDic = new List<ResourceUsageCheck.IconFileInfo>();
    List<ResourceUsageCheck.IconFileInfo> m_searchTargetDic = new List<ResourceUsageCheck.IconFileInfo>();
    public IconProcess(string type, List<ResourceUsageCheck.IconFileInfo> fileDic, List<ResourceUsageCheck.IconFileInfo> searchTargetDic)
        : base(type)
    {
        m_fileDic = fileDic;
        m_searchTargetDic = searchTargetDic;

        max = m_searchTargetDic.Count;
        Do = DoIt;
        Name = GetCurrentProcessName;
        Finish = OnFinish;

    }
    public bool OnFinish()
    {
        // finish 了 写文件.
        m_fileDic = m_fileDic.OrderBy(p => p.usageList.Count).ToList();
        List<string> content = new List<string>();
        content.Add("词汇:,,查找路径:mobile_dancer_resource\\Resources\\美术资源\\UI\\图标\\background");
        content.Add("[not used:]没有在使用");
        content.Add("[u:]正在使用");
        content.Add("[u xml:]配置正在使用");
        content.Add("[u cs:]客户端代码正在使用");
        content.Add("[u cpp:]服务器代码正在使用");
        content.Add("");
        bool lastIsNotUse = false;
        bool firstEnter = true;
        foreach (var item in m_fileDic)
        {
            if (item.usageList.Count == 0)
            {
                if (!lastIsNotUse || firstEnter)
                {
                    content.Add("not used:");
                    lastIsNotUse = true;
                    firstEnter = false;
                }
                content.Add("," + item.fileInfo.FullName);
            }
            else
            {
                if (lastIsNotUse || firstEnter)
                {
                    content.Add("u:");
                    lastIsNotUse = false;
                    firstEnter = false;
                }
                content.Add("," + item.fileInfo.FullName);
                foreach (string useFile in item.usageList)
                {
                    string exter = ResourceUsageCheck.GetFileExtension(useFile);
                    //content.Add("[u xml:]配置正在使用");
                    //content.Add("[u cs:]客户端代码正在使用");
                    //content.Add("[u cpp:]服务器代码正在使用");
                    string pre = "";
                    if (exter.Equals(".xml"))
                    {
                        pre = "u xml:";
                    }
                    else if (exter.Equals(".cs"))
                    {
                        pre = "u cs:";
                    }
                    else if (exter.Equals(".cpp"))
                    {
                        pre = "u cpp:";
                    }
                    content.Add(",      " + pre + useFile);
                }
            }
        }
        Write(content);
        return true;
    }
    public void DoIt()
    {
        string kvp = GetCurrentProcessName();
        string maohao = "";
        if (kvp.EndsWith(".cs") || kvp.EndsWith(".cpp"))
        {
            maohao = "\"";
        }
        string allText = File.ReadAllText(kvp);
        if (string.IsNullOrEmpty(allText))
        {
            return;
        }
        allText = allText.ToLower();
        foreach (var item in m_fileDic)
        {
            if (allText.Contains(item.fileSimpleName + maohao))
            {
                item.usageList.Add(kvp);
            }
        }
    }
    public string GetCurrentProcessName()
    {
        ResourceUsageCheck.IconFileInfo kvp = m_searchTargetDic[index];
        return kvp.fileInfo.FullName;
    }
}
public class TextureProcess : LoadingProcess
{
    private string[] m_prefab_files;
    private Dictionary<string, ResourceUsageCheck.TextureFileInfo> m_guidAndFile;
    public TextureProcess(string type, Dictionary<string, ResourceUsageCheck.TextureFileInfo> guidAndFile)
        : base(type)
    {
        m_prefab_files = Directory.GetFiles(Application.dataPath + "/resources/Art", "*.prefab", SearchOption.AllDirectories);
        //m_prefab_files = Directory.GetFiles(Application.dataPath + "/resources/Art/", "*.prefab", SearchOption.AllDirectories);
        m_guidAndFile = guidAndFile;

        max = m_prefab_files.Length;
        Do = DoSomeThing;
        Name = GetCurrentProcessName;
        Finish = OnFinish;
    }
    public bool OnFinish()
    {
        //把有mat文件的png 给去了
        List<string> pngList = new List<string>();
        foreach (var item in m_guidAndFile)
        {
            string matPath = item.Value.GetMatPathIfIsPng();
            if (!string.IsNullOrEmpty(matPath))
            {
                ResourceUsageCheck.TextureFileInfo matFile;
                if (m_guidAndFile.TryGetValue(matPath, out matFile))
                {
                    matFile.otherOneInfo = item.Value;
                    item.Value.otherOneInfo = matFile;
                    pngList.Add(item.Value.fInfo.FullName);
                }
            }
        }
        foreach (string item in pngList)
        {
            m_guidAndFile.Remove(item);
        }

        m_guidAndFile = m_guidAndFile.OrderBy(p => p.Value.useAge.Count).ToDictionary(p => p.Key, o => o.Value);
        List<string> content = new List<string>();
        content.Add("词汇:,,查找路径:Assets\\StaticResources\\art\\UITexture");
        content.Add("[not used:]没有在使用");
        content.Add("[has mat but there is no png!!]  有mat但是没有png.");
        content.Add("[u:]正在使用");
        content.Add("[Illegal use:]生成了mat文件_但是还是使用的png");
        content.Add("");
        bool lastIsNotUse = false;
        bool firstEnter = true;
        foreach (var item in m_guidAndFile)
        {
            if (item.Value.useAge.Count > 0)
            {
                if (lastIsNotUse || firstEnter)
                {
                    content.Add("u:");
                    lastIsNotUse = false;
                    firstEnter = false;
                }
                content.Add("," + item.Value.fileAbsPath);
                for (int i = 0; i < item.Value.useAge.Count; i++)
                {
                    content.Add(",   " + item.Value.useAge[i]);
                }
            }
            else
            {
                if (!lastIsNotUse || firstEnter)
                {
                    content.Add("not used:");
                    lastIsNotUse = true;
                    firstEnter = false;
                }
                string appendStr = "," + item.Value.fileAbsPath;
                if (!item.Value.isPng)//不是png 就是mat  检查一下 png的使用. 非法使用.
                {
                    if (item.Value.otherOneInfo == null)
                    {
                        appendStr += ",has mat but there is no png!!";
                    }
                    else
                    {
                        if (item.Value.otherOneInfo.useAge.Count > 0)
                        {
                            appendStr += ",Illegal use:" + item.Value.otherOneInfo.fileAbsPath;
                            for (int i = 0; i < item.Value.useAge.Count; i++)
                            {
                                appendStr += "," + item.Value.otherOneInfo.useAge[i];
                            }
                        }
                    }
                }
                content.Add(appendStr);
            }
        }
        Write(content);
        return true;
    }
    public string GetCurrentProcessName()
    {
        string prefab_path = m_prefab_files[index];
        return prefab_path;
    }
    public void DoSomeThing()
    {
        //v1.0
        string allText = File.ReadAllText(GetCurrentProcessName());
        foreach (var item in m_guidAndFile)
        {
            if (allText.Contains(item.Value.guid))
            {
                item.Value.useAge.Add(GetCurrentProcessName());
            }
        }
    }

}
public class UIPrefabProcess : LoadingProcess
{
    private string[] sectionName = new string[] { "查找过滤所有prefab", "在UIRegister/UIHotRegister上查找引用", "查找Pool加载引用关系", "其他cs引用", "再次查找Pool加载引用关系", "写入" };
    static Regex prefabRegex = new Regex(@"objectGUID: \S+|selectObj: {fileID: .+guid: \S+");//prefabRegex

    List<string> prefabConvertList = new List<string>();
    List<UIPrefabInfo> prefabInfoList = new List<UIPrefabInfo>();
    List<UIPrefabInfo> unUseInfodic = new List<UIPrefabInfo>();//完全找不到
    List<UIPrefabInfo> cSharpUse = new List<UIPrefabInfo>();//cs文件加载
    private List<string> m_filterPrefab;
    public UIPrefabProcess(string type, List<string> filterPrefab)
        : base(type)
    {
        m_filterPrefab = filterPrefab;

        max = sectionName.Length;
        Do = DoSomeThing;
        Name = GetCurrentProcessName;
    }

    public string GetCurrentProcessName()
    {
        return sectionName[index];
    }
    public void DoSomeThing()
    {
        switch (index)
        {
            case 0://查找prefab
                {
                    //string[] m_prefab_files = new string[] { "F:/p4_workspace/DGM/x5_mobile/mobile_dancer/trunk/client/Assets/resources/Art/UIPrefabs/General/SwitchScene.prefab", "F:/p4_workspace/DGM/x5_mobile/mobile_dancer/trunk/client/Assets/resources/Art/UIPrefabs/NewUI/PayNewYear/UIPayNewYearBubble.prefab" };
                    //string[] m_prefab_files = new string[] { "F:/p4_workspace/DGM/x5_mobile/mobile_dancer/trunk/client/Assets/resources/Art/UIPrefabs/NewUI/Arena/UIArenaEnter.prefab"};
                    string[] m_prefab_files = Directory.GetFiles(Application.dataPath + "/resources/Art/UIPrefabs", "*.prefab", SearchOption.AllDirectories);
                    for (int i = 0; i < m_prefab_files.Length; i++)
                    {
                        bool ignore = false;
                        for (int j = 0; j < m_filterPrefab.Count; j++)
                        {
                            if (m_prefab_files[i].Contains(m_filterPrefab[j]))
                            {
                                ignore = true;
                                break;
                            }
                        }
                        if (!ignore)
                        {
                            prefabConvertList.Add(m_prefab_files[i].Replace('\\', '/'));
                        }
                    }
                }
                break;

            case 1://UIRegister find
                {
                    string registPath = Application.dataPath + "/Scripts/Framework/Global/UIRegister.cs";
                    string hotRegistPath = Application.dataPath + "/Scripts/LSharpToCS/UIHotRegister.cs";
                    string registContent = File.ReadAllText(registPath);
                    registContent += File.ReadAllText(hotRegistPath);
                    registContent = registContent.ToLower();
                    foreach (var item in prefabConvertList)//查找被regist的界面.
                    {
                        UIPrefabInfo upi = new UIPrefabInfo();
                        upi.InitPrefabPath(item);
                        upi.prefabName = System.IO.Path.GetFileNameWithoutExtension(item);
                        string simplePath = upi.GetPrefabSimplePath();
                        if (registContent.Contains(simplePath + "\""))
                        {
                            upi.prefabType = "registerUI";
                            prefabInfoList.Add(upi);
                        }
                        else
                        {
                            unUseInfodic.Add(upi);
                        }
                    }
                }
                break;

            case 2://查找Register里面的UIPrefab的 Pool
                {
                    List<UIPrefabInfo> removeInUnUseList = new List<UIPrefabInfo>();
                    //查找regist的界面中, 动态加载的界面.
                    foreach (UIPrefabInfo item in prefabInfoList)
                    {
                        FindChildPrefab(item, unUseInfodic, removeInUnUseList);
                    }
                    for (int i = 0; i < removeInUnUseList.Count; i++)
                    {
                        if (unUseInfodic.Contains(removeInUnUseList[i]))
                        {
                            unUseInfodic.Remove(removeInUnUseList[i]);
                        }
                    }
                }
                break;
            case 3://查找其他cs文件引用.
                {
                    //Waring
                    string[] m_prefab_files = Directory.GetFiles(Application.dataPath + "/Scripts", "*.cs", SearchOption.AllDirectories);
                    List<string> cSharpList = new List<string>();
                    for (int i = 0; i < m_prefab_files.Length; i++)
                    {
                        if (m_prefab_files[i].EndsWith("UIRegister.cs") || m_prefab_files[i].EndsWith("UIHotRegister.cs"))
                        {

                        }
                        else
                        {
                            cSharpList.Add(m_prefab_files[i].Replace('\\', '/'));
                        }
                    }
                    foreach (var item in cSharpList)//查找被c#引用的界面.
                    {
                        string allText = File.ReadAllText(item);
                        if (!string.IsNullOrEmpty(allText))
                        {

                            for (int i = 0; i < unUseInfodic.Count; i++)
                            {
                                if (allText.Contains(unUseInfodic[i].prefabName + "\""))
                                {
                                    unUseInfodic[i].cShapUse.Add(item);
                                    cSharpUse.Add(unUseInfodic[i]);
                                }
                            }
                        }
                    }
                    for (int i = 0; i < cSharpUse.Count; i++)
                    {
                        if (unUseInfodic.Contains(cSharpUse[i]))
                        {
                            unUseInfodic.Remove(cSharpUse[i]);
                        }
                    }
                }
                break;
            case 4://查找c#里面的UIPrefab的 Pool
                {
                    List<UIPrefabInfo> removeInUnUseList = new List<UIPrefabInfo>();
                    //查找regist的界面中, 动态加载的界面.
                    foreach (UIPrefabInfo item in cSharpUse)
                    {
                        FindChildPrefab(item, unUseInfodic, removeInUnUseList);
                    }
                    for (int i = 0; i < removeInUnUseList.Count; i++)
                    {
                        if (unUseInfodic.Contains(removeInUnUseList[i]))
                        {
                            unUseInfodic.Remove(removeInUnUseList[i]);
                        }
                    }

                }
                break;
            default://写文件
                {
                    List<string> content = new List<string>();
                    content.Add("not used:");
                    foreach (var item in unUseInfodic)
                    {
                        content.Add(item.prefabSimplePath);
                    }
                    content.Add("");

                    content.Add("c# waring:");
                    foreach (var item in cSharpUse)
                    {
                        content.Add(item.prefabPath);
                        for (int i = 0; i < item.cShapUse.Count; i++)
                        {
                            content.Add("     " + item.cShapUse[i]);
                        }
                    }
                    content.Add("");

                    content.Add("被UIRegister注册的UI:");
                    foreach (UIPrefabInfo item in prefabInfoList)
                    {
                        content.Add(item.prefabPath);
                        if (item.prefabPoolList.Count > 0)
                        {
                            content.Add("prefabPool加载出来的界面");
                            AddPrefabChildContent(content, item, 1);
                        }
                        content.Add("");
                    }
                    ResourceUsageCheck.WriteOnly(GetMyType(), content);
                }
                break;
        }
    }
    public void AddPrefabChildContent(List<string> content, UIPrefabInfo item, int level)
    {
        foreach (var childItem in item.prefabPoolList)
        {
            string preContent = "";
            for (int i = 0; i < level; i++)
            {
                preContent += "     ";
            }
            content.Add(preContent + childItem.prefabPath);
            AddPrefabChildContent(content, childItem, level + 1);
        }
    }


    public void FindChildPrefab(UIPrefabInfo path, List<UIPrefabInfo> unUseInfodic, List<UIPrefabInfo> removeInUnUseList)
    {
        string allText = File.ReadAllText(path.prefabPath);
        //v2.0
        if (prefabRegex.IsMatch(allText))
        {
            MatchCollection collection = prefabRegex.Matches(allText);
            foreach (Match item in collection)
            {
                string prefabPath = item.Value;
                for (int i = 0; i < unUseInfodic.Count; i++)
                {
                    if (prefabPath.Contains(unUseInfodic[i].prefabSimplePath) || prefabPath.Contains(unUseInfodic[i].guid))
                    {
                        //unUseInfodic.Remove(unUseInfodic[i]);
                        if (!path.prefabPoolList.Contains(unUseInfodic[i]))
                        {
                            path.prefabPoolList.Add(unUseInfodic[i]);
                            removeInUnUseList.Add(unUseInfodic[i]);
                        }
                        FindChildPrefab(unUseInfodic[i], unUseInfodic, removeInUnUseList);
                        break;
                    }
                }
            }
        }

    }
    public class UIPrefabInfo
    {
        public string prefabPath;
        public string prefabSimplePath;//   Assets/resources/Art/UIPrefabs/NewUI/Community/CommunityDigTreasure/UIDigTreasureProgress.prefab
        public string prefabName;//UIDigTreasureProgress
        public string guid;
        public string prefabType;//registerUI, loadUI
        public List<string> cShapUse = new List<string>();
        public List<UIPrefabInfo> prefabPoolList = new List<UIPrefabInfo>();
        public void InitPrefabPath(string path)
        {
            prefabPath = path;
            prefabSimplePath = prefabPath.Replace(Application.dataPath, "Assets");
            guid = AssetDatabase.AssetPathToGUID(prefabSimplePath);
        }
        public string GetPrefabSimplePath()
        {
            string[] prefabdirs = Regex.Split(prefabPath, "/|\\\\");
            string simplePath = prefabdirs[prefabdirs.Length - 3] + "/" + prefabdirs[prefabdirs.Length - 2] + "/" + ResourceUsageCheck.GetFileNameWithoutExtension(prefabdirs[prefabdirs.Length - 1]);
            return simplePath.ToLower();
        }
    }
}


public class UICodeProcess : LoadingProcess
{
    public UICodeProcess(string type, List<string> configList)
        : base(type)
    {
        //先查找 mono的类
        string[] m_cs_files = Directory.GetFiles(Application.dataPath + "/Scripts", "*.cs", SearchOption.AllDirectories);
        List<string> csFilterFiles = new List<string>();
        csFilterFiles.AddRange(m_cs_files);
        List<string> baseList = configList.Where(o => !o.StartsWith("ignore")).ToList();
        List<string> ignoreList = configList.Where(o => o.StartsWith("ignore")).ToList();
        for (int i = 0; i < ignoreList.Count; i++)
        {
            ignoreList[i] = ignoreList[i].Split(':')[1];
        }
        for (int i = 0; i < csFilterFiles.Count; )
        {
            if (ignoreList.Contains(ResourceUsageCheck.GetFileName(csFilterFiles[i])))
            {
                csFilterFiles.Remove(csFilterFiles[i]);
            }
            else
            {
                i++;
            }
        }
        //string regexStr = "";
        //for (int i = 0; i < baseList.Count; i++)
        //{
        //    regexStr += ": *" + baseList[i] + "[ \r\n]*{|";
        //}
        //regexStr = regexStr.Substring(0, regexStr.Length - 1);

        //Regex fileRegex = new Regex(regexStr);//继承该类的
        //for (int i = 0; i < csFilterFiles.Count; i++)
        //{
        //    string allText = File.ReadAllText(csFilterFiles[i]);
        //    if (fileRegex.IsMatch(allText))
        //    {
        //        string a = "";
        //    }
        //}


        max = 0;
        Do = DoSomeThing;
        Name = GetCurrentProcessName;
        Finish = OnFinish;
    }
    public float GetCurrentProcessPercent()
    {
        return 0.0f / 0.0f;
    }
    public bool OnFinish()
    {
        //bool isFinish = searchTargetIndex >= sectionName.Length;
        //if (isFinish)
        //{
        //    // finish 了 写文件.
        //    StopTagTime(TAG_CODE);

        //}
        //return isFinish;
        return true;
    }
    public string GetCurrentProcessName()
    {
        return "";
        //return sectionName[GetCurrentProcess()];
    }
    public void DoSomeThing()
    {
        //然后再去prefab上查找引用.

        //最后在剩下的去代码看有没有动态add
    }
}



public class UIFileProcess : LoadingProcess
{
    private List<string> m_moveFile;
    private bool is_move;
    private string outpath;
    public UIFileProcess(string type, List<string> moveFile, bool isMove, string output)
        : base(type)
    {
        m_moveFile = moveFile;
        is_move = isMove;
        outpath = output;
        piceofFragmentCount = 2;

        max = m_moveFile.Count;
        Do = DoSomeThing;
        Name = GetCurrentProcessName;
    }
    public string GetCurrentProcessName()
    {
        return m_moveFile[index];
        //return sectionName[GetCurrentProcess()];
    }
    int piceofFragmentCount = 2;
    public void DoSomeThing()
    {
        string source = GetCurrentProcessName();
        if (string.IsNullOrEmpty(source))
        {
            return;
        }
        string outputDir = outpath;
        if (source.StartsWith("file_pre_save"))
        {
            piceofFragmentCount = int.Parse(source.Split('=')[1]);
            return;
        }
        source = source.Replace('/', '\\');
        string[] sourceSplit = source.Split('\\');
        for (int i = piceofFragmentCount; i >= 2; i--)
        {
            outputDir += sourceSplit[sourceSplit.Length - i] + "\\";
        }
        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        if (!File.Exists(source))
        {
            return;
        }
        if (!is_move)
        {
            System.IO.File.Copy(source, outputDir + "\\" + System.IO.Path.GetFileName(source), true);
        }
        else
        {
            try
            {
                System.IO.File.Move(source, outputDir + "\\" + System.IO.Path.GetFileName(source));
            }
            catch (Exception e)
            {
                Debug.Log(e);
                Debug.Log(outputDir + "\\" + System.IO.Path.GetFileName(source));
            }
        }
    }
}




public class UISoundProcess : LoadingProcess
{
    public class SoundBean
    {
        public string simpleName;
        //public string fullname;
        public List<string> funnNameList = new List<string>();
        public void AddFullName(string full)
        {
            funnNameList.Add(full);
        }
    }
    List<SoundBean> use = new List<SoundBean>();
    List<SoundBean> noUse = new List<SoundBean>();

    List<SoundBean> soundBeanList;
    private string audioManageText;
    private List<string> renderList;
    private List<string> ingameSoundEnumList;//InGameSoundEffect
    private List<string> configList;//配置
    public UISoundProcess(string type, string sourcePath)
        : base(type)
    {


        //x5_mobile/mobile_dancer_resource/Resources/ResourcePublish/CDN/SourceFiles/android/assetbundles/audio/ui_sound_effect/
        //x5_mobile/mobile_dancer/trunk/client/Assets/StreamingAssets/audio/sound_effect/


        //F:\p4_workspace\DGM\x5_mobile\mobile_dancer\trunk\client\Assets\Standard Assets\X5ExtendComponent\InGameSoundEffect.cs  加了一个ui_
        //F:\p4_workspace\DGM\x5_mobile\mobile_dancer\trunk\client\Assets\Scripts\Framework\Global\AudioEffectManager.cs  /ui_kuaimen"
        //F:\p4_workspace\DGM\x5_mobile\mobile_dancer\trunk\client\Assets\Scripts\CoreGame\Render\ModeSpecial\GameRenderClassic.cs  /ingame_classic_miss.wav"
        string ab_sound_effect = sourcePath + @"\android\assetbundles\audio\ui_sound_effect";
        string[] ab_source = Directory.GetFiles(ab_sound_effect, "*", SearchOption.AllDirectories);

        string streaming_sound_effect = Application.dataPath + @"\StreamingAssets\audio\sound_effect";
        string[] streaming_source = Directory.GetFiles(streaming_sound_effect, "*", SearchOption.AllDirectories);

        soundBeanList = new List<SoundBean>();
        for (int i = 0; i < ab_source.Length + streaming_source.Length; i++)
        {
            string item = i >= ab_source.Length ? streaming_source[i - ab_source.Length] : ab_source[i];

            string simpleName = System.IO.Path.GetFileName(item).Split('.')[0];
            simpleName = simpleName.ToLower();
            SoundBean sb = soundBeanList.Find(x => x.simpleName.Equals(simpleName));
            if (sb != null)
            {

            }
            else
            {
                sb = new SoundBean();
                soundBeanList.Add(sb);
                sb.simpleName = simpleName;
            }
            sb.AddFullName(item);
        }

        string audioEffectManager = Application.dataPath + @"\Scripts\Framework\Global\AudioEffectManager.cs";
        audioManageText = File.ReadAllText(audioEffectManager).ToLower();
        string[] render = new string[] {
            Application.dataPath + @"\Scripts\CoreGame\Render\ModeSpecial\GameRenderClassic.cs",
            Application.dataPath + @"\Scripts\CoreGame\Render\GameRender.cs",
            Application.dataPath + @"\Scripts\CoreGame\Render\ModeSpecial\GameRenderPinball.cs",
            Application.dataPath + @"\Scripts\CoreGame\Render\ModeSpecial\ModeJoyParty\GameRenderJoyParty.cs",
            Application.dataPath+@"\Scripts\CoreGame\Render\ModeSpecial\ModeTeamArena\GameRenderTeamArena.cs",
            Application.dataPath+@"\Scripts\CoreGame\Render\ModeSpecial\ModeTryst\GameRenderTryst.cs"};
        renderList = new List<string>();
        for (int i = 0; i < render.Length; i++)
        {
            renderList.Add(File.ReadAllText(render[i]).ToLower());
        }
        ingameSoundEnumList = new List<string>();
        foreach (int myCode in Enum.GetValues(typeof(InGameSoundEffect)))
        {
            string strName = Enum.GetName(typeof(InGameSoundEffect), myCode);//获取名称
            string strVaule = strName.ToLower();//获取值
            ingameSoundEnumList.Add(strVaule.StartsWith("ui_") ? strVaule : "ui_" + strVaule);
        }
        configList = new List<string>();
        string[] configs = new string[] {
              sourcePath + @"\crossplatform\config\shared\community\community_npc_list.xml",
              sourcePath + @"\crossplatform\config\shared\guide\guide.xml",
              sourcePath + @"\crossplatform\config\shared\pop_tip.xml",
          };
        for (int i = 0; i < configs.Length; i++)
        {
            configList.Add(File.ReadAllText(configs[i]).ToLower());
        }



        max = soundBeanList.Count;
        Do = DoSomeThing;
        Name = GetCurrentProcessName;
        Finish = OnFinish;
    }
    public bool OnFinish()
    {
        // finish 了 写文件.
        //WirteUsageFile
        List<string> content = new List<string>();
        for (int i = 0; i < use.Count; i++)
        {
            for (int j = 0; j < use[i].funnNameList.Count; j++)
            {
                content.Add(use[i].funnNameList[j]);
            }
        }
        ResourceUsageCheck.WriteOnly(myType + "/audio/在使用的音效", content, WriteType.EXCEL);

        content = new List<string>();
        for (int i = 0; i < noUse.Count; i++)
        {
            for (int j = 0; j < noUse[i].funnNameList.Count; j++)
            {
                content.Add(noUse[i].funnNameList[j]);
                content.Add(noUse[i].funnNameList[j].Replace("android", "iOS"));
            }
        }
        ResourceUsageCheck.WriteOnly(myType + "/audio/不在使用", content, WriteType.EXCEL);
        return true;

    }
    public string GetCurrentProcessName()
    {
        return soundBeanList[index].simpleName;
    }


    public void DoSomeThing()
    {
        SoundBean sb = soundBeanList[index];
        if (audioManageText.Contains("/" + sb.simpleName + "\""))
        {
            use.Add(sb);
        }
        else if (!string.IsNullOrEmpty(renderList.Find(x => x.Contains(sb.simpleName + ".wav\""))))
        {
            use.Add(sb);
        }
        else if (!string.IsNullOrEmpty(ingameSoundEnumList.Find(x => x.Equals(sb.simpleName))))
        {
            use.Add(sb);
        }
        else if (!string.IsNullOrEmpty(configList.Find(x => x.Contains(sb.simpleName + "\""))) || !string.IsNullOrEmpty(configList.Find(x => x.Contains(sb.simpleName + "<"))))
        {
            use.Add(sb);
        }
        else
        {
            noUse.Add(sb);
        }
    }
}




//打包的时候测


//分支资源挪到主支
public class ResourceB2TProcess : LoadingProcess
{
    private List<FileBean> branchFiles = new List<FileBean>();
    private List<FileBean> trunkFiles = new List<FileBean>();
    private List<FileBean> pendingFiles = new List<FileBean>();
    private string m_bPath;
    private string m_trunkPath;
    public ResourceB2TProcess(string type, string bPath, string trunkPath)
        : base(type)
    {
        m_bPath = bPath;
        m_trunkPath = trunkPath.Substring(0, trunkPath.IndexOf("CDN"));

        max = 1;
        Do = DoSomeThing;
        Name = GetCurrentProcessName;
        Finish = OnFinish;
    }

    private void RefreshList()
    {

        string android = "/android";
        string ios = "/iOS";
        string bodypart = "/assetbundles/art/role/bodypart";
        string link = "/assetbundles/art/role/link";
        string icon = "/assetbundles/texture/item_icon";


        string cdn_pre = "/CDN/SourceFiles";
        AddFile(cdn_pre, android + bodypart, FileType.CDN);
        AddFile(cdn_pre, android + link, FileType.CDN);
        AddFile(cdn_pre, android + icon, FileType.CDN);
        AddFile(cdn_pre, ios + bodypart, FileType.CDN);
        AddFile(cdn_pre, ios + link, FileType.CDN);
        AddFile(cdn_pre, ios + icon, FileType.CDN);

        string inv_pre = "/Inventory/";
        AddFile(inv_pre, android + bodypart, FileType.INV);
        AddFile(inv_pre, android + link, FileType.INV);
        AddFile(inv_pre, android + icon, FileType.INV);
        AddFile(inv_pre, ios + bodypart, FileType.INV);
        AddFile(inv_pre, ios + link, FileType.INV);
        AddFile(inv_pre, ios + icon, FileType.INV);

        string pending = "/PendingResource/";
        string newPath = m_bPath + pending;
        string[] pending_src = Directory.GetFiles(newPath, "*", SearchOption.AllDirectories);
        for (int i = 0; i < pending_src.Length; i++)
        {
            string simpleName = System.IO.Path.GetFileName(pending_src[i]);
            string id = simpleName.Split('_')[0];

            if (!Regex.IsMatch(id, @"\d{10}"))
            {
                continue;
            }


            FileBean fb = new FileBean();
            fb.pre = m_bPath;
            fb.middlePath1 = pending;
            fb.full = pending_src[i];
            fb.simple = fb.full.Replace(m_bPath + pending, "").Replace('\\', '/');
            pendingFiles.Add(fb);
        }

    }
    private void AddFile(string add1, string add2, FileType type)
    {
        m_bPath = m_bPath.Replace('\\', '/');
        string newPath = m_bPath + add1 + add2;
        //newPath = newPath.Replace()
        if (Directory.Exists(newPath))
        {

            string[] b_bodypart_src = Directory.GetFiles(newPath, "*", SearchOption.AllDirectories);
            for (int i = 0; i < b_bodypart_src.Length; i++)
            {
                string simpleName = System.IO.Path.GetFileName(b_bodypart_src[i]);
                string id = simpleName.Split('_')[0];

                if (!Regex.IsMatch(id, @"\d{10}"))
                {
                    continue;
                }


                FileBean fb = new FileBean();
                fb.pre = m_bPath;
                fb.middlePath1 = add1;
                fb.middlePath2 = add2;
                fb.full = b_bodypart_src[i];
                fb.simple = fb.full.Replace(m_bPath + add1, "").Replace('\\', '/');
                branchFiles.Add(fb);
            }
        }

        m_trunkPath = m_trunkPath.Replace('\\', '/');
        newPath = m_trunkPath + add1 + add2;
        if (Directory.Exists(newPath))
        {
            string[] t_bodypart_src = Directory.GetFiles(newPath, "*", SearchOption.AllDirectories);
            for (int i = 0; i < t_bodypart_src.Length; i++)
            {
                string simpleName = System.IO.Path.GetFileName(t_bodypart_src[i]);
                string id = simpleName.Split('_')[0];

                if (!Regex.IsMatch(id, @"\d{10}"))
                {
                    continue;
                }
                FileBean fb = new FileBean();
                fb.pre = m_trunkPath;
                fb.middlePath1 = add1;
                fb.middlePath2 = add2;
                fb.full = t_bodypart_src[i];
                fb.simple = fb.full.Replace(m_trunkPath + add1, "").Replace('\\', '/');
                trunkFiles.Add(fb);
            }
        }
    }
    public bool OnFinish()
    {
        return true;
    }
    public string GetCurrentProcessName()
    {
        return "检查中";
        //return branchFiles[index].full;
        //return prefab_path;
    }
    public void DoSomeThing()
    {
        ////都存在的列表
        //List<FileBean> equList = branchFiles.FindAll(x => trunkFiles.Find(y => x.simple.Equals(y.simple)) != null);

        //第一次刷新
        RefreshList();
        //这里面就做了所有的事情
        addList.Clear();
        deleteList.Clear();
        branchFiles.FindAll(x => B2T(x, trunkFiles));
        checkList.Add(" add");
        checkList.AddRange(addList);
        checkList.Add(" delete");
        checkList.AddRange(deleteList);
        ResourceUsageCheck.WriteOnly(GetMyType(), checkList);


        ////要确认的(这里面有挪到库存的.需要找许亮确认  删除)
        //List<FileBean> bNotFound = trunkFiles.FindAll(x => branchFiles.Find(y => x.simple.Equals(y.simple)) == null);

        string a = "";
    }
    //private int copyCount = 0;
    //分支挪到主支.
    private List<string> addList = new List<string>();
    private List<string> checkList = new List<string>();
    private List<string> deleteList = new List<string>();
    private bool B2T(FileBean file, List<FileBean> thisList)
    {
        List<FileBean> fbList = thisList.FindAll(y => file.simple.Equals(y.simple));
        string copyPath = m_trunkPath + file.middlePath1 + file.simple;
        if (fbList == null || fbList.Count == 0)
        {
            //trunk找不到的,直接拷贝过来.

            string strPath = Path.GetDirectoryName(copyPath);
            if (!Directory.Exists(strPath))
            {
                Directory.CreateDirectory(strPath);
            }
            FileBean pendingFile = pendingFiles.Find(y => y.simple.Equals(file.simple));
            if (pendingFile != null)
            {
                addList.Add(copyPath);
                //System.IO.File.Copy(file.full, copyPath);
            }
            else
            {
                checkList.Add(copyPath);
                System.IO.File.Copy(file.full, copyPath);

            }
        }
        else
        {

            //需要保持一致. 删除trunk中的原有, 把branches里面的东西拷贝过来.
            //这里有个前提:保证branches里面的cdn和inv里面的文件没有重复!!!!!!!!!这个前提很重要!!!
            for (int i = 0; i < fbList.Count; i++)
            {
                //System.IO.File.SetAttributes(fbList[i].full, System.IO.FileAttributes.Normal);
                //System.IO.File.Delete(fbList[i].full);
                deleteList.Add(fbList[i].full);
            }
            string strPath = Path.GetDirectoryName(copyPath);
            if (!Directory.Exists(strPath))
            {
                Directory.CreateDirectory(strPath);
            }
            addList.Add(copyPath);
            //System.IO.File.Copy(file.full, copyPath);
        }
        return false;
    }
    private class FileBean
    {
        public string full;
        public string pre;
        public string middlePath1;
        public string middlePath2;
        public string simple;
        public FileType type;
        public FileBean otherFile;//比如当前是cdn.需要知道另外的
    }

    private enum FileType
    {
        CDN,
        INV,
        PEND,
    }
}