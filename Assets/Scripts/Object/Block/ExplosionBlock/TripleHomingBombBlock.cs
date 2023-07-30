using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleHomingBombBlock : BombBlock
{
    public static string spriteString = "HomingExplosionBlock_{0}";
    public override string SpriteString { get => spriteString; }

    public override void ExplosionBombBlock()
    {
        StartCoroutine(ExplosionBombBlockCoroutine());
    }

    private IEnumerator ExplosionBombBlockCoroutine()
    {
        TileMapManager.Instance.CreateTileListByHomingOrder(explosionTileAreaList, 3);
        int loopCount = explosionTileAreaList.Count;

        for (int index = 0; index < loopCount; index++)
        {
            instEffect = GameObjectPool.Instantiate<BlockEffect>(explosionEffectPrefab.gameObject);
            instEffect.SetEffectDataByData(posTile.transform.position, explosionTileAreaList[index].transform.position, BlockSprite);
            instEffect.PlayEffect();
        }
        yield return instEffect.YieldEffectDuration;
        base.ExplosionBombBlock();

        BombBlockBasicHit(true);
    }

    protected override string GetSpriteNameByBlockNumber()
    {
        return string.Format(SpriteString, BlockNumber);
    }
}
