using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigAroundBombBlock : BombBlock
{
    public override void ExplosionBombBlock()
    {
        StartCoroutine(ExplosionBombBlockCoroutine());
    }

    private IEnumerator ExplosionBombBlockCoroutine()
    {
        TileMapManager.Instance.CreateTileListByAreaList(explosionTileAreaList, posTile.Coordi, TileArea.bigaround);

        instEffect = GameObjectPool.Instantiate<BlockEffect>(explosionEffectPrefab.gameObject);
        instEffect.SetEffectDataByData(posTile.transform.position, Vector3.one * 5);
        instEffect.PlayEffect();
        yield return instEffect.YieldEffectDuration;
        base.ExplosionBombBlock();

        BombBlockBasicHit(true, 3);
    }
}
