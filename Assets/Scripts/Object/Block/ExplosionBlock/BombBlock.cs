using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBlock : Block
{
    public BlockEffect ExplosionEffect { get => explosionEffectPrefab; }

    [SerializeField] protected Tile posTile;
    [SerializeField] protected List<Tile> explosionTileAreaList;

    [SerializeField] protected BlockEffect explosionEffectPrefab;

    protected BlockEffect instEffect;

    public virtual void ExplosionBombBlock()
    {
        RemoveBlockToBlockContianer(posTile.BlockContainerOrNull);
        posTile.CheckBlockContainer();
    }

    public override void HitBlock(Tile tile, BlockContainer blockContainer, bool bExplosion)
    {
        BlockHP -= 1;
        if (BlockHP <= 0)
        {
            posTile = tile;
            CheckMissionBlock();
            TileMapManager.Instance.AddExplosionBlockQueueByBlock(this);
        }
    }

    protected void BombBlockBasicHit(bool bPushEffectPlay, int pushDegree = 2)
    {
        int loopCount = explosionTileAreaList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            explosionTileAreaList[index].HitTile(true);
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
