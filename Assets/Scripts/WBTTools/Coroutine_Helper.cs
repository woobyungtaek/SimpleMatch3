using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coroutine_Helper : MonoBehaviour
{
    private static MonoBehaviour Instance;

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        if(Instance == null)
        {
            Instance = new GameObject($"CoroutineHelper").AddComponent<Coroutine_Helper>();
            DontDestroyOnLoad(Instance);
        }
    }

    public new static Coroutine StartCoroutine(IEnumerator coroutine)
    {
        return Instance.StartCoroutine(coroutine);
    }

    public new static void StopCoroutine(Coroutine coroutine)
    {
        StopCoroutine(coroutine);
    }
}