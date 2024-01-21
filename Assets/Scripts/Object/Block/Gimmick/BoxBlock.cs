using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxBlock : Block, IForceCreateOnBoard
{
    public static string spriteString = "Box_{0}";
    public override string SpriteString { get => string.Format(spriteString, mBlockHP); }

    public override void SetBlockData(int blockNumber, int blockHP)
    {
        mBlockNumber = -1;// blockNumber;
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

            // blockContainer에 새 랜덤 블럭 추가해야함
            BlockManager.Instance.CreateBlockInBlockContainerReserve(blockContainer, typeof(NormalBlock), Random.Range(4,5), 1);
            return;
        }
        mBlockSprite.sprite = SpriteManager.Instance.GetPuzzleSpriteByName(SpriteString);
    }
    public override void SplashHitBlock(Tile tile, BlockContainer blockContainer)
    {
        HitBlock(tile, blockContainer, false);
    }
}
