using UnityEditor;
public class YangAssestOpenMenu
{

    [MenuItem("MrYangAssistant/open_file/打开我自己")]
    static void OpenSelf()
    {
        System.Diagnostics.Process.Start(YangMenuHelper.helperIns.editorPath);
    }


    [MenuItem("MrYangAssistant/open_file/audio文件")]
    static void OpenAudio()
    {
        System.Diagnostics.Process.Start(YangMenuHelper.helperIns.projResourcePath + YangMenuHelper.helperIns.GetSourceFilesPath() + "crossplatform/audio");
    }


    [MenuItem("MrYangAssistant/open_file/基准图")]
    static void Open基准图()
    {
        System.Diagnostics.Process.Start(YangMenuHelper.helperIns.projResourcePath + "Resources/美术资源/UI/新界面基准图");
    }

    [MenuItem("MrYangAssistant/open_file/配置路径", false, 0)]
    static void Open配置路径()
    {
        System.Diagnostics.Process.Start(YangMenuHelper.helperIns.configPath);
    }


    [MenuItem("MrYangAssistant/open_file/服务器配置路径")]
    static void Open服务器配置路径()
    {
        System.Diagnostics.Process.Start(YangMenuHelper.helperIns.projAbsPath + "../exe/resources/");
    }

    //F:\p4_workspace\DGM\x5_mobile\mobile_dancer_resource\Resources\ResourcePublish\CDN\SourceFiles\android\assetbundles
    [MenuItem("MrYangAssistant/open_file/assetbundles")]
    static void 打开assetbundles()
    {
        string bundlePath = YangMenuHelper.helperIns.projResourcePath + YangMenuHelper.helperIns.GetSourceFilesPath() + "android/assetbundles";
        System.Diagnostics.Process.Start(bundlePath);
    }
    [MenuItem("MrYangAssistant/open_file/打开美术卡牌图片路径")]
    static void 打开美术卡牌图片路径()
    {
        string bundlePath = YangMenuHelper.helperIns.projResourcePath + YangMenuHelper.helperIns.GetResourcePath() + "美术资源/UI/图标/";
        System.Diagnostics.Process.Start(bundlePath);
    }
    [MenuItem("MrYangAssistant/open_file/打包路径")]
    static void 打开打包路径()
    {
        System.Diagnostics.Process.Start(YangMenuHelper.helperIns.projAbsPath + "player_output/apk");
    }

    [MenuItem("MrYangAssistant/open_file/3dEffects_Tex")]
    static void Open3dEffects_Tex()
    {
        ////x5_mobile/mobile_dancer/trunk/client/Assets/StaticResources/art/3dEffects_Tex/
        System.Diagnostics.Process.Start(YangMenuHelper.helperIns.projAbsPath + "Assets/StaticResources/art/3dEffects_Tex/");
    }
    [MenuItem("MrYangAssistant/open_file/Effects_common")]
    static void Open3dEffects_common()
    {
        ////x5_mobile/mobile_dancer/trunk/client/Assets/StaticResources/art/UIEffects/common/
        System.Diagnostics.Process.Start(YangMenuHelper.helperIns.projAbsPath + "Assets/StaticResources/art/UIEffects/common/");
    }

    [MenuItem("MrYangAssistant/open_file/TempUIDesign")]
    static void OpenTempUIDesign()
    {
        ////x5_mobile/mobile_dancer/trunk/client/Assets/StaticResources/TempUIDesign/
        System.Diagnostics.Process.Start(YangMenuHelper.helperIns.projAbsPath + "Assets/StaticResources/TempUIDesign/");
    }



    [MenuItem("MrYangAssistant/open_file/原图自测路径")]
    static void Open原图自测路径()
    {
        //x5_mobile/mobile_dancer_resource/Resources/美术资源/UI/图标/background/  美术上传的对应路径.
        //x5_mobile/mobile_dancer_resource/Resources/ResourcePublish/CDN/SourceFiles/android/assetbundles/texture/background/  自测路径不上传.
        System.Diagnostics.Process.Start(YangMenuHelper.helperIns.projResourcePath + YangMenuHelper.helperIns.GetSourceFilesPath() + "android/assetbundles/texture/background/");
    }

    [MenuItem("MrYangAssistant/open_file/日志路径")]
    static void Open日志路径()
    {
        //F:\p4_workspace\DGM\x5_mobile\mobile_dancer\trunk\client\DGM\logs
        System.Diagnostics.Process.Start(YangMenuHelper.helperIns.projAbsPath + "DGM/logs/");
    }

    [MenuItem("MrYangAssistant/open_file/开始记录按键 &3")]
    public static void 开始记录按键()
    {
        YangOverallSituationWindow yangOverallSituationWindow = EditorWindow.GetWindow<YangOverallSituationWindow>(true, "开始记录按键", true);
    }

    [MenuItem("MrYangAssistant/open_file/安装apk")]
    public static void 安装apk()
    {
        System.Diagnostics.Process p = new System.Diagnostics.Process();
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.FileName = "cmd.exe";
        p.StartInfo.Arguments = "";
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardOutput = true;

        p.Start();

        string outtr = p.StandardOutput.ReadToEnd();
        UnityEngine.Debug.Log(outtr);
        p.Close();
    }
}
