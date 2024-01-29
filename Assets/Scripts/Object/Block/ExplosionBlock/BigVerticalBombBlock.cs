using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigVerticalBombBlock : BombBlock
{
    protected override IEnumerator ExplosionBombBlockCoroutine()
    {
        TileMapManager.Instance.CreateTileListByAreaList(explosionTileAreaList, posTile.Coordi, TileArea.bigvertical);

        instEffect = GameObjectPool.Instantiate<BlockEffect>(explosionEffectPrefab.gameObject);
        instEffect.SetEffectDataByData(posTile.transform.position, Vector3.zero);
        instEffect.PlayEffect();

        yield return explosionEffectPrefab.YieldEffectDuration;
        BaseExplosionBomobBlock();

        BombBlockBasicHit(true, 3);
    }
}
