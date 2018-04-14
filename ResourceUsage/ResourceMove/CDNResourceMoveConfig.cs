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


[System.Serializable]
public class PathInfo
{
    public string path;
    public bool enable = false;
    public int widget = 1;//权重. 越大越先移动.
}
[System.Serializable]

public class BasePathInfo
{
    public List<PathInfo> src = new List<PathInfo>();
    public List<PathInfo> desc = new List<PathInfo>();
    public bool revertAction = false;//反向操作.
    public bool isMove = true;
}


#region 音乐关卡文件 //x5_mobile/mobile_dancer_resource/Resources/ResourcePublish/CDN/SourceFiles/crossplatform/audio/ & //x5_mobile/mobile_dancer_resource/Resources/ResourcePublish/CDN/SourceFiles/crossplatform/level/
[System.Serializable]
public class CheckPoint : BasePathInfo
{
    //src = F:\p4_workspace\DGM\x5_mobile\mobile_dancer_resource\Resources\ResourcePublish\CDN\SourceFiles
    //desc = x5_mobile/mobile_dancer_resource/Resources/关卡资源/库存关卡/
    public string idList;
    public string GetBgmPath()
    {
        return @"\crossplatform\audio\bgm";
    }
    public string GetLevelPath()
    {
        return @"\crossplatform\level";
    }

    public string[] GetIgnoreDir()
    {
        return new string[] { "guide", "wedding" };
    }
}
#endregion

#region 服装资源和图标 assetbundles/art/role & assetbundles/texture/item_icon
[System.Serializable]
public class RoleTexture : BasePathInfo
{
    public string idcList;

}
#endregion

#region 一些图标,背景图,卡牌,删除 //x5_mobile/mobile_dancer_resource/Resources/ResourcePublish/CDN/SourceFiles/android/assetbundles/texture & //x5_mobile/mobile_dancer_resource/Resources/美术资源/UI/图标
[System.Serializable]
public class CDNTexture
{
    //src = F:\p4_workspace\DGM\x5_mobile\mobile_dancer_resource\Resources\ResourcePublish\CDN\SourceFiles
    public List<PathInfo> src = new List<PathInfo>();
    public string AndroidPath()
    {
        return @"\android\assetbundles\texture";
    }
    public string IosPath()
    {
        return @"\iOS\assetbundles\texture";
    }

    public List<PathInfo> artSrc = new List<PathInfo>();//美术资源\UI\图标 
    public string iconList;//只取 _和字符串

}
#endregion

#region 其他操作
[System.Serializable]
public class OtherInfo
{
    //card1
    public string cdnSource;
    public string art;
    //end
}
#endregion
/**
 * 
 * 1.inspector上分区
 * 2.增加统一替换方式, 因为几乎所有分区的根目录都是指向到SourceFiles
 * 
 * */
public class CDNResourceMoveConfig : ScriptableObject
{

    [NonSerialized]
    public int cardIndex = 0;

    public string logOutPath;

    public bool pathListInHide = false;
    public List<string> pathList;

    #region 分区1:移动服装素材,和对应的图标.
    public RoleTexture roleTexture;
    #endregion

    //要尝试在inspector上分区.
    #region 分区2:移动关卡
    public CheckPoint checkPoint;

    #endregion

    #region 分区3:移动图标
    public CDNTexture cdnTexture;
    #endregion

    #region 分区4 其他操作的数据
    public OtherInfo otherInfo;
    #endregion
    private static CDNResourceMoveConfig instance;
    static string path = "Assets/Editor/ResourceUsage/ResourceMove/CDNResourceMoveConfig.asset";


    public static CDNResourceMoveConfig Instance()
    {
        if (instance == null)
        {
            instance = AssetDatabase.LoadAssetAtPath<CDNResourceMoveConfig>(path);
        }
        if (instance == null)
        {
            ReInit();
        }
        return instance;
    }
    //public void ModifyPath(BasePathInfo baseInfo, bool isSrc, int index, string path, bool state)
    //{
    //    bool changed = false;
    //    path = path.Replace('/', '\\');
    //    if (isSrc)
    //    {
    //        if (srcEnable[index] != state)
    //        {
    //            srcEnable[index] = state;
    //            changed = true;
    //        }
    //        if (!src[index].Equals(path))
    //        {
    //            src[index] = path;
    //            changed = true;
    //        }
    //    }
    //    else
    //    {
    //        if (descEnable[index] != state)
    //        {
    //            descEnable[index] = state;
    //            changed = true;
    //        }
    //        if (!desc[index].Equals(path))
    //        {
    //            desc[index] = path;
    //            changed = true;
    //        }
    //    }
    //    if (changed)
    //    {
    //        EditorUtility.SetDirty(this);
    //        AssetDatabase.SaveAssets();
    //        AssetDatabase.Refresh();
    //    }
    //}
    public static void ReInit()
    {
        instance = ScriptableObject.CreateInstance<CDNResourceMoveConfig>();
        AssetDatabase.CreateAsset(instance, path);
    }
}