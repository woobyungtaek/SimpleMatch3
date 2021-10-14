using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AroundBombBlock : BombBlock
{
    public static string spriteString = "AroundBombBlock_{0}";
    public override string SpriteString { get => spriteString; }

    public override void ExplosionBombBlock()
    {
        StartCoroutine(ExplosionBombBlockCoroutine());
    }

    private IEnumerator ExplosionBombBlockCoroutine()
    {
        TileMapManager.Instance.CreateTileListByAreaList(explosionTileAreaList, posTile.Coordi, TileArea.around);

        instEffect = GameObjectPool.Instantiate<BlockEffect>(explosionEffectPrefab.gameObject);
        instEffect.SetEffectDataByData(posTile.transform.position, Vector3.one * 3);
        instEffect.PlayEffect();
        yield return instEffect.YieldEffectDuration;
        base.ExplosionBombBlock();
        int loopCount = explosionTileAreaList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            explosionTileAreaList[index].HitTile(true);
        }
    }
    protected override string GetSpriteNameByBlockNumber()
    {
        return string.Format(SpriteString, BlockNumber);
    }
}
