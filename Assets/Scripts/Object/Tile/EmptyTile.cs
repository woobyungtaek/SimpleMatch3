using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyTile : Tile
{
    public override void CheckAvailableSendTile()
    {
        if (IsCreateTile == false) { return; }
        if (SendTileList[0].BlockContainerOrNull != null) { return; }
        SendTileOrNull = SendTileList[0];
    }
}
