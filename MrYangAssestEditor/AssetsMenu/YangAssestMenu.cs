using UnityEditor;
using System.Collections;
using System;
using System.Diagnostics;
using System.Net;
using System.IO.Ports;
using System.Net.NetworkInformation;
using System.Net.Sockets;
//public static enum YangMenuWidget
//{
//    ROOT,
//    RUN,
//    OPEN,
//}
public class YangAssestMenu
{

    //F:\p4_workspace\DGM\x5_mobile\mobile_dancer\trunk\exe\resources\config\shared
    //F:\p4_workspace\DGM\x5_mobile\mobile_dancer\trunk\exe\bin
    [MenuItem("MrYangAssistant/重新初始化")]
    static void 重新初始化()
    {
        YangMenuHelper.helperIns.init();
        //EditorApplication.update = OnThreadLoop;
        //Thread t = new Thread(new ThreadStart(Cal));
        //t.Start();
    }

    [MenuItem("MrYangAssistant/看一下端口")]
    static void LookPort()
    {
        Process cur = Process.GetCurrentProcess();
        //当前进程的id
        UnityEngine.Debug.Log(cur.Id);
        //GetPort(cur.Id);
        ////当前进程的名称
        //UnityEngine.Debug.Log(cur.ProcessName);
        ////当前进程的启动时间
        //UnityEngine.Debug.Log(cur.StartTime);
        ////获取关联进程终止时指定的值,在退出事件中使用
        ////Console.WriteLine(cur.ExitCode);
        ////获取进程的当前机器名称
        //UnityEngine.Debug.Log(cur.MachineName); //.代表本地
        ////获取进程的主窗口标题。
        //UnityEngine.Debug.Log(cur.MainWindowTitle);

        ////UnityEngine.Debug.Log("我的端口: " + server.Connected + "   ip:" + ep.ToString());

        //IPEndPoint p = null;
        ////获取本地计算机的网络连接和通信统计数据的信息
        //IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
        ////返回本地计算机上的所有Tcp监听程序
        //IPEndPoint[] ipsTCP = ipGlobalProperties.GetActiveTcpListeners();
        //for (int i = 0; i < ipsTCP.Length; i++)
        //{
        //    UnityEngine.Debug.Log(ipsTCP[i].Port);
        //    if (ipsTCP[i].Port == 56948)
        //    {
        //        p = ipsTCP[i];
        //    }
        //}


        ////返回本地计算机上的所有UDP监听程序
        //IPEndPoint[] ipsUDP = ipGlobalProperties.GetActiveUdpListeners();
        //for (int i = 0; i < ipsUDP.Length; i++)
        //{
        //    if (ipsUDP[i].Port == 56948)
        //    {
        //        p = ipsUDP[i];
        //    }
        //    UnityEngine.Debug.Log(ipsUDP[i].Port);
        //}

        ////返回本地计算机上的Internet协议版本4(IPV4 传输控制协议(TCP)连接的信息。
        //TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
        //for (int i = 0; i < tcpConnInfoArray.Length; i++)
        //{
        //    UnityEngine.Debug.Log(tcpConnInfoArray[i].LocalEndPoint);
        //    if (tcpConnInfoArray[i].LocalEndPoint.Port == 56948)
        //    {
        //        p = tcpConnInfoArray[i].LocalEndPoint;
        //    }
        //}
        //UnityEngine.Debug.Log(p);


        //ArrayList list = PortIsUsed();
    }

    private static ArrayList PortIsUsed()
    {

        string[] ses = SerialPort.GetPortNames();

        //获取本地计算机的网络连接和通信统计数据的信息 
        IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();

        //返回本地计算机上的所有Tcp监听程序 
        IPEndPoint[] ipsTCP = ipGlobalProperties.GetActiveTcpListeners();

        //返回本地计算机上的所有UDP监听程序 
        IPEndPoint[] ipsUDP = ipGlobalProperties.GetActiveUdpListeners();

        //返回本地计算机上的Internet协议版本4(IPV4 传输控制协议(TCP)连接的信息。 
        TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

        ArrayList allPorts = new ArrayList();
        foreach (IPEndPoint ep in ipsTCP)
        {
            if (ep.Port == 56272)
            {
                UnityEngine.Debug.Log(ep.Port);
            }
            if (ep.Port == 56360)
            {
                UnityEngine.Debug.Log(ep.Port);
            }
            allPorts.Add(ep.Port);
        }
        foreach (IPEndPoint ep in ipsUDP)
        {
            if (ep.Port == 56272)
            {
                UnityEngine.Debug.Log(ep.Port);
            }

            if (ep.Port == 56360)
            {
                UnityEngine.Debug.Log(ep.Port);
            }
            allPorts.Add(ep.Port);
        }
        foreach (TcpConnectionInformation conn in tcpConnInfoArray)
        {
            if (conn.LocalEndPoint.Port == 56272)
            {
                UnityEngine.Debug.Log(conn.LocalEndPoint.Port);
            }
            if (conn.LocalEndPoint.Port == 56360)
            {
                UnityEngine.Debug.Log(conn.LocalEndPoint.Port);
            }
            allPorts.Add(conn.LocalEndPoint.Port);
        }
        allPorts.Sort();
        return allPorts;
    }
    [MenuItem("MrYangAssistant/修改配置")]
    static public void OpenPanelOverview()
    {
        YangMenuHelper.helperIns.ReloadAll();
        //YangMenuHelper.helperIns.ReadHitCache();
        YangMenuHelper.helperIns.StartPing();
        EditorWindow.GetWindow<SwitchFolderWindow>(true, "修改配置", true);
    }

    [MenuItem("MrYangAssistant/复制Animation文件夹路径")]
    static void CopyAnimDir()
    {
        YangMenuHelper.ControlTextEditor(YangMenuHelper.helperIns.projAbsPath + "Assets/StaticResources/art/UIAnim");
    }

    //[MenuItem("MrYangAssistant/开!")]

    //public static void ping各网段()
    //{
    //    List<IPAddress> ipList = new List<IPAddress>();
    //    string ip = "";
    //    for (int i = 0; i < 3; i++)
    //    {
    //        for (int j = 1; j < 256; j++)
    //        {
    //            ip = "192.168." + i + "." + j;
    //            ipList.Add(IPAddress.Parse(ip));
    //        }
    //    }
    //    YangMenuHelper.helperIns.ClearIp();
    //    foreach (var item in ipList)
    //    {
    //        Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    //        IAsyncResult ar = clientSocket.BeginConnect(item, 32017, new AsyncCallback(ConnectCallback), clientSocket);
    //    }
    //}
    //[MenuItem("MrYangAssistant/放!")]

    //public static void ping完了放进去()
    //{
    //    YangMenuHelper.helperIns.UpdateServerList();
    //}

    public static void ConnectCallback(IAsyncResult ar)
    {
        Socket server = ar.AsyncState as Socket;
        IPEndPoint ep = server.RemoteEndPoint as IPEndPoint;
        string ip = ep.Address.ToString();
        YangMenuHelper.helperIns.SaveIp(ip);
        UnityEngine.Debug.Log("connectioned:" + ip);
    }

}
