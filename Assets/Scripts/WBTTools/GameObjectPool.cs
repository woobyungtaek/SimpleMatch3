using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//2020-02-23
//2022-05-04 : AllClear 추가 / Destroy명칭 변경
//2022-11-25 : Object.name 반복 호출 문제 있음 우선 메서드 시작시 캐싱으로 호출수는 줄였는데 근본적인건 아직 남아있는 상태

public class GameObjectPool : MonoBehaviour
{
    private static Dictionary<string, Queue<GameObject>> gameObjectPoolQueueDict = new Dictionary<string, Queue<GameObject>>();
    private static Dictionary<GameObject, Dictionary<System.Type, Component>> typeObjectPoolDict
        = new Dictionary<GameObject, Dictionary<System.Type, Component>>();

    private static void ResetTransform(GameObject _obj)
    {
        _obj.transform.localPosition = Vector3.zero;
        _obj.transform.rotation = Quaternion.identity;
    }

    public static void ClearPool()
    {
        typeObjectPoolDict.Clear();

        foreach (var queue in gameObjectPoolQueueDict.Values)
        {
            while (queue.Count > 0)
            {
                GameObject obj = queue.Dequeue();
                if (obj == null) { continue; }
                Destroy(obj);
            }
        }
        gameObjectPoolQueueDict.Clear();
    }

    public static GameObject Instantiate(GameObject prefab, Transform parentOrNull = null)
    {
        string name = prefab.name;

        if (!gameObjectPoolQueueDict.ContainsKey(name))
        {
            Queue<GameObject> inst_pool_list = new Queue<GameObject>();
            gameObjectPoolQueueDict.Add(name, inst_pool_list);
        }

        GameObject inst_obj = null;
        int loopCount = gameObjectPoolQueueDict[name].Count;
        while (inst_obj == null && loopCount > 0)
        {
            inst_obj = gameObjectPoolQueueDict[name].Dequeue();
            loopCount -= 1;
        }
        if (inst_obj == null)
        {
            inst_obj = Instantiate(prefab, parentOrNull, false);
        }

        inst_obj.name = name;

        if (parentOrNull == null)
        {
            inst_obj.transform.SetParent(null);
        }
        else
        {
            if (inst_obj.transform.parent != parentOrNull)
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
        if (!instDict.ContainsKey(typeof(T)))
        {
            instDict.Add(typeof(T), instObject.GetComponent<T>());
        }

        T result = instDict[typeof(T)] as T;

        if (result == null) { Debug.AssertFormat(false, "해당 컴포넌트가 없습니다."); return null; }
        return result;
    }

    public static void ReturnObject(GameObject _obj)
    {
        if (gameObjectPoolQueueDict.ContainsKey(_obj.name))
        {
            if (gameObjectPoolQueueDict[_obj.name].Contains(_obj))
            {
                return;
            }
            _obj.SetActive(false);
            gameObjectPoolQueueDict[_obj.name].Enqueue(_obj);
        }
        else
        {
            Destroy(_obj);
        }
    }
}