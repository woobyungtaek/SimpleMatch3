using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalTile : Tile
{
    [SerializeField] private List<Tile> mAroundTileList = new List<Tile>();

    private List<TileGimmick> mTileGimmickList = new List<TileGimmick>();


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            BlockContainerOrNull?.StartMovePositionByRoute();
        }
    }

    public int HomingOrder
    {
        get
        {
            int result = 0;
            if (BlockContainerOrNull != null)
            {
                result = BlockContainerOrNull.HomingOrder;
            }

            for (int index = 0; index < mTileGimmickList.Count; ++index)
            {
                if (result >= mTileGimmickList[index].HomingOrder) { continue; }
                result = mTileGimmickList[index].HomingOrder;
            }
            return result;
        }
    }


    public override bool IsCanSend
    {
        get
        {
            if (ReserveData == null) { return false; }
            if (ReserveData.IsFixed) { return false; }
            if (mSendTileList.Count == 0) { return false; }

            Tile underTile = mSendTileList[0];
            if (underTile == null) { return false; }
            if (underTile.ReserveData == null) { return true; }
            return false;
        }
    }
    public override Tile IsCanSend_Flow
    {
        get
        {
            if (ReserveData == null) { return null; }
            if (ReserveData.IsFixed) { return null; }
            if (mSendTileList.Count == 0) { return null; }

            for (int idx = 1; idx < mSendTileList.Count; ++idx)
            {
                if (mSendTileList[idx] == null) { continue; }
                if (mSendTileList[idx].ReserveData != null) { continue; }
                if (!mSendTileList[idx].IsCanFlow_Up) { continue; }
                if (!mSendTileList[idx].IsCanFlow_Down) { continue; }
                return mSendTileList[idx];
            }
            return null;
        }
    }

    public override bool IsCanFlow_Up
    {
        // 현재 타일에 블록이 흘러도 괜찮은가(위로 체크)
        // false > 흐르면 안된다.
        // true  > 흘러도 된다.
        // 기본) 위의 타일을 확인한다.
        get
        {
            if (mRecieveTileList.Count == 0) { return true; }

            Tile upTile = mRecieveTileList[0];
            if (upTile == null) { return true; }

            if (upTile.IsCreateTile) { return false; }
            if (upTile.ReserveData != null)
            {
                return upTile.ReserveData.IsFixed; // false
            }

            // 위 타일로는 판단이 불가능하므로 그 위를 체크한다.
            return upTile.IsCanFlow_Up;
        }
    }
    public override bool IsCanFlow_Down
    {
        get
        {
            // 현재 타일에 블록이 흘러도 괜찮은가 (아래로 체크)
            // false > 흐르면 안된다.
            // true > 흘러도 된다.
            // 기본) 아래의 타일을 확인한다.
            if (mSendTileList.Count == 0) { return true; }

            Tile underTile = mSendTileList[0];
            if (underTile == null) { return true; }
            //if (underTile.ReserveData == null) { return false; } // 비어있는데 위 아래로 
            //if (underTile.ReserveData.IsFixed) { return true; }
            if (underTile.ReserveData != null) { if (underTile.ReserveData.IsFixed) { return true; } }

            // 아래 타일로는 판단이 불가능하므로 그 아래를 체크한다.
            return underTile.IsCanFlow_Down;
        }
    }

    public override bool IsFullUnder
    {
        get
        {
            if (ReserveData == null) { return false; }
            if (ReserveData.IsFixed) { return true; }

            foreach (var tile in mSendTileList)
            {
                if (tile == null) { continue; }
                if (!tile.IsFullUnder) { return false; }
            }

            return true;
        }
    }

    public override bool IsCanFlow_UpAtEmpty
    {
        // 위쪽 타일에 블록이 있는지와 상관없이
        // 흐르는 타일이라면 체크 한다.
        // false > 흐르면 안된다.
        // true  > 흘러도 된다.
        // 기본) 위의 타일을 확인한다.
        get
        {
            if (mRecieveTileList.Count == 0) { return true; }

            Tile upTile = mRecieveTileList[0];
            if (upTile == null) { return true; }
            if (upTile.IsCreateTile) { return false; }
            if (upTile.ReserveData != null)
            {
                if (upTile.ReserveData.IsFixed)
                {
                    return true;
                }
            }

            // 위 타일로는 판단이 불가능하므로 그 위를 체크한다.
            return upTile.IsCanFlow_UpAtEmpty;
        }
    }

    public override void IsCanFlow_Empty(ref bool result)
    {
        if (!result) { return; }

        bool bUseResult = false;
        // 이미 체크가 된 경우, 체크된 값을 쓴다.
        if (mFlowStateDict.ContainsKey(Coordi))
        {
            bUseResult = mFlowStateDict[Coordi];
        }
        // 아니라면 새로 체크해야한다.
        else
        {
            bUseResult = IsCanFlow_UpAtEmpty;
            AddFlowStateDict(bUseResult);
        }

        if (!bUseResult)
        {
            //Debug.Log($"{Coordi} : 흐르기 가능 타일이 아님");
            return;
        }

        result &= bUseResult;
        // 있어야할 곳에 블럭이 없으므로 false
        if (ReserveData == null)
        {
            result = false;
            //Debug.Log($"{Coordi} : 데이터가 없음");
            return;
        }

        foreach (var sendTile in mSendTileList)
        {
            if (sendTile == null) { continue; }
            sendTile.IsCanFlow_Empty(ref result);
            if (!result) { return; }
        }

    }

    public override bool IsCanFlow_UpEmpty
    {
        get
        {
            if (mRecieveTileList[0] == null) { return true; }
            if (mRecieveTileList[0].BlockContainerOrNull != null)
            {
                return mRecieveTileList[0].BlockContainerOrNull.IsFixed;
            }
            return mRecieveTileList[0].IsCanFlow_UpEmpty;
        }
    }

    public override bool IsReady
    {
        get
        {
            return ReserveData != null;
        }
    }

    public override EDropableState DropableState
    {
        set => mDropAbleState = value;
    }

    // PushEffect
    private int mPushEffectOrder;
    private Vector2 mPushDir;
    private float mPushPower;

    public void CreateAroundSquareList()
    {
        TileMapManager.Instance.CreateTileListIncludeNullByAreaList(mAroundTileList, Coordi, TileArea.smallcross);
    }
    public void CheckChainTileReculsive(int blockNum, HashSet<Tile> checkHashSet, List<Tile> chainList)
    {
        if (checkHashSet.Contains(this)) { return; }

        int loopCount;
        checkHashSet.Add(this);
        chainList.Add(this);

        loopCount = mAroundTileList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            // 같은 번호 체인 체크
            if (mAroundTileList[index] == null) { continue; }
            if (!(mAroundTileList[index] is NormalTile)) { continue; }
            if (mAroundTileList[index].BlockContainerOrNull == null) { continue; }
            if (mAroundTileList[index].BlockContainerOrNull.BlockContainerNumber != blockNum) { continue; }
            (mAroundTileList[index] as NormalTile)?.CheckChainTileReculsive(blockNum, checkHashSet, chainList);
        }
    }
    public Tile GetSameNumberTileOrNullByDir(int dir)
    {
        if (!(mAroundTileList[dir] is NormalTile)) { return null; }
        if (mAroundTileList[dir].BlockContainerOrNull == null) { return null; }
        if (mAroundTileList[dir].BlockContainerOrNull.BlockContainerNumber == -1) { return null; }
        if (BlockContainerOrNull.BlockContainerNumber != mAroundTileList[dir].BlockContainerOrNull.BlockContainerNumber) { return null; }
        return mAroundTileList[dir];
    }
    public Tile GetAroundNormalTileByDir(int dir)
    {
        if (mAroundTileList[dir] is NormalTile)
        {
            return mAroundTileList[dir];
        }
        return null;
    }

    public override void ResetTileState()
    {
        base.ResetTileState();
        mPushEffectOrder = 99999;
        mPushDir = Vector3.zero;
        mPushPower = 0;
    }

    public override void AddFlowStateDict(bool checkResult)
    {
        if (mFlowStateDict.ContainsKey(Coordi) == false)
        {
            mFlowStateDict.Add(Coordi, checkResult);
        }
        if (mSendTileList[0] == null) { return; }
        mSendTileList[0].AddFlowStateDict(checkResult);
    }

    public override void HitTile(bool bExplosionHit, bool bOnSplashHit)
    {
        //if (IsHit == true) { return; }
        if (IsMerged == true) { return; }
        if (BlockContainerOrNull == null) { return; }
        if (BlockContainerOrNull.MainBlock == null)
        {
            GameObjectPool.ReturnObject(BlockContainerOrNull.gameObject);
            BlockContainerOrNull = null;
            return; 
        }
        IsHit = true;

        // 기믹 : block은 hit 안하는 기믹이 있을 수 있다.
        // 순서가 낮은 기믹은 Hit 안하는 기믹이 있을 수 있다.
        int loopCount = mTileGimmickList.Count - 1;
        for (int index = loopCount; index >= 0; index--)
        {
            mTileGimmickList[index].Hit(this);
        }

        // 기믹중 막는 기믹이 있다면 여기서 리턴
        BlockContainerOrNull.StopPushEffect(this);

        // 밀려나는 연출용 오더값
        mPushEffectOrder = 0;

        // 블록 히트, 스플래쉬 히트
        if (bOnSplashHit)
        {
            foreach (var tile in mAroundTileList)
            {
                // 블럭컨테이너의 Color값이 전달되어야한다.
                tile.HitTile_Splash();
            }
        }
        BlockContainerOrNull.HitBlockContainer(this, bExplosionHit);
        TileMapManager.Instance.CreateTileHitEffect(gameObject.transform.position);
    }

    public override void HitTile_Splash()
    {
        //if (IsHit == true) { return; }
        if (IsMerged == true) { return; }
        if (IsSplashHit == true) { return; }
        IsSplashHit = true;

        if (BlockContainerOrNull == null) { return; }
        if (BlockContainerOrNull.MainBlock == null)
        {
            GameObjectPool.ReturnObject(BlockContainerOrNull.gameObject);
            BlockContainerOrNull = null;
            return;
        }
        BlockContainerOrNull.SplashHitBlockContainer(this);
    }

    public override void StopPushEffect()
    {
        if (BlockContainerOrNull == null) { return; }
        // 기믹중 막는 기믹이 있다면 여기서 리턴
        BlockContainerOrNull.StopPushEffect(this);
    }
    public override void StartPushEffect(int pushDegree = 2)
    {
        foreach (var tile in mAroundTileList)
        {
            tile.SetPushEffect(0.75f, mPushEffectOrder + 1, pushDegree, this);
        }
    }
    public override void SetPushEffect(float pushPower, int order, int pushDegree, Tile callTile)
    {
        if (pushDegree <= 0) { return; }
        if (IsHit == true) { return; }
        if (BlockContainerOrNull == null) { return; }
        if (BlockContainerOrNull.IsFixed) { return; }

        // 오더에 따른 방향 설정
        if (mPushEffectOrder < order) { return; }
        else if (mPushEffectOrder > order)
        {
            // 오더가 더 낮은게 들어오면 초기화
            mPushDir = Coordi - callTile.Coordi;
            mPushPower = pushPower;
        }
        else
        {
            // 오더가 같은게 들어오면 방향 더하기
            mPushDir += Coordi - callTile.Coordi;
            mPushPower = pushPower * 0.75f;
        }

        mPushEffectOrder = order;
        pushPower -= mPushPower * 0.5f;

        foreach (var tile in mAroundTileList)
        {
            tile.SetPushEffect(pushPower, mPushEffectOrder + 1, pushDegree - 1, this);
        }

        TileMapManager.Instance.AddPushTile(this);
    }
    public void StartPushCoroutine()
    {
        if (IsHit == true) { return; }
        if (BlockContainerOrNull == null) { return; }
        if (BlockContainerOrNull.IsFixed) { return; }
        BlockContainerOrNull.StartPushEffectCoroutine(mPushDir, mPushPower, this);
    }

    public override void Dispose()
    {
        mAroundTileList.Clear();
        base.Dispose();
    }

    public override void SendReserveData(Tile sendTile)
    {
        ReserveData = sendTile.ReserveData;
        sendTile.ReserveData = null;

        ReserveData.Enqueue(this);
    }

    public override bool StraightMove()
    {
        if (ReserveData == null) { return false; }
        if (ReserveData.IsFixed) { return false; }
        if (mSendTileList.Count == 0) { return false; }

        Tile underTile = mSendTileList[0];
        if (underTile == null) { return false; }
        if (underTile.ReserveData != null) { return false; }

        underTile.SendReserveData(this);
        return true;
    }

    public override bool FlowingMove()
    {
        if (ReserveData == null) { return false; }
        if (ReserveData.IsFixed) { return false; }
        if (mSendTileList.Count == 0) { return false; }

        Tile underTile = mSendTileList[0];
        if (underTile == null) { return false; }
        if (underTile.ReserveData == null) { return false; }

        Tile targetTile = null;
        for (int index = 1; index < mSendTileList.Count; ++index)
        {
            if (mSendTileList[index] == null) { continue; }
            if (mSendTileList[index].ReserveData != null) { continue; }
            targetTile = mSendTileList[index];
        }

        if (targetTile == null) { return false; }
        if (!targetTile.IsCanFlow_Up) { return false; }

        if (!underTile.IsFullUnder) { return false; }

        targetTile.SendReserveData(this);

        return true;
    }

    public override bool CheckDropableState()
    {
        switch (DropableState)
        {
            case EDropableState.Possible: return true;
            case EDropableState.Impossible: return false;
        }

        if (BlockContainerOrNull != null)
        {
            if (BlockContainerOrNull.IsFixed)
            {
                DropableState = EDropableState.Impossible;
                return false;
            }
        }

        for (int index = 0; index < mRecieveTileList.Count; ++index)
        {
            if (mRecieveTileList[index] == null) { continue; }
            bool bDropable = mRecieveTileList[index].CheckDropableState();

            if (!bDropable) { continue; }
            DropableState = EDropableState.Possible;
            return true;
        }

        DropableState = EDropableState.Impossible;
        return false;
    }

    public override bool CheckDropReadyState()
    {
        if (ReserveData != null)
        {
            if (ReserveData.IsFixed) { return true; }
            return false;
        }

        for (int index = 0; index < mRecieveTileList.Count; ++index)
        {
            if (mRecieveTileList[index] == null) { continue; }
            bool bReady = mRecieveTileList[index].CheckDropReadyState();
            if (bReady) { continue; }
            return false;
        }

        return true;
    }

    public override void StartDrop()
    {
        if (BlockContainerOrNull == null) { return; }
        if (BlockContainerOrNull.RouteTileQueue.Count == 0) { return; }
        BlockContainerOrNull.StartMovePositionByRoute();
        BlockContainerOrNull = null;
    }


    // 타일 기믹
    public override TileGimmick IsContainTileGimmick(System.Type type)
    {
        foreach (var tg in mTileGimmickList)
        {
            if (tg.GetType() == type)
            {
                return tg;
            }
        }
        return null;
    }
    public override void AddTileGimmick(TileGimmick gimmick)
    {
        gimmick.gameObject.transform.position = transform.position;

        mTileGimmickList.Add(gimmick);
        mTileGimmickList.Sort((TileGimmick a, TileGimmick b) => a.Order.CompareTo(b.Order));
    }
    public override void RemoveTileGimmick(TileGimmick gimmick)
    {
        mTileGimmickList.Remove(gimmick);
        GameObjectPool.ReturnObject(gimmick.gameObject);
    }
}
