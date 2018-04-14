using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Net.Sockets;
using System.Net;
using System;
public class SwitchFolderWindow : EditorWindow
{

    bool[] radioState = new bool[] { false, false, false };          //状态
    private Vector2 m_ScrollPosition;
    private string newSelfIp = "";
    private string selfIP = "";
    public void Awake()
    {
        selfIP = YangMenuHelper.helperIns.InitSelfIp();
        newSelfIp = selfIP;
    }

    void OnGUI()
    {
        Event e = Event.current;
        //trunk/branches
        //GUI.color = Color.white;
        //Dictionary<string, ProxyBool> fs = YangMenuHelper.helperIns.switchFloders;
        m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
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
        //server list
        GUI.color = Color.white;
        Dictionary<ServerInfo, ProxyBool> serverf = YangMenuHelper.helperIns.serverFloders;
        if (NGUIEditorTools.DrawHeader("切换服务器", "切换服务器key"))
        {
            NGUIEditorTools.BeginContents();
            GUILayout.BeginHorizontal();
            bool selectSelf = YangMenuHelper.helperIns.currentServerIP.Equals(selfIP);
            if (selectSelf != EditorGUILayout.ToggleLeft(selfIP, selectSelf))
            {
                if (!string.IsNullOrEmpty(selfIP))
                {
                    YangMenuHelper.helperIns.currentServerIP = selfIP;
                    WriteIP();
                }
            }

            newSelfIp = GUILayout.TextField(newSelfIp, 25);
            if (GUILayout.Button("保存"))
            {
                if (!string.IsNullOrEmpty(newSelfIp) && !selfIP.Equals(newSelfIp) && YangMenuHelper.helperIns.IsIP(newSelfIp))
                {
                    selfIP = newSelfIp;

                    YangMenuHelper.helperIns.WriteConfigLine(0, "temp/selfIp.txt", selfIP);

                }
            }
            GUILayout.EndHorizontal();

            int index = 0;
            foreach (var item in serverf)//用复选框实现单选按钮
            {
                if (item.Key.connectioned)
                {
                    GUI.color = Color.green;
                }
                else
                {
                    GUI.color = Color.red;
                }
                if (item.Value.m_bool != EditorGUILayout.ToggleLeft(item.Key.content + ":" + (item.Key.connectioned ? "已开" : "未开"), item.Value.m_bool))
                {
                    if (!item.Value.m_bool)
                    {
                        UnityEngine.Debug.Log("item:" + item.Key + "  item.Value.m_bool:" + item.Value.m_bool);
                        item.Value.m_bool = true;
                        YangMenuHelper.helperIns.currentServerIP = item.Key.ip;//选中.
                        WriteIP();
                    }
                }
                else
                {
                    if (!item.Key.ip.Equals(YangMenuHelper.helperIns.currentServerIP))
                    {
                        item.Value.m_bool = false;
                    }
                }
                index++;
            }

            NGUIEditorTools.EndContents();
        }
        //log level
        GUI.color = Color.white;
        Dictionary<int, LineConfigInfo> clientConfig = YangMenuHelper.helperIns.ywindow_clientConfigBean.configDic;
        if (NGUIEditorTools.DrawHeader("修改ClientConfig", "修改ClientConfigkey"))
        {
            NGUIEditorTools.BeginContents();
            foreach (var item in clientConfig)//用复选框实现单选按钮
            {
                if (item.Value.myType == CONFIG_TYPE.STRING || item.Value.myType == CONFIG_TYPE.BOOL)
                {
                    if (GUILayout.Button(item.Value.lineValue))
                    {
                        if (item.Value.myType == CONFIG_TYPE.STRING)
                        {
                            GenericMenu menu = new GenericMenu();
                            for (int i = 0; i < item.Value.lineValues.Count; i++)
                            {
                                menu.AddItem(new GUIContent(item.Value.lineValues[i]), false, StringChangeCallBack, new object[] { item.Value, item.Value.lineValues[i] });
                            }
                            menu.ShowAsContext();

                        }
                        else
                        {
                            item.Value.OnBoolChange();
                        }
                    }
                }
                else if (item.Value.myType == CONFIG_TYPE.INT || item.Value.myType == CONFIG_TYPE.INPUT)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(item.Value.lineValue);
                    item.Value.xmlValue = GUILayout.TextField(item.Value.xmlValue, 25);

                    if (GUILayout.Button("更改"))
                    {
                        item.Value.OnInputChange();
                    }

                    GUILayout.EndHorizontal();

                }
            }
            //ButtonCtrl layoutBtn = new ButtonCtrl();
            //layoutBtn.Caption = "操作区布局";
            //layoutBtn.Name = "LayoutButton";
            //layoutBtn.Size = btnRect;
            //layoutBtn.onClick = OnLayoutBtnClick;
            NGUIEditorTools.EndContents();
        }
        EditorGUILayout.EndScrollView();
    }


    void StringChangeCallBack(object obj)
    {
        object[] data = obj as object[];
        LineConfigInfo lci = data[0] as LineConfigInfo;
        string value = data[1] as string;
        lci.OnStringChange(value);

    }
    private void ConnectCallback(IAsyncResult ar)
    {
        Socket server = ar.AsyncState as Socket;
        EndPoint ep = server.RemoteEndPoint;
        UnityEngine.Debug.Log("建立成功: " + server.Connected + "   ip:" + ep.ToString());
    }
    void OnInspectorUpdate()
    {
        //Debug.Log("窗口面板的更新");
        //这里开启窗口的重绘，不然窗口信息不会刷新
        this.Repaint();
    }

    private void WriteIP()
    {
        YangMenuHelper.helperIns.WriteLine(YangMenuHelper.helperIns.GetLineIndex(YangMenuHelper.helperIns.versionList_PC_Path, "ip="), YangMenuHelper.helperIns.versionList_PC_Path, "ip=" + YangMenuHelper.helperIns.currentServerIP + ":33018");

        YangMenuHelper.helperIns.WriteLine(YangMenuHelper.helperIns.GetLineIndex(YangMenuHelper.helperIns.versionList_Mobile_Path, "ip="), YangMenuHelper.helperIns.versionList_Mobile_Path, "ip=" + YangMenuHelper.helperIns.currentServerIP + ":33018");

        YangMenuHelper.helperIns.WriteLine(YangMenuHelper.helperIns.GetLineIndex(YangMenuHelper.helperIns.gm_config_path + "shared/gm_client.xml", "<SDKServer"), YangMenuHelper.helperIns.gm_config_path + "shared/gm_client.xml", "\t\t<SDKServer sdk_server_ip=\"" + YangMenuHelper.helperIns.currentServerIP + "\" sdk_server_port=\"32011\" />");

    }
}
