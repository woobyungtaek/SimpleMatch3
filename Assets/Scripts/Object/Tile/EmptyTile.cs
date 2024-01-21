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

    public override bool CheckDropableState()
    {
        if (IsCreateTile) { return true; }
        return false;
    }

    public override void StartDrop()
    {
        if (IsCreateTile == false) { return; }
        mCreateCoroutine = StartCoroutine(CreateBlockCoroutine());
    }
}
