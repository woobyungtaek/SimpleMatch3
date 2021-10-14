using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockContainer : MonoBehaviour
{
    public bool IsMove { get=> true; }
    public bool IsOnlyNormalBlock
    {
        get
        {
            int loopCount = mBlockList.Count;
            for(int index =0; index< loopCount; index++)
            {
                if(mBlockList[index] is BombBlock) { return false; }
            }
            return true;
        }
    }

    public int HomingOrder { get => mMainBlock.HomingOrder; }
    public int BlockContainerNumber { get => mMainBlock.BlockNumber; }
    public int BlockCount { get => mBlockList.Count; }
    public Sprite BlockSprite { get => mMainBlock.BlockSprite; }
    public Block MainBlock { get => mMainBlock; }    

    private Vector3 mStartPos;
    private float mTime = 0f;


    [SerializeField] private Block mMainBlock;
    [SerializeField] private List<Block> mBlockList = new List<Block>();

    public void AddBlockToBlockList(Block block)
    {
        block.transform.SetParent(transform);
        block.transform.localPosition = Vector3.zero;
        block.transform.localScale = Vector3.one;
        mBlockList.Add(block);

        if(mMainBlock != null)
        {
            if (block.Order < mMainBlock.Order) { return; }
        }
        mMainBlock = block;
    }
    
    public void StopMove()
    {
        StopAllCoroutines();
    }

    public void StartMovePositionByTile(Tile targetTile, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(MovePositionCoroutine(targetTile, duration));
    }
    IEnumerator MovePositionCoroutine(Tile targetTile, float duration)
    {
        mStartPos = transform.position;
        mTime = 0f;
        while(mTime < 1)
        {
            mTime += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(mStartPos, targetTile.transform.position, mTime);
            yield return null;
        }
        targetTile.BlockContainerOrNull = this;
        transform.position = targetTile.transform.position;
        targetTile.IsDroping = false;
    }

    public void HitBlockContainer(Tile tile, bool bExplosionHit)
    {
        int loopCount = mBlockList.Count - 1;
        for (int index = loopCount; index >= 0; index--)
        {
            mBlockList[index].HitBlock(tile, this, bExplosionHit);
        }
        if(mBlockList.Count == 0)
        {
            tile.BlockContainerOrNull = null;
            GameObjectPool.Destroy(gameObject);
        }
    }

    public void RemoveAllBlock()
    {
        int loopCount = mBlockList.Count;
        for(int index =0; index< loopCount; index++)
        {
            mMainBlock = null;
            GameObjectPool.Destroy(mBlockList[index].gameObject);
        }
        mBlockList.Clear();
    }
    public void RemoveBlockByBlock(Block block)
    {
        mBlockList.Remove(block);    
        if(block.Equals(mMainBlock))
        {
            mMainBlock = null;
        }
        GameObjectPool.Destroy(block.gameObject);
    }
}
