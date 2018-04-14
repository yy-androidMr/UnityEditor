using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public enum CONFIG_TYPE
{
    STRING,
    INT,
    BOOL,
    INPUT,
}
public class LineConfigInfo
{
    public List<string> lineValues;//类型是string 才有值
    public CONFIG_TYPE myType;
    public string lineValue;
    public string xmlValue;//值 YangMenuHelper.GetXMLValue(lineValue)
    public int lineIndex;
    public void OnIntChange(int addValue)
    {
        //int now = int.Parse(xmlValue);
        //lineValue = YangMenuHelper.ReplaceXMLItemValue(lineValue, (now + addValue) + "");
        //WriteToConfig();
    }
    public void OnInputChange()
    {
        //string nowValue = YangMenuHelper.GetXMLValue(lineValue);
        //int now = int.Parse(nowValue);
        //lineValue = YangMenuHelper.ReplaceXMLItemValue(lineValue, (now + addValue) + "");
        //WriteToConfig();
        lineValue = YangMenuHelper.ReplaceKVValue(lineValue, xmlValue);
        WriteToConfig();
    }
    public void OnBoolChange()
    {
        if (xmlValue.ToLower().Equals("true"))
        {
            xmlValue = "false";
            lineValue = YangMenuHelper.ReplaceKVValue(lineValue, xmlValue);
        }
        else
        {
            xmlValue = "true";
            lineValue = YangMenuHelper.ReplaceKVValue(lineValue, xmlValue);
        }
        WriteToConfig();
    }
    public void OnStringChange(string value)
    {
        if (value != null)
        {
            lineValue = YangMenuHelper.ReplaceKVValue(lineValue, value);
        }
        WriteToConfig();
    }

    private void WriteToConfig()
    {
        YangMenuHelper.helperIns.WriteLine(lineIndex, YangMenuHelper.helperIns.configPath + "client_config.xml", lineValue);
    }
}
public class YangWindowClientConfig
{
    public Dictionary<int, LineConfigInfo> configDic = new Dictionary<int, LineConfigInfo>();

    public string BasePath = "client_config_values/";

    public delegate void ReadConfigDelegate(string path, YangMenuHelper.OnReadLineHandler handler);
    public void Init(ReadConfigDelegate delegate_, ReadConfigDelegate editConfigReader, string clientConfigPath, string editorPath)
    {
        configDic.Clear();
        List<string> showConfigLine = new List<string>();
        editConfigReader(BasePath + "filter.txt", delegate(int currentLine, string line)
        {

            if (!YangMenuHelper.digOutAnnotation(line))
            {
                showConfigLine.Add(line);
            }
            return true;
        });

        List<LineConfigInfo> configLines = new List<LineConfigInfo>();
        bool startTag = false;
        delegate_(clientConfigPath, delegate(int currentLine, string line)
        {
            line = line.Trim();
            //if (!startTag)
            //{
            //    if (line.Contains("<ClientConfig>"))
            //    {
            //        startTag = true;
            //    }
            //}
            //else
            //{
            //    if (line.Contains("</ClientConfig>"))
            //    {
            //        return false;//末尾了
            //    }

            //    if (startTag)
            //    {
            if (!YangMenuHelper.digOutAnnotation(line))
            {

                for (int i = 0; i < showConfigLine.Count; i++)
                {
                    if (line.Contains(showConfigLine[i] + "="))
                    {
                        LineConfigInfo lci = new LineConfigInfo();
                        lci.myType = GetType(YangMenuHelper.GetKVValue(line));
                        if (lci.myType == CONFIG_TYPE.STRING)
                        {
                            ReadStringConfigValue(editConfigReader, BasePath + "_" + YangMenuHelper.GetKVKey(line) + ".txt", lci);
                        }
                        else if (lci.myType == CONFIG_TYPE.INPUT || lci.myType == CONFIG_TYPE.INT)
                        {

                        }

                        lci.lineValue = line;
                        lci.xmlValue = YangMenuHelper.GetKVValue(line);
                        lci.lineIndex = currentLine;
                        configDic.Add(currentLine, lci);
                        break;
                    }
                }
            }
            //    }
            //}
            return true;
        });
    }
    private void ReadStringConfigValue(ReadConfigDelegate editConfigReader, string path, LineConfigInfo lci)
    {
        List<string> values = new List<string>();
        editConfigReader(path, delegate(int currentLine, string line)
        {
            values.Add(line);
            return true;
        });
        lci.lineValues = values;
    }
    private CONFIG_TYPE GetType(string value)
    {
        value = value.ToLower();
        if (value.Equals("true") || value.Equals("false"))
        {
            return CONFIG_TYPE.BOOL;
        }
        else if (YangMenuHelper.IsInt(value))
        {
            return CONFIG_TYPE.INT;
        }
        else if (YangMenuHelper.IsInput(value))
        {
            return CONFIG_TYPE.INPUT;
        }
        else
        {
            return CONFIG_TYPE.STRING;
        }
    }
}
