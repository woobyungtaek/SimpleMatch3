using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingChangeBombBlock : ChangeBombBlock
{
    public static string spriteString = "HomingExplosionBlock_{0}";
    public override string SpriteString { get => string.Format(spriteString, BlockNumber); }

    public override void ExplosionBombBlock()
    {
        StartCoroutine(ExplosionBombBlockCoroutine());
    }

    private new IEnumerator ExplosionBombBlockCoroutine()
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

            explosionTileAreaList[index].BlockContainerOrNull.MainBlock.CheckMissionBlock();
            Block instBlock = BlockManager.Instance.GetCreateBlockByBlockDataInTile(explosionTileAreaList[index], ChangeType, -100, 1);
        }

        BombBlockBasicHit(false, true);
    }
}
