using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    private static object lockobj = new object();

    public static bool IsExist
    {
        get
        {
            return (_instance != null);
        }
    }
    public static T Instance
    {
        get
        {
            lock (lockobj)
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(T)) as T;

                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = typeof(T).Name;
                    }
                }
                DontDestroyOnLoad(_instance);
                return _instance;
            }
        }
    }
}
