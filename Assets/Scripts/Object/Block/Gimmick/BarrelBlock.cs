using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelBlock : Block, IReserveBlockMaker
{
    public static string spriteString = "NormalBlock_1";
    public override string SpriteString { get => spriteString; }
    public override void SetBlockData(int blockNumber, int blockHP)
    {
        mBlockNumber = -1; // blockNumber;
        mBlockHP = blockHP;
        ExcuteCalculateHomiingOrder(null);
        mBlockSprite.sprite = SpriteManager.Instance.GetPuzzleSpriteByName(SpriteString);
    }

    public override void HitBlock(Tile tile, BlockContainer blockContainer, bool bExplosion)
    {
        if (BlockHP <= 0) { return; }

        BlockHP -= 1;
        if (BlockHP <= 0)
        {
            CheckMissionBlock();
            base.RemoveBlockToBlockContianer(blockContainer);
            return;
        }
        mBlockSprite.sprite = SpriteManager.Instance.GetPuzzleSpriteByName(SpriteString);
    }
    public override void SplashHitBlock(Tile tile, BlockContainer blockContainer)
    {
        HitBlock(tile, blockContainer, false);
    }
}
