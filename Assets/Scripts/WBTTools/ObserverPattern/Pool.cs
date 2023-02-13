using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// GameObject가 아닌 일반 객체를 Pooling 합니다.
/// </summary>
/// <typeparam name="T">Disposable, new() 가능한 객체만 Pooling됩니다.</typeparam>
public class Pool<T> : IDisposable where T : IDisposable, new()
{
    static Queue<T> tPool;
    static Queue<T> TPool
    {
        get
        {
            if (tPool == null)
            {
                tPool = new Queue<T>();
            }
            return tPool;
        }
    }

    /// <summary>
    /// Pool에 있는 객체를 가져옵니다.
    /// </summary>
    /// <returns></returns>
    public static T Instantiate()
    {
        T inst;
        if (TPool.Count <= 0)
        {
            inst = new T();
        }
        else
        {
            inst = TPool.Dequeue();
        }
        return inst;
    }

    /// <summary>
    /// Pool에 객체를 반환합니다.
    /// </summary>
    /// <param name="target"></param>
    public static void Destroy(T target)
    {
        if (TPool.Contains(target)) { return; }
        target.Dispose();
        TPool.Enqueue(target);
    }

    public virtual void Dispose() { }
}
