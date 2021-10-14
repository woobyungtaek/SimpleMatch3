using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllClearBombBlock : BombBlock
{
    public override void ExplosionBombBlock()
    {
        StartCoroutine(ExplosionBombBlockCoroutine());
    }

    private IEnumerator ExplosionBombBlockCoroutine()
    {
        TileMapManager.Instance.CreateTileListByAllClearArea(explosionTileAreaList);

        //yield return new WaitForSeconds(5f);
        yield return null;

        base.ExplosionBombBlock();
        int loopCount = explosionTileAreaList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            explosionTileAreaList[index].HitTile(true);
        }
        yield return null;
    }
}
