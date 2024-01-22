using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeTile : Tile
{

    public void SetCreateTileBySendTileList()
    {
        IsCreateTile = false;
        if (SendTileList[0] == null) { return; }
        if (!(SendTileList[0] is NormalTile)) { return; }
        IsCreateTile = true;
        BlockMakerOrNull = null;
    }

    public override bool IsCanSend
    {
        get
        {
            if (!IsCreateTile) { return false; }

            Tile underTile = mSendTileList[0];
            if (underTile == null) { return false; }
            if (underTile.ReserveData == null) { return true; }
            return false;
        }
    }

    public override bool StraightMove()
    {
        if (!IsCanSend) { return false; }

        // [생성]
        var data = ObjectPool.GetInst<ReserveData>();
        data.ClearQueue();
        data.DestTile = null;
        ReserveData = data;
        mCreateReserveDataQueue.Enqueue(data);

        // [보내기]
        mSendTileList[0].SendReserveData(this);
        return true;
    }
    public override bool FlowingMove()
    {
        return false;
    }
    public override bool CheckDropableState()
    {
        if (IsCreateTile) { return true; }
        return false;
    }
    public override bool CheckDropReadyState()
    {
        if (IsCreateTile) { return false; }
        return true;
    }

    public override void StartDrop()
    {
        if (IsCreateTile == false) { return; }
        mCreateCoroutine = StartCoroutine(CreateBlockCoroutine());
    }

}
