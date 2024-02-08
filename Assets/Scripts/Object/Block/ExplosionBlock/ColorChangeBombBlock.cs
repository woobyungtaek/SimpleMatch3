using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChangeBombBlock : ChangeBombBlock
{
    public override void ExplosionBombBlock()
    {
        StartCoroutine(ExplosionBombBlockCoroutine());
    }

    private new IEnumerator ExplosionBombBlockCoroutine()
    {
        AudioManager.Instance.PlayByType(EAudioPlayType.EColorBombLine);
        TileMapManager.Instance.CreateTileListBySameNumber(explosionTileAreaList, BlockNumber);
        int loopCount = explosionTileAreaList.Count;

        for (int index = 0; index < loopCount; index++)
        {
            instEffect = GameObjectPool.Instantiate<BlockEffect>(explosionEffectPrefab.gameObject);
            instEffect.SetEffectDataByData(posTile.transform.position, explosionTileAreaList[index].transform.position, BlockSprite);
            instEffect.PlayEffect();
        }
        yield return instEffect.YieldEffectDuration;

        for (int index = 0; index < loopCount; index++)
        {
            if (explosionTileAreaList[index].BlockContainerOrNull == null) { continue; }
            if (!(explosionTileAreaList[index].BlockContainerOrNull.MainBlock is NormalBlock)) { continue; }

            explosionTileAreaList[index].BlockContainerOrNull.MainBlock.CheckMissionBlock();
            BlockManager.Instance.CreateBlockByBlockDataInTile(explosionTileAreaList[index], ChangeType, BlockNumber, 1);
        }

        base.ExplosionBombBlock();
        BombBlockBasicHit(false, true);
    }
}
