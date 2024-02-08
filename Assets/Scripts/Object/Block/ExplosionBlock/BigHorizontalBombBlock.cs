using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigHorizontalBombBlock : BombBlock
{
    protected override IEnumerator ExplosionBombBlockCoroutine()
    {
        TileMapManager.Instance.CreateTileListByAreaList(explosionTileAreaList, posTile.Coordi, TileArea.bighorizontal);

        instEffect = GameObjectPool.Instantiate<BlockEffect>(explosionEffectPrefab.gameObject);
        instEffect.SetEffectDataByData(posTile.transform.position, Vector3.zero);
        instEffect.PlayEffect();
        yield return instEffect.YieldEffectDuration;
        BaseExplosionBombBlock();

        BombBlockBasicHit(true, false, 3);
    }
}
