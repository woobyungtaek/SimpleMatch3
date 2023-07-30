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
        BombBlockBasicHit(false);
        yield return null;
    }
}
