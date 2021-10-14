using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class Localization
{
    private static string Korean = "Korean";

    private static string LocalizationFileName = "Localization";
    private static Dictionary<string, Dictionary<string, string>> mLocalizationDict = new Dictionary<string, Dictionary<string, string>>();

    static Localization()
    {
        LoadLocalizationFile();
    }

    private static void LoadLocalizationFile()
    {
        string path = string.Format("CSV/{0}", LocalizationFileName);
        TextAsset txtAsset = Resources.Load<TextAsset>(path);
        string txtFile = txtAsset.text;

        string[] loaddata = Regex.Split(txtFile, "\r\n|\n|\r");
        string[] header = loaddata[0].Split(',');

        int loopCount = loaddata.Length;
        int contentCount = 0;
        for(int index = 1; index < loopCount; index++)
        {
            string[] instContentData = loaddata[index].Split(',');

            if(mLocalizationDict.ContainsKey(instContentData[0]) == false)
            {
                mLocalizationDict.Add(instContentData[0], new Dictionary<string, string>());
            }

            contentCount = header.Length;
            for(int c_index =1; c_index < contentCount; c_index++)
            {
                if(mLocalizationDict[instContentData[0]].ContainsKey(header[c_index]) == false)
                {
                    mLocalizationDict[instContentData[0]].Add(header[c_index], null);
                }
                mLocalizationDict[instContentData[0]][header[c_index]] = instContentData[c_index];
            }
        }
    }

    public static string GetString(string key)
    {
        if(mLocalizationDict.ContainsKey(key) == false) { return null; }
        if(mLocalizationDict[key].ContainsKey(Korean) == false){ return null; }
        return mLocalizationDict[key][Korean];
    }
}
