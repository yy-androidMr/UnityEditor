using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Aspose.Cells;
public class ProxyBool
{
    public bool m_bool;
}
public class ServerInfo
{
    public string ip;
    public string name;
    public bool connectioned = false;
    public string lineContent;
    public IPAddress address;
    //public KVPair<int, int> hitCount = new KVPair<int, int>() { Key = -1, Value = -1 };
    public string content
    {
        get
        {
            return lineContent;
        }
        set
        {
            lineContent = value;
            string[] kv = lineContent.Split('=');
            name = kv[0];
            ip = kv[1];
            address = IPAddress.Parse(ip);
        }
    }
}
public class YangMenuHelper
{
    private YangMenuHelper()
    {
        LOCK_TOKEN = "temp/LockToken.txt";
        //EditorApplication.playmodeStateChanged += PlayModeChanged;
        init();
    }
    //void PlayModeChanged()
    //{
    //    //if (!(EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode))
    //    //{
    //    //    return;
    //    //}
    //    if (!string.IsNullOrEmpty(YangMenuHelper.helperIns.LockToken))
    //    {
    //        UnityEngine.Debug.Log("强制设置token 至:" + YangMenuHelper.helperIns.LockToken);
    //        UnityEngine.PlayerPrefs.SetString(VersionModule.LAST_SEL_TOKEN, YangMenuHelper.helperIns.LockToken);
    //    }
    //}

    public static YangMenuHelper helperIns = new YangMenuHelper();

    private string m_ProjPath;
    public string configPath = "";
    public string crossplatformPath = "";//configPath 上一级.
    public string versionList_PC_Path = "";
    public string versionList_Mobile_Path = "";
    public string gm_path = "";
    public string gm_config_path = "";
    public string projMiniPath = "/trunk";//差异化路径.
    public string currentServerIP = "1.1.1.1";
    public List<string> currentClientConfig = new List<string>();
    public Dictionary<ServerInfo, ProxyBool> serverFloders = new Dictionary<ServerInfo, ProxyBool>();
    //命中的逻辑尸体
    //public Dictionary<string, KVPair<int, int>> hitCache = new Dictionary<string, KVPair<int, int>>();
    //private int deleteUnHitCount = 3;
    //-------------尸体的脚
    private FileSystemWatcher fileSystemWatcher;
    public YangWindowClientConfig ywindow_clientConfigBean = new YangWindowClientConfig();
    private string lock_token = "";
    public static string LOCK_TOKEN;
    public string LockToken
    {
        get
        {
            return lock_token;
        }
        set
        {
            lock_token = value;
        }

    }
    public string projAbsPath
    {
        get
        {
            //init();
            return m_ProjPath;
        }
    }

    public string editorPath
    {
        get
        {
            return m_ProjPath + "Assets/Editor/MrYangAssestEditor/";
        }
    }
    public string editorConfigPath
    {
        get
        {
            return m_ProjPath + "Assets/Editor/MrYangAssestEditor/config/";
        }
    }

    public string projResourcePath
    {
        get
        {
            //init();
            return m_ProjPath + GetResourcePreLevel() + "../../mobile_dancer_resource/";
        }
    }

    //public bool useTrunk
    //{
    //    get
    //    {
    //        return trunkPath;
    //    }
    //}
    public void init()
    {

        m_ProjPath = Application.dataPath.Replace("/Assets", "/");
        ReloadTrunkConfig();
        configPath = projResourcePath + GetSourceFilesPath() + "crossplatform/config/";
        crossplatformPath = projResourcePath + GetSourceFilesPath() + "crossplatform/";
        //configPath = projResourcePath + GetResourcePublishPath() + "/CDN/SourceFiles/android/assetbundles/config/";
        versionList_PC_Path = configPath + "shared/sdklogin/version_list_d.xml";
        versionList_Mobile_Path = configPath + "shared/sdklogin/version_list.xml";
        ReloadAll();

        UnityEngine.Debug.Log(editorPath + "config/");
        if (fileSystemWatcher == null)
        {
            fileSystemWatcher = new FileSystemWatcher(editorPath + "/config/");
            fileSystemWatcher.Changed += OnFileChangeHandler; //文件被修改时
            fileSystemWatcher.EnableRaisingEvents = true;
        }

        UnityEngine.Debug.Log("初始化完成!");
    }
    private void OnFileChangeHandler(object sender, FileSystemEventArgs e)
    {
        UnityEngine.Debug.Log("正在重新加载配置...");
        ReloadAll();
        UnityEngine.Debug.Log("配置重新加载成功!");
    }

    public static bool IsDirectory(string path)
    {
        if (System.IO.Directory.Exists(path))
        {
            //文件夹
            return true;
        }
        else if (System.IO.File.Exists(path))
        {
            //文件
            return false;
        }
        return false;
    }
    public string GetDirectoryPath(string filePath)
    {
        int lastIndex = filePath.LastIndexOf('/');
        string newDir = filePath.Substring(0, lastIndex + 1);
        return newDir;
    }

    public delegate bool OnReadLineHandler(int currentLine, string line);

    public void ReadLine(string path, OnReadLineHandler handler)
    {

        if (!Directory.Exists(path) && !File.Exists(path))
        {
            UnityEngine.Debug.Log("ReadLine, path is not exist:" + path);
            return;
        }
        string[] allLine = System.IO.File.ReadAllLines(path, GetEncoding(path));
        int currentLine = 0;
        foreach (string item in allLine)
        {
            if (handler != null)
            {
                if (digOutAnnotation(item))
                {

                }
                else if (handler(currentLine, item))
                {

                }
                else
                {
                    break;//返回false代表不读了
                }
                currentLine++;
            }
        }
    }

    public void ReadConfigLine(string configName, OnReadLineHandler handler)
    {
        ReadLine(editorConfigPath + configName, handler);
    }

    public void WriteConfigLine(int index, string configName, string content)
    {
        WriteLine(index, editorConfigPath + configName, content);
    }


    public void WriteLine(int index, string path, string content)
    {
        if (index == -1)
        {
            return;
        }
        fileSystemWatcher.EnableRaisingEvents = false;
        string[] allLine = System.IO.File.ReadAllLines(path);
        if (allLine.Count() < (index + 1))
        {
            allLine.ToList();

            string[] newLine = new string[index + 1];
            Array.Copy(allLine, 0, newLine, 0, allLine.Length);

            allLine = newLine;
            //allLine.CopyTo(newLineList, 0);
        }
        allLine[index] = content;
        System.IO.File.SetAttributes(path, System.IO.FileAttributes.Normal);
        System.IO.File.WriteAllLines(path, allLine);
        //YangMenuHelper.helperIns.StartTask(OpenRaisingEvents());
        //
        //Invoke("OpenRaisingEvents", 1.0f);
    }
    public void WriteAllConfigLines(string configName, List<string> content)
    {
        System.IO.File.SetAttributes(editorConfigPath + configName, System.IO.FileAttributes.Normal);
        System.IO.File.WriteAllLines(editorConfigPath + configName, content.ToArray());
    }
    private void OpenRaisingEvents()
    {
        fileSystemWatcher.EnableRaisingEvents = true;
        //yield return new WaitForSeconds(1.0f);
        //fileSystemWatcher.EnableRaisingEvents = true;
    }
    public string GetResourcePreLevel()
    {
        //向上退多少个层级?才能找得到resource
        int count = projMiniPath.ToCharArray().Count(x => x == '/');
        string temp = "";
        for (int i = 0; i < count; i++)
        {
            temp += "../";
        }
        return temp;
    }
    public string GetSourceFilesPath()
    {
        //比如获取0.1.0这个字符串. 用做路径拼接
        return GetResourcePath() + "ResourcePublish/CDN/SourceFiles/";
    }
    public string GetResourcePath()
    {
        if (projMiniPath.Contains("trunk"))
        {
            return "Resources/";
        }
        else
        {
            return projMiniPath.Substring(1, 1).ToUpper() + projMiniPath.Substring(2) + "/";
        }
    }
    public string Get美术资源Path()
    {
        //比如获取0.1.0这个字符串. 用做路径拼接
        if (projMiniPath.Contains("trunk"))
        {
            return "Resources/美术资源/";
        }
        else
        {
            return projMiniPath.Substring(1, 1).ToUpper() + projMiniPath.Substring(2) + "/美术资源/";
        }
    }
    public void ReloadTrunkConfig()
    {
        //trunkPath
        ReadConfigLine("switchFloder.txt", delegate(int currentLine, string line)
        {
            if (line != null && line.Contains("="))
            {
                projMiniPath = line.Split('=')[1];
                //LogWrapper.LogError("projMiniPath:" + projMiniPath);
                return false;
            }
            //return false;//读一行就不读了
            return true;
        });
    }
    public void ReloadAll()
    {
        ReloadTrunkConfig();
        //version_list_d.xml
        ReadLine(versionList_PC_Path, delegate(int currentLine, string line)
        {
            if (line.Contains("ip="))
            {
                string[] ipport = GetKVValue(line).Split(':');
                currentServerIP = ipport[0];
                //int index = line.IndexOf("ip") + 2;
                //int endIndex = line.IndexOf("port");
                //currentServerIP = line.Substring(index, endIndex - index);
                //currentServerIP = currentServerIP.Trim().Split('=')[1].Trim();
                //currentServerIP = currentServerIP.Substring(1, currentServerIP.Length - 2);
                return false;
            }
            return true;
        });


        //server_list.txt
        serverFloders.Clear();
        ReadConfigLine("server_list.txt", delegate(int currentLine, string line)
        {
            ServerInfo digoutServer = null;
            foreach (var item in serverFloders)
            {
                ServerInfo si = item.Key;
                if (si.content.Equals(currentLine))
                {
                    digoutServer = si;
                    break;
                }
            }

            Dictionary<ServerInfo, ProxyBool>.KeyCollection keys = serverFloders.Keys;
            //keys.
            if (digoutServer != null)
            {
                serverFloders.Remove(digoutServer);
            }
            digoutServer = new ServerInfo();
            digoutServer.content = line;
            serverFloders.Add(digoutServer, new ProxyBool() { m_bool = currentServerIP.Equals(line.Split('=')[1]) });
            return true;
        });

        ReadConfigLine(LOCK_TOKEN, delegate(int currentLine, string line)
        {
            UnityEngine.Debug.Log("LockToken:" + line);
            LockToken = line;
            return true;
        });

        string root = projAbsPath.Replace("/client", "");
        //System.Diagnostics.Process.Start(root + "exe/bin");///GMClient.exe 运行起来会出问题.
        if (!root.Contains("/mobile_dancer/trunk/"))
        {
            root = Regex.Replace(root, "/mobile_dancer/.+", "/mobile_dancer/trunk/");//(.+?)+
            //root = root.Replace("/mobile_dancer/*/", "/mobile_dancer/trunk/");
            UnityEngine.Debug.Log("root: " + root);
        }
        gm_path = (root + "exe/bin/");
        gm_config_path = (root + "../../mobile_dancer_resource/Resources/ResourcePublish/CDN/SourceFiles/crossplatform/config/");


        //client_config.xml
        ywindow_clientConfigBean.Init(ReadLine, ReadConfigLine, configPath + "client_config.xml", editorPath);

    }
    public List<string> InitOpenDir()
    {
        List<string> targetDirs = new List<string>();
        //初始化打开文件的路径
        ReadConfigLine("temp/openDir.txt", delegate(int currentLine, string line)
        {
            targetDirs.Add(line);
            return true;
        });
        return targetDirs;
    }
    public string InitSelfIp()
    {
        string self_ip = "";
        ReadConfigLine("temp/selfIp.txt", delegate(int currentLine, string line)
        {
            self_ip = line;
            return false;
        });
        return self_ip;
    }
    public static bool digOutAnnotation(string str)
    {
        str = str.Trim();
        if (str != null && str != "" && (str.StartsWith("//") || str.StartsWith("<!--") || str.StartsWith("#")))
        {
            return true;//这一行是注释.
        }
        return false;//现在只能逐行解析.并不能完成/**/的解析.
    }

    public static List<string> GetFileDir(string path, string regix = null)
    {
        DirectoryInfo folder = new DirectoryInfo(path);
        List<string> list = new List<string>();
        if (regix == null)
        {
            foreach (FileInfo file in folder.GetFiles())//"*.txt"
            {
                list.Add(file.FullName);
            }
        }
        else
        {
            foreach (FileInfo file in folder.GetFiles(regix))//"*.txt"
            {
                list.Add(file.FullName);
            }
        }
        return list;
    }
    private static void GetAllFile(string dir, List<FileInfo> fileList, string regix = null)
    {
        DirectoryInfo theFolder = new DirectoryInfo(dir);
        FileInfo[] fileInfo = theFolder.GetFiles();
        foreach (FileInfo NextFile in fileInfo)  //遍历文件  
        {
            if (!string.IsNullOrEmpty(regix))//如果有类型筛选.
            {
                if (NextFile.Name.EndsWith(regix))
                {
                    fileList.Add(NextFile);
                }
            }
        }
        DirectoryInfo[] dirInfo = theFolder.GetDirectories();
        for (int i = 0; i < dirInfo.Length; i++)
        {
            GetAllFile(dirInfo[i].FullName, fileList, regix);
        }
    }

    //获取文件名,不带后缀.
    private static string GetFileNameWithoutExtension(string fileName)
    {
        string without = System.IO.Path.GetFileNameWithoutExtension(fileName);
        return without;
    }
    //获取拓展名,  后缀.
    private static string GetFileExtension(string fileName)
    {
        string extension = System.IO.Path.GetExtension(fileName);//扩展名 
        return extension;
    }
    public static List<string> GetFileListByDir()
    {
        return null;
    }
    public static string GetXMLValue(string line)
    {
        Regex reg = new Regex(">.*<");
        Match match = reg.Match(line);
        string value = match.Groups[0].Value;
        value = value.Substring(1, value.Length - 2);
        return value;
    }
    public static string GetKVValue(string line)
    {
        return line.Split('=')[1].Trim();
    }
    public static string GetKVKey(string line)
    {
        return line.Split('=')[0].Trim();
    }
    public static string GetXMLItemName(string line)
    {
        Regex reg = new Regex("</.*>");
        Match match = reg.Match(line);
        string value = match.Groups[0].Value;
        value = value.Substring(2, value.Length - 3);
        return value;
    }
    public static string ReplaceXMLItemValue(string item, string newValue)
    {
        item = Regex.Replace(item, ">.*<", ">" + newValue + "<");
        return item;
    }

    public static string ReplaceKVValue(string item, string newValue)
    {
        item = item.Split('=')[0] + "=" + newValue; //Regex.Replace(item, ">.*<", ">" + newValue + "<");
        return item;
    }
    static public void ControlTextEditor(string ss)
    {
        TextEditor te = new TextEditor();
        te.text = ss;
        te.OnFocus();
        te.Copy();
    }
    public static bool IsInt(string value)
    {
        return Regex.IsMatch(value, @"^[+-]?\d*$");
    }
    public static bool IsInput(string value)
    {
        return value.Contains(".");
    }

    public int GetLineIndex(string path, string regix)
    {
        int index = -1;
        ReadLine(path, delegate(int currentLine, string line)
        {
            if (line.Contains(regix))
            {
                index = currentLine;
                return false;
            }
            return true;
        });
        return index;
    }


    public void StartPing()
    {
        //serverFloders.OrderBy(o=>)
        if (serverFloders != null && serverFloders.Count > 0)
        {
            Dictionary<ServerInfo, ProxyBool> s2 = new Dictionary<ServerInfo, ProxyBool>(serverFloders);
            foreach (var item in s2)
            {
                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IAsyncResult ar = clientSocket.BeginConnect(item.Key.address, 32017, new AsyncCallback(ConnectCallback), clientSocket);
            }
        }
    }
    private void ConnectCallback(IAsyncResult ar)
    {
        Socket server = ar.AsyncState as Socket;
        IPEndPoint ep = server.RemoteEndPoint as IPEndPoint;
        string ip = ep.Address.ToString();
        foreach (var item in serverFloders)
        {
            ServerInfo si = item.Key;
            if (si.ip.Equals(ip))
            {
                si.connectioned = server.Connected;
                // 命中次数的逻辑 尸体
                //KVPair<int, int> kvp = null;
                //if (hitCache.TryGetValue(ip, out kvp))
                //{
                //}
                //else
                //{
                //    kvp = si.hitCount;
                //    hitCache.Add(ip, kvp);
                //}
                //if (kvp != null)
                //{
                //    if (kvp.Key <= 0)
                //    {
                //        kvp.Key = 1;
                //    }
                //    else
                //    {
                //        kvp.Key++;
                //    }
                //    kvp.Value = 0;
                //-------------尸体的脚
                OrderDescendingServerList();
                //}
            }
        }
        //UnityEngine.Debug.Log("建立成功: " + server.Connected + "   ip:" + ep.ToString() + "  type:" + ip);
    }
    private void RefreshHitServer(string serverIp)
    {

    }

    public static Encoding GetEncoding(string fileName)
    {
        FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        Encoding targetEncoding = GetEncoding(fs, Encoding.Default);
        fs.Close();
        //UnityEngine.Debug.Log("GetEncoding:" + targetEncoding);
        return targetEncoding;
    }


    public static Encoding GetEncoding(FileStream stream, Encoding defaultEncoding)
    {

        byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
        byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
        byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM 
        Encoding reVal = Encoding.Default;

        BinaryReader r = new BinaryReader(stream, System.Text.Encoding.Default);
        int i;
        int.TryParse(stream.Length.ToString(), out i);
        byte[] ss = r.ReadBytes(i);
        if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
        {
            reVal = Encoding.UTF8;
        }
        else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
        {
            reVal = Encoding.BigEndianUnicode;
        }
        else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
        {
            reVal = Encoding.Unicode;
        }
        r.Close();
        return reVal;
    }


    private static bool IsUTF8Bytes(byte[] data)
    {
        int charByteCounter = 1; //计算当前正分析的字符应还有的字节数 
        byte curByte; //当前分析的字节. 
        for (int i = 0; i < data.Length; i++)
        {
            curByte = data[i];
            if (charByteCounter == 1)
            {
                if (curByte >= 0x80)
                {
                    //判断当前 
                    while (((curByte <<= 1) & 0x80) != 0)
                    {
                        charByteCounter++;
                    }
                    //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X 
                    if (charByteCounter == 1 || charByteCounter > 6)
                    {
                        return false;
                    }
                }
            }
            else
            {
                //若是UTF-8 此时第一位必须为1 
                if ((curByte & 0xC0) != 0x80)
                {
                    return false;
                }
                charByteCounter--;
            }
        }
        if (charByteCounter > 1)
        {
            throw new Exception("非预期的byte格式");
        }
        return true;
    }




    public void WaitForTime(int time = 2000)
    {
        Thread tr = Thread.CurrentThread;
        Thread.Sleep(time);
    }

    public long GetNowTimeMilliseconds()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        long time = Convert.ToInt64(ts.TotalMilliseconds);
        return time;

    }

    //public void ReadHitCache()
    //{
    //命中的逻辑尸体
    //string fileLocalPath = "temp/hitCount.txt";
    //ReadConfigLine(fileLocalPath, delegate(int currentLine, string line)
    //{
    //    string[] content = line.Split('=');
    //    if (content.Contains("deleteUnHitCount"))
    //    {
    //        deleteUnHitCount = int.Parse(content[1]);
    //    }
    //    else
    //    {
    //        if (hitCache.ContainsKey(content[0]))
    //        {
    //            return true;
    //        }
    //        KVPair<int, int> kvp = new KVPair<int, int>();
    //        string[] useData = content[1].Split('_');
    //        int unUseCount = int.Parse(useData[1]);
    //        if (unUseCount >= deleteUnHitCount)
    //        {
    //            return true;//超过检测次数
    //        }
    //        kvp.Key = int.Parse(useData[0]);
    //        kvp.Value = int.Parse(useData[1]) + 1;//默认设置没中标.到connection的时候. 再给清空
    //        hitCache.Add(content[0], kvp);
    //    }
    //    return true;
    //});
    //foreach (var item in serverFloders)
    //{
    //    KVPair<int, int> kvp = null;
    //    hitCache.TryGetValue(item.Key.ip, out kvp);
    //    if (kvp != null)
    //    {
    //        item.Key.hitCount = kvp;
    //    }
    //}
    //OrderDescendingServerList();
    //-------------尸体的脚
    //}
    private void OrderDescendingServerList()
    {
        serverFloders = serverFloders.OrderByDescending(p => p.Key.connectioned).ToDictionary(p => p.Key, o => o.Value);//排序
    }
    private List<string> saveIpList = new List<string>();
    public void SaveIp(string ip)
    {
        saveIpList.Add(ip + "=" + ip);
    }
    public void ClearIp()
    {
        saveIpList.Clear();
    }

    public void UpdateServerList()
    {
        string configPath = editorPath + "/config/server_list.txt";
        string[] allLine = System.IO.File.ReadAllLines(configPath, GetEncoding(configPath));
        List<string> newIpList = new List<string>();
        int nowIpListCount = allLine.Length;
        for (int i = 0; i < nowIpListCount; i++)
        {
            string item = allLine[i];
            string currentIp = item.Split('=')[1];
            if (saveIpList.Contains(currentIp + "=" + currentIp))
            {
                saveIpList.Remove(currentIp + "=" + currentIp);
            }
        }
        saveIpList.InsertRange(0, allLine);
        System.IO.File.SetAttributes(configPath, System.IO.FileAttributes.Normal);
        System.IO.File.WriteAllLines(configPath, saveIpList.ToArray());
        saveIpList.Clear();
    }

    public bool IsIP(string ipaddress)
    {
        Regex validipregex = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
        return (ipaddress != "" && validipregex.IsMatch(ipaddress.Trim())) ? true : false;
    }

    public static void CreateExcel(string filePath)
    {
        Workbook workbook = new Workbook();
        workbook.Save(filePath);

        //string file = "C:\\newdoc.xls";
        //Workbook workbook = new Workbook();
        //Worksheet worksheet = new Worksheet("First Sheet");
        //worksheet.Cells[0, 1] = new Cell((short)1);
        //worksheet.Cells[2, 0] = new Cell(9999999);
        //worksheet.Cells[3, 3] = new Cell((decimal)3.45);
        //worksheet.Cells[2, 2] = new Cell("Text string");
        //worksheet.Cells[2, 4] = new Cell("Second string");
        //worksheet.Cells[4, 0] = new Cell(32764.5, "#,##0.00");
        //worksheet.Cells[5, 1] = new Cell(DateTime.Now, @"YYYY\-MM\-DD");
        //worksheet.Cells.ColumnWidth[0, 1] = 3000;
        //workbook.Worksheets.Add(worksheet);
        //workbook.Save(file);
        //// open xls file
        //Workbook book = Workbook.Load(file);
        //Worksheet sheet = book.Worksheets[0];
        //// traverse cells
        //foreach (Pair<Pair<int, int>, Cell> cell in sheet.Cells)
        //{
        //    dgvCells[cell.Left.Right, cell.Left.Left].Value = cell.Right.Value;
        //}
        //// traverse rows by Index
        //for (int rowIndex = sheet.Cells.FirstRowIndex;
        //rowIndex <= sheet.Cells.LastRowIndex; rowIndex++)
        //{
        //    Row row = sheet.Cells.GetRow(rowIndex);
        //    for (int colIndex = row.FirstColIndex;
        //    colIndex <= row.LastColIndex; colIndex++)
        //    {
        //        Cell cell = row.GetCell(colIndex);
        //    }
        //}
    }
}
