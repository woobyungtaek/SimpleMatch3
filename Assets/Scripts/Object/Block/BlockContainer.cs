using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockContainer : MonoBehaviour, IReserveData
{
    public bool IsFixed { get => mMainBlock.IsFixed; }
    public bool IsOnlyNormalBlock
    {
        get
        {
            int loopCount = mBlockList.Count;
            for (int index = 0; index < loopCount; index++)
            {
                if (mBlockList[index] is BombBlock) { return false; }
            }
            return true;
        }
    }

    public int HomingOrder { get => mMainBlock.HomingOrder; }
    public int BlockContainerNumber { get => mMainBlock.BlockNumber; }
    public int BlockCount { get => mBlockList.Count; }
    public Sprite BlockSprite { get => mMainBlock.BlockSprite; }
    public Block MainBlock { get => mMainBlock; }

    // 블록 정보
    [SerializeField] private Block mMainBlock;
    [SerializeField] private List<Block> mBlockList = new List<Block>();

    // 드랍(무빙)
    private Vector3 mStartPos;
    private float mTime = 0f;

    // 이동시 경로
    private Queue<Tile> mRouteTileQueue = new Queue<Tile>();
    public Queue<Tile> RouteTileQueue { get => mRouteTileQueue; }
    public Tile DestTile;

    // 스왑
    public void StartMovePositionByTile(Tile targetTile, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(MovePositionCoroutine(targetTile, duration));
    }
    private IEnumerator MovePositionCoroutine(Tile targetTile, float duration)
    {
        mStartPos = transform.position;
        mTime = 0f;
        while (mTime < 1)
        {
            mTime += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(mStartPos, targetTile.transform.position, mTime);
            yield return null;
        }
        targetTile.BlockContainerOrNull = this;
        transform.position = targetTile.transform.position;
    }

    float speed = 1f;
    // 드랍
    private Coroutine mMoveRouteCoroutine;
    public void StartMovePositionByRoute(Tile startTile = null)
    {
        if (IsFixed) { return; }
        if (mRouteTileQueue.Count == 0) { return; }
        if (mMoveRouteCoroutine != null) { return; }
        speed = mRouteTileQueue.Count;
        if (speed > 2f) { speed = 2.25f; }
        mMoveRouteCoroutine = StartCoroutine(MoveRouteCoroutine(startTile));
    }

    private IEnumerator MoveRouteCoroutine(Tile startTile = null)
    {
        Tile targetTile = null;
        Tile beforeTile = startTile;

        while (mRouteTileQueue.Count != 0)
        {
            mStartPos = transform.position;

            // 목표 설정          
            targetTile = mRouteTileQueue.Dequeue();

            // 흐르는 상태인지 확인
            bool bFlowState = false;
            if (beforeTile != null)
            {
               if(targetTile != beforeTile.SendTileList[0])
                {
                    bFlowState = true;
                }
            }

            if(bFlowState)
            {
                // 위로 블록이 없어야함
                // 아래가 Arrive여야함

                while ( !targetTile.IsPossibleFlowDrop )
                {
                    yield return null;
                }
            }
            else
            {
                // 내리기 가능한 상태인지 확인
                while (targetTile.BlockContainerOrNull != null)
                {
                    yield return null;
                }
            }

            if(beforeTile != null)
            {
                beforeTile.BlockContainerOrNull = null;
            }
            targetTile.BlockContainerOrNull = this;
            var targetPos = targetTile.transform.position;
            
            // 이동
            mTime = 0f;
            while (mTime < 1)
            {
                mTime += (Time.deltaTime / GameConfig.DROP_DURATION) * speed;
                transform.position = Vector3.LerpUnclamped(mStartPos, targetPos, mTime);
                yield return null;
            }
            beforeTile = targetTile;
        }

        targetTile.IsArrive = true;
        transform.position = targetTile.transform.position;

        mMoveRouteCoroutine = null;
        ObserverCenter.Instance.SendNotification(Message.DropEndCheck);
    }

    // 블록 관리
    public Block GetContainSameBlock(System.Type type, int blockColor)
    {
        foreach(var block in mBlockList)
        {
            if(block.IsSameBlock(type, blockColor))
            {
                return block;
            }
        }
        return null;
    }

    public void HitBlockContainer(Tile tile, bool bExplosionHit)
    {
        int loopCount = mBlockList.Count - 1;
        for (int index = loopCount; index >= 0; index--)
        {
            mBlockList[index].HitBlock(tile, this, bExplosionHit);
        }
        if (mBlockList.Count == 0)
        {
            tile.BlockContainerOrNull = null;
            GameObjectPool.ReturnObject(gameObject);
        }
    }

    // SplashHitBlockContainer > 이미 Hit거나 SpalshHit면 반려 한다.
    public void SplashHitBlockContainer(Tile tile)
    {
        int loopCount = mBlockList.Count - 1;
        for (int index = loopCount; index >= 0; index--)
        {
            mBlockList[index].SplashHitBlock(tile, this);
        }
        if (mBlockList.Count == 0)
        {
            tile.BlockContainerOrNull = null;
            GameObjectPool.ReturnObject(gameObject);
        }
    }

    public void AddBlockToBlockList(Block block)
    {
        block.transform.SetParent(transform);
        block.transform.localPosition = Vector3.zero;
        block.transform.localScale = Vector3.one;
        mBlockList.Add(block);

        if (mMainBlock != null)
        {
            if (block.Order < mMainBlock.Order) { return; }
        }
        mMainBlock = block;
    }
    public void RemoveAllBlock()
    {
        int loopCount = mBlockList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            mMainBlock = null;
            GameObjectPool.ReturnObject(mBlockList[index].gameObject);
        }
        mBlockList.Clear();
    }
    public void RemoveBlockByBlock(Block block)
    {
        mBlockList.Remove(block);
        if (block.Equals(mMainBlock))
        {
            mMainBlock = null;
        }
        GameObjectPool.ReturnObject(block.gameObject);
    }
}
