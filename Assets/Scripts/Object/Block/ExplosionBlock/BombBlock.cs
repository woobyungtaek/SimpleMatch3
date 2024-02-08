using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBlock : Block
{
    public override bool IsVineTarget { get => true; }

    public BlockEffect ExplosionEffect { get => explosionEffectPrefab; }

    [SerializeField] protected Tile posTile;
    [SerializeField] protected List<Tile> explosionTileAreaList;

    [SerializeField] protected BlockEffect explosionEffectPrefab;

    protected BlockEffect instEffect;

    private Coroutine mExplosionCoroutine;

    public virtual void ExplosionBombBlock()
    {
        if(mExplosionCoroutine != null)
        {
            Coroutine_Helper.StopCoroutine(mExplosionCoroutine);
        }
        mExplosionCoroutine = Coroutine_Helper.StartCoroutine(ExplosionBombBlockCoroutine());
    }

    protected void BaseExplosionBombBlock()
    {
        RemoveBlockToBlockContianer(posTile.BlockContainerOrNull);
        posTile.CheckBlockContainer();
    }

    protected virtual IEnumerator ExplosionBombBlockCoroutine()
    {
        yield return null;
    }

    public override void HitBlock(Tile tile, BlockContainer blockContainer, bool bExplosion)
    {
        if (BlockHP <= 0) { return; }

        BlockHP -= 1;
        if (BlockHP <= 0)
        {
            posTile = tile;
            CheckMissionBlock();
            TileMapManager.Instance.AddExplosionBlockQueueByBlock(this);
        }
    }

    protected void BombBlockBasicHit(bool bPushEffectPlay, bool bOnSplashHit, int pushDegree = 2)
    {
        int loopCount = explosionTileAreaList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            explosionTileAreaList[index].HitTile(true, bOnSplashHit);
        }

        if (bPushEffectPlay)
        {
            for (int index = 0; index < loopCount; index++)
            {
                explosionTileAreaList[index].StartPushEffect(pushDegree);
            }
            TileMapManager.Instance.StartPushEffect();
        }
    }
}
