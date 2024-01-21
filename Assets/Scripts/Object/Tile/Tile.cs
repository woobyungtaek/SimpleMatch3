using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EDropableState
{
    NeedCheck,
    Possible,
    Impossible
}

public class Tile : MonoBehaviour, System.IDisposable
{
    protected static Dictionary<Vector2, bool> mFlowStateDict = new Dictionary<Vector2, bool>(new Vector2Comparer());

    //public static bool IsNotReady;
    public static bool IsMoveStart;

    [Header("Private")]
    [SerializeField] private Vector2 mCoordi;
    [SerializeField] private Vector2 mGravity;
    [SerializeField] private bool mbCreateTile;
    [SerializeField] private bool mbReservationTile;
    [SerializeField] private bool mbHit;
    [SerializeField] private bool mbHit_Spalsh;
    [SerializeField] private bool mbArrive;

    [Header("Protected")]
    [SerializeField] protected bool mbFlowCheckTile;
    [SerializeField] protected IReserveData mReserveData;
    [SerializeField] protected EDropableState mDropAbleState;

    [Header("BlockContainer")]
    public BlockContainer BlockContainerOrNull;
    protected BlockContainer mStartBlockContainer;

    [Header("Send And Recieve")]
    [SerializeField] protected List<Tile> mSendTileList = new List<Tile>();
    [SerializeField] protected List<Tile> mRecieveTileList = new List<Tile>();

    [Header("BlockMaker")]
    public BlockMaker BlockMakerOrNull;
    protected Coroutine mCreateCoroutine;
    protected Queue<ReserveData> mCreateReserveDataQueue = new Queue<ReserveData>();


    public Vector2 Coordi { get => mCoordi; set => mCoordi = value; }
    public Vector2 Gravity { get => mGravity; set => mGravity = value; }
    public bool IsCreateTile { get => mbCreateTile; set => mbCreateTile = value; }

    public bool IsPossibleFlowDrop
    {
        get
        {
            if (BlockContainerOrNull != null)
            {
                return false;
            }
            if (mSendTileList[0] == null)
            {
                return true;
            }
            if (!IsCanFlow_UpEmpty)
            {
                return false;
            }
            return mSendTileList[0].IsArrive;
        }
    }

    public virtual bool IsCanSend { get => false; }
    public virtual Tile IsCanSend_Flow { get => null; }

    public virtual bool IsCanFlow_Up { get => false; }
    public virtual bool IsCanFlow_Down { get => true; }

    public virtual bool IsFullUnder { get => true; }

    public virtual bool IsCanFlow_UpAtEmpty { get => false; }
    public virtual void IsCanFlow_Empty(ref bool result)
    {
        result &= true;
    }

    public virtual bool IsCanFlow_UpEmpty { get => true; }

    public virtual EDropableState DropableState
    {
        get => mDropAbleState;
        set => mDropAbleState = EDropableState.Impossible;
    }

    public virtual bool IsChecked { get => true; set { } }
    public virtual bool IsReady { get => true; }

    public bool IsArrive { get => mbArrive; set => mbArrive = value; }
    public bool IsHit { get => mbHit; set => mbHit = value; }
    public bool IsSplashHit { get => mbHit_Spalsh; set => mbHit_Spalsh = value; }

    public IReserveData ReserveData
    {
        get
        {
            IReserveData rData = BlockContainerOrNull;
            if (rData == null) { rData = mReserveData; }

            return rData;
        }
        set
        {
            BlockContainerOrNull = value as BlockContainer;
            if (BlockContainerOrNull != null) { return; }

            mReserveData = value;
        }
    }

    public List<Tile> SendTileList { get => mSendTileList; set => mSendTileList = value; }
    public List<Tile> RecieveTileList { get => mRecieveTileList; set => mRecieveTileList = value; }

    public void CreateSendTileListByMapSize(Vector2 mapSize)
    {
        Vector2[] gravityDirArr = new Vector2[3];

        gravityDirArr[0] = mGravity;
        if (mGravity.x == 0)
        {
            if (Coordi.x <= mapSize.x / 2)
            {
                gravityDirArr[1] = mGravity + Vector2.right;
                gravityDirArr[2] = mGravity + Vector2.left;
            }
            else
            {
                gravityDirArr[1] = mGravity + Vector2.left;
                gravityDirArr[2] = mGravity + Vector2.right;
            }
        }
        else
        {
            if (Coordi.y <= mapSize.y / 2)
            {
                gravityDirArr[1] = mGravity + Vector2.down;
                gravityDirArr[2] = mGravity + Vector2.up;
            }
            else
            {
                gravityDirArr[1] = mGravity + Vector2.up;
                gravityDirArr[2] = mGravity + Vector2.down;
            }
        }

        Tile instTile;
        for (int dirIndex = 0; dirIndex < gravityDirArr.Length; dirIndex++)
        {
            instTile = TileMapManager.Instance.GetTileByCoordiOrNull(Coordi + gravityDirArr[dirIndex]);
            if (instTile is NormalTile) { SendTileList.Add(instTile); }
            else { SendTileList.Add(null); }
        }
    }
    public void SetRecieveTileListOfSendTileList()
    {
        int loopCount = mSendTileList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            if (mSendTileList[index] == null) { continue; }

            if (index == 0) { mSendTileList[index].RecieveTileList.Insert(0, this); }
            else { mSendTileList[index].RecieveTileList.Add(this); }

        }
    }
    public void AdjustRecieveTileListByMapSize(Vector2 mapSize)
    {
        if (RecieveTileList.Count < 3) { return; }

        Vector2 checkCoordi;
        Tile tempTile;

        if (mGravity.x == 0)
        {
            if (Coordi.x <= mapSize.x / 2) { checkCoordi = Vector2.left + Coordi; }
            else { checkCoordi = Vector2.right + Coordi; }
        }
        else
        {
            if (Coordi.y <= mapSize.y / 2) { checkCoordi = Vector2.down + Coordi; }
            else { checkCoordi = Vector2.up + Coordi; }
        }

        checkCoordi += (mGravity * -1);
        if (RecieveTileList[1].Coordi == checkCoordi)
        {
            tempTile = RecieveTileList[1];
            RecieveTileList[1] = RecieveTileList[2];
            RecieveTileList[2] = tempTile;
        }
    }

    public void CreateBlockByCreateTileData()
    {
        BlockData blockData = new BlockData();

        // 블록 생성기
        if (BlockMakerOrNull != null)
        {
            // 순서대로 생성이 먼저
            if (!BlockMakerOrNull.IsEnd)
            {
                blockData = BlockMakerOrNull.CreateBlockList[BlockMakerOrNull.CurrentIndex];
                BlockManager.Instance.CreateBlockByBlockDataInTile(this, blockData.BlockType, blockData.BlockColor, blockData.BlockHP, TileMapManager.Instance.TileParentTransform);
                BlockMakerOrNull.CurrentIndex += 1;
                if (BlockMakerOrNull.IsEnd)
                {
                    BlockMakerOrNull = null;
                }
                return;
            }
        }

        // 예약된 블록 확인
        if (BlockMaker.GetReserveBlockData(ref blockData))
        {
            // 예약 데이터가 있다면 예약 데이터를 우선 생성한다.
            // 블록 타입, 색상, hp 필요함
            BlockManager.Instance.CreateBlockByBlockDataInTile(this, blockData.BlockType, blockData.BlockColor, blockData.BlockHP, TileMapManager.Instance.TileParentTransform);
            return;
        }

        BlockManager.Instance.CreateBlockByBlockDataInTile(this, typeof(NormalBlock), Random.Range(0, BlockMaker.MaxColor), 1, TileMapManager.Instance.TileParentTransform);
    }
    public virtual void ResetTileState()
    {
        IsHit = false;
        IsSplashHit = false;
    }

    public void CheckBlockContainer()
    {
        if (BlockContainerOrNull == null) { return; }
        if (BlockContainerOrNull.BlockCount == 0)
        {
            BlockContainerOrNull = null;
        }
    }

    public void RemoveBlockContainer()
    {
        if (BlockContainerOrNull == null) { return; }
        BlockContainerOrNull.RemoveAllBlock();
        GameObjectPool.ReturnObject(BlockContainerOrNull.gameObject);
        BlockContainerOrNull = null;
    }

    public virtual void AddFlowStateDict(bool checkResult) { }
    public virtual void HitTile(bool bExplosionHit) { }
    public virtual void HitTile_Splash() { }
    public virtual void StopPushEffect() { }
    public virtual void StartPushEffect(int pushDegree = 2) { }
    public virtual void SetPushEffect(float pushPower, int order, int pushDegree, Tile callTile) { }

    public virtual void Dispose()
    {
        BlockContainerOrNull = null;

        IsCreateTile = false;
        mbHit = false;
        mbHit_Spalsh = false;

        mSendTileList.Clear();
        mRecieveTileList.Clear();
    }

    public void SaveStartBlockContainer()
    {
        mStartBlockContainer = BlockContainerOrNull;
    }

    #region DropBlockFinal

    // 생성 타일인가? ReserveData가 있는가? yes
    // ReserveData가 고정이 아닌가? yes
    // 아래 타일이 비어있는가? yes
    // 아래로 1칸 옮기기 (true) 반환, 그 외 (false)
    public virtual bool StraightMove() { return false; }

    // 생성 타일인가? ReserveData가 있는가?
    // 좌,우 하단의 타일에 ReserveData가 없는가?
    // 좌, 우 하단의 타일이 흘러내림 가능 타일인가? 
    // 아래이 타일이 고정이 된 상태인가?
    // 대상 타일로 1칸 옮기기 (true) 반환, 그 외 (false)
    public virtual bool FlowingMove() { return false; }

    #endregion


    public virtual bool CheckDropableState() { return false; }
    public virtual bool CheckDropReadyState() { return true; }

    public virtual void SendReserveData(Tile sendTile) { }

    protected IEnumerator CreateBlockCoroutine()
    {
        if (mCreateReserveDataQueue.Count != 0)
        {
            IsMoveStart = true;
        }
        yield return GameConfig.yieldDropDuraion;
        while (true)
        {
            if (mCreateReserveDataQueue.Count == 0) { break; }

            // 데이터 획득
            var data = mCreateReserveDataQueue.Dequeue();

            // 실제 보내야할 Tile 획득
            var targetTile = data.RouteTileQueue.Dequeue();
            while (targetTile.BlockContainerOrNull != null)
            {
                yield return null;
            }

            //var destUnderTile = data.DestTile.SendTileList[0];
            //if (destUnderTile != null)
            //{
            //    while (destUnderTile.IsArrive == false)
            //    {
            //        yield return null;
            //    }
            //}

            // 행동
            CreateBlockByCreateTileData();          // 생성기 위치에 블록컨테이너 생성

            BlockContainerOrNull.ClearQueue();
            BlockContainerOrNull.Enqueue(targetTile);     // 체크를 위해 뺏으므로 복사전 추가
            BlockContainerOrNull.CopyQueue(data);   // 예약 데이터를 실제 블록컨테이너에 복사

            BlockContainerOrNull.StartMovePositionByRoute(); // 이동 시작
            BlockContainerOrNull = null;            // 생성기 위치의 블록컨테이너는 해제
            ObjectPool.ReturnInst(data);

            yield return GameConfig.yieldDropDuraion;
        }

        // 생성이 끝나면 코루틴 해제
        mCreateCoroutine = null;
    }
    public void AddDestTile()
    {
        if (ReserveData != null && ReserveData.RouteTileQueue.Count > 0)
        {
            IsMoveStart = true;
            IsArrive = false;
            //ReserveData.RouteTileQueue.Dequeue(); // 시작지점 빼기
            //ReserveData.Enqueue(this);            // 도착지점 넣기
            ReserveData.DestTile = this;

            //var bc = ReserveData as BlockContainer;
            //if (bc != null) { bc.DestTile = this; }
        }

        BlockContainerOrNull = mStartBlockContainer;
        mStartBlockContainer = null;
        mReserveData = null;
    }

    public virtual void StartDrop() { }


    public virtual void AddTileGimmick(TileGimmick gimmick)
    {
        // 반드시 오버라이드 해서 사용하세요.
    }
    public virtual void RemoveTileGimmick(TileGimmick gimmick)
    {
        // 반드시 오버라이드 해서 사용하세요.
    }
    public virtual TileGimmick IsContainTileGimmick(System.Type type)
    {
        return null;
    }
}
