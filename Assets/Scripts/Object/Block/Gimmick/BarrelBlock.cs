using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelBlock : Block
{
    public static string spriteString = "NormalBlock_1";
    public override string SpriteString { get => spriteString; }
    public override void SetBlockData(int blockNumber, int blockHP)
    {
        mBlockNumber = blockNumber;
        mBlockHP = blockHP;
        ExcuteCalculateHomiingOrder(null);
    }

    public override void HitBlock(Tile tile, BlockContainer blockContainer, bool bExplosion)
    {
        BlockHP -= 1;
        if (BlockHP <= 0)
        {
            CheckMissionBlock();
            base.RemoveBlockToBlockContianer(blockContainer);
        }
    }
    public override void SplashHitBlock(Tile tile, BlockContainer blockContainer)
    {
        HitBlock(tile, blockContainer, false);
    }
}
