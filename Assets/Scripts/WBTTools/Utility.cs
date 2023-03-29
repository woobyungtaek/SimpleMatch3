using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;

//2020-02-13
//2022-01-23 : CSV 파싱 시 Enum 자동 파싱
//2022-03-04 : Load 결과값을 받는 메서드 추가, 리소스 폴더에 Json저장 추가, 경로내 파일 모두 불러오기
//2022-11-03 : CSV 파싱 시 속성에도 파싱가능
//2022-11-04 : CSV 파싱 시 실제 타입과 반환 타입이 다른 리스트 구현 (RT 가 T를 상속받아야 사용 가능)

public static class Utility
{
    // Persistent Path
    public static void SaveJsonFilePersistent(string filename, object savedata)
    {
        string parseStr = JsonUtility.ToJson(savedata);

        string savePath = $"{Application.persistentDataPath}/{filename}.json";
        File.WriteAllText(savePath, parseStr);
    }
    public static T LoadJsonFilePersistent<T>(string filename)
    {
        string loadPath = $"{Application.persistentDataPath}/{filename}.json";
        //Debug.Log(loadPath);
        if (File.Exists(loadPath))
        {
            string parseStr = File.ReadAllText(loadPath);
            return JsonUtility.FromJson<T>(parseStr);
        }
        return default;
    }
    public static T LoadJsonFilePersistent<T>(string filename, out bool result)
    {
        string loadPath = string.Format($"{Application.persistentDataPath}/{filename}.json");
        //Debug.Log(loadPath);
        if (File.Exists(loadPath))
        {
            string parseStr = File.ReadAllText(loadPath);
            result = true;
            return JsonUtility.FromJson<T>(parseStr);
        }
        result = false;
        return default;
    }

    public static void SaveJsonFileExeFilePath(string filename, object savedata)
    {
#if UNITY_STANDALONE_OSX
        string savePath = $"{Application.dataPath}/../../{filename}";
#elif UNITY_STANDALONE_WIN
        string savePath = $"{Application.dataPath}/../../{filename}";
#else
        string savePath = $"{Application.dataPath}/../../{filename}";
#endif

        Debug.Log(savePath);

        string parseStr = JsonUtility.ToJson(savedata);
        File.WriteAllText(savePath, parseStr);
    }

    // Resource Path
    public static void SaveJsonFile(string filename, object savedata)
    {
        string savePath = $"{Application.dataPath}/Resources/JSON/{filename}.json";

        string parseStr = JsonUtility.ToJson(savedata);
        File.WriteAllText(savePath, parseStr);
    }

    public static T LoadJsonFile<T>(string filename)
    {
        string path = $"JSON/{filename}";
        TextAsset txtAsset = Resources.Load<TextAsset>(path);
        if (txtAsset != null)
        {
            return JsonUtility.FromJson<T>(txtAsset.text);
        }
        return default;
    }
    public static T LoadJsonFile<T>(string filename, out bool result)
    {
        string path = $"JSON/{filename}";
        TextAsset txtAsset = Resources.Load<TextAsset>(path);
        if (txtAsset != null)
        {
            result = true;
            return JsonUtility.FromJson<T>(txtAsset.text);
        }
        result = false;
        return default;
    }

    public static T LoadJsonFileExeFilePath<T>(string filename, out bool result)
    {

#if UNITY_STANDALONE_OSX
        string loadPath = $"{Application.dataPath}/../../{filename}";
#elif UNITY_STANDALONE_WIN
        string loadPath = $"{Application.dataPath}/../../{filename}";
#else
        string loadPath = $"{Application.dataPath}/../../{filename}";
#endif

        string data = File.ReadAllText(loadPath);
        if (data != null)
        {
            result = true;
            return JsonUtility.FromJson<T>(data);
        }
        result = false;
        return default;
    }

    // Resource Path AllLoad
    // 경로내 모든 Json파일을 로드한다. List나 Arr를 반환해준다.
    public static List<T> LoadJsonFileInPath<T>(string pathName)
    {
        string path = $"JSON/{pathName}";
        var txtAssetArr = Resources.LoadAll<TextAsset>(path);
        if (txtAssetArr != null && txtAssetArr.Length > 0)
        {
            List<T> loadDataList = new List<T>();

            foreach (TextAsset textAsset in txtAssetArr)
            {
                T inst = JsonUtility.FromJson<T>(textAsset.text);

                loadDataList.Add(inst);
            }
            return loadDataList;
        }
        return null;
    }

    public static List<T> LoadCSVFile<T>(string filename)
    {
        string path = string.Format("CSV/{0}", filename);
        TextAsset txtAsset = Resources.Load<TextAsset>(path);
        string txtFile = txtAsset.text;

        return ParseCSV<T>(txtFile);
    }
    public static List<T> LoadCSVFile<T, RT>(string filename) where RT : T
    {
        //// 반환 타입이 인터페이스여야 합니다.
        //if( !typeof(T).IsInterface)
        //{
        //    Debug.LogError("T가 인터페이스가 아니므로 해당 메서드를 사용할 수 없습니다.");
        //    return null;
        //}

        string path = string.Format("CSV/{0}", filename);
        TextAsset txtAsset = Resources.Load<TextAsset>(path);
        string txtFile = txtAsset.text;

        return ParseCSV<T, RT>(txtFile);
    }

    public static List<T> LoadCSVFileFullPath<T>(string fullPath)
    {
        string parseStr = File.ReadAllText(fullPath);
        return ParseCSV<T>(parseStr);
    }

    private static List<T> ParseCSV<T>(string parseStr)
    {
        List<T> create_list = new List<T>();
        DoParsing<T, T>(parseStr, ref create_list);
        return create_list;
    }
    private static List<T> ParseCSV<T, RT>(string parseStr)
    {
        List<T> create_list = new List<T>();
        DoParsing<T, RT>(parseStr, ref create_list);
        return create_list;
    }
    private static void DoParsing<T, RT>(string parseStr, ref List<T> list)
    {
        string[] loaddata = Regex.Split(parseStr, "\r\n|\n|\r");
        string[] header = loaddata[0].Split(',');

        Type tp = typeof(RT);
        for (int index = 1; index < loaddata.Length; index++)
        {
            object inst_obj = Activator.CreateInstance<RT>();
            string[] split_data = loaddata[index].Split(',');

            for (int cnt = 0; cnt < header.Length; cnt++)
            {
                if (string.IsNullOrEmpty(split_data[cnt])) { continue; }

                // 필드값일때
                FieldInfo fld = tp.GetField(header[cnt], BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (fld != null)
                {
                    var type = fld.FieldType;
                    if (type == typeof(bool))
                    {
                        if (bool.TryParse(split_data[cnt], out bool result))
                        {
                            fld.SetValue(inst_obj, result);
                        }
                    }
                    else if (type == typeof(byte))
                    {
                        if (byte.TryParse(split_data[cnt], out byte result))
                        {
                            fld.SetValue(inst_obj, result);
                        }
                    }
                    else if (type == typeof(sbyte))
                    {
                        if (sbyte.TryParse(split_data[cnt], out sbyte result))
                        {
                            fld.SetValue(inst_obj, result);
                        }
                    }
                    else if (type == typeof(char))
                    {
                        if (char.TryParse(split_data[cnt], out char result))
                        {
                            fld.SetValue(inst_obj, result);
                        }
                    }
                    else if (type == typeof(decimal))
                    {
                        if (decimal.TryParse(split_data[cnt], out decimal result))
                        {
                            fld.SetValue(inst_obj, result);
                        }
                    }
                    else if (type == typeof(double))
                    {
                        if (double.TryParse(split_data[cnt], out double result))
                        {
                            fld.SetValue(inst_obj, result);
                        }
                    }
                    else if (type == typeof(float))
                    {
                        if (float.TryParse(split_data[cnt], out float result))
                        {
                            fld.SetValue(inst_obj, result);
                        }
                    }
                    else if (type == typeof(int))
                    {
                        if (int.TryParse(split_data[cnt], out int result))
                        {
                            fld.SetValue(inst_obj, result);
                        }
                    }
                    else if (type == typeof(uint))
                    {
                        if (uint.TryParse(split_data[cnt], out uint result))
                        {
                            fld.SetValue(inst_obj, result);
                        }
                    }
                    else if (type == typeof(long))
                    {
                        if (long.TryParse(split_data[cnt], out long result))
                        {
                            fld.SetValue(inst_obj, result);
                        }
                    }
                    else if (type == typeof(ulong))
                    {
                        if (ulong.TryParse(split_data[cnt], out ulong result))
                        {
                            fld.SetValue(inst_obj, result);
                        }
                    }
                    else if (type == typeof(short))
                    {
                        if (short.TryParse(split_data[cnt], out short result))
                        {
                            fld.SetValue(inst_obj, result);
                        }
                    }
                    else if (type == typeof(ushort))
                    {
                        if (ushort.TryParse(split_data[cnt], out ushort result))
                        {
                            fld.SetValue(inst_obj, result);
                        }
                    }
                    else if (type == typeof(string))
                    {
                        fld.SetValue(inst_obj, split_data[cnt]);
                    }
                    else if (type.IsEnum)
                    {
                        fld.SetValue(inst_obj, Enum.Parse(type, split_data[cnt]));
                    }

                    continue;
                }

                // 속성값일때
                PropertyInfo property = tp.GetProperty(header[cnt]);
                if (property != null)
                {
                    var type = property.PropertyType;
                    if (type == typeof(bool))
                    {
                        if (bool.TryParse(split_data[cnt], out bool result))
                        {
                            property.SetValue(inst_obj, result);
                        }
                    }
                    else if (type == typeof(byte))
                    {
                        if (byte.TryParse(split_data[cnt], out byte result))
                        {
                            property.SetValue(inst_obj, result);
                        }
                    }
                    else if (type == typeof(sbyte))
                    {
                        if (sbyte.TryParse(split_data[cnt], out sbyte result))
                        {
                            property.SetValue(inst_obj, result);
                        }
                    }
                    else if (type == typeof(char))
                    {
                        if (char.TryParse(split_data[cnt], out char result))
                        {
                            property.SetValue(inst_obj, result);
                        }
                    }
                    else if (type == typeof(decimal))
                    {
                        if (decimal.TryParse(split_data[cnt], out decimal result))
                        {
                            property.SetValue(inst_obj, result);
                        }
                    }
                    else if (type == typeof(double))
                    {
                        if (double.TryParse(split_data[cnt], out double result))
                        {
                            property.SetValue(inst_obj, result);
                        }
                    }
                    else if (type == typeof(float))
                    {
                        if (float.TryParse(split_data[cnt], out float result))
                        {
                            property.SetValue(inst_obj, result);
                        }
                    }
                    else if (type == typeof(int))
                    {
                        if (int.TryParse(split_data[cnt], out int result))
                        {
                            property.SetValue(inst_obj, result);
                        }
                    }
                    else if (type == typeof(uint))
                    {
                        if (uint.TryParse(split_data[cnt], out uint result))
                        {
                            property.SetValue(inst_obj, result);
                        }
                    }
                    else if (type == typeof(long))
                    {
                        if (long.TryParse(split_data[cnt], out long result))
                        {
                            property.SetValue(inst_obj, result);
                        }
                    }
                    else if (type == typeof(ulong))
                    {
                        if (ulong.TryParse(split_data[cnt], out ulong result))
                        {
                            property.SetValue(inst_obj, result);
                        }
                    }
                    else if (type == typeof(short))
                    {
                        if (short.TryParse(split_data[cnt], out short result))
                        {
                            property.SetValue(inst_obj, result);
                        }
                    }
                    else if (type == typeof(ushort))
                    {
                        if (ushort.TryParse(split_data[cnt], out ushort result))
                        {
                            property.SetValue(inst_obj, result);
                        }
                    }
                    else if (type == typeof(string))
                    {
                        property.SetValue(inst_obj, split_data[cnt]);
                    }
                    else if (type.IsEnum)
                    {
                        property.SetValue(inst_obj, Enum.Parse(type, split_data[cnt]));
                    }

                    continue;
                }
            }
            list.Add((T)inst_obj);
        }
    }
}