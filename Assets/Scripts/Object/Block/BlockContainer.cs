using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockContainer : MonoBehaviour, IReserveData
{
    public bool IsFixed
    {
        get
        {
            if (mMainBlock != null)
            {
                return mMainBlock.IsFixed;
            }
            return false;
        }
    }

    public bool IsVineTarget
    {
        get
        {
            int loopCount = mBlockList.Count;
            for (int index = 0; index < loopCount; index++)
            {
                if (!mBlockList[index].IsVineTarget) { return false; }
            }
            return true;
        }
    }
    public bool IsOnlyNormalBlock
    {
        get
        {
            int loopCount = mBlockList.Count;
            for (int index = 0; index < loopCount; index++)
            {
                if (!(mBlockList[index] is NormalBlock)) { return false; }
            }
            return true;
        }
    }

    public int HomingOrder { get => mMainBlock.HomingOrder; }
    public int BlockContainerNumber
    {
        get
        {
            if(mMainBlock == null) { return -1; }
            return mMainBlock.BlockNumber;
        }
    }
    public int BlockCount { get => mBlockList.Count; }
    public Sprite BlockSprite { get => mMainBlock.BlockSprite; }
    public Block MainBlock { get => mMainBlock; }

    // 블록 정보
    [SerializeField] private Block mMainBlock;
    [SerializeField] private List<Block> mBlockList = new List<Block>();
    [SerializeField] private List<Block> mReserveBlockList = new List<Block>();

    // 드랍(무빙)
    private Vector3 mStartPos;
    private float mTime = 0f;
    private AnimationCurve mMoveAniCurve;

    // 밀려나는 연출
    private Vector3 mPushPos;
    private Coroutine mPushCoroutine;
    private static AnimationCurve mPushAniCurve;

    // 이동시 경로
    private Queue<Tile> mRouteTileQueue = new Queue<Tile>();
    public Queue<Tile> RouteTileQueue { get => mRouteTileQueue; }
    public Tile DestTile { get; set; }

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
    public void StartMovePositionByRoute()
    {
        if (IsFixed) { return; }
        if (mRouteTileQueue.Count == 0) { return; }
        if (mMoveRouteCoroutine != null) { return; }
        speed = mRouteTileQueue.Count;
        if (speed > 2f) { speed = 2.25f; }

        if (mPushCoroutine != null)
        {
            StopCoroutine(mPushCoroutine);
            mPushCoroutine = null;
        }
        mMoveRouteCoroutine = StartCoroutine(MoveRouteCoroutine());
    }
    private IEnumerator MoveRouteCoroutine()
    {
        Tile targetTile = null;

        float mTimeLimit = 1f;
        mMoveAniCurve = BlockManager.Instance.BlockMoveAniCurve;

        while (mRouteTileQueue.Count != 0)
        {
            mStartPos = transform.position;

            // 목표 설정          
            targetTile = mRouteTileQueue.Dequeue();

            // 바운스 상태 체크 (뿌요오옹~ 효과)
            if (mRouteTileQueue.Count == 0)
            {
                mTimeLimit = 1.5f;
                mMoveAniCurve = BlockManager.Instance.BlockArriveAniCurve;
            }

            var targetPos = targetTile.transform.position;

            //// 현재 블록 컨테이너의 목표의 타일의 아래가 도착상태일때
            //if (DestTile != null)
            //{
            //    if (DestTile.SendTileList[0] != null)
            //    {
            //        while (DestTile.SendTileList[0].IsArrive == false)
            //        {
            //            yield return null;
            //        }
            //    }
            //}

            // 이동
            mTime = 0f;
            while (mTime < mTimeLimit)
            {
                mTime += (Time.deltaTime / GameConfig.DROP_DURATION) * speed;
                transform.position = Vector3.LerpUnclamped(mStartPos, targetPos, mMoveAniCurve.Evaluate(mTime));
                yield return null;
            }
        }
        targetTile.BlockContainerOrNull = this;

        targetTile.IsArrive = true;
        transform.position = targetTile.transform.position;

        mMoveRouteCoroutine = null;

        ObserverCenter.Instance.SendNotification(Message.DropEndCheck);
    }

    // 밀려 나기 연출
    public void StartPushEffectCoroutine(Vector2 dir, float pushPower, Tile startTile)
    {
        if (mPushCoroutine != null)
        {
            StopCoroutine(mPushCoroutine);
            mPushCoroutine = null;
        }

        mStartPos = startTile.transform.position;
        mPushPos.x = mStartPos.x + dir.x;
        mPushPos.y = mStartPos.y - dir.y;

        if (mPushAniCurve == null)
        {
            mPushAniCurve = BlockManager.Instance.BlockPushAniCurve;
        }

        mPushCoroutine = StartCoroutine(PushCoroutine(pushPower));
    }
    private IEnumerator PushCoroutine(float pushPower)
    {
        mTime = 0;
        while (mTime < 1f)
        {
            mTime += (Time.deltaTime / GameConfig.PUSH_DURATION);
            transform.position = Vector3.LerpUnclamped(mStartPos, mPushPos, mPushAniCurve.Evaluate(mTime) * pushPower);
            yield return null;
        }

        // 종료 시
        transform.position = mStartPos;
        mPushCoroutine = null;
    }

    public void StopPushEffect(Tile tile)
    {
        if (mPushCoroutine != null)
        {
            StopCoroutine(mPushCoroutine);
            mPushCoroutine = null;
            transform.position = tile.transform.position;
        }
    }

    // 블록 관리
    public Block GetContainSameBlock(System.Type type, int blockColor)
    {
        foreach (var block in mBlockList)
        {
            if (block.IsSameBlock(type, blockColor))
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
            if(mBlockList[index] == null) { continue; }
            mBlockList[index].HitBlock(tile, this, bExplosionHit);
        }

        // Reserve를 여기다 걸어 놓으면?
        loopCount = mReserveBlockList.Count;
        for (int index = 0; index < loopCount; ++index)
        {
            AddBlockToBlockList(mReserveBlockList[index]);
        }
        mReserveBlockList.Clear();

        if (mBlockList.Count == 0)
        {
            tile.BlockContainerOrNull = null;
            GameObjectPool.ReturnObject(gameObject);
        }
    }

    public void SplashHitBlockContainer(Tile tile)
    {
        int loopCount = mBlockList.Count - 1;
        for (int index = loopCount; index >= 0; index--)
        {
            mBlockList[index].SplashHitBlock(tile, this);
        }

        // Reserve를 여기다 걸어 놓으면?
        loopCount = mReserveBlockList.Count;
        for (int index = 0; index < loopCount; ++index)
        {
            AddBlockToBlockList(mReserveBlockList[index]);
        }
        mReserveBlockList.Clear();

        if (mBlockList.Count == 0)
        {
            tile.BlockContainerOrNull = null;
            GameObjectPool.ReturnObject(gameObject);
        }
    }

    public void AddBlockToReserveList(Block block)
    {
        block.transform.SetParent(transform);
        mReserveBlockList.Add(block);
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
        if (mPushCoroutine != null)
        {
            StopCoroutine(mPushCoroutine);
            mPushCoroutine = null;
        }

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