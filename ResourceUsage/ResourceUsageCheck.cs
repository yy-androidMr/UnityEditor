using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Xml;
using OfficeOpenXml;
using Aspose.Cells;

public class ResourceUsageCheck
{
    public static bool isXULIANG_LOOK = true;
    static string m_checkLocalPath = Application.dataPath + "/Editor/ResourceUsage/";
    static string m_ArtPath = "";
    static string m_SourcePath = "";
    public static string m_outputPath = "";
    static string addPath_art = "ArtPath";
    //static Regex prefabRegex = new Regex(@"^  objectPath: \S+UIPrefabs\S+\.prefab$", RegexOptions.Multiline);//prefabRegex
    static Dictionary<string, System.Diagnostics.Stopwatch> stopwatchDic;
    const string TAG_TEXTURE = "texture";
    const string TAG_ICON = "图标";
    const string TAG_BACKUP = "文件操作";
    const string TAG_UIPREFAB = "uiPrefab";
    const string TAG_CODE = "code";
    const string TAG_ACTION = "action";
    const string TAG_P4 = "p4";
    const string TAG_AUDIO = "audio_effect";
    const string TAG_AB_ACTION = "ab局内动作一致性";
    const string TAG_MOVE_INVENTORY = "库存移动";
    const string TAG_MOVE_PENDING = "pending移动";
    const string TAG_TRUNK2BRANCH = "主支cdn复制到分支";
    const string BRANCH_RESOURCE_MOVE = "分支资源挪主支";

    public class TextureFileInfo
    {
        public bool isPng = false;
        public FileInfo fInfo;
        public TextureFileInfo otherOneInfo;
        public List<string> useAge = new List<string>();
        public string guid;
        public string fileAbsPath;
        public string GetPngPathIfIsMat()
        {
            if (!isPng)
            {
                string tempName = GetFileNameWithoutExtension(fInfo.FullName);
                tempName = Path.GetDirectoryName(fInfo.FullName) + "\\" + tempName;
                tempName = tempName.Replace("_mat", ".png");
                return tempName;
            }
            return string.Empty;
        }
        public string GetMatPathIfIsPng()
        {
            if (isPng)
            {
                string tempName = fInfo.FullName.Replace(".png", "_mat.mat");
                return tempName;
            }
            return string.Empty;
        }
    }

    private static T ReadConfig2<T>(string type)
    {
        XmlDocument xmlDoc = new XmlDocument();
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.IgnoreComments = true;//忽略文档里面的注释
        XmlReader reader = XmlReader.Create(m_checkLocalPath + "PathConfig.xml", settings);
        xmlDoc.Load(reader);
        if (string.IsNullOrEmpty(m_ArtPath))
        {
            XmlNode xnode = xmlDoc.SelectSingleNode("PathConfig/ArtPath");
            m_ArtPath = xnode.InnerText;
        }
        if (string.IsNullOrEmpty(m_outputPath))
        {
            XmlNode xnode = xmlDoc.SelectSingleNode("PathConfig/OutPutPath");
            m_outputPath = xnode.InnerText;
        }

        if (string.IsNullOrEmpty(m_SourcePath))
        {
            XmlNode xnode = xmlDoc.SelectSingleNode("PathConfig/SourceFiles");
            m_SourcePath = xnode.InnerText;
        }
        switch (type)
        {
            case TAG_P4:
                {
                    XmlNode node = xmlDoc.SelectSingleNode("PathConfig/" + type + "/p4Compair");
                    List<string> content = new List<string>();
                    content.AddRange(File.ReadAllLines(m_outputPath + node.InnerText));

                    List<string> deleteList = new List<string>();
                    node = xmlDoc.SelectSingleNode("PathConfig/" + TAG_BACKUP + "/FileListPath");
                    deleteList.AddRange(File.ReadAllLines(m_outputPath + node.InnerText));

                    return (T)(object)(new List<string>[] { content, deleteList });
                }

            case TAG_AUDIO:
                {
                    XmlNode xnode = xmlDoc.SelectSingleNode("PathConfig/SourceFiles");
                    return (T)(object)xnode.InnerText;
                }

            case TAG_AB_ACTION:
                {
                    XmlNode xnode = xmlDoc.SelectSingleNode("PathConfig/SourceFiles");
                    return (T)(object)xnode.InnerText;
                }
            case TAG_MOVE_INVENTORY:
                {
                    XmlNode xnode = xmlDoc.SelectSingleNode("PathConfig/moveBack/src");
                    string src = xnode.InnerText;

                    xnode = xmlDoc.SelectSingleNode("PathConfig/moveBack/desc");
                    string desc = xnode.InnerText;

                    xnode = xmlDoc.SelectSingleNode("PathConfig/moveBack/idList");
                    if (xnode != null)
                    {

                        string ids = xnode.InnerText;//用,隔开.
                        MatchCollection mc = Regex.Matches(ids, "\\d+");
                        List<string> idList = new List<string>();
                        for (int i = 0; i < mc.Count; i++)
                        {
                            idList.Add(mc[i].Value);
                        }
                        return (T)(object)new object[] { src, desc, idList };
                    }
                    return (T)(object)new string[] { src, desc };

                }
            case TAG_MOVE_PENDING:
                {

                    XmlNode xnode = xmlDoc.SelectSingleNode("PathConfig/moveBack/pending");
                    string pending = xnode.InnerText;
                    xnode = xmlDoc.SelectSingleNode("PathConfig/moveBack/desc");
                    string desc = xnode.InnerText;

                    xnode = xmlDoc.SelectSingleNode("PathConfig/moveBack/idList");
                    if (xnode != null)
                    {

                        string ids = xnode.InnerText;//用,隔开.
                        MatchCollection mc = Regex.Matches(ids, "\\d+");
                        List<string> idList = new List<string>();
                        for (int i = 0; i < mc.Count; i++)
                        {
                            idList.Add(mc[i].Value);
                        }
                        return (T)(object)new object[] { pending, desc, idList };
                    }
                    return (T)(object)new string[] { pending, desc };
                }
            case TAG_TRUNK2BRANCH:
                {

                    XmlNode xnode = xmlDoc.SelectSingleNode("PathConfig/moveBack/src");
                    string pending = xnode.InnerText;
                    xnode = xmlDoc.SelectSingleNode("PathConfig/moveBack/desc");
                    string desc = xnode.InnerText;

                    xnode = xmlDoc.SelectSingleNode("PathConfig/moveBack/idList");
                    if (xnode != null)
                    {
                        string ids = xnode.InnerText;//用,隔开.
                        MatchCollection mc = Regex.Matches(ids, "\\d+");
                        List<string> idList = new List<string>();
                        for (int i = 0; i < mc.Count; i++)
                        {
                            idList.Add(mc[i].Value);
                        }
                        return (T)(object)new object[] { pending, desc, idList };
                    }
                    return (T)(object)new string[] { pending, desc };
                }


            case BRANCH_RESOURCE_MOVE:
                {
                    XmlNode xnode = xmlDoc.SelectSingleNode("PathConfig/branchPublish");
                    string src = xnode.InnerText;
                    return (T)(object)src;
                }
        }
        return default(T);
    }
    private static List<string> ReadConfig(string type)
    {
        XmlDocument xmlDoc = new XmlDocument();
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.IgnoreComments = true;//忽略文档里面的注释
        XmlReader reader = XmlReader.Create(m_checkLocalPath + "PathConfig.xml", settings);
        xmlDoc.Load(reader);
        if (string.IsNullOrEmpty(m_ArtPath))
        {
            XmlNode xnode = xmlDoc.SelectSingleNode("PathConfig/ArtPath");
            m_ArtPath = xnode.InnerText;
        }
        if (string.IsNullOrEmpty(m_outputPath))
        {
            XmlNode xnode = xmlDoc.SelectSingleNode("PathConfig/OutPutPath");
            m_outputPath = xnode.InnerText;
        }
        if (string.IsNullOrEmpty(m_SourcePath))
        {
            XmlNode xnode = xmlDoc.SelectSingleNode("PathConfig/SourceFiles");
            m_SourcePath = xnode.InnerText;
        }
        switch (type)
        {
            case TAG_TEXTURE:
                {
                    XmlNode typeNode = xmlDoc.SelectSingleNode("PathConfig/" + type + "/path");
                    List<string> content = new List<string>();
                    content.Add(typeNode.InnerText);
                    return content;
                }

            case TAG_ICON:
                {
                    XmlNodeList nodeList = xmlDoc.SelectNodes("PathConfig/" + type + "/search");
                    List<string> content = new List<string>();
                    for (int i = 0; i < nodeList.Count; i++)
                    {
                        XmlNode nodeItem = nodeList[i];
                        content.Add("search:" + nodeItem.Attributes["takeType"].Value + "=" + nodeItem.InnerText);
                    }

                    nodeList = xmlDoc.SelectNodes("PathConfig/" + type + "/IconPath");
                    for (int i = 0; i < nodeList.Count; i++)
                    {
                        XmlNode nodeItem = nodeList[i];
                        if (addPath_art.Equals(nodeItem.Attributes["addPath"].Value))
                        {
                            XmlAttribute attribute = nodeItem.Attributes["filter"];
                            if (attribute == null)
                            {
                                content.Add(m_ArtPath + nodeItem.InnerText);
                            }
                            else
                            {
                                content.Add(m_ArtPath + nodeItem.InnerText + " " + attribute.Value);
                            }
                        }
                    }
                    return content;
                }
            case TAG_BACKUP:
                {
                    XmlNode node = xmlDoc.SelectSingleNode("PathConfig/" + type + "/FileListPath");
                    List<string> content = new List<string>();
                    string[] lines = File.ReadAllLines(m_outputPath + node.InnerText);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (Directory.Exists(lines[i]))
                        {
                            string[] files = Directory.GetFiles(lines[i], "*", SearchOption.AllDirectories);
                            content.AddRange(files);
                        }
                        else
                        {
                            content.Add(lines[i]);
                        }

                    }
                    return content;
                }
            case TAG_UIPREFAB:
                {
                    XmlNodeList nodeList = xmlDoc.SelectNodes("PathConfig/" + type + "/ignore");
                    List<string> content = new List<string>();
                    for (int i = 0; i < nodeList.Count; i++)
                    {
                        XmlNode nodeItem = nodeList[i];
                        content.Add(nodeItem.Attributes["name"].Value);
                    }
                    return content;
                }
            case TAG_CODE:
                {
                    XmlNodeList nodeList = xmlDoc.SelectNodes("PathConfig/" + type + "/baseClass");
                    List<string> content = new List<string>();
                    for (int i = 0; i < nodeList.Count; i++)
                    {
                        XmlNode nodeItem = nodeList[i];
                        content.Add(nodeItem.InnerText);
                    }

                    //忽略列表.
                    nodeList = xmlDoc.SelectNodes("PathConfig/" + type + "/ignore");
                    for (int i = 0; i < nodeList.Count; i++)
                    {
                        XmlNode nodeItem = nodeList[i];
                        content.Add("ignore:" + nodeItem.Attributes["name"].Value);
                    }
                    return content;
                }
            case TAG_ACTION:
                {
                    XmlNode node = xmlDoc.SelectSingleNode("PathConfig/" + type + "/root");
                    List<string> content = new List<string>();
                    content.Add(m_ArtPath + node.InnerText);//动作路径 index=0
                    content.Add(m_SourcePath + @"\crossplatform\config\action_config\dance.xml");//局内配置路径. index=1

                    XmlNode xnode = xmlDoc.SelectSingleNode("PathConfig/SourceFiles");
                    content.Add(xnode.InnerText);//cdn目录下的SourceFiles index=2


                    XmlNodeList nodeList = xmlDoc.SelectNodes("PathConfig/" + type + "/regex4");
                    for (int i = 0; i < nodeList.Count; i++)
                    {
                        XmlNode nodeItem = nodeList[i];
                        content.Add("regex4:" + nodeItem.Attributes["preLine"].Value + ":" + nodeItem.InnerText);
                    }
                    nodeList = xmlDoc.SelectNodes("PathConfig/" + type + "/regex5");
                    for (int i = 0; i < nodeList.Count; i++)
                    {
                        XmlNode nodeItem = nodeList[i];
                        content.Add("regex5:" + nodeItem.Attributes["preLine"].Value + ":" + nodeItem.InnerText);
                    }
                    return content;
                }

        }
        return null;
    }
    [MenuItem("H3D/使用查找/读配置", false, -1)]
    static void rrrrrrrrrrrrrrrr()
    {


        string file = @"G:\cache\nt\\newdoc11.xls";
        Workbook workbook = new Workbook();

        Worksheet ws = workbook.Worksheets[0];

        ws.Cells[0, 0].PutValue(11);
        ws.Cells[0, 2].PutValue(11);
        workbook.Save(file);
        //Worksheet worksheet = new Worksheet();
        //FileInfo newFile = new FileInfo(@"G:\cache\nt\abc.xlsx");
        //if (!newFile.Exists)
        //{
        //    newFile.Create();
        //}

        //using (var package = new ExcelPackage(newFile))
        //{

        //    ExcelWorksheets sheets = package.Workbook.Worksheets;
        //    ExcelWorksheet sheet = sheets["Sheet1"];
        //    string a = "";
        //}

        //ReadConfig("图标");
        //ReadConfig(TAG_TEXTURE);
    }

    [MenuItem("H3D/gm_压测工具_编译检查", false, -1)]
    static void gm_compile()
    {
        System.Diagnostics.Process p = System.Diagnostics.Process.Start(System.IO.Path.GetFullPath("../client/tool_compile.bat"));
    }

    [MenuItem("H3D/使用查找/p4测试", false, -1)]
    static void p4Test()
    {

        System.Diagnostics.Process process_ = new System.Diagnostics.Process();
        process_.StartInfo.UseShellExecute = false;
        process_.StartInfo.RedirectStandardOutput = true;
        process_.StartInfo.RedirectStandardError = true;

        process_.StartInfo.EnvironmentVariables.Add("P4PORT", "172.17.100.135:3668");
        process_.StartInfo.EnvironmentVariables.Add("P4CLIENT", "yaoxiang_DGM");

        process_.StartInfo.EnvironmentVariables.Add("P4USER", "yaoxiang");
        process_.StartInfo.EnvironmentVariables.Add("P4PASSWD", "5b7dk8");
        process_.StartInfo.FileName = "p4";
        process_.StartInfo.CreateNoWindow = true;

        process_.StartInfo.Arguments = "delete //x5_mobile/mobile_dancer/trunk/client/Assets/engine/Editor/ResourceUsage/list/action/ingame_dance/dance - 副本.txt";
        process_.Start();
        process_.WaitForExit();

        if (process_.ExitCode != 0)
        {
            string error = process_.StandardError.ReadToEnd();
            Debug.Log("发生错误:" + error);
        }
        else
        {
            string output = process_.StandardOutput.ReadToEnd();
            Debug.Log("o了:" + output);
        }

        process_.Close();
    }

    [MenuItem("H3D/使用查找/UITexture输出", false, -1)]
    static void FildTextureUsage()
    {
        //规则.  查找png的guid,查找mat的guid.其中有一个呗prefab引用.则代表被使用.
        //StaticResources\art\UITexture
        List<string> path = ReadConfig(TAG_TEXTURE);
        List<FileInfo> fileList = new List<FileInfo>();
        for (int i = 0; i < path.Count; i++)
        {
            GetAllFile(path[i], fileList, ".png", ".mat");
        }
        Dictionary<string, TextureFileInfo> guidAndFile = new Dictionary<string, TextureFileInfo>();
        for (int i = 0; i < fileList.Count; i++)
        {
            string absPath = fileList[i].FullName.Replace('\\', '/').Replace(Application.dataPath, "Assets");
            string guid = AssetDatabase.AssetPathToGUID(absPath);
            if (!string.IsNullOrEmpty(guid))
            {
                TextureFileInfo fib = new TextureFileInfo();
                fib.fInfo = fileList[i];
                string extension = GetFileExtension(fileList[i].FullName);
                //if(tempName.EndsWith)
                fib.isPng = extension.Equals(".png");
                fib.guid = guid;
                fib.fileAbsPath = absPath;
                guidAndFile.Add(fileList[i].FullName, fib);
            }
            else
            {
                Debug.Log("该文件没有找到guid!!!:" + fileList[i].FullName);
            }
        }
        StartProcess(new TextureProcess(TAG_TEXTURE, guidAndFile));
    }
    private static void StartProcess(LoadingProcess lp)
    {
        StartTagTime(lp.GetMyType());
        EditorApplication.update = delegate()
        {
            bool isCancel = false;
            try
            {

                isCancel = EditorUtility.DisplayCancelableProgressBar("正在查找" + lp.GetMyType() + "引用...", lp.Name == null ? "" : lp.Name(), lp.CurentPercent());
                if (isCancel || lp.DoAdd())
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                }
            }
            catch (Exception e)
            {
                isCancel = true;
                Debug.Log(e);
            }

        };
    }

    public class IconFileInfo
    {
        public FileInfo fileInfo;
        public string fileSimpleName;
        public string typeFilter;
        public bool isTarget = false;
        public string rootPath = "";
        public List<string> usageList = new List<string>();
        public string ConvertSearchParten(string typeFilter)//.xml里面 或者代码里面
        {
            return GetFileNameWithoutExtension(fileInfo.Name);
        }

        //public List<IconFileInfo> ConvertFileInfo(bool target, string filter)
        //{

        //}
    }

    [MenuItem("H3D/使用查找/美术资源图标输出")]
    static void ExprotExcel()
    {
        List<IconFileInfo> fileDic = new List<IconFileInfo>();
        List<IconFileInfo> searchTargetDic = new List<IconFileInfo>();
        InitCheckFile(fileDic, searchTargetDic);

        StartProcess(new IconProcess(TAG_ICON, fileDic, searchTargetDic));
    }

    [MenuItem("H3D/使用查找/文件列表拷贝")]
    static void CopyIconFile()
    {
        FileFun(false);
    }

    [MenuItem("H3D/使用查找/文件列表移动")]
    static void MoveIconFile()
    {
        FileFun(true);
    }
    [MenuItem("H3D/使用查找/库存挪动到ab", false, -1)]
    static void MoveInventory()
    {
        object config = ReadConfig2<object>(TAG_MOVE_INVENTORY);
        object[] configList = config as object[];
        string src = configList[0].ToString();
        string desc = configList[1].ToString();
        List<string> idList = (List<string>)configList[2];
        List<string> id2List = new List<string>();
        ReadIds(idList);

        List<string> content = new List<string>();
        List<string> content2 = new List<string>();
        for (int i = 0; i < idList.Count; i++)
        {
            string[] inv = Directory.GetFiles(src, idList[i] + "*", SearchOption.AllDirectories);
            for (int j = 0; j < inv.Length; j++)
            {
                string inv_file = inv[j];
                if (string.IsNullOrEmpty(inv_file))
                {
                    continue;
                }
                if (!File.Exists(inv_file))
                {
                    continue;
                }
                string desc_file = inv_file.Replace(src, desc);
                //Debug.Log("invs:" + inv[j] + "     j:" + j + "  desc_file:" + desc_file);
                if (File.Exists(desc_file))
                {
                    content2.Add(desc_file);
                    System.IO.File.SetAttributes(desc_file, System.IO.FileAttributes.Normal);
                    System.IO.File.Delete(desc_file);
                }
                System.IO.File.Move(inv_file, desc_file);
                content.Add(inv_file);
            }
        }
        WriteOnly("moveBack/移动的文件列表", content, WriteType.TEXT);
        WriteOnly("moveBack/已存在", content2, WriteType.TEXT);

    }

    [MenuItem("H3D/使用查找/两条线路资源同步", false, -1)]
    static void CopyIDS()
    {
        object config = ReadConfig2<object>(TAG_MOVE_INVENTORY);
        object[] configList = config as object[];
        string src = configList[0].ToString();
        string desc = configList[1].ToString();
        List<string> idList = (List<string>)configList[2];
        List<string> id2List = new List<string>();
        for (int i = 0; i < idList.Count; i++)
        {
            if (idList[i].EndsWith("1"))
            {
                string Id2 = idList[i].Substring(0, idList[i].Length - 1) + "2";
                id2List.Add(Id2);
            }
        }
        idList.AddRange(id2List);

        List<string> content = new List<string>();
        List<string> content2 = new List<string>();
        for (int i = 0; i < idList.Count; i++)
        {
            string[] inv = Directory.GetFiles(src, idList[i] + "*", SearchOption.AllDirectories);
            for (int j = 0; j < inv.Length; j++)
            {
                string inv_file = inv[j];
                if (string.IsNullOrEmpty(inv_file))
                {
                    continue;
                }
                if (!File.Exists(inv_file))
                {
                    continue;
                }
                string desc_file = inv_file.Replace(src, desc);
                //Debug.Log("invs:" + inv[j] + "     j:" + j + "  desc_file:" + desc_file);
                if (File.Exists(desc_file))
                {
                    content2.Add(desc_file);
                    System.IO.File.SetAttributes(desc_file, System.IO.FileAttributes.Normal);
                    System.IO.File.Delete(desc_file);
                }
                System.IO.File.Copy(inv_file, desc_file);
                content.Add(inv_file);
            }
        }
        WriteOnly("moveBack/移动的文件列表", content, WriteType.TEXT);
        WriteOnly("moveBack/已存在", content2, WriteType.TEXT);

    }

    [MenuItem("H3D/使用查找/pending挪动到ab", false, -1)]
    static void MovePending()
    {
        object config = ReadConfig2<object>(TAG_MOVE_PENDING);
        object[] configList = config as object[];
        string src = configList[0].ToString();
        string desc = configList[1].ToString();
        List<string> idList = (List<string>)configList[2];
        List<string> id2List = new List<string>();
        ReadIds(idList);

        List<string> content = new List<string>();
        List<string> content2 = new List<string>();
        for (int i = 0; i < idList.Count; i++)
        {
            string[] inv = Directory.GetFiles(src, idList[i] + "*", SearchOption.AllDirectories);
            for (int j = 0; j < inv.Length; j++)
            {
                string inv_file = inv[j];
                if (string.IsNullOrEmpty(inv_file))
                {
                    continue;
                }
                if (!File.Exists(inv_file))
                {
                    continue;
                }
                string desc_file = inv_file.Replace(src, desc);
                //Debug.Log("invs:" + inv[j] + "     j:" + j + "  desc_file:" + desc_file);
                if (File.Exists(desc_file))
                {
                    content2.Add(desc_file);
                    System.IO.File.SetAttributes(desc_file, System.IO.FileAttributes.Normal);
                    System.IO.File.Delete(desc_file);
                }
                System.IO.File.Move(inv_file, desc_file);
                content.Add(inv_file);
            }
        }
        WriteOnly("moveBack/pending移动的文件列表", content, WriteType.TEXT);
        WriteOnly("moveBack/pendingCDN已存在", content2, WriteType.TEXT);

    }
    [MenuItem("H3D/使用查找/主支cdn挪动到分之cdn", false, -1)]
    static void CopyTrunk2Branch()
    {
        object config = ReadConfig2<object>(TAG_TRUNK2BRANCH);
        object[] configList = config as object[];
        string src = configList[0].ToString();
        string desc = configList[1].ToString();
        List<string> idList = (List<string>)configList[2];
        List<string> id2List = new List<string>();
        ReadIds(idList);

        List<string> content = new List<string>();
        List<string> content2 = new List<string>();
        for (int i = 0; i < idList.Count; i++)
        {
            string[] inv = Directory.GetFiles(src, idList[i] + "*", SearchOption.AllDirectories);
            for (int j = 0; j < inv.Length; j++)
            {
                string inv_file = inv[j];
                if (string.IsNullOrEmpty(inv_file))
                {
                    continue;
                }
                if (!File.Exists(inv_file))
                {
                    continue;
                }
                string desc_file = inv_file.Replace(src, desc);
                //Debug.Log("invs:" + inv[j] + "     j:" + j + "  desc_file:" + desc_file);
                if (File.Exists(desc_file))
                {
                    content2.Add(desc_file);
                }
                else
                {
                    content.Add(inv_file);
                }
                System.IO.File.SetAttributes(desc_file, System.IO.FileAttributes.Normal);
                System.IO.File.Delete(desc_file);
                System.IO.File.Copy(inv_file, desc_file);
            }
        }
        WriteOnly("moveBack/trunk复制的文件列表", content, WriteType.TEXT);
        WriteOnly("moveBack/branch已存在", content2, WriteType.TEXT);

    }



    [MenuItem("H3D/使用查找/ab挪动到库存", false, -1)]
    static void MoveAB()
    {
        object config = ReadConfig2<object>(TAG_MOVE_INVENTORY);
        object[] configList = config as object[];
        string desc = configList[0].ToString();
        string src = configList[1].ToString();
        List<string> idList = (List<string>)configList[2];
        List<string> content = new List<string>();
        List<string> content2 = new List<string>();
        ReadIds(idList);
        for (int i = 0; i < idList.Count; i++)
        {
            string[] ab = Directory.GetFiles(src, idList[i] + "*", SearchOption.AllDirectories);
            for (int j = 0; j < ab.Length; j++)
            {
                string inv_file = ab[j];
                if (string.IsNullOrEmpty(inv_file))
                {
                    continue;
                }
                if (!File.Exists(inv_file))
                {
                    continue;
                }
                string desc_file = inv_file.Replace(src, desc);
                //Debug.Log("invs:" + inv[j] + "     j:" + j + "  desc_file:" + desc_file);
                if (File.Exists(desc_file))
                {
                    File.SetAttributes(desc_file, FileAttributes.Normal);
                    System.IO.File.Delete(desc_file);
                }
                System.IO.File.Move(inv_file, desc_file);
                content.Add(inv_file);
            }
        }
        WriteOnly("moveTo/移动的文件列表", content, WriteType.TEXT);
        WriteOnly("moveTo/已存在", content2, WriteType.TEXT);

    }
    private static void ReadIds(List<string> idList)
    {
        List<string> id2List = new List<string>();
        for (int i = 0; i < idList.Count; i++)
        {
            if (idList[i].EndsWith("1"))
            {
                string Id2 = idList[i].Substring(0, idList[i].Length - 1) + "2";
                id2List.Add(Id2);
            }
            if (idList[i].EndsWith("2"))
            {
                string Id2 = idList[i].Substring(0, idList[i].Length - 1) + "1";
                id2List.Add(Id2);
            }
        }
        idList.AddRange(id2List);
        idList.Sort();
        idList = idList.Distinct().ToList();
    }

    [MenuItem("H3D/使用查找/比对库存和上架资源", false, -1)]
    static void CompairInventory()
    {
        string[] config = ReadConfig2<string[]>(TAG_MOVE_INVENTORY);
        string src = config[0];//inv
        string desc = config[1];
        List<string> content = new List<string>();
        List<string> content2 = new List<string>();
        string[] inv = Directory.GetFiles(src, "*", SearchOption.AllDirectories);
        for (int j = 0; j < inv.Length; j++)
        {
            string inv_file = inv[j];
            if (string.IsNullOrEmpty(inv_file))
            {
                continue;
            }
            string desc_file = inv_file.Replace(src, desc);
            //Debug.Log("invs:" + inv[j] + "     j:" + j + "  desc_file:" + desc_file);
            if (File.Exists(desc_file))
            {
                content2.Add(desc_file);
            }
            else
            {
                //System.IO.File.Move(inv_file, desc_file);
            }
            //content.Add(inv_file);
        }
        //WriteOnly("moveBack/移动的文件列表", content, WriteType.TEXT);
        WriteOnly("moveBack/已存在", content2, WriteType.TEXT);

    }

    private static void FileFun(bool isMove)
    {
        List<string> fileList = ReadConfig(TAG_BACKUP);

        StartProcess(new UIFileProcess(TAG_BACKUP, fileList, isMove, m_outputPath + "cacheFile\\"));


    }

    [MenuItem("H3D/使用查找/UIprefab输出")]
    static void ExprotPrefab()
    {
        List<string> path = ReadConfig(TAG_UIPREFAB);
        StartProcess(new UIPrefabProcess(TAG_UIPREFAB, path));
    }

    [MenuItem("H3D/使用查找/代码输出")]
    static void CSCode()
    {
        List<string> baseList = ReadConfig(TAG_CODE);
        ////先查找 mono的类
        //string[] m_cs_files = Directory.GetFiles(Application.dataPath + "/Scripts", "*.cs", SearchOption.AllDirectories);
        ////string[] m_cs_files = new string[] { @"F:\p4_workspace\DGM\x5_mobile\mobile_dancer\trunk\client\Assets\Scripts\GameModuleMsg\ArenaInfo.cs", @"F:\p4_workspace\DGM\x5_mobile\mobile_dancer\trunk\client\Assets\Scripts\UI\NewUI\Arena\songitemctrl.cs", @"F:\p4_workspace\DGM\x5_mobile\mobile_dancer\trunk\client\Assets\Scripts\Framework\UI\UIController.cs", @"F:\p4_workspace\DGM\x5_mobile\mobile_dancer\trunk\client\Assets\Scripts\UI\MainMission\UIOfficeCtrl.cs" };
        //List<string> csFilterFiles = new List<string>();
        //csFilterFiles.AddRange(m_cs_files);


        //baseList = baseList.Where(o => !o.StartsWith("ignore")).ToList();
        //List<string> ignoreList = baseList.Where(o => o.StartsWith("ignore")).ToList();


        //for (int i = 0; i < ignoreList.Count; i++)
        //{
        //    ignoreList[i] = ignoreList[i].Split(':')[1];
        //}
        //for (int i = 0; i < csFilterFiles.Count; )
        //{
        //    if (ignoreList.Contains(GetFileName(csFilterFiles[i])))
        //    {
        //        csFilterFiles.Remove(csFilterFiles[i]);
        //    }
        //    else
        //    {
        //        i++;
        //    }
        //}

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


        //static Regex prefabRegex = new Regex(@"objectGUID: \S+|selectObj: {fileID: .+guid: \S+");//prefabRegex


        //然后再去prefab上查找引用.

        //最后在剩下的去代码看有没有动态add
        StartProcess(new UICodeProcess(TAG_CODE, baseList));
    }

    [MenuItem("H3D/使用查找/动作输出")]
    static void ActionOutPut()
    {
        List<string> configList = ReadConfig(TAG_ACTION);

        StartProcess(new UIActionProcess(TAG_ACTION, configList));
    }
    [MenuItem("H3D/使用查找/音效输出")]
    static void AudioOut()
    {

        string sourceFilePath = ReadConfig2<string>(TAG_AUDIO);
        StartProcess(new UISoundProcess(TAG_AUDIO, sourceFilePath));

    }
    [MenuItem("H3D/使用查找/p4比对")]
    static void P4ToLocal()
    {
        object[] objList = ReadConfig2<object[]>(TAG_P4);
        List<string> local_delete = objList[1] as List<string>;
        List<string> p4_miss = objList[0] as List<string>;
        List<string> lcoalFileName = new List<string>();
        for (int i = 0; i < local_delete.Count; i++)
        {
            lcoalFileName.Add(System.IO.Path.GetFileName(local_delete[i]).ToLower());
        }
        List<string> p4FileName = new List<string>();
        for (int i = 0; i < p4_miss.Count; i++)
        {
            p4FileName.Add(System.IO.Path.GetFileName(p4_miss[i]).ToLower());
        }
        List<string> dif = p4FileName.FindAll(x => lcoalFileName.Contains(x));


        string outputPath = ResourceUsageCheck.m_outputPath + "p4比对结果" + ".txt";
        if (!File.Exists(outputPath))
        {
            FileStream fs1 = new FileStream(outputPath, FileMode.Create, FileAccess.Write);//创建写入文件 
            fs1.Close();
        }
        System.IO.File.WriteAllLines(outputPath, dif.ToArray());

        //string outputPath = ResourceUsageCheck.m_outputPath + "action.txt";
        //if (!File.Exists(outputPath))
        //{
        //    FileStream fs1 = new FileStream(outputPath, FileMode.Create, FileAccess.Write);//创建写入文件 
        //    fs1.Close(); 
        //}
        //System.IO.File.WriteAllLines(outputPath, m_ingame_actions.ToArray());
    }
    [MenuItem("H3D/使用查找/挪动上架服装")]
    static void MoveCloth()
    {
        string sourceFilePath = ReadConfig2<string>(TAG_AUDIO);
        //F:\p4_workspace\DGM\x5_mobile\mobile_dancer_resource\Branches2\1.2.0\ResourcePublish\CDN\SourceFiles\iOS\assetbundles\texture\item_icon
        string[] item_icon_source = Directory.GetFiles(m_outputPath + "服装资源/texture/item_icon", "*", SearchOption.AllDirectories);
        string[] item_icon_ab_source = Directory.GetFiles(@"F:\p4_workspace\DGM\x5_mobile\mobile_dancer_resource\Branches2\1.2.0\ResourcePublish\CDN\SourceFiles" + @"\android\assetbundles\texture\item_icon", "*", SearchOption.AllDirectories);

        List<MoveClothBean> ab_item_icon_list = new List<MoveClothBean>();


        //这些是配置上的.  需要挪回来的id . 在这边要做一个统一过滤.不需要挪动了.
        //        <idList>0006015701,0001019001,0008013601,1006020001,1001020901,1008019401,0013001301,1013001301,0013001201,1013001201,0013001202,1013001202,0013001101,1013001101,0013001801,1013001801,0013001803,1013001803,0013001802,1013001802,0013001201,1013001201,0013006501,1013006501,0013006401,1013006401,1017000801,0001031901,0008045001,0006024501,1001036501,1008045001,1006029601,0011008101,0013004601,1011009101,1013005801,0011008101,0013004601,1011009101,1013005801,0001032001,0008045101,0006032101,1001036601,1008045101,1006030701,0013004701,1013005901</idList>
        //<idList>1002008101,1003003101,1001005101,1006013101,1008025001,1008025501,1002009401,1003005001,1001001201,1006002601,1008009401,1008025601,1002002101,1001003301,1006003802,1002012101,1002017401,1003001601,1003011701,1001012201,1001012301,1006012901,1006008101</idList>
        //<idList>0011008301</idList>
        //<idList>0013005601,0011008001,0038000101,1013005601,1011008001,0038000101</idList>
        //<idList>0011011401,0011011402</idList>
        //<idList>1011008301</idList>

        //    <idList>1013006601,1013006701</idList>
        //        <idList>0015004101,0015004201,1015004401,1015004501</idList>
        //        <idList>1001003302,1006013102,1001005102,1006003801,1002008102,1002002102</idList>
        //MatchCollection mc = Regex.Matches(ignoreId, "\\d+");
        List<string> ignoreList = new List<string>();
        //for (int i = 0; i < mc.Count; i++)
        //{
        //    ignoreList.Add(mc[i].Value);
        //}

        WriteOnly("上架服装/忽略列表id", ignoreList, WriteType.TEXT);

        for (int i = 0; i < item_icon_ab_source.Length; i++)
        {
            string item = item_icon_ab_source[i];
            string simpleName = System.IO.Path.GetFileName(item);
            string id = simpleName.Split('_')[0];
            string simpleId = id;

            if (!Regex.IsMatch(id, @"\d{10}"))
            {
                continue;
            }
            if (ignoreList.Contains(id))
            {
                //忽略列表
                continue;
            }
            else if (simpleId.EndsWith("2"))//生成id
            {
                simpleId = simpleId.Substring(0, simpleId.Length - 1) + "1";
            }
            MoveClothBean mcb = ab_item_icon_list.Find(x => x.id.Equals(simpleId));
            if (mcb == null)
            {
                mcb = new MoveClothBean();
                mcb.id = simpleId;
                mcb.simpleName = System.IO.Path.GetFileNameWithoutExtension(simpleName);
                ab_item_icon_list.Add(mcb);
            }
            //真实的自己的id
            mcb.AddFullPath(item, id);
            //new MoveClothBean();
        }
        List<string> use_item_icon_list = new List<string>();
        for (int i = 0; i < item_icon_source.Length; i++)
        {
            use_item_icon_list.Add(System.IO.Path.GetFileName(item_icon_source[i]).Split('_')[0]);
        }
        List<MoveClothBean> delete_item_icon = ab_item_icon_list.FindAll(x => !use_item_icon_list.Contains(x.id));
        List<string> content = new List<string>();
        for (int i = 0; i < delete_item_icon.Count; i++)
        {
            for (int j = 0; j < delete_item_icon[i].fullPath.Count; j++)
            {
                content.Add(delete_item_icon[i].fullPath[j]);
                content.Add(delete_item_icon[i].fullPath[j].Replace("android", "iOS"));
            }
        }
        WriteOnly("上架服装/需要挪动的item_icon", content, WriteType.TEXT);


        //body party
        string[] bodypart_source = Directory.GetFiles(m_outputPath + @"服装资源\art\role", "*", SearchOption.AllDirectories);

        //F:\p4_workspace\DGM\x5_mobile\mobile_dancer_resource\Branches2\1.2.0\ResourcePublish\CDN\SourceFiles\iOS\assetbundles\art\role\bodypart\female
        string[] bodypart_ab_source = Directory.GetFiles(@"F:\p4_workspace\DGM\x5_mobile\mobile_dancer_resource\Branches2\1.2.0\ResourcePublish\CDN\SourceFiles" + @"\android\assetbundles\art\role\bodypart", "*", SearchOption.AllDirectories);
        string[] link_ab_source = Directory.GetFiles(@"F:\p4_workspace\DGM\x5_mobile\mobile_dancer_resource\Branches2\1.2.0\ResourcePublish\CDN\SourceFiles" + @"\android\assetbundles\art\role\link", "*", SearchOption.AllDirectories);

        // 标准文件夹. 从许亮拿来的要上架的服饰
        List<string> bodypart_simple = new List<string>();
        for (int i = 0; i < bodypart_source.Length; i++)
        {
            //MoveClothBean mcb = new MoveClothBean();
            //mcb.simpleName =;
            //mcb.AddFullPath(bodypart_source[i]);
            bodypart_simple.Add(System.IO.Path.GetFileName(bodypart_source[i]));
        }
        List<string> ignore = new List<string>();
        ignore.Add("face");
        ignore.Add("facetattoo");
        ignore.Add("skincolor");
        List<MoveClothBean> bodypart_ab_simple = new List<MoveClothBean>();
        for (int i = 0; i < bodypart_ab_source.Length; i++)
        {
            bool co = false;
            for (int j = 0; j < ignore.Count; j++)
            {
                if (bodypart_ab_source[i].Contains(ignore[j]))
                {
                    co = true;
                    break;
                }
            }
            if (co)
            {
                continue;
            }

            MoveClothBean mcb = new MoveClothBean();
            mcb.simpleName = System.IO.Path.GetFileName(bodypart_ab_source[i]);

            if (ignoreList.Contains(mcb.simpleName))
            {
                //忽略列表
                continue;
            }
            mcb.AddFullPath(bodypart_ab_source[i], mcb.simpleName);
            bodypart_ab_simple.Add(mcb);
        }
        for (int i = 0; i < link_ab_source.Length; i++)
        {

            MoveClothBean mcb = new MoveClothBean();
            mcb.simpleName = System.IO.Path.GetFileName(link_ab_source[i]);
            if (ignoreList.Contains(mcb.simpleName))
            {
                //忽略列表
                continue;
            }
            mcb.AddFullPath(link_ab_source[i], mcb.simpleName);
            bodypart_ab_simple.Add(mcb);
        }
        List<MoveClothBean> deletePart = bodypart_ab_simple.FindAll(x => !bodypart_simple.Contains(x.simpleName));

        content = new List<string>();
        for (int i = 0; i < deletePart.Count; i++)
        {
            for (int j = 0; j < deletePart[i].fullPath.Count; j++)
            {
                content.Add(deletePart[i].fullPath[j]);
                content.Add(deletePart[i].fullPath[j].Replace("android", "iOS"));

            }
        }
        WriteOnly("上架服装/需要挪动的role", content, WriteType.TEXT);


        content = new List<string>();
        for (int i = 0; i < delete_item_icon.Count; i++)
        {
            for (int j = 0; j < delete_item_icon[i].idList.Count; j++)
            {
                content.Add(delete_item_icon[i].idList[j]);
            }
        }

        for (int i = 0; i < deletePart.Count; i++)
        {
            content.Add(deletePart[i].idList[0]);
        }
        content = content.Distinct().ToList();
        content.Sort();
        WriteOnly("上架服装/需要挪动的id列表", content, WriteType.TEXT);
    }

    private class MoveClothBean
    {
        public string id;
        public string simpleName;
        public List<string> fullPath = new List<string>();
        public List<string> idList = new List<string>();
        public void AddFullPath(string full, string relaId)
        {
            fullPath.Add(full);
            idList.Add(relaId);
        }
    }
    [MenuItem("H3D/使用查找/场景")]
    static void FindStage()
    {
        //只是初始化一下配置.
        ReadConfig2<string>("abc");
        //StartProcess(new Dengluchangjing01().Begin());
        StartProcess(new WaitingRoom01().Begin());
    }

    [MenuItem("H3D/使用查找/分支资源合到主支")]
    static void Branches2TrunkResource()
    {
        string bPublishPath = ReadConfig2<string>(BRANCH_RESOURCE_MOVE);

        /**
         * 
         * 1.分支的在cdn下,需要挪回来.
         * 2.分之的在inventory下,需要挪回来
         * 3.主支多余的,保留多余.
         * 4.分支上有,主支没有, 确认
         * 
         * */
        StartProcess(new ResourceB2TProcess(BRANCH_RESOURCE_MOVE, bPublishPath, m_SourcePath));
    }

    [MenuItem("H3D/使用查找/CDNTexture删除")]
    static void DeleteCDNTexture()
    {
        CDNTexture cdt = CDNResourceMoveConfig.Instance().cdnTexture;

        Regex r = new Regex(@"[a-zA-Z0-9_]+"); // 定义一个Regex对象实例
        MatchCollection mc = r.Matches(cdt.iconList); // 在字符串中匹配
        List<string> iconList = new List<string>();
        for (int i = 0; i < mc.Count; i++) //在输入字符串中找到所有匹配
        {
            iconList.Add(mc[i].Value);
        }
        iconList = iconList.Distinct().ToList();

        for (int i = 0; i < cdt.src.Count; i++)
        {
            string rootPath = cdt.src[i].path;
            string androidPath = rootPath + cdt.AndroidPath();
            string iosPath = rootPath + cdt.IosPath();

            for (int j = 0; j < iconList.Count; j++)
            {
                string[] allAMat = Directory.GetFiles(androidPath, iconList[j] + "_mat*", SearchOption.AllDirectories);
                DeleteStrList(allAMat);
                string[] allIMat = Directory.GetFiles(iosPath, iconList[j] + "_mat*", SearchOption.AllDirectories);
                DeleteStrList(allIMat);
                string[] allPng = Directory.GetFiles(cdt.artSrc[i].path, iconList[j] + ".png", SearchOption.AllDirectories);
                DeleteStrList(allPng);
            }

        }
    }
    private static void DeleteStrList(string[] deleteList)
    {
        for (int i = 0; i < deleteList.Length; i++)
        {
            if (File.Exists(deleteList[i]))
            {
                System.IO.File.SetAttributes(deleteList[i], System.IO.FileAttributes.Normal);
                System.IO.File.Delete(deleteList[i]);
            }
        }
    }

    public static void WriteOnly(string outFileName, List<string> content, WriteType type = WriteType.TEXT)
    {
        string outputPath = ResourceUsageCheck.m_outputPath + outFileName;

        string dir = Path.GetDirectoryName(outputPath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        if (type == WriteType.EXCEL)
        {
            ExcelIns ei = ExcelManager.GetExcel(outputPath + ".xls");
            ei.WriteSimpleList(content);
            //ei.GetWorkSheet();
        }
        else
        {
            outputPath = outputPath + ".txt";
            if (!File.Exists(outputPath))
            {
                FileStream fs1 = new FileStream(outputPath, FileMode.Create, FileAccess.Write);//创建写入文件 
                fs1.Close();
            }
            System.IO.File.WriteAllLines(outputPath, content.ToArray());
        }
    }
    public static void StartTagTime(string key)
    {
        if (stopwatchDic == null)
        {
            stopwatchDic = new Dictionary<string, System.Diagnostics.Stopwatch>();
        }
        System.Diagnostics.Stopwatch watch;
        if (stopwatchDic.TryGetValue(key, out  watch))
        {

        }
        else
        {
            watch = new System.Diagnostics.Stopwatch();
            stopwatchDic.Add(key, watch);
        }
        watch.Start();
    }
    public static string StopTagTime(string key, string tag = "")
    {
        if (stopwatchDic != null)
        {
            System.Diagnostics.Stopwatch watch;
            if (stopwatchDic.TryGetValue(key, out  watch))
            {
                watch.Stop();
                if (tag == "")
                {
                    tag = key;
                }
                string content = tag + ":消耗时间:" + watch.Elapsed.TotalSeconds + "s";
                Debug.Log(content);
                stopwatchDic.Remove(key);
                return content;
            }
            else
            {
                Debug.Log("该key没有被记录");
            }
        }
        return "该key没有被记录";
    }

    //配置文件读取和初始化.
    private static void InitCheckFile(List<IconFileInfo> configFileDic, List<IconFileInfo> targetFileDic)
    {
        List<string> path = ReadConfig(TAG_ICON);
        for (int i = 0; i < path.Count; i++)
        {
            string temp = path[i];
            if (temp.StartsWith("search:"))
            {
                string[] targetKV = temp.Split('=');
                string fileTypeFilter = string.Empty;
                {
                    string[] fileTypeFilters = targetKV[0].Split(':');
                    if (fileTypeFilters.Length == 2)//证明有类型筛选
                    {
                        fileTypeFilter = fileTypeFilters[1];//将类型匹配查找出来.
                    }
                }
                List<IconFileInfo> iconfileList = new List<IconFileInfo>();
                List<FileInfo> fileList = new List<FileInfo>();
                GetAllFile(targetKV[1], fileList, fileTypeFilter);
                for (int j = 0; j < fileList.Count; j++)
                {
                    IconFileInfo ifi = new IconFileInfo();
                    ifi.isTarget = true;
                    ifi.fileInfo = fileList[j];
                    ifi.rootPath = targetKV[1];
                    ifi.typeFilter = fileTypeFilter;
                    iconfileList.Add(ifi);
                }
                targetFileDic.AddRange(iconfileList);
            }
            else
            {
                List<IconFileInfo> iconfileList = new List<IconFileInfo>();
                List<FileInfo> fileList = new List<FileInfo>();
                if (temp.Contains(' '))
                {
                    string[] pathRegix = temp.Split(' ');
                    // a\b\c a|b
                    string dest = pathRegix[1];
                    //// 将数组 src 中元素 2,3,4,5 复制到 dest  
                    //Array.Copy(pathRegix, 1, dest, 0, dest.Length);
                    Regex prefabRegex = new Regex(dest);//prefabRegex
                    GetAllFileByFilter(pathRegix[0], fileList, prefabRegex);
                }
                else
                {
                    GetAllFileByRegix(temp, fileList, null);
                }
                for (int j = 0; j < fileList.Count; j++)
                {
                    IconFileInfo ifi = new IconFileInfo();
                    ifi.isTarget = false;
                    ifi.fileInfo = fileList[j];
                    ifi.fileSimpleName = GetFileNameWithoutExtension(ifi.fileInfo.Name).ToLower();
                    ifi.rootPath = temp;
                    iconfileList.Add(ifi);
                }
                configFileDic.AddRange(iconfileList);
            }
        }
    }
    private static void GetAllFileByFilter(string dir, List<FileInfo> fileList, Regex regix)
    {
        DirectoryInfo theFolder = new DirectoryInfo(dir);
        FileInfo[] fileInfo = theFolder.GetFiles();
        foreach (FileInfo NextFile in fileInfo)  //遍历文件  
        {
            if (regix == null)
            {
                fileList.Add(NextFile);
            }
            else if (!regix.IsMatch(NextFile.Name))
            {
                fileList.Add(NextFile);
            }
        }
        DirectoryInfo[] dirInfo = theFolder.GetDirectories();
        for (int i = 0; i < dirInfo.Length; i++)
        {

            if (regix == null)
            {
                GetAllFileByRegix(dirInfo[i].FullName, fileList, regix);
            }
            else if (!regix.IsMatch(dirInfo[i].Name))
            {
                GetAllFileByRegix(dirInfo[i].FullName, fileList, regix);
            }
        }
    }
    private static void GetAllFileByRegix(string dir, List<FileInfo> fileList, Regex regix)
    {
        DirectoryInfo theFolder = new DirectoryInfo(dir);
        FileInfo[] fileInfo = theFolder.GetFiles();
        foreach (FileInfo NextFile in fileInfo)  //遍历文件  
        {
            if (regix == null)
            {
                fileList.Add(NextFile);
            }
            else if (!regix.IsMatch(NextFile.Name))
            {
                fileList.Add(NextFile);
            }
        }
        DirectoryInfo[] dirInfo = theFolder.GetDirectories();
        for (int i = 0; i < dirInfo.Length; i++)
        {
            if (regix == null)
            {
                GetAllFileByRegix(dirInfo[i].FullName, fileList, regix);
            }
            else if (!regix.IsMatch(dirInfo[i].Name))
            {
                GetAllFileByRegix(dirInfo[i].FullName, fileList, regix);
            }
        }
    }
    private static void GetAllFile(string dir, List<FileInfo> fileList, params string[] regix)
    {
        DirectoryInfo theFolder = new DirectoryInfo(dir);
        FileInfo[] fileInfo = theFolder.GetFiles();
        foreach (FileInfo NextFile in fileInfo)  //遍历文件  
        {
            if (regix != null && regix.Length > 0)//如果有类型筛选.
            {
                for (int i = 0; i < regix.Length; i++)
                {
                    if (NextFile.Name.EndsWith(regix[i]))
                    {
                        fileList.Add(NextFile);
                        break;//加入进去.
                    }
                }
            }
            else
            {
                fileList.Add(NextFile);
            }
        }
        DirectoryInfo[] dirInfo = theFolder.GetDirectories();
        for (int i = 0; i < dirInfo.Length; i++)
        {
            //bool con = false;
            //foreach (string noLook in noLookDirs)
            //{
            //    if (dirInfo[i].FullName.Contains(noLook))
            //    {
            //        Debug.Log("该文件夹被过滤:" + fileList[i].FullName);
            //        con = true;
            //        break;
            //    }
            //}
            //if (con)
            //{
            //    continue;
            //}
            //else
            //{
            GetAllFile(dirInfo[i].FullName, fileList, regix);
            //}
        }
    }
    private static bool NotCheck(string line)
    {
        if (line.StartsWith("#"))
        {
            return true;
        }
        return false;

    }
    public static string GetFileNameWithoutExtension(string fileName)
    {
        string without = System.IO.Path.GetFileNameWithoutExtension(fileName);
        return without;
    }
    public static string GetFileExtension(string fileName)
    {
        string extension = System.IO.Path.GetExtension(fileName);//扩展名 
        return extension;
    }
    public static string GetFileName(string fileName)//文件名+拓展名
    {
        return System.IO.Path.GetFileName(fileName);
    }

}
