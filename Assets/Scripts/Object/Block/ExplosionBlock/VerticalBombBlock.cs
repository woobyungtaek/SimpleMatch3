﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalBombBlock : BombBlock
{
    public static string spriteString = "VerticalBombBlock_{0}";
    public override string SpriteString { get => string.Format(spriteString, BlockNumber); }


    protected override IEnumerator ExplosionBombBlockCoroutine()
    {
        TileMapManager.Instance.CreateTileListByAreaList(explosionTileAreaList, posTile.Coordi, TileArea.vertical);

        instEffect = GameObjectPool.Instantiate<BlockEffect>(explosionEffectPrefab.gameObject);
        instEffect.SetEffectDataByData(posTile.transform.position, Vector3.zero);
        instEffect.PlayEffect();

        yield return instEffect.YieldEffectDuration;
        BaseExplosionBombBlock();

        BombBlockBasicHit(false, false);
    }
}
