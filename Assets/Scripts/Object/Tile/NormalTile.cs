using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalTile : Tile
{
    [SerializeField] private List<Tile> mAroundTileList = new List<Tile>();

    private List<TileGimmick> mTileGimmickList = new List<TileGimmick>();

    public int HomingOrder
    {
        get
        {
            int result = 0;
            if(BlockContainerOrNull != null)
            {
                result = BlockContainerOrNull.HomingOrder;
            }

            for(int index =0; index < mTileGimmickList.Count; ++index)
            {
                if(result >= mTileGimmickList[index].HomingOrder) { continue; }
                result = mTileGimmickList[index].HomingOrder;
            }
            return result;
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
            if (underTile.ReserveData == null) { return false; }
            if (underTile.ReserveData.IsFixed)
            {
                return true;
            }

            // 아래 타일로는 판단이 불가능하므로 그 아래를 체크한다.
            return underTile.IsCanFlow_Down;
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
            (mAroundTileList[index] as NormalTile).CheckChainTileReculsive(blockNum, checkHashSet, chainList);
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

    public override void AddFlowStateDict(bool checkResult)
    {
        if (mFlowStateDict.ContainsKey(Coordi) == false)
        {
            mFlowStateDict.Add(Coordi, checkResult);
        }
        if (mSendTileList[0] == null) { return; }
        mSendTileList[0].AddFlowStateDict(checkResult);
    }

    public override void HitTile(bool bExplosionHit)
    {
        if (IsHit == true) { return; }
        if (BlockContainerOrNull == null) { return; }
        IsHit = true;

        // 기믹 : block은 hit 안하는 기믹이 있을 수 있다.
        // 순서가 낮은 기믹은 Hit 안하는 기믹이 있을 수 있다.
        int loopCount = mTileGimmickList.Count - 1;
        for (int index = loopCount; index >= 0; index--)
        {
            mTileGimmickList[index].Hit(this);
        }

        // 기믹중 막는 기믹이 있다면 여기서 리턴

        // 블록
        if (!bExplosionHit)
        {
            foreach (var tile in mAroundTileList)
            {
                // 블럭컨테이너의 Color값이 전달되어야한다.
                tile.HitTile_Splash();
            }
        }
        BlockContainerOrNull.HitBlockContainer(this, bExplosionHit);

    }
    public override void HitTile_Splash()
    {
        if (IsHit == true) { return; }
        if (IsSplashHit == true) { return; }
        IsSplashHit = true;

        if (BlockContainerOrNull == null) { return; }
        BlockContainerOrNull.SplashHitBlockContainer(this);
    }
    public override void Dispose()
    {
        mAroundTileList.Clear();
        base.Dispose();
    }

    public override void CheckDrop()
    {
        //데이터가 있다면 행동하지 않는다.
        if (ReserveData != null)
        {
            return;
        }
        if (RecieveTileList[0] == null) { return; }

        // 바로 위 타일로부터 받아올 수 있는지 확인한다.
        // 생성자 타일이거나 또는 데이터가 있거나
        if (RecieveTileList[0].IsCreateTile)
        {
            RecieveTileList[0].RequestReserveData(this); // 가져오기 실행
            return;
        }
        if (RecieveTileList[0].ReserveData != null)
        {
            if (!RecieveTileList[0].ReserveData.IsFixed)
            {
                RecieveTileList[0].RequestReserveData(this); // 가져오기 실행
                return;
            }
        }

        // 위로 확인해서 떨어질 블록이 있는지 확인한다.
        // 흐르면 안된다. false => return;
        if (!IsCanFlow_Up) { return; }

        // 아래로 확인해서 빈블록이 있는지 확인한다.
        if (!IsCanFlow_Down) { return; }

        // 아래 확인시 부터 필요한 결과 값 Dict
        mFlowStateDict.Clear();
        // 내 아래쪽으로는 모두 추가
        AddFlowStateDict(true);

        // 아래 타일이 안움직인다는 확인이 필요하다.
        bool bResult = true;
        if (SendTileList[0] != null)
        {
            SendTileList[0].IsCanFlow_Empty(ref bResult);
        }
        if (!bResult) { return; }

        // 좌우로 확인해서 받아올수있는 블록이 있는지 확인한다.
        // 예를 먼저하면 좀 줄 일 수 있을 듯
        Tile flowTargetTile = null;
        for (int idx = 1; idx < RecieveTileList.Count; ++idx)
        {
            var tile = RecieveTileList[idx];
            if (tile == null) { continue; }
            if (tile.ReserveData == null) { continue; }
            if (tile.ReserveData.IsFixed) { continue; }
            if (!tile.IsCanFlow_Down) { continue; }
            flowTargetTile = tile;
        }
        if (flowTargetTile == null) { return; }

        flowTargetTile.RequestReserveData(this); // 가져오기 실행
    }

    // 가져오기
    public override void RequestReserveData(Tile requestTile)
    {
        IsNotReady = true;

        // 경로에 추가
        ReserveData.Enqueue(this);

        // requestTile  > 데이터를 달라고 요청한 쪽
        requestTile.ReserveData = ReserveData;
        ReserveData = null;

        // 넘긴쪽은 다시 체크
        CheckDrop();
    }
    public override void StartDrop()
    {
        if (BlockContainerOrNull == null) { return; }
        if (BlockContainerOrNull.RouteTileQueue.Count == 0) { return; }

        BlockContainerOrNull.StartMovePositionByRoute(this);
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
