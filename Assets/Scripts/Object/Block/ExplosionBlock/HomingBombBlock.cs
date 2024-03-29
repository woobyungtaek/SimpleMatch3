﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingBombBlock : BombBlock
{
    public static string spriteString = "HomingExplosionBlock_{0}";
    public override string SpriteString { get => string.Format(spriteString, BlockNumber); }

    protected override IEnumerator ExplosionBombBlockCoroutine()
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
        BaseExplosionBombBlock();

        BombBlockBasicHit(true, false);
        AudioManager.Instance.PlayByType(EAudioPlayType.EMatchEffect);
    }
}
