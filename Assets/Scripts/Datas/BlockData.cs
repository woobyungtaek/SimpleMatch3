using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BlockData : System.IDisposable
{
    public string   BlockName;
    public int      BlockColor;
    public int      BlockHP;

    public System.Type BlockType
    {
        get
        {
            if(mBlockType == null) { mBlockType = System.Type.GetType(BlockName); }
            return mBlockType;
        }
        set
        {
            mBlockType = value;
        }
    }
    private System.Type mBlockType;

    public void Dispose() { }
}
