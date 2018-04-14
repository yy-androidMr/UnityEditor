using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.Text;
using System.IO;
using UnityEditor.SceneManagement;
public class YangGameObjectMenu : EditorWindow
{

    // foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
    //{
    //    if(scene.enabled)
    //    {

    [MenuItem("GameObject/选择并复制路径", false, -100)]
    static void 选择并复制路径()
    {
        //Selection.activeGameObject//选择的
        Transform[] ts = Selection.transforms;
        foreach (Transform t in ts)
        {
            UnityEngine.Object ossssA = PrefabUtility.GetPrefabParent(t.gameObject);
            EditorGUIUtility.PingObject(ossssA);
            if (null != ossssA)
            {
                EditorGUIUtility.PingObject(ossssA);
                string path = AssetDatabase.GetAssetPath(ossssA);
                YangMenuHelper.ControlTextEditor(YangMenuHelper.helperIns.projAbsPath + path);
                return;//这里找不到. 证明是运行期间.
            }
        }
        选择并复制运行期间路径();
    }
    static void 选择并复制运行期间路径()
    {
        Transform[] ts = Selection.transforms;
        foreach (Transform t in ts)
        {
            string name = t.name;
            string[] allGuids = AssetDatabase.FindAssets(name, new string[] { "Assets" });
            string nearPath = String2NearPath(name, allGuids);
            if (nearPath != "")
            {
                //找到了
                UnityEngine.Object o = AssetDatabase.LoadAssetAtPath(nearPath, typeof(UnityEngine.Object));
                if (null != o)
                {
                    EditorGUIUtility.PingObject(o);
                }
                YangMenuHelper.ControlTextEditor(YangMenuHelper.helperIns.projAbsPath + nearPath);
                break;
            }
        }

    }

    //根据string字符串.比如(UILogin),找到对应的prefab
    public static string String2NearPath(string selectName, string[] allGuids)
    {
        selectName = selectName + ".prefab";
        int nameLength = selectName.Length;

        string currentPath = "";
        string currentFilePath = "";
        foreach (string guid in allGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (assetPath.EndsWith(".prefab"))//只找prefab
            {
                string[] filesName = assetPath.Split('/');
                string fileName = filesName[filesName.Length - 1];
                if (currentPath == "" || (Math.Abs(currentFilePath.Length - nameLength) > Math.Abs(fileName.Length - nameLength)))
                {
                    currentPath = assetPath;
                    currentFilePath = fileName;
                }
            }
        }
        return currentPath;
    }


    public static void save()
    {
        //foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)//场景
        //{
        //    if (scene.enabled)
        //    {
        //        //打开场景
        //        //EditorSceneManager.OpenScene(scene.path);
        //        //获取场景中的所有游戏对象
        //        GameObject[] gos = (GameObject[])FindObjectsOfType(typeof(GameObject));
        //----------
        //string[] allGuids = AssetDatabase.FindAssets("t:Prefab t:Scene", new string[] { "Assets" });   //查找可以filter
        //------------
        //PrefabUtility.GetPrefabType(go) //类型  
        //--------------
        //string selectPath = AssetDatabase.GetAssetOrScenePath(Selection.activeObject);//被选取的目录
        //UnityEngine.Debug.Log((int)JAVAExceptionCode.UPLOADPHOTO_EXCEPTION + "");  
        //UnityEngine.Debug.Log(Application.streamingAssetsPath);
        //UnityEngine.Debug.Log(Application.dataPath);
        //UnityEngine.Debug.Log(projAbsPath);
        //---------------
        //EditorApplication.update = null;
        //---------------

    }
}
