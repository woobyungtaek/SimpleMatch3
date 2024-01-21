using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlankTile : Tile
{

    public override bool CheckDropableState()
    {
        if (IsCreateTile) { return true; }
        return false;
    }
}
