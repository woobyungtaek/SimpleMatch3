using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeTile : Tile
{
    public void SetCreateTileBySendTileList()
    {
        IsCreateTile = false;
        if (SendTileList[0] == null) { return; }
        if ( !(SendTileList[0] is NormalTile) ) { return; }
        IsCreateTile = true;
    }

    public override void CheckAvailableSendTile()
    {
        if (IsCreateTile == false) { return; }
        if (SendTileList[0].BlockContainerOrNull != null) { return; }
        SendTileOrNull = SendTileList[0];
    }
}
