using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReuseHomingOrderTileList : Pool<ReuseHomingOrderTileList>
{
    private int mOrder = 0;
    public int HomingOrder { get => mOrder; set => mOrder = value; }
    public List<Tile> tileList = new List<Tile>();
    public override void Dispose()
    {
        HomingOrder = 0;
        tileList.Clear();
    }
}
public class ReuseTileList : Pool<ReuseTileList>
{
    public List<Tile> tileList = new List<Tile>();
    public override void Dispose()
    {
        tileList.Clear();
    }
}