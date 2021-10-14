using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;

//2020-02-13
public static class Utility
{
    public static void SaveJsonFilePersistent(string filename,object savedata)
    {
        string parseStr = JsonUtility.ToJson(savedata);

        string savePath = string.Format("{0}/{1}.json",Application.persistentDataPath,filename);
        File.WriteAllText(savePath, parseStr);
    }
    public static T LoadJsonFilePersistent<T>(string filename)
    {
        string loadPath = string.Format("{0}/{1}.json", Application.persistentDataPath, filename);
        if (File.Exists(loadPath))
        { 
            string parseStr = File.ReadAllText(loadPath);
            return JsonUtility.FromJson<T>(parseStr);
        }
        return default;
    }
        
    public static List<T> LoadCSVFile<T>(string filename)
    {
        List<T> create_list = new List<T>();

        string path = string.Format("CSV/{0}", filename);
        TextAsset txtAsset = Resources.Load<TextAsset>(path);
        string txtFile = txtAsset.text;

        string[] loaddata = Regex.Split(txtFile, "\r\n|\n|\r");
        string[] header = loaddata[0].Split(',');

        Type tp = typeof(T);
        for (int index = 1; index < loaddata.Length; index++)
        {
            object inst_obj = Activator.CreateInstance<T>();

            string[] split_data = loaddata[index].Split(',');

            for (int cnt = 0; cnt < header.Length; cnt++)
            {
                FieldInfo fld = tp.GetField(header[cnt], BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (fld != null)
                {
                    if(fld.FieldType == typeof(bool))
                    {
                        if (bool.TryParse(split_data[cnt], out bool result))
                        {
                            fld.SetValue(inst_obj, result);
                        }
                    }
                    else if (fld.FieldType == typeof(byte))
                    {
                        if (byte.TryParse(split_data[cnt], out byte result))
                        {
                            fld.SetValue(inst_obj, result);
                        }
                    }
                    else if (fld.FieldType == typeof(sbyte))
                    {
                        if (sbyte.TryParse(split_data[cnt], out sbyte result))
                        {
                            fld.SetValue(inst_obj, result);
                        }
                    }
                    else if (fld.FieldType == typeof(char))
                    {
                        if (char.TryParse(split_data[cnt], out char result))
                        {
                            fld.SetValue(inst_obj, result);
                        }
                    }
                    else if (fld.FieldType == typeof(decimal))
                    {
                        if (decimal.TryParse(split_data[cnt], out decimal result))
                        {
                            fld.SetValue(inst_obj, result);
                        }
                    }
                    else if (fld.FieldType == typeof(double))
                    {
                        if (double.TryParse(split_data[cnt], out double result))
                        {
                            fld.SetValue(inst_obj, result);
                        }
                    }
                    else if (fld.FieldType == typeof(float))
                    {
                        if (float.TryParse(split_data[cnt], out float result))
                        {
                            fld.SetValue(inst_obj, result);
                        }
                    }
                    else if (fld.FieldType == typeof(int))
                    {
                        if (int.TryParse(split_data[cnt], out int result))
                        {
                            fld.SetValue(inst_obj, result);
                        }
                    }
                    else if (fld.FieldType == typeof(uint))
                    {
                        if (uint.TryParse(split_data[cnt], out uint result))
                        {
                            fld.SetValue(inst_obj, result);
                        }
                    }
                    else if (fld.FieldType == typeof(long))
                    {
                        if (long.TryParse(split_data[cnt], out long result))
                        {
                            fld.SetValue(inst_obj, result);
                        }
                    }
                    else if (fld.FieldType == typeof(ulong))
                    {
                        if (ulong.TryParse(split_data[cnt], out ulong result))
                        {
                            fld.SetValue(inst_obj, result);
                        }
                    }
                    else if (fld.FieldType == typeof(short))
                    {
                        if (short.TryParse(split_data[cnt], out short result))
                        {
                            fld.SetValue(inst_obj, result);
                        }
                    }
                    else if (fld.FieldType == typeof(ushort))
                    {
                        if (ushort.TryParse(split_data[cnt], out ushort result))
                        {
                            fld.SetValue(inst_obj, result);
                        }
                    }
                    else if (fld.FieldType == typeof(string))
                    {
                        fld.SetValue(inst_obj, split_data[cnt]);
                    }
                }
            }
            create_list.Add((T)inst_obj);
        }

        return create_list;
    }

    public static T LoadJsonFile<T>(string filename)
    {
        string path = string.Format("JSON/{0}", filename);
        TextAsset txtAsset = Resources.Load<TextAsset>(path);
        if (txtAsset != null)
        {
            return JsonUtility.FromJson<T>(txtAsset.text);
        }
        return default;
    }
}
