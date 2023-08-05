using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorBombBlock : BombBlock
{
    public static string spriteString = "ColorExplosionBlock";
    public override string SpriteString { get => spriteString; }

    public override int HomingOrder
    {
        get { return -100; }
    }

    public override void SetBlockData(int blockNumber, int blockHP)
    {
        base.SetBlockData(-1, blockHP);
    }
    public override void HitBlock(Tile tile, BlockContainer blockContainer, bool bExplosion)
    {
        if (bExplosion == true) { return; }
        base.HitBlock(tile, blockContainer, bExplosion);
    }

    public override void ExplosionBombBlock()
    {
        StartCoroutine(ExplosionBombBlockCoroutine());
    }

    private IEnumerator ExplosionBombBlockCoroutine()
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
        base.ExplosionBombBlock();

        BombBlockBasicHit(true);
    }
    protected override string GetSpriteNameByBlockNumber()
    {
        return SpriteString;
    }
}
