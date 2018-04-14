using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
public class YangOverallSituationWindow : EditorWindow
{
    private bool updated = false;
    private long startTagTime = 0;
    private string targetFileName = "";
    private List<string> targetDirNames;

    private bool deleteIsDown = false;
    private bool beginTT = true;
    private List<FileSystemInfo> dirfiles = new List<FileSystemInfo>();
    private Vector2 m_ScrollPosition;
    private List<FData> digOutFiles = new List<FData>();
    private int selectIndex;
    void LateUpdate()
    {
    }
    void OnGUI()
    {
        //可以注册想要的快捷操作!!!!!!!!!例如f1的快捷操作等等 之后直接关闭自己

        if (!updated)
        {
            updated = true;
            EditorApplication.update = UpdateTime;
        }

        Event e = Event.current;
        if (e.isKey)
        {
            if (e.keyCode == KeyCode.Escape)
            {
                Close();
                return;
            }
        }

        GUILayout.Label("当前工程路径:" + YangMenuHelper.helperIns.projMiniPath);
        ShowLoginOpenId();

        if (beginTT)
        {
            BeginTT();
        }
        else
        {
            EditTT();
        }
        ShowDigoutFileList();


        //description = targetFileName;
        //description = EditorGUILayout.TextArea(description, GUILayout.MaxHeight(20));
    }
    private int selectOpition = 0;

    void UpdateTime()
    {
        if (targetFileName != "" && YangMenuHelper.helperIns.GetNowTimeMilliseconds() - startTagTime > 800)
        {
            Debug.Log("超过延迟时间" + startTagTime + "  zifuchuan:" + targetFileName);
            beginFind(false);
        }
    }
    private string lockId = "";
    private void ShowLoginOpenId()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("登录的token:" + UnityEngine.PlayerPrefs.GetString(VersionModule.LAST_SEL_TOKEN));
        if (GUILayout.Button("复制token", GUILayout.Width(70)))
        {
            YangMenuHelper.ControlTextEditor(UnityEngine.PlayerPrefs.GetString(VersionModule.LAST_SEL_TOKEN));
        }
        if (!string.IsNullOrEmpty(YangMenuHelper.helperIns.LockToken))
        {
            lockId = YangMenuHelper.helperIns.LockToken;
            GUI.color = Color.green;
            if (GUILayout.Button("token已锁定,锁定的token:", GUILayout.Width(200)))
            {
                YangMenuHelper.helperIns.LockToken = "";
                YangMenuHelper.helperIns.WriteConfigLine(0, YangMenuHelper.LOCK_TOKEN, YangMenuHelper.helperIns.LockToken);
            }
            GUI.color = Color.white;
        }
        else
        {
            GUI.color = Color.gray;
            if (GUILayout.Button("token未锁定,点击将token锁定至:", GUILayout.Width(200)))
            {
                YangMenuHelper.helperIns.LockToken = lockId;
                YangMenuHelper.helperIns.WriteConfigLine(0, YangMenuHelper.LOCK_TOKEN, YangMenuHelper.helperIns.LockToken);
            }
            GUI.color = Color.white;
        }
        lockId = EditorGUILayout.TextField("", lockId);
        if (!string.IsNullOrEmpty(lockId) && !string.IsNullOrEmpty(YangMenuHelper.helperIns.LockToken) && !lockId.Equals(YangMenuHelper.helperIns.LockToken))
        {
            YangMenuHelper.helperIns.LockToken = "";//改变字符串初始化
            YangMenuHelper.helperIns.WriteConfigLine(0, YangMenuHelper.LOCK_TOKEN, YangMenuHelper.helperIns.LockToken);
        }
        //Debug.Log("lockId:" + lockId);

        GUILayout.EndHorizontal();

    }
    private void ShowDigoutFileList()
    {
        m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
        try
        {
            for (int i = 0; i < digOutFiles.Count; i++)
            {
                GUIStyle style = "Label";
                Rect rt = GUILayoutUtility.GetRect(GUIContent.none, style);
                string buttonName = digOutFiles[i].fileName;
                if (i == selectIndex)
                {
                    EditorGUI.DrawRect(rt, Color.gray);
                    buttonName += ":" + digOutFiles[i].filePath;
                }
                //GUI.DrawTexture
                //rt.x += (16 * EditorGUI.indentLevel);
                if (GUI.Button(rt, buttonName, style))
                {
                    if (selectIndex == i)
                    {
                        OpenSelect();
                    }
                    else
                    {
                        selectIndex = i;
                    }
                }
            }
        }
        catch (ArgumentException ae)
        {
            Debug.Log("ae" + ae);
        }
        EditorGUILayout.EndScrollView();
    }
    private void EditTT()
    {
        GUILayout.Label("寻找的文件夹路径");
        for (int i = 0; i < targetDirNames.Count; i++)
        {
            targetDirNames[i] = EditorGUILayout.TextField("", targetDirNames[i]);
        }
        if (GUILayout.Button("增加路径", GUILayout.Width(200)))
        {
            targetDirNames.Add("");
        }
        if (GUILayout.Button("点我:开始打开文件操作", GUILayout.Width(200)))
        {
            for (int i = 0; i < targetDirNames.Count; i++)
            {
                if (YangMenuHelper.IsDirectory(targetDirNames[i]))
                {
                    if (!targetDirNames[i].EndsWith("\\"))
                    {
                        targetDirNames[i] += "\\";
                    }
                }
                else
                {
                    targetDirNames.RemoveAt(i);
                }
            }
            YangMenuHelper.helperIns.WriteAllConfigLines("temp/openDir.txt", targetDirNames);
            InitFileDir();
            beginTT = true;

        }
    }
    private void BeginTT()
    {
        GUILayout.Label("输入的字符串:" + targetFileName);
        GUILayout.Label("路径条目");
        for (int i = 0; i < targetDirNames.Count; i++)
        {
            GUILayout.Label(targetDirNames[i]);
        }
        if (GUILayout.Button("点我:不做记录.修改本地路径", GUILayout.Width(200)))
        {
            beginTT = false;
        }
        //GUILayout.EndHorizontal();
        Event e = Event.current;
        if (e.isKey)
        {
            if ((e.keyCode >= KeyCode.A && e.keyCode <= KeyCode.Z) || (e.keyCode >= KeyCode.Keypad0 && e.keyCode <= KeyCode.Keypad9) || (e.keyCode >= KeyCode.Alpha0 && e.keyCode <= KeyCode.Alpha9))
            {
                int intoffset = KeyCode.Keypad0 - KeyCode.Alpha0;
                if (e.type == EventType.keyDown)
                {
                    startTagTime = YangMenuHelper.helperIns.GetNowTimeMilliseconds();
                    if ((e.keyCode >= KeyCode.Keypad0 && e.keyCode <= KeyCode.Keypad9))
                    {
                        targetFileName += ((char)((int)e.keyCode - intoffset));

                    }
                    else
                    {
                        targetFileName += ((char)(int)e.keyCode);

                    }
                    Debug.Log("当前buffer:" + targetFileName);
                }
            }
            else if (e.keyCode == KeyCode.Backspace || e.keyCode == KeyCode.Delete)//返回键
            {
                if (targetFileName.Length > 0)
                {
                    if (e.type == EventType.keyDown)
                    {
                        startTagTime = YangMenuHelper.helperIns.GetNowTimeMilliseconds();
                        if (deleteIsDown)
                        {
                            targetFileName = "";
                        }
                        else
                        {
                            deleteIsDown = true;
                            targetFileName = targetFileName.Substring(0, targetFileName.Length - 1);
                        }
                        Debug.Log("delete当前buffer:" + targetFileName);
                    }
                }

                if (e.type == EventType.keyUp)
                {
                    deleteIsDown = false;
                }
            }
            else if (e.type == EventType.keyUp && e.keyCode == KeyCode.Return)//enter
            {
                Debug.Log("关闭界面  最终字符串:" + targetFileName);
                if (string.IsNullOrEmpty(targetFileName) && selectIndex >= 0)
                {
                    OpenSelect();
                }
                else
                {
                    beginFind(true);
                }
            }
            else if (e.type == EventType.keyDown && (e.keyCode == KeyCode.UpArrow || e.keyCode == KeyCode.DownArrow))
            {
                if (e.keyCode == KeyCode.UpArrow)
                {
                    if (selectIndex > 0)
                    {
                        selectIndex--;
                    }
                }
                else
                {

                    if (digOutFiles.Count > selectIndex + 1)
                    {
                        selectIndex++;
                    }
                }
            }
        }
        //dirPath = EditorGUILayout.TextField("路径名", dirPath);
    }
    private void beginFind(bool openimmed)
    {
        if (string.IsNullOrEmpty(targetFileName))
        {
            return;
        }
        targetFileName = targetFileName.ToLower();
        digOutFiles.Clear();
        for (int i = 0; i < dirfiles.Count; i++)
        {

            if (dirfiles[i].Name.ToLower().StartsWith(targetFileName))
            {
                FData fd = new FData()
                {
                    fileName = dirfiles[i].Name,
                    filePath = dirfiles[i].FullName,
                };

                digOutFiles.Add(fd);
                if (openimmed)
                {
                    selectIndex = 0;
                    OpenSelect();
                    return;
                }
            }
        }

        if (digOutFiles.Count > 0)
        {
            selectIndex = 0;
        }
        else
        {
            selectIndex = -1;
        }
        this.Repaint();
        targetFileName = "";
    }
    void OnInspectorUpdate()
    {
        //Debug.Log("窗口面板的更新");
        //这里开启窗口的重绘，不然窗口信息不会刷新
        this.Repaint();
    }
    void OnDestroy()
    {
        EditorApplication.update = null;
        dirfiles.Clear();
        digOutFiles.Clear();
    }
    public void Awake()
    {
        targetDirNames = YangMenuHelper.helperIns.InitOpenDir();
        InitFileDir();
    }


    private void InitFileDir()
    {
        dirfiles.Clear();
        for (int i = 0; i < targetDirNames.Count; i++)
        {
            if (YangMenuHelper.IsDirectory(targetDirNames[i]))
            {
                DirectoryInfo dir = new DirectoryInfo(targetDirNames[i]);

                FileSystemInfo[] files = dir.GetFileSystemInfos();
                dirfiles.AddRange(files);
            }
        }
    }

    private void OpenSelect()
    {
        if (selectIndex >= 0)
        {
            System.Diagnostics.Process.Start(digOutFiles[selectIndex].GetFileAbsPath());
            Close();
        }
    }
    private class FData
    {
        public string fileName;
        public string filePath;
        public string GetFileAbsPath()
        {
            return filePath;
        }
    }
}
