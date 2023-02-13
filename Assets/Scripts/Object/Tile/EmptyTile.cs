using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyTile : Tile
{
    public override bool IsCanFlow_Up { get => true; }
    public override bool IsCanFlow_UpAtEmpty { get => true; }
}
