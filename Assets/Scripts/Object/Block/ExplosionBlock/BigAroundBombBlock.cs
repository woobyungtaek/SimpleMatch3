using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigAroundBombBlock : BombBlock
{
    protected override IEnumerator ExplosionBombBlockCoroutine()
    {
        TileMapManager.Instance.CreateTileListByAreaList(explosionTileAreaList, posTile.Coordi, TileArea.bigaround);

        instEffect = GameObjectPool.Instantiate<BlockEffect>(explosionEffectPrefab.gameObject);
        instEffect.SetEffectDataByData(posTile.transform.position, Vector3.one * 5);
        instEffect.PlayEffect();
        yield return instEffect.YieldEffectDuration;
        BaseExplosionBomobBlock();

        BombBlockBasicHit(true, 3);
    }
}
