using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class WaitingRoom01
{
    //F:\p4_workspace\DGM\x5_mobile\mobile_dancer\branches2\1.2.0\client\Assets\Scenes\Stage
    //F:\p4_workspace\DGM\x5_mobile\mobile_dancer\branches2\1.2.0\client\Assets\StaticResources\art\3d\stage\waitingroom_01

    public LoadingProcess Begin()
    {
        return new WaitingRoom01Process("stage/waitingRoom01");
    }
}


public class WaitingRoom01Process : LoadingProcess
{
    List<string> res_files;
    private string stageText;
    private string prePath = Application.dataPath.Replace("Assets", "");
    public WaitingRoom01Process(string type)
        : base(type)
    {
        string stagePath = Application.dataPath + @"\Scenes\Stage\waitingroom_01.unity";
        stageText = File.ReadAllText(stagePath);
        string resPath = @"Assets\StaticResources\art\3d\stage\waitingroom_01";
        //Application.dataPath +
        string[] res_list = Directory.GetFiles(resPath, "*", SearchOption.AllDirectories);
        res_files = new List<string>();
        for (int i = 0; i < res_list.Length; i++)
        {
            if (!res_list[i].EndsWith(".meta"))
            {
                res_files.Add(res_list[i].ToLower());
            }
        }

        //清除被mat引用的文件.
        string[] mats = Directory.GetFiles(resPath, "*.mat", SearchOption.AllDirectories);
        List<string> moveList = new List<string>();
        for (int j = 0; j < res_files.Count; j++)
        {
            if (res_files[j].EndsWith(".png"))
            {
                bool use = false;
                for (int i = 0; i < mats.Length; i++)
                {
                    string matContent = File.ReadAllText(prePath + mats[i]);
                    string guid = AssetDatabase.AssetPathToGUID(res_files[j]);
                    if (matContent.Contains(guid))
                    {
                        moveList.Add(res_files[j]);
                        use = true;
                    }
                    if (use)
                    {
                        break;
                    }
                }
            }
        }

        for (int i = 0; i < moveList.Count; i++)
        {
            res_files.Remove(moveList[i]);
        }

        max = res_files.Count;
        Do = DoSome;
        Name = delegate()
        {
            return "正在比对资源...";
        };
        Finish = OnFinish;
    }

    List<string> use = new List<string>();
    List<string> unUse = new List<string>();
    public void DoSome()
    {
        string guid = AssetDatabase.AssetPathToGUID(res_files[index]);
        if (stageText.Contains(guid))
        {
            use.Add(prePath + res_files[index]);
        }
        else
        {
            unUse.Add(prePath + res_files[index]);
        }
    }

    public bool OnFinish()
    {
        Write(use, WriteType.TEXT);//, "/use"
        Write(unUse, WriteType.TEXT);//, "/unuse"
        return true;
    }

}