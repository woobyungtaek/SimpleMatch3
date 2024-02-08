using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AroundBombBlock : BombBlock
{
    public static string spriteString = "AroundBombBlock_{0}";
    public override string SpriteString { get => string.Format(spriteString, BlockNumber); }

    protected override IEnumerator ExplosionBombBlockCoroutine()
    {
        TileMapManager.Instance.CreateTileListByAreaList(explosionTileAreaList, posTile.Coordi, TileArea.around);

        instEffect = GameObjectPool.Instantiate<BlockEffect>(explosionEffectPrefab.gameObject);
        instEffect.SetEffectDataByData(posTile.transform.position, Vector3.one * 3);
        instEffect.PlayEffect();
        yield return instEffect.YieldEffectDuration;
        BaseExplosionBombBlock();
        BombBlockBasicHit(true, false, 3);
    }
}
