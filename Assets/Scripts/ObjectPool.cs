using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 22-01-29 : string, object 형태의 풀로 변경, 문자열로 반환 기능 추가
public static class ObjectPool
{
    static Dictionary<string, Queue<object>> tQueueDict = new Dictionary<string, Queue<object>>();

    public static T GetInst<T>() where T : class, IReUseObject, new()
    {
        T inst = null;

        string key = typeof(T).Name;
       if ( tQueueDict.ContainsKey(key) == false)
        {
            tQueueDict.Add(key, new Queue<object>());
        }

        while (tQueueDict[key].Count > 0)
        {
            inst = tQueueDict[key].Dequeue() as T;
            if (inst != null)
            {
                break;
            }
        }
        if(inst == null)
        {
            inst = new T();
        }
        inst.ResetObject();


#if UNITY_EDITOR
        //Debug.Log($"{key} pool : {tQueueDict[key].Count}");
#endif

        return inst;
    }

    public static object GetInstByStr(string str)
    {
        object inst = null;
        if(tQueueDict.ContainsKey(str) == false)
        {
            tQueueDict.Add(str, new Queue<object>());
        }

        while(tQueueDict[str].Count > 0)
        {
            inst = tQueueDict[str].Dequeue();
            if (inst != null)
            {
                break;
            }
        }

        if (inst == null)
        {
            Type type = Type.GetType(str);
            inst = Activator.CreateInstance(type);
        }

        return inst;
    }

    public static void ReturnInst<T>(T inst) where T : class, IReUseObject, new()
    {
        string key = typeof(T).Name;
        if (tQueueDict.ContainsKey(key) == false)
        {
            //Disposble 필요?
            return;
        }
        tQueueDict[key].Enqueue(inst);
#if UNITY_EDITOR
        //Debug.Log($"{key} pool : {tQueueDict[key].Count}");
#endif
    }
    public static void ReturnInstByStr(string key, object inst)
    {
        if (tQueueDict.ContainsKey(key) == false)
        {
            //Disposble 필요?
            return;
        }
        tQueueDict[key].Enqueue(inst);
#if UNITY_EDITOR
       // Debug.Log($"{key} pool : {tQueueDict[key].Count}");
#endif
    }
    public static void ClearPool<T>() where T : class, IReUseObject, new()
    {
        string key = typeof(T).Name;
        if (tQueueDict.ContainsKey(key) == false)
        {
            return;
        }
        //이전에 Disposble필요?
        tQueueDict[key].Clear();
    }
}

public interface IReUseObject
{
    void ResetObject();
}