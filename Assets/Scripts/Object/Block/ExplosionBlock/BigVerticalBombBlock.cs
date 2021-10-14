using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigVerticalBombBlock : BombBlock
{
    public override void ExplosionBombBlock()
    {
        StartCoroutine(ExplosionBombBlockCoroutine());
    }

    private IEnumerator ExplosionBombBlockCoroutine()
    {
        TileMapManager.Instance.CreateTileListByAreaList(explosionTileAreaList, posTile.Coordi, TileArea.bigvertical);

        instEffect = GameObjectPool.Instantiate<BlockEffect>(explosionEffectPrefab.gameObject);
        instEffect.SetEffectDataByData(posTile.transform.position, Vector3.zero);
        instEffect.PlayEffect();

        yield return explosionEffectPrefab.YieldEffectDuration;
        base.ExplosionBombBlock();

        int loopCount = explosionTileAreaList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            explosionTileAreaList[index].HitTile(true);
        }
    }
}
