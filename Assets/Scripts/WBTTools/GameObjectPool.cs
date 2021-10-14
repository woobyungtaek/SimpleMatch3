using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//2020-02-23
public class GameObjectPool : MonoBehaviour
{
    static Dictionary<string, Queue<GameObject>> gameObjectPoolQueueDict = new Dictionary<string, Queue<GameObject>>();
    static Dictionary<GameObject, Dictionary<System.Type, Component>> typeObjectPoolDict = new Dictionary<GameObject, Dictionary<System.Type, Component>>();

    static void ResetTransform(GameObject _obj)
    {
        _obj.transform.localPosition = Vector3.zero;
        _obj.transform.rotation = Quaternion.identity;
    }

    public static GameObject Instantiate(GameObject prefab, Transform parentOrNull = null)
    {
        if ( !(gameObjectPoolQueueDict.ContainsKey(prefab.name)) )
        {
            Queue<GameObject> inst_pool_list = new Queue<GameObject>();
            gameObjectPoolQueueDict.Add(prefab.name, inst_pool_list);
        }

        GameObject inst_obj = null;
        int loopCount = gameObjectPoolQueueDict[prefab.name].Count;
        while (inst_obj == null && loopCount > 0)
        {
            inst_obj = gameObjectPoolQueueDict[prefab.name].Dequeue();
            loopCount -= 1;
        }
        if (inst_obj == null)
        {
            inst_obj = Object.Instantiate(prefab, parentOrNull,false) as GameObject;
        }

        inst_obj.name = prefab.name;

        if (parentOrNull == null)
        {
            inst_obj.transform.SetParent(null);
        }
        else
        {
            if(inst_obj.transform.parent != parentOrNull)
            {
                inst_obj.transform.SetParent(parentOrNull);
            }
        }

        ResetTransform(inst_obj);
        inst_obj.SetActive(true);
        return inst_obj;
    }
    public static T Instantiate<T>(GameObject prefab, Transform parentOrNull = null) where T : Component
    {
        GameObject instObject = Instantiate(prefab, parentOrNull);

        Dictionary<System.Type, Component> instDict;
        if (!typeObjectPoolDict.ContainsKey(instObject))
        {
            instDict = new Dictionary<System.Type, Component>();
            typeObjectPoolDict.Add(instObject, instDict);
        }

        instDict = typeObjectPoolDict[instObject];
        if(!instDict.ContainsKey(typeof(T)))
        {
            instDict.Add(typeof(T), instObject.GetComponent<T>());
        }

        T result = instDict[typeof(T)] as T;
        
        if(result == null) { Debug.AssertFormat(false, "해당 컴포넌트가 없습니다."); return null; }
        return result;
    }
    
    public static void Destroy(GameObject _obj)
    {
        if (gameObjectPoolQueueDict.ContainsKey(_obj.name) )
        {
            if (gameObjectPoolQueueDict[_obj.name].Contains(_obj) )
            {
                return;
            }
            _obj.SetActive(false);
            gameObjectPoolQueueDict[_obj.name].Enqueue(_obj);
        }
        else
        {
            Object.Destroy(_obj);
        }
    }
}