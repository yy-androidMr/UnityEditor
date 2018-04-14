using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.Text;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Net;
using System.IO.Ports;
using System.Net.NetworkInformation;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
public class YangAssestRunMenu
{
    //给他10个 位置.
    [MenuItem("MrYangAssistant/运行/gm", false, 1)]
    static void 运行gm()
    {
        //F:\p4_workspace\DGM\x5_mobile\mobile_dancer\trunk\exe\bin
        if (System.IO.File.Exists(YangMenuHelper.helperIns.gm_path + "GMClient.exe"))
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = YangMenuHelper.helperIns.gm_path + "GMClient.exe";
            psi.UseShellExecute = false;
            psi.WorkingDirectory = YangMenuHelper.helperIns.gm_path;
            psi.CreateNoWindow = false;
            psi.Arguments = "_blank";
            Process.Start(psi);
        }
        else
        {
            System.Diagnostics.Process.Start(YangMenuHelper.helperIns.projAbsPath + "GMClient.sln");
        }
    }

    [MenuItem("MrYangAssistant/运行/server的vs", false, 3)]
    static void 运行server的vs()
    {
        //F:\p4_workspace\DGM\x5_mobile\mobile_dancer\branches\0.3.0\server\products\Project_DGM\dgm_sln
        System.Diagnostics.Process.Start(YangMenuHelper.helperIns.projAbsPath + "../server/products/Project_DGM/dgm_sln/DGM_Server.sln");

    }

    [MenuItem("MrYangAssistant/运行/server", false, 1)]
    static void 运行server()
    {
        //F:\p4_workspace\DGM\x5_mobile\mobile_dancer\branches\0.3.0\server\products\Project_DGM\dgm_sln
        if (!File.Exists(YangMenuHelper.helperIns.projAbsPath + "../exe/bin/svc_launch_d.exe"))
        {
            return;
        }
        System.Diagnostics.Process.Start(YangMenuHelper.helperIns.projAbsPath + "../exe/start_global_d.bat");
        YangMenuHelper.helperIns.WaitForTime();
        System.Diagnostics.Process.Start(YangMenuHelper.helperIns.projAbsPath + "../exe/start_allserver_d.bat");

    }


    [MenuItem("MrYangAssistant/运行/MainEnter &1", false, 50)]
    public static void 运行MainEnter()
    {

        //string curr = EditorSceneManager.loadedSceneCount;
        //Scene ss = EditorSceneManager.GetActiveScene();
        //int count = EditorSceneManager.loadedSceneCount;
        //if (EditorApplication.isPlaying && EditorApplication.currentScene == "")
        //{
        if (!string.IsNullOrEmpty(YangMenuHelper.helperIns.LockToken))
        {
            UnityEngine.Debug.Log("强制设置token 至:" + YangMenuHelper.helperIns.LockToken);
            UnityEngine.PlayerPrefs.SetString(VersionModule.LAST_SEL_TOKEN, YangMenuHelper.helperIns.LockToken);
        }
        EditorSceneManager.OpenScene("Assets/Scenes/Logic/MainEntry.unity");
        EditorApplication.ExecuteMenuItem("Edit/Play");
        //}
        //bool sadf = EditorApplication.isPlaying;
        //bool asdfsadfa = EditorApplication.isPaused;
    }
    [MenuItem("MrYangAssistant/运行/跳到UIEditor &2", false, 51)]
    public static void 跳到UIEditor()
    {
        //EditorApplication.isPlaying = false;
        //if (EditorApplication.isPlayingOrWillChangePlaymode)
        //{
        UnityEngine.Object o = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>("Assets/Scenes/Edit/UIEdit_New.unity");
        if (null != o)
        {
            AssetDatabase.OpenAsset(o);

            GameObject camera = GameObject.Find("Camera");
            Selection.activeGameObject = camera;
            //EditorGUIUtility.PingObject(o);
        }
        //}
        //EditorApplication.ExecuteMenuItem("Assets/Open");
        //EditorSceneManager.OpenScene("Assets/Scenes/Edit/UIEdit_New.unity");
        //Assets/Scenes/Edit/UIEdit_New.unity

    }

    [MenuItem("MrYangAssistant/运行/生成丢失引用文件list", false, 52)]
    public static void 生成丢失引用文件list()
    {
        string[] cs_files = Directory.GetFiles(YangMenuHelper.helperIns.projAbsPath + "Assets/Scripts", "*.cs", SearchOption.AllDirectories);
        string[] prefab_files = Directory.GetFiles(YangMenuHelper.helperIns.projAbsPath + "Assets/resources/Art/UIPrefabs", "*.prefab", SearchOption.AllDirectories);

        int file_index = 0;
        bool checkPrefabGuid = true;

        Regex prefab_guidReg = new Regex("guid: .*,");
        //先读取prefab,查找script引用
        Dictionary<string, List<string>> guidMirro = new Dictionary<string, List<string>>();
        List<string> digoutMsg = new List<string>();
        EditorApplication.update = delegate()
        {
            if (checkPrefabGuid)
            {
                string prefab_path = prefab_files[file_index];
                bool isCancel = EditorUtility.DisplayCancelableProgressBar("正在prefab中查找guid...", prefab_path, (float)file_index / (float)(prefab_files.Length));
                string allText = File.ReadAllText(prefab_path);
                //string[] allline = System.IO.File.ReadAllLines(prefab_path);
                MatchCollection mc = prefab_guidReg.Matches(allText);
                foreach (Match item in mc)
                {
                    string guid = item.Groups[0].Value;
                    guid = guid.Split(' ')[1];

                    guid = guid.Substring(0, guid.Length - 1);
                    List<string> prefabPath;
                    if (guidMirro.TryGetValue(guid, out prefabPath))
                    {

                    }
                    else
                    {
                        prefabPath = new List<string>();
                        guidMirro.Add(guid, prefabPath);
                    }
                    prefabPath.Add(prefab_path);
                }
                file_index++;
                if (isCancel || file_index >= prefab_files.Length)
                {
                    checkPrefabGuid = false;
                    EditorUtility.ClearProgressBar();
                    file_index = 0;
                    EditorApplication.update = null;
                    EditorApplication.update = delegate()
                             {
                                 string scrpit_path = cs_files[file_index];
                                 bool isSecondCancel = EditorUtility.DisplayCancelableProgressBar("比对guid中...", scrpit_path, (float)(file_index) / (float)cs_files.Length);

                                 string secondallText = File.ReadAllText(scrpit_path);
                                 if (Regex.IsMatch(secondallText, " : MonoBehaviour"))//| : UIDynamicScrollItem| : MonoBehaviour     UIController : MonoBehaviour
                                 {
                                     scrpit_path = scrpit_path.Replace(YangMenuHelper.helperIns.projAbsPath, "");
                                     string cs_guid = AssetDatabase.AssetPathToGUID(scrpit_path);
                                     List<string> prefabPath;
                                     if (guidMirro.TryGetValue(cs_guid, out prefabPath))
                                     {
                                         //digoutMsg.Add(" prefabPath.Count" + prefabPath.Count + ":" + scrpit_path);
                                         //UnityEngine.Debug.Log("get value :" + prefabPath.Count + "   num1:" + prefabPath[0]);
                                     }
                                     else
                                     {
                                         digoutMsg.Add(" prefabPath.Count" + 0 + ":" + scrpit_path);
                                     }
                                     //if()
                                 }
                                 file_index++;
                                 if (isSecondCancel || file_index >= cs_files.Length)
                                 {
                                     System.IO.File.WriteAllLines("G:\\cache\\tew2.txt", digoutMsg.ToArray());
                                     checkPrefabGuid = true;
                                     EditorUtility.ClearProgressBar();
                                     file_index = 0;
                                     EditorApplication.update = null;
                                 }
                             };
                }
            }
        };
    }
    [MenuItem("MrYangAssistant/运行/检查jar包重复类", false, 53)]
    public static void 检查jar包重复类()
    {
        ZipInputStream zipInStream = new ZipInputStream(File.OpenRead(@"H:\Android逆向助手_v2.2\classes_dex2jar.jar"));
        ZipEntry zip = null;
        List<string> source_jarList = new List<string>();
        while ((zip = zipInStream.GetNextEntry()) != null)
        {
            source_jarList.Add(zip.Name);
        }
        try
        {
            zipInStream.Close();
            List<string> package_fullName = new List<string>();
            List<string> info = WriteJars(source_jarList, ref package_fullName);
            List<string> usage = FindUseageDir(package_fullName);

            info.Add("-------------------");
            info.AddRange(usage);
            System.IO.File.WriteAllLines(@"G:\cache\重复jar\重复class输出.txt", info.ToArray());

        }
        catch (Exception ex)
        {
            UnityEngine.Debug.Log("UnZip Error");
            throw ex;
        }
    }
    private static List<string> FindUseageDir(List<string> fullName)
    {
        string proj_path = @"F:\p4_workspace\DGM\x5_mobile\mobile_dancer\trunk\client\Assets\Plugins\Android";

        DirectoryInfo direction = new DirectoryInfo(proj_path);
        FileInfo[] files = direction.GetFiles("*.jar", SearchOption.AllDirectories);

        List<string> proj_useageJarList = new List<string>();
        for (int i = 0; i < files.Length; i++)
        {
            List<string> target_item = UnZipJar(files[i].FullName);
            for (int j = 0; j < target_item.Count; j++)
            {
                if (fullName.Contains(target_item[j]))
                {
                    if (!proj_useageJarList.Contains(files[i].FullName))
                    {
                        proj_useageJarList.Add(files[i].FullName);
                    }
                }
            }
        }
        return proj_useageJarList;
    }
    private static List<string> WriteJars(List<string> source_jarList, ref List<string> m_target_packageFullNameList)
    {
        DirectoryInfo direction = new DirectoryInfo(@"G:\cache\重复jar\target\libs");
        FileInfo[] files = direction.GetFiles("*.jar", SearchOption.AllDirectories);

        List<string> info = new List<string>();
        bool isFullJar = true;

        for (int i = 0; i < files.Length; i++)
        {
            List<string> target_item = UnZipJar(files[i].FullName);
            isFullJar = true;
            List<string> tempInfoList = new List<string>();
            for (int j = 0; j < target_item.Count; j++)
            {
                if (target_item[j].EndsWith(".class"))
                {
                    if (source_jarList.Contains(target_item[j]))
                    {
                        tempInfoList.Add(files[i].Name + "---" + target_item[j]);
                        m_target_packageFullNameList.Add(target_item[j]);
                    }
                    else
                    {
                        isFullJar = false;
                    }
                }
            }
            //if (isFullJar)
            //{
            //    info.Add("该jar包里面所有class都重复:" + files[i].Name);
            //}
            //else
            //{
            info.AddRange(tempInfoList);
            //}
            tempInfoList.Clear();
            //ZipInputStream zipInStream = new ZipInputStream(File.OpenRead(@"G:\cache\重复jar\package\classes_dex2jar.jar"));
        }
        return info;
    }
    private static List<string> UnZipJar(string path)
    {
        ZipInputStream zipInStream = new ZipInputStream(File.OpenRead(path));
        ZipEntry zip = null;
        List<string> jarList = new List<string>();
        while ((zip = zipInStream.GetNextEntry()) != null)
        {
            jarList.Add(zip.Name);
        }
        try
        {
            zipInStream.Close();
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.Log("UnZip Error");
            throw ex;
        }
        return jarList;
    }
}
