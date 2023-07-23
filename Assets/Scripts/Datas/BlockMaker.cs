using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockMaker : System.IDisposable
{
    public static int MaxColor = 5;
    private static int mRandomDelayCreateCount;
    private static Queue<BlockData> ReserveBlockDataQueue = new Queue<BlockData>();

    public static void AddReserveBlockData(BlockData data)
    {
        ReserveBlockDataQueue.Enqueue(data);
    }
    public static bool GetReserveBlockData(ref BlockData data)
    {
        if(mRandomDelayCreateCount > 0) { mRandomDelayCreateCount--; return false; }
        if(ReserveBlockDataQueue.Count == 0) { return false; }
        data = ReserveBlockDataQueue.Dequeue();
        SetRandomDelayCreateCount();
        return true;
    }
    public static void SetRandomDelayCreateCount()
    {
        mRandomDelayCreateCount = Random.Range(0, 8);
    }

    public Vector2 Coordi;
    public bool IsLoop;
    public List<BlockData> CreateBlockList;

    public bool IsEnd { get => mbEnd; }

    public int CurrentIndex
    {
        get => mCurrentIndex;
        set
        {
            mCurrentIndex = value;
            if(mCurrentIndex >= CreateBlockList.Count)
            {
                if (IsLoop) { mCurrentIndex = 0; return; }
                mbEnd = true;
            }
        }
    }

    private bool mbEnd;
    private int mCurrentIndex;
    
    public void Dispose()
    {
        mCurrentIndex = 0;
        Coordi = Vector2.one * -1;
        IsLoop = false;
        mbEnd = false;
        CreateBlockList.Clear();
    }
}
