using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyTile : Tile
{
    public override bool IsCanFlow_Up { get => true; }
    public override bool IsCanFlow_UpAtEmpty { get => true; }


    public void SetCreateTileBySendTileList()
    {
        IsCreateTile = false;
        if (SendTileList[0] == null) { return; }
        if (!(SendTileList[0] is NormalTile)) { return; }
        IsCreateTile = true;
        BlockMakerOrNull = null;
    }

    public override void RequestReserveData(Tile requestTile)
    {
        if (IsCreateTile == false) { return; }

        IsNotReady = true;

        var data = ObjectPool.GetInst<ReserveData>();
        data.ClearQueue();
        data.Enqueue(this);
        requestTile.ReserveData = data;
        mCreateReserveDataQueue.Enqueue(data);
    }

    public override void StartDrop()
    {
        if (IsCreateTile == false) { return; }
        mCreateCoroutine = StartCoroutine(CreateBlockCoroutine());
    }
}
