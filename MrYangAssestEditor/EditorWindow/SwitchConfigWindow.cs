using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Net.Sockets;
using System.Net;
using System;
public class SwitchConfigWindow : EditorWindow
{

    bool[] radioState = new bool[] { false, false, false };          //状态
    private Vector2 m_ScrollPosition;
    void OnGUI()
    {
        //trunk/branches
        //GUI.color = Color.white;
        //Dictionary<string, ProxyBool> fs = YangMenuHelper.helperIns.switchFloders;
        //m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
        //if (NGUIEditorTools.DrawHeader("切换主分支", "切换主分支key"))
        //{
        //    NGUIEditorTools.BeginContents();
        //    int index = 0;
        //    foreach (var item in fs)//用复选框实现单选按钮
        //    {

        //        if (item.Value.m_bool != EditorGUILayout.ToggleLeft(item.Key, item.Value.m_bool))
        //        {
        //            if (!item.Value.m_bool)
        //            {
        //                UnityEngine.Debug.Log("item:" + item.Key + "  item.Value.m_bool:" + item.Value.m_bool);
        //                item.Value.m_bool = true;
        //                YangMenuHelper.helperIns.projMiniPath = item.Key;//选中.
        //                YangMenuHelper.helperIns.WriteConfigLine(0, "switchFloder.txt", "path=" + YangMenuHelper.helperIns.projMiniPath);
        //            }
        //        }
        //        else
        //        {
        //            //UnityEngine.Debug.Log(" false !!!item:" + item.Key + "  item.Value.m_bool:" + item.Value.m_bool);

        //            if (!item.Key.Equals(YangMenuHelper.helperIns.projMiniPath))
        //            {
        //                item.Value.m_bool = false;
        //            }
        //        }
        //        index++;
        //    }
        //    NGUIEditorTools.EndContents();
        //}
        ////server list
        //GUI.color = Color.white;
        //Dictionary<ServerInfo, ProxyBool> serverf = YangMenuHelper.helperIns.serverFloders;
        //if (NGUIEditorTools.DrawHeader("切换服务器", "切换服务器key"))
        //{
        //    NGUIEditorTools.BeginContents();
        //    int index = 0;
        //    foreach (var item in serverf)//用复选框实现单选按钮
        //    {
        //        if (item.Key.connectioned)
        //        {
        //            GUI.color = Color.green;
        //        }
        //        else
        //        {
        //            GUI.color = Color.red;
        //        }
        //        if (item.Value.m_bool != EditorGUILayout.ToggleLeft(item.Key.content + ":" + (item.Key.connectioned ? "已开" : "未开"), item.Value.m_bool))
        //        {
        //            if (!item.Value.m_bool)
        //            {
        //                UnityEngine.Debug.Log("item:" + item.Key + "  item.Value.m_bool:" + item.Value.m_bool);
        //                item.Value.m_bool = true;
        //                YangMenuHelper.helperIns.currentServerIP = item.Key.ip;//选中.
        //                YangMenuHelper.helperIns.WriteLine(YangMenuHelper.helperIns.GetLineIndex(YangMenuHelper.helperIns.versionList_PC_Path, "<server ip=\""), YangMenuHelper.helperIns.versionList_PC_Path, "\t\t<server ip=\"" + YangMenuHelper.helperIns.currentServerIP + "\" port=\"33018\"/>");

        //                YangMenuHelper.helperIns.WriteLine(YangMenuHelper.helperIns.GetLineIndex(YangMenuHelper.helperIns.versionList_Mobile_Path, "<server ip=\""), YangMenuHelper.helperIns.versionList_Mobile_Path, "\t\t<server ip=\"" + YangMenuHelper.helperIns.currentServerIP + "\" port=\"33018\"/>");

        //                YangMenuHelper.helperIns.WriteLine(YangMenuHelper.helperIns.GetLineIndex(YangMenuHelper.helperIns.configPath + "shared/gm_client.xml", "<SDKServer"), YangMenuHelper.helperIns.configPath + "shared/gm_client.xml", "\t\t<SDKServer sdk_server_ip=\"" + YangMenuHelper.helperIns.currentServerIP + "\" sdk_server_port=\"32011\" />");
        //            }
        //        }
        //        else
        //        {
        //            if (!item.Key.ip.Equals(YangMenuHelper.helperIns.currentServerIP))
        //            {
        //                item.Value.m_bool = false;
        //            }
        //        }
        //        index++;
        //    }

        //    NGUIEditorTools.EndContents();
        //}
        ////log level
        //GUI.color = Color.white;
        //EditorGUILayout.EndScrollView();
    }
    //private void ConnectCallback(IAsyncResult ar)
    //{
    //    Socket server = ar.AsyncState as Socket;
    //    EndPoint ep = server.RemoteEndPoint;
    //    UnityEngine.Debug.Log("建立是否成功: " + server.Connected + "   ip:" + ep.ToString());
    //}

}
