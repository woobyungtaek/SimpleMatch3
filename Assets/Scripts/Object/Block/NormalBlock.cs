using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBlock : Block
{
    public static string spriteString = "NormalBlock_{0}";
    public override string SpriteString { get => spriteString; }

    public override void HitBlock(Tile tile, BlockContainer blockContainer, bool bExplosion)
    {
        BlockHP -= 1;
        if(BlockHP <= 0)
        {
            CheckMissionBlock();
            base.RemoveBlockToBlockContianer(blockContainer);
        }
    }

    protected override string GetSpriteNameByBlockNumber()
    {
        return string.Format(SpriteString, BlockNumber);
    }
}
