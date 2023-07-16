using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineBlock : Block
{
    public static string spriteString = "NormalBlock_1";
    public override string SpriteString { get => spriteString; }


    public static readonly WaitForSeconds mVineDelayTime = new WaitForSeconds(0.25f);
    public static bool IsVineHited;
    public static int VineBlockCount;


    public override void SetBlockData(int blockNumber, int blockHP)
    {
        VineBlockCount++;

        mBlockNumber = blockNumber;
        mBlockHP = blockHP;
        ExcuteCalculateHomiingOrder(null);
    }

    public override void HitBlock(Tile tile, BlockContainer blockContainer, bool bExplosion)
    {
        IsVineHited = true;

        BlockHP -= 1;
        if (BlockHP <= 0)
        {
            VineBlockCount--;
            CheckMissionBlock();
            base.RemoveBlockToBlockContianer(blockContainer);
        }
    }
    public override void SplashHitBlock(Tile tile, BlockContainer blockContainer)
    {
        HitBlock(tile, blockContainer, false);
    }
}
