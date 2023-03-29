using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingChangeBombBlock : ChangeBombBlock
{
    public static string spriteString = "HomingExplosionBlock_{0}";
    public override string SpriteString { get => spriteString; }

    public override void ExplosionBombBlock()
    {
        StartCoroutine(ExplosionBombBlockCoroutine());
    }

    private IEnumerator ExplosionBombBlockCoroutine()
    {
        TileMapManager.Instance.CreateTileListByHomingOrder(explosionTileAreaList, 1);

        int loopCount = explosionTileAreaList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            instEffect = GameObjectPool.Instantiate<BlockEffect>(explosionEffectPrefab.gameObject);
            instEffect.SetEffectDataByData(posTile.transform.position, explosionTileAreaList[index].transform.position, BlockSprite);
            instEffect.PlayEffect();
        }
        yield return instEffect.YieldEffectDuration;
        base.ExplosionBombBlock();

        for (int index = 0; index < loopCount; index++)
        {
            if (explosionTileAreaList[index].BlockContainerOrNull == null) { continue; }
            Block instBlock = BlockManager.Instance.GetCreateBlockByBlockDataInTile(explosionTileAreaList[index], ChangeType, -100, 1);
        }
        for (int index = 0; index < loopCount; index++)
        {
            // 한번에 치는 게 맞고
            // 결국 폭탄 큐에 들어가면 차례대로 실행...
            explosionTileAreaList[index].HitTile(true);
        }
    }
    protected override string GetSpriteNameByBlockNumber()
    {
        return string.Format(SpriteString, BlockNumber);
    }
}
