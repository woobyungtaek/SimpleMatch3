using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockMaker : System.IDisposable
{
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
