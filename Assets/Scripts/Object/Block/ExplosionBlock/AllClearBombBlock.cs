using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllClearBombBlock : BombBlock
{
    protected override IEnumerator ExplosionBombBlockCoroutine()
    {
        TileMapManager.Instance.CreateTileListByAllClearArea(explosionTileAreaList);

        //yield return new WaitForSeconds(5f);
        yield return null;

        BaseExplosionBombBlock();
        BombBlockBasicHit(false, false);
        yield return null;
    }
}
