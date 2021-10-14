using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BlockData : System.IDisposable
{
    public string   BlockName;
    public int      BlockColor;
    public int      BlockHP;
    public int      BlockCount;

    public System.Type BlockType
    {
        get
        {
            if(mBlockType == null) { mBlockType = System.Type.GetType(BlockName); }
            return mBlockType;
        }
    }
    private System.Type mBlockType;

    public void Dispose() { }
}
