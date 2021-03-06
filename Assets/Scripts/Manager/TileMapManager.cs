using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.U2D;

public class TileMapManager : SceneSingleton<TileMapManager>
{
    public Transform TileParentTransform { get => mTileParentTransform; }
    public Transform PuzzleBoard { get => mBoardTransform; }
    public Transform MoveCountUI { get => mMoveCountUI; }
    public SpriteRenderer BoardBackGround
    {
        get
        {
            return mBackgroundRenderer;
        }
    }

    public bool IsNextStep { get; set; }
    public int MoveCount
    {
        get => mMoveCount;
        set
        {
            mMoveCount = value;
            ObserverCenter.Instance.SendNotification(Message.RefreshMoveCount);
        }
    }

    public Vector3 RandCollectPos
    {
        get
        {
            Vector3 pos = mCollectTransform.position;

            float randX = Random.Range(-mCollectTransform.lossyScale.x, mCollectTransform.lossyScale.x);
            float randY = Random.Range(-mCollectTransform.lossyScale.y, mCollectTransform.lossyScale.y);

            pos.x += randX / 2f;
            pos.y += randY / 2f;

            return pos;
        }
    }

    private List<List<Tile>> mAllTileList = new List<List<Tile>>();
    private List<Vector2> mCoordiList = new List<Vector2>();
    private List<Tile> mTileList = new List<Tile>();
    private List<NormalTile> mNormalTileList = new List<NormalTile>();

    [Header("StageInfo")]
    [SerializeField] private int mMoveCount;

    [Header("TileMapData")]
    [SerializeField] private RectTransform mBoardTransform;
    [SerializeField] private Transform mMoveCountUI;
    [SerializeField] private Transform mCollectTransform;
    [SerializeField] private Transform mAllPuzzleTransform;
    [SerializeField] private Transform mTileParentTransform;
    [SerializeField] private Vector2 mCellSize;
    [SerializeField] private float mUiCellSize;
    [SerializeField] private float mRatioGap;
    [SerializeField] private SpriteAtlas mEdgeAtlas;
    [SerializeField] private SpriteRenderer mBackgroundRenderer;
    [SerializeField] private Color mEdgeColor;
    [SerializeField] private Color mOddColor;
    [SerializeField] private Color mEvenColor;

    private bool mbBombMerge = false;

    [Header("TilePrefabs")]
    [SerializeField] private Tile normalTilePrefab;
    [SerializeField] private Tile edgeTilePrefab;
    [SerializeField] private Tile blankTilePrefab;
    [SerializeField] private Tile emptyTilePrefab;

    [Header("EffectPrefabs")]
    [SerializeField] private MergeEffect mergeEffectPrefab;

    [Header("MatchPossible")]
    [SerializeField] private List<MatchInfo> mMatchPossibleInfoList = new List<MatchInfo>();

    [Header("Match")]
    [SerializeField] private List<MatchInfo> mMatchInfoList = new List<MatchInfo>();
    private Queue<BombBlock> mExplosionBlockQueue = new Queue<BombBlock>();
    [SerializeField] private List<MatchInfo> mDuplicateMatchInfoList = new List<MatchInfo>();
    [SerializeField] private List<MatchInfo> mSquareMatchInfoList = new List<MatchInfo>();

    private HashSet<Tile> mCheckHashSet = new HashSet<Tile>();
    private List<Tile> remainChainList = new List<Tile>();
    private List<Tile> hitTileList = new List<Tile>();
    private List<Tile> mSquareTileList = new List<Tile>();

    //타일 리스트 : 매치 체크리스트
    private List<ReuseTileList> mChainListList = new List<ReuseTileList>();
    private List<ReuseTileList> mAllVerticalListList = new List<ReuseTileList>();
    private List<ReuseTileList> mAllHorizontalListList = new List<ReuseTileList>();
    private List<ReuseHomingOrderTileList> mHomingOrderList = new List<ReuseHomingOrderTileList>();

    private Dictionary<System.Type, Dictionary<System.Type, System.Type>> mExplosionMergeConditionDict = new Dictionary<System.Type, Dictionary<System.Type, System.Type>>();

    ///콤보 : 매치 성공 카운트
    [SerializeField] private int TestComboCount;

    private void Awake()
    {
        MatchInfo.CreateMatchTypeChangeConditionDict();
        CreateExplosionMergeConditionInternal();

        ObserverCenter observerCenter = ObserverCenter.Instance;
        observerCenter.AddObserver(ExecuteTileReadyCheckByNoti, EGameState.TileReadyCheck.ToString());
        observerCenter.AddObserver(ExecuteDropBlockByNoti, EGameState.Drop.ToString());
        observerCenter.AddObserver(ExecuteMatchCheckByNoti, EGameState.MatchCheck.ToString());
        observerCenter.AddObserver(ExcuteMatchByNoti, EGameState.Match.ToString());
        observerCenter.AddObserver(ExcuteResultCheckByNoti, EGameState.ResultCheck.ToString());
    }
    private void CreateExplosionMergeConditionInternal()
    {
        mExplosionMergeConditionDict.Clear();
        mExplosionMergeConditionDict.Add(typeof(VerticalBombBlock), new Dictionary<System.Type, System.Type>());
        mExplosionMergeConditionDict.Add(typeof(HorizontalBombBlock), new Dictionary<System.Type, System.Type>());
        mExplosionMergeConditionDict.Add(typeof(AroundBombBlock), new Dictionary<System.Type, System.Type>());
        mExplosionMergeConditionDict.Add(typeof(HomingBombBlock), new Dictionary<System.Type, System.Type>());
        mExplosionMergeConditionDict.Add(typeof(CrossBombBlock), new Dictionary<System.Type, System.Type>());
        mExplosionMergeConditionDict.Add(typeof(ColorBombBlock), new Dictionary<System.Type, System.Type>());

        mExplosionMergeConditionDict[typeof(VerticalBombBlock)].Add(typeof(VerticalBombBlock), typeof(CrossBombBlock));
        mExplosionMergeConditionDict[typeof(VerticalBombBlock)].Add(typeof(HorizontalBombBlock), typeof(CrossBombBlock));
        mExplosionMergeConditionDict[typeof(VerticalBombBlock)].Add(typeof(AroundBombBlock), typeof(BigVerticalBombBlock));
        mExplosionMergeConditionDict[typeof(VerticalBombBlock)].Add(typeof(HomingBombBlock), typeof(HomingChangeBombBlock));
        mExplosionMergeConditionDict[typeof(VerticalBombBlock)].Add(typeof(ColorBombBlock), typeof(ColorChangeBombBlock));
        //mExplosionMergeConditionDict[typeof(VerticalBombBlock)].Add(typeof(CrossBombBlock), typeof(BigCrossBombBlock));

        mExplosionMergeConditionDict[typeof(HorizontalBombBlock)].Add(typeof(HorizontalBombBlock), typeof(CrossBombBlock));
        mExplosionMergeConditionDict[typeof(HorizontalBombBlock)].Add(typeof(VerticalBombBlock), typeof(CrossBombBlock));
        mExplosionMergeConditionDict[typeof(HorizontalBombBlock)].Add(typeof(AroundBombBlock), typeof(BigHorizontalBombBlock));
        mExplosionMergeConditionDict[typeof(HorizontalBombBlock)].Add(typeof(HomingBombBlock), typeof(HomingChangeBombBlock));
        mExplosionMergeConditionDict[typeof(HorizontalBombBlock)].Add(typeof(ColorBombBlock), typeof(ColorChangeBombBlock));
        //mExplosionMergeConditionDict[typeof(HorizontalBombBlock)].Add(typeof(CrossBombBlock), typeof(BigCrossBombBlock));

        mExplosionMergeConditionDict[typeof(AroundBombBlock)].Add(typeof(VerticalBombBlock), typeof(BigVerticalBombBlock));
        mExplosionMergeConditionDict[typeof(AroundBombBlock)].Add(typeof(HorizontalBombBlock), typeof(BigHorizontalBombBlock));
        mExplosionMergeConditionDict[typeof(AroundBombBlock)].Add(typeof(AroundBombBlock), typeof(BigAroundBombBlock));
        mExplosionMergeConditionDict[typeof(AroundBombBlock)].Add(typeof(HomingBombBlock), typeof(HomingChangeBombBlock));
        mExplosionMergeConditionDict[typeof(AroundBombBlock)].Add(typeof(ColorBombBlock), typeof(ColorChangeBombBlock));
        //mExplosionMergeConditionDict[typeof(AroundBombBlock)].Add(typeof(CrossBombBlock), typeof(BigCrossBombBlock));

        //mExplosionMergeConditionDict[typeof(CrossBombBlock)].Add(typeof(HorizontalBombBlock), typeof(BigCrossBombBlock));
        //mExplosionMergeConditionDict[typeof(CrossBombBlock)].Add(typeof(VerticalBombBlock), typeof(BigCrossBombBlock));
        //mExplosionMergeConditionDict[typeof(CrossBombBlock)].Add(typeof(AroundBombBlock), typeof(BigCrossBombBlock));
        //mExplosionMergeConditionDict[typeof(CrossBombBlock)].Add(typeof(HomingBombBlock), typeof(HomingChangeBombBlock));
        //mExplosionMergeConditionDict[typeof(CrossBombBlock)].Add(typeof(ColorBombBlock), typeof(ColorChangeBombBlock));
        //mExplosionMergeConditionDict[typeof(CrossBombBlock)].Add(typeof(CrossBombBlock), typeof(BigCrossBombBlock));

        mExplosionMergeConditionDict[typeof(HomingBombBlock)].Add(typeof(VerticalBombBlock), typeof(HomingChangeBombBlock));
        mExplosionMergeConditionDict[typeof(HomingBombBlock)].Add(typeof(HorizontalBombBlock), typeof(HomingChangeBombBlock));
        mExplosionMergeConditionDict[typeof(HomingBombBlock)].Add(typeof(AroundBombBlock), typeof(HomingChangeBombBlock));
        mExplosionMergeConditionDict[typeof(HomingBombBlock)].Add(typeof(HomingBombBlock), typeof(TripleHomingBombBlock));
        mExplosionMergeConditionDict[typeof(HomingBombBlock)].Add(typeof(ColorBombBlock), typeof(ColorChangeBombBlock));
        //mExplosionMergeConditionDict[typeof(HomingBombBlock)].Add(typeof(CrossBombBlock), typeof(HomingChangeBombBlock));

        mExplosionMergeConditionDict[typeof(ColorBombBlock)].Add(typeof(VerticalBombBlock), typeof(ColorChangeBombBlock));
        mExplosionMergeConditionDict[typeof(ColorBombBlock)].Add(typeof(HorizontalBombBlock), typeof(ColorChangeBombBlock));
        mExplosionMergeConditionDict[typeof(ColorBombBlock)].Add(typeof(AroundBombBlock), typeof(ColorChangeBombBlock));
        mExplosionMergeConditionDict[typeof(ColorBombBlock)].Add(typeof(HomingBombBlock), typeof(ColorChangeBombBlock));
        mExplosionMergeConditionDict[typeof(ColorBombBlock)].Add(typeof(ColorBombBlock), typeof(AllClearBombBlock));
        //mExplosionMergeConditionDict[typeof(ColorBombBlock)].Add(typeof(CrossBombBlock), typeof(ColorChangeBombBlock));
    }

    /*상태변화시 실행*/
    private void ExecuteTileReadyCheckByNoti(Notification noti)
    {
        bool bAllReady = true;

        PuzzleInputManager.SelectTileOrNull = null;
        PuzzleInputManager.TargetTileOrNull = null;

        int loopCount = mTileList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            mTileList[index].ResetTileState();
        }
        for (int index = 0; index < loopCount; index++)
        {
            mTileList[index].CheckAvailableSendTile();
        }
        for (int index = 0; index < loopCount; index++)
        {
            if (mTileList[index].SendTileOrNull == null) { continue; }
            bAllReady = false;
            break;
        }

        if (bAllReady == false)
        {
            PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.Drop);
        }
        else
        {
            PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.MatchCheck);
        }
    }
    private void ExecuteDropBlockByNoti(Notification noti)
    {
        StartCoroutine(DropBlockContainerCoroutine());
    }
    private void ExecuteMatchCheckByNoti(Notification noti)
    {
        StartCoroutine(MatchCheckCoroutine());
    }
    private void ExcuteMatchByNoti(Notification noti)
    {
        StartCoroutine(MatchCoroutine());
    }
    private void ExcuteResultCheckByNoti(Notification noti)
    {
        StartCoroutine(ResultCheckCoroutine());
    }

    private void CheckLineMatchInternal(List<MatchInfo> matchInfoList, List<ReuseTileList> lineListList, EMatchType matchType , int matchID)
    {
        int blockNumber = -1;
        int loopCount;
        int secLoopCount;

        bool bNewMerge = false;
        EMatchType instMatchType = matchType;
        MatchInfo duplicateInfo = null;

        loopCount = lineListList.Count;
        for (int lineIndex = 0; lineIndex < loopCount; lineIndex++)
        {
            bNewMerge = true;
            blockNumber = lineListList[lineIndex].tileList[0].BlockContainerOrNull.BlockContainerNumber;
            if (lineListList[lineIndex].tileList.Count >= 5) { instMatchType = EMatchType.FiveLine; }

            secLoopCount = matchInfoList.Count;
            for (int infoIndex = 0; infoIndex < secLoopCount; infoIndex++)
            {
                if (matchInfoList[infoIndex].ID != matchID) { continue; }
                if (!matchInfoList[infoIndex].IsOverlaps(lineListList[lineIndex].tileList)) { continue; }

                matchInfoList[infoIndex].AddMatchInfoByTypeAndList(instMatchType, lineListList[lineIndex].tileList);
                bNewMerge = false;
                duplicateInfo = matchInfoList[infoIndex];
                break;
            }
            if (!bNewMerge)
            {
                mDuplicateMatchInfoList.Clear();
                secLoopCount = matchInfoList.Count;
                for (int infoIndex = 0; infoIndex < secLoopCount; infoIndex++)
                {
                    if (matchInfoList[infoIndex] == duplicateInfo) { continue; }
                    if (!matchInfoList[infoIndex].IsOverlaps(duplicateInfo.MatchTileList)) { continue; }
                    mDuplicateMatchInfoList.Add(matchInfoList[infoIndex]);
                }
                if (mDuplicateMatchInfoList.Count > 0)
                {
                    secLoopCount = mDuplicateMatchInfoList.Count;
                    for (int index = 0; index < secLoopCount; index++)
                    {
                        duplicateInfo.AddMatchInfoByTypeAndList(mDuplicateMatchInfoList[index].MatchType, mDuplicateMatchInfoList[index].MatchTileList);
                    }
                    for (int index = 0; index < secLoopCount; index++)
                    {
                        MatchInfo.Destroy(mDuplicateMatchInfoList[index]);
                        matchInfoList.Remove(mDuplicateMatchInfoList[index]);
                    }
                    mDuplicateMatchInfoList.Clear();
                }
                continue;
            }

            matchInfoList.Add(MatchInfo.Instantiate());
            matchInfoList[matchInfoList.Count - 1].SetMatchInfoByData(blockNumber, instMatchType, lineListList[lineIndex].tileList);
            matchInfoList[matchInfoList.Count - 1].ID = matchID;
            instMatchType = matchType;
        }
    }
    private void CheckSquareMatchInternal()
    {
        int loopCount;
        int secLoopCount;
        int thirdLoopCount;

        bool bSquareCheck = true;
        bool bSquareMatch = false;
        Tile tile;
        mSquareMatchInfoList.Clear();

        loopCount = mChainListList.Count;
        for (int chainIndex = 0; chainIndex < loopCount; chainIndex++)
        {
            if (mChainListList[chainIndex].tileList.Count <= 3) { continue; }

            secLoopCount = mChainListList[chainIndex].tileList.Count;
            for (int index = 0; index < secLoopCount; index++)
            {
                tile = mChainListList[chainIndex].tileList[index];
                if (!(tile is NormalTile)) { continue; }

                bSquareCheck = true;
                bSquareMatch = true;
                mSquareMatchInfoList.Clear();

                mSquareTileList.Clear();
                CreateTileListIncludeNullByAreaList(mSquareTileList, tile.Coordi, TileArea.square);

                thirdLoopCount = mSquareTileList.Count;
                for (int squareIndex = 0; squareIndex < thirdLoopCount; squareIndex++)
                {
                    if (!(mSquareTileList[squareIndex] is NormalTile)) { bSquareCheck = false; break; }
                    if (mSquareTileList[squareIndex].BlockContainerOrNull == null) { bSquareCheck = false; break; }
                    if (tile.BlockContainerOrNull.BlockContainerNumber != mSquareTileList[squareIndex].BlockContainerOrNull.BlockContainerNumber) { bSquareCheck = false; break; }
                }

                if (bSquareCheck == false) { continue; }
                mSquareTileList.Add(tile);

                thirdLoopCount = mMatchInfoList.Count;
                for (int infoIndex = 0; infoIndex < thirdLoopCount; infoIndex++)
                {
                    if (!mMatchInfoList[infoIndex].IsOverlaps(mSquareTileList)) { continue; }
                    mSquareMatchInfoList.Add(mMatchInfoList[infoIndex]);
                }

                thirdLoopCount = mSquareMatchInfoList.Count;
                for (int infoIndex = 0; infoIndex < thirdLoopCount; infoIndex++)
                {
                    if (mSquareMatchInfoList[infoIndex].MatchType > EMatchType.VLine)
                    {
                        bSquareMatch = false;
                        break;
                    }
                    if (mSquareMatchInfoList[infoIndex].MatchTileList.Count >= 4)
                    {
                        bSquareMatch = false;
                        break;
                    }
                }

                if (bSquareMatch == false) { continue; }

                MatchInfo addMatchInfo = MatchInfo.Instantiate();
                addMatchInfo.MatchNumber = tile.BlockContainerOrNull.BlockContainerNumber;

                thirdLoopCount = mSquareMatchInfoList.Count;
                for (int infoIndex = 0; infoIndex < thirdLoopCount; infoIndex++)
                {
                    addMatchInfo.AddMatchInfoByTypeAndList(EMatchType.noType, mSquareMatchInfoList[infoIndex].MatchTileList);
                    mMatchInfoList.Remove(mSquareMatchInfoList[infoIndex]);
                    MatchInfo.Destroy(mSquareMatchInfoList[infoIndex]);
                }
                addMatchInfo.AddMatchInfoByTypeAndList(EMatchType.Square, mSquareTileList);

                mMatchInfoList.Add(addMatchInfo);
            }
        }
        mSquareMatchInfoList.Clear();
    }

    private IEnumerator DropBlockContainerCoroutine()
    {
        int loopCount = mTileList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            if (mTileList[index].SendTileOrNull == null) { continue; }
            if (mTileList[index].SendTileOrNull.IsDroping) { continue; }
            mTileList[index].SendTileOrNull.IsDroping = true;

            if (mTileList[index].BlockContainerOrNull == null)
            {
                if (mTileList[index].IsCreateTile)
                {
                    mTileList[index].CreateBlockByCreateTileData();
                }
            }
            mTileList[index].BlockContainerOrNull.StartMovePositionByTile(mTileList[index].SendTileOrNull, GameConfig.DROP_DURATION);
            mTileList[index].BlockContainerOrNull = null;
        }
        yield return GameConfig.yieldDropDuraion;

        //대기 시간 보다 1프레임 더 기다린다.
#if UNITY_EDITOR || UNITY_STANDRAD
        yield return null;
#endif

        PuzzleManager.Instance.ChangeCurrentGameState(EGameState.DropEnd);
        PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.TileReadyCheck);
    }
    private IEnumerator MatchCheckCoroutine()
    {
        mbBombMerge = false;

        if (PuzzleInputManager.SelectTileOrNull != null && PuzzleInputManager.TargetTileOrNull != null)
        {
            Block selectBlock = PuzzleInputManager.SelectTileOrNull.BlockContainerOrNull.MainBlock;
            Block targetBlock = PuzzleInputManager.TargetTileOrNull.BlockContainerOrNull.MainBlock;

            if (selectBlock is ColorBombBlock && targetBlock is NormalBlock)
            {
                selectBlock.BlockNumber = targetBlock.BlockNumber;
                PuzzleInputManager.TargetTileOrNull.HitTile(false);
                PuzzleInputManager.SelectTileOrNull.HitTile(false);
                mbBombMerge = true;
            }
            else if (selectBlock is NormalBlock && targetBlock is ColorBombBlock)
            {
                targetBlock.BlockNumber = selectBlock.BlockNumber;
                PuzzleInputManager.SelectTileOrNull.HitTile(false);
                PuzzleInputManager.TargetTileOrNull.HitTile(false);
                mbBombMerge = true;
            }
            else if (selectBlock is BombBlock && targetBlock is BombBlock)
            {
                System.Type resultTypeOrNull = GetExplosionBlockTypeOrNullByMergeBlock(selectBlock.GetType(), targetBlock.GetType());
                if (resultTypeOrNull != null)
                {
                    selectBlock.CheckMissionBlock();
                    targetBlock.CheckMissionBlock();

                    PuzzleInputManager.SelectTileOrNull.BlockContainerOrNull.RemoveBlockByBlock(selectBlock);
                    PuzzleInputManager.TargetTileOrNull.BlockContainerOrNull.RemoveBlockByBlock(targetBlock);

                    ChangeBombBlock instBlockOrNull = BlockManager.Instance.GetCreateBlockByBlockDataInTile(PuzzleInputManager.TargetTileOrNull, resultTypeOrNull, targetBlock.BlockNumber, 1) as ChangeBombBlock;

                    if (instBlockOrNull != null)
                    {
                        int blockNumber = -1;
                        System.Type changeType = null;
                        if (selectBlock is HomingBombBlock || selectBlock is ColorBombBlock)
                        {
                            if (targetBlock is ColorBombBlock)
                            {
                                blockNumber = selectBlock.BlockNumber;
                                changeType = selectBlock.GetType();
                            }
                            else
                            {
                                blockNumber = targetBlock.BlockNumber;
                                changeType = targetBlock.GetType();
                            }
                        }
                        else
                        {
                            if (selectBlock is ColorBombBlock)
                            {
                                blockNumber = targetBlock.BlockNumber;
                                changeType = targetBlock.GetType();
                            }
                            else
                            {
                                blockNumber = selectBlock.BlockNumber;
                                changeType = selectBlock.GetType();
                            }
                        }
                        instBlockOrNull.ChangeType = changeType;
                        instBlockOrNull.SetBlockData(blockNumber, 1);
                    }

                    PuzzleInputManager.SelectTileOrNull.HitTile(false);
                    PuzzleInputManager.TargetTileOrNull.HitTile(false);

                    mbBombMerge = true;
                }
            }
        }

        if (!mbBombMerge)
        {
            int loopCount;
            int secLoopCount;

            mCheckHashSet.Clear();
            mChainListList.Clear();

            //매치 체크 : 노멀 타일 연결리스트 생성
            loopCount = mNormalTileList.Count;
            for (int index = 0; index < loopCount; index++)
            {
                if (mCheckHashSet.Contains(mNormalTileList[index])) { continue; }
                if (mNormalTileList[index].BlockContainerOrNull == null) { continue; }
                if (mNormalTileList[index].BlockContainerOrNull.BlockContainerNumber < 0) { continue; }

                mChainListList.Add(ReuseTileList.Instantiate());

                mNormalTileList[index].CheckChainTileReculsive(mNormalTileList[index].BlockContainerOrNull.BlockContainerNumber, mCheckHashSet, mChainListList[mChainListList.Count - 1].tileList);
            }


            //매치 체크 : 연결리스트 개수 확인
            loopCount = mChainListList.Count - 1;
            for (int index = loopCount; index >= 0; index--)
            {
                if (mChainListList[index].tileList.Count <= 2)
                {
                    ReuseTileList.Destroy(mChainListList[index]);
                    mChainListList.RemoveAt(index);
                }
            }

            //매치 체크 : 가로, 세로 연결 확인
            loopCount = mChainListList.Count;
            for (int chainIndex = 0; chainIndex < loopCount; chainIndex++)
            {
                remainChainList.Clear();

                //매치 체크 : 체크전 List 생성
                secLoopCount = mChainListList[chainIndex].tileList.Count;
                for (int remainIndex = 0; remainIndex < secLoopCount; remainIndex++)
                {
                    remainChainList.Add(mChainListList[chainIndex].tileList[remainIndex]);
                }

                //매치 체크 : 모든 연결 타일을 확인
                while (remainChainList.Count > 0)
                {
                    Tile checkTile = remainChainList[0];
                    Tile standardTile = checkTile;
                    remainChainList.Remove(checkTile);

                    ReuseTileList verticalList = ReuseTileList.Instantiate();
                    ReuseTileList horizontalList = ReuseTileList.Instantiate();

                    for (int dir = 0; dir < 4;)
                    {
                        checkTile = (checkTile as NormalTile).GetSameNumberTileOrNullByDir(dir);
                        if (checkTile == null)
                        {
                            checkTile = standardTile;
                            dir += 1;
                        }
                        else
                        {
                            if (dir % 2 == 0) { verticalList.tileList.Add(checkTile); }
                            else if (dir % 2 == 1) { horizontalList.tileList.Add(checkTile); }
                            remainChainList.Remove(checkTile);
                        }
                    }

                    if (!verticalList.tileList.Contains(standardTile)) { verticalList.tileList.Add(standardTile); }
                    if (!horizontalList.tileList.Contains(standardTile)) { horizontalList.tileList.Add(standardTile); }

                    if (verticalList.tileList.Count >= 3)
                    {
                        mAllVerticalListList.Add(verticalList);
                    }
                    else { ReuseTileList.Destroy(verticalList); }

                    if (horizontalList.tileList.Count >= 3)
                    {
                        mAllHorizontalListList.Add(horizontalList);
                    }
                    else { ReuseTileList.Destroy(horizontalList); }
                }
            }

            CheckLineMatchInternal(mMatchInfoList ,mAllHorizontalListList, EMatchType.HLine, -1);
            CheckLineMatchInternal(mMatchInfoList, mAllVerticalListList, EMatchType.VLine, -1);
            CheckSquareMatchInternal();

            loopCount = mChainListList.Count;
            for (int index = 0; index < loopCount; index++)
            {
                ReuseTileList.Destroy(mChainListList[index]);
            }
            mChainListList.Clear();

            loopCount = mAllHorizontalListList.Count;
            for (int index = 0; index < loopCount; index++)
            {
                ReuseTileList.Destroy(mAllHorizontalListList[index]);
            }
            mAllHorizontalListList.Clear();

            loopCount = mAllVerticalListList.Count;
            for (int index = 0; index < loopCount; index++)
            {
                ReuseTileList.Destroy(mAllVerticalListList[index]);
            }
            mAllVerticalListList.Clear();
        }
        if (!mbBombMerge && mMatchInfoList.Count == 0)
        {
            PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.ResultCheck);
            yield break;
        }

        if (PuzzleInputManager.SelectTileOrNull != null && PuzzleInputManager.TargetTileOrNull != null)
        {
            MoveCount -= 1;
        }

        PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.Match);
    }
    private IEnumerator MatchCoroutine()
    {
        int loopCount;
        int secLoopCount;
        bool bMerge;

        BlockEffect mergeEffect;
        Vector3 startPos;
        Vector3 targetPos;
        Sprite blockSprite;
        BombBlock bombBlock;
        WaitForSeconds yieldExplosionEffectDuration;

        if (!mbBombMerge)
        {
            //mExplosionTileListList.Clear();

            #region StepCode
            //IsNextStep = false;
            #endregion

            loopCount = mMatchInfoList.Count;
            for (int index = 0; index < loopCount; index++)
            {
                #region StepCode
                //while (!IsNextStep)
                //{
                //    yield return null;
                //}
                //IsNextStep = false;
                #endregion

                bMerge = false;

                if (mMatchInfoList[index].MergePosTileOrNull != null) { bMerge = true; }

                hitTileList = mMatchInfoList[index].MatchTileList;
                secLoopCount = hitTileList.Count;

                for (int matchTileIndex = 0; matchTileIndex < secLoopCount; matchTileIndex++)
                {
                    if (bMerge == true)
                    {
                        mergeEffect = GameObjectPool.Instantiate<MergeEffect>(mergeEffectPrefab.gameObject);
                        startPos = hitTileList[matchTileIndex].transform.position;
                        targetPos = mMatchInfoList[index].MergePosTileOrNull.transform.position;
                        blockSprite = hitTileList[matchTileIndex].BlockContainerOrNull.BlockSprite;

                        mergeEffect.SetEffectDataByData(startPos, targetPos, blockSprite);
                        mergeEffect.PlayEffect();
                    }
                    hitTileList[matchTileIndex].HitTile(false);
                }
            }
            yield return GameConfig.yieldMergeDuration;
            yield return null;

            loopCount = mMatchInfoList.Count;
            for (int index = 0; index < loopCount; index++)
            {
                if (mMatchInfoList[index].MergePosTileOrNull != null)
                {
                    BlockManager.Instance.CreateBlockByBlockDataInTile
                        (mMatchInfoList[index].MergePosTileOrNull, mMatchInfoList[index].MergeBlockTypeOrNull, mMatchInfoList[index].MatchNumber, 1, TileParentTransform);
                }
                MatchInfo.Destroy(mMatchInfoList[index]);
            }
            mMatchInfoList.Clear();
        }

        #region StepCode
        //IsNextStep = false;
        #endregion

        while (mExplosionBlockQueue.Count > 0)
        {
            #region StepCode
            //while (!IsNextStep)
            //{
            //    yield return null;
            //}
            //IsNextStep = false;
            #endregion

            bombBlock = mExplosionBlockQueue.Dequeue();
            if (bombBlock == null) { continue; }
            bombBlock.ExplosionBombBlock();
            if (bombBlock.ExplosionEffect != null)
            {
                yieldExplosionEffectDuration = bombBlock.ExplosionEffect.YieldEffectDuration;
                yield return yieldExplosionEffectDuration;
            }
            yield return null;
        }

        #region StepCode
        //while (!IsNextStep)
        //{
        //    yield return null;
        //}
        //IsNextStep = false;
        #endregion

        TestComboCount++;

        PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.TileReadyCheck);
    }
    private IEnumerator ResultCheckCoroutine()
    {
        TestComboCount = 0;
        if (MissionManager.Instance.IsMissionClear)
        {
            //콜레팅 이펙트가 끝날때 까지 대기해야함, 간단하게 Max시간만큼 대기
            yield return MissionCollectEffect.MaxDuration;
            PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.StageSuccess);
            yield break;
        }
        if (MoveCount <= 0)
        {
            yield return GameConfig.yieldGameOverDuration;//임의의로 
            PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.StageFail);
            yield break;
        }

        PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.ReturnSwap);
    }

    public void MatchPossibleCheck()
    {
        NormalTile instTile;
        ReuseTileList checkTileList = ReuseTileList.Instantiate();

        int matchID = 0;

        int loopCount = mMatchPossibleInfoList.Count;
        for (int index = 0; index < mMatchPossibleInfoList.Count; index++)
        {
            MatchInfo.Destroy(mMatchPossibleInfoList[index]);
        }
        mMatchPossibleInfoList.Clear();

        loopCount = mNormalTileList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            if (mNormalTileList[index].BlockContainerOrNull == null) { continue; }
            if (!mNormalTileList[index].BlockContainerOrNull.IsMove) { continue; }
            if (checkTileList.tileList.Contains(mNormalTileList[index])) { continue; }
            checkTileList.tileList.Add(mNormalTileList[index]);

            int aroundCount = 4;
            for (int dir = 0; dir < aroundCount; dir++)
            {
                instTile = mNormalTileList[index].GetAroundTileByDir(dir) as NormalTile;
                if (instTile == null) { continue; }
                if (instTile.BlockContainerOrNull == null) { continue; }
                if (!instTile.BlockContainerOrNull.IsMove) { continue; }

                if(mNormalTileList[index].BlockContainerOrNull.MainBlock is BombBlock && instTile.BlockContainerOrNull.MainBlock is BombBlock)
                {
                    //추가, 폭탄 합성의 경우(합성표에서 확인 까지 해야함)
                    continue;
                } 

                //블록 스왑 : 체크용 교체
                BlockContainer temp = instTile.BlockContainerOrNull;
                instTile.BlockContainerOrNull = mNormalTileList[index].BlockContainerOrNull;
                mNormalTileList[index].BlockContainerOrNull = temp;

                //매치 체크
                MatchCheckByTile(instTile, ref matchID);
                matchID += 1;

                //블록 스왑 : 원상 복귀
                temp = instTile.BlockContainerOrNull;
                instTile.BlockContainerOrNull = mNormalTileList[index].BlockContainerOrNull;
                mNormalTileList[index].BlockContainerOrNull = temp;
            }
        }
        
    }
    private void MatchCheckByTile(NormalTile checkTile, ref int matchID)
    {
        mCheckHashSet.Clear();
        ReuseTileList chainList = ReuseTileList.Instantiate();
        checkTile.CheckChainTileReculsive(checkTile.BlockContainerOrNull.BlockContainerNumber, mCheckHashSet, chainList.tileList);

        if (chainList.tileList.Count <= 2)
        {
            ReuseTileList.Destroy(chainList);
            return;
        }

        ReuseTileList remainList = ReuseTileList.Instantiate();

        int loopCount = chainList.tileList.Count;
        for(int index =0; index < loopCount; index++)
        {
            remainList.tileList.Add(chainList.tileList[index]);
        }

        List<ReuseTileList> allVerticalList = new List<ReuseTileList>();
        List<ReuseTileList> allHorizontalList = new List<ReuseTileList>();

        while (remainList.tileList.Count > 0)
        {
            Tile instTile = remainList.tileList[0];
            Tile standardTile = instTile;
            remainList.tileList.Remove(instTile);

            ReuseTileList verticalList = ReuseTileList.Instantiate();
            ReuseTileList horizontalList = ReuseTileList.Instantiate();

            for (int dir = 0; dir < 4;)
            {
                instTile = (instTile as NormalTile).GetSameNumberTileOrNullByDir(dir);
                if (instTile == null)
                {
                    instTile = standardTile;
                    dir += 1;
                }
                else
                {
                    if (dir % 2 == 0)
                    {
                        verticalList.tileList.Add(instTile); 
                    }
                    else if (dir % 2 == 1) 
                    { 
                        horizontalList.tileList.Add(instTile); 
                    }

                    remainList.tileList.Remove(instTile);
                }
            }

            if (!verticalList.tileList.Contains(standardTile)) { verticalList.tileList.Add(standardTile); }
            if (!horizontalList.tileList.Contains(standardTile)) { horizontalList.tileList.Add(standardTile); }


            if (verticalList.tileList.Count >= 3)
            {
                allVerticalList.Add(verticalList);
            }
            else { ReuseTileList.Destroy(verticalList); }

            if (horizontalList.tileList.Count >= 3)
            {
                allHorizontalList.Add(horizontalList);
            }
            else { ReuseTileList.Destroy(horizontalList); }
        }

        CheckLineMatchInternal(mMatchPossibleInfoList, allHorizontalList, EMatchType.HLine, matchID);
        CheckLineMatchInternal(mMatchPossibleInfoList, allVerticalList, EMatchType.VLine, matchID);
        ReuseTileList.Destroy(chainList);

        loopCount = allVerticalList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            ReuseTileList.Destroy(allVerticalList[index]);
        }
        allVerticalList.Clear();

        loopCount = allHorizontalList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            ReuseTileList.Destroy(allHorizontalList[index]);
        }
        allHorizontalList.Clear();

    }

    public void AddExplosionBlockQueueByBlock(BombBlock exBlock)
    {
        mExplosionBlockQueue.Enqueue(exBlock);
    }

    /*반환*/
    private System.Type GetExplosionBlockTypeOrNullByMergeBlock(System.Type selectType, System.Type targetType)
    {
        if (!mExplosionMergeConditionDict.ContainsKey(selectType)) { return null; }
        if (!mExplosionMergeConditionDict[selectType].ContainsKey(targetType)) { return null; }
        return mExplosionMergeConditionDict[selectType][targetType];
    }

    public Tile GetTileByCoordiOrNull(Vector2 coordi)
    {
        int yCount = mAllTileList.Count;
        int xCount = 0;

        if (!(coordi.y >= 0 && coordi.y < yCount)) { return null; }
        if (coordi.x < 0) { return null; }

        for (int yIndex = 0; yIndex < yCount; yIndex++)
        {
            xCount = mAllTileList[yIndex].Count;
            if (coordi.x >= xCount) { return null; }
            for (int xIndex = 0; xIndex < xCount; xIndex++)
            {
                if (mAllTileList[yIndex][xIndex].Coordi == coordi) { return mAllTileList[yIndex][xIndex]; }
            }
        }
        return null;
    }
    public Tile GetRandomNormalTileOrNull()
    {
        if (mNormalTileList.Count == 0) { return null; }
        int randIndex = Random.Range(0, mNormalTileList.Count);
        return mNormalTileList[randIndex];
    }
    public Tile GetRandomNormalTileByOrderOrNull()
    {
        //노멀 타일 중에 노멀 블록 또는 블록이 없는 타일을 우선 적으로 선택한다.
        ReuseTileList reuseTileList = ReuseTileList.Instantiate();
        if (mNormalTileList.Count == 0) { return null; }
        int loopCount = mNormalTileList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            if (mNormalTileList[index].BlockContainerOrNull == null)
            {
                reuseTileList.tileList.Add(mNormalTileList[index]);
                continue;
            }
            if (mNormalTileList[index].BlockContainerOrNull.IsOnlyNormalBlock)
            {
                reuseTileList.tileList.Add(mNormalTileList[index]);
                continue;
            }
        }
        if (reuseTileList.tileList.Count <= 0) { return null; }
        int randIndex = Random.Range(0, reuseTileList.tileList.Count);

        Tile resultTile = reuseTileList.tileList[randIndex];
        ReuseTileList.Destroy(reuseTileList);
        return resultTile;
    }
    public void CreateTileListByAllClearArea(List<Tile> resultList)
    {
        int loopCount = mNormalTileList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            if (mNormalTileList[index].BlockContainerOrNull == null) { continue; }
            resultList.Add(mNormalTileList[index]);
        }
    }
    public void CreateTileListBySameNumber(List<Tile> resultList, int blockNumber)
    {
        int loopCount;
        resultList.Clear();

        loopCount = mNormalTileList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            if (mNormalTileList[index].BlockContainerOrNull == null) { continue; }
            if (mNormalTileList[index].BlockContainerOrNull.BlockContainerNumber != blockNumber) { continue; }
            resultList.Add(mNormalTileList[index]);
        }
    }
    public void CreateTileListByHomingOrder(List<Tile> resultList, int targetCount)
    {
        int loopCount;
        int secLoopCount;
        int rndIndex = 0;
        ReuseHomingOrderTileList instList;
        resultList.Clear();
        mHomingOrderList.Clear();

        loopCount = mNormalTileList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            if (mNormalTileList[index].IsHit) { continue; }
            if (mNormalTileList[index].BlockContainerOrNull == null) { continue; }

            instList = null;

            secLoopCount = mHomingOrderList.Count;
            for (int orderIndex = 0; orderIndex < secLoopCount; orderIndex++)
            {
                if (mHomingOrderList[orderIndex].HomingOrder == mNormalTileList[index].BlockContainerOrNull.HomingOrder)
                {
                    instList = mHomingOrderList[orderIndex];
                }
            }

            if (instList == null)
            {
                instList = ReuseHomingOrderTileList.Instantiate(); ;
                mHomingOrderList.Add(instList);
            }
            instList.HomingOrder = mNormalTileList[index].BlockContainerOrNull.HomingOrder;
            instList.tileList.Add(mNormalTileList[index]);
        }
        mHomingOrderList.Sort(SortHomingOrder);

        loopCount = mHomingOrderList.Count;
        for (int listIndex = 0; listIndex < loopCount;)
        {
            if (mHomingOrderList[listIndex].tileList.Count == 0) { listIndex++; }
            rndIndex = Random.Range(0, mHomingOrderList[listIndex].tileList.Count);
            resultList.Add(mHomingOrderList[listIndex].tileList[rndIndex]);
            mHomingOrderList[listIndex].tileList.RemoveAt(rndIndex);
            if (resultList.Count == targetCount) { break; }
        }

        if (resultList.Count < targetCount)
        {
            int normalTileCount = mNormalTileList.Count;
            while (resultList.Count < targetCount)
            {
                rndIndex = Random.Range(0, normalTileCount);
                resultList.Add(mNormalTileList[rndIndex]);
            }
        }

        loopCount = mHomingOrderList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            ReuseHomingOrderTileList.Destroy(mHomingOrderList[index]);
        }
        mHomingOrderList.Clear();
    }
    public void CreateTileListByAreaList(List<Tile> resultList, Vector2 coordi, List<Vector2> areaList)
    {
        resultList.Clear();
        Vector2 tileCoordi = Vector2.zero;
        int loopCount = areaList.Count;
        Tile instTile;

        for (int areaIndex = 0; areaIndex < loopCount; areaIndex++)
        {
            tileCoordi = coordi + areaList[areaIndex];

            instTile = GetTileByCoordiOrNull(tileCoordi);
            if (instTile == null) { continue; }
            if (!(instTile is NormalTile)) { continue; }
            resultList.Add(instTile);
        }
    }
    public void CreateTileListIncludeNullByAreaList(List<Tile> resultList, Vector2 coordi, List<Vector2> areaList)
    {
        resultList.Clear();
        Vector2 tileCoordi = Vector2.zero;
        int loopCount = areaList.Count;

        for (int areaIndex = 0; areaIndex < loopCount; areaIndex++)
        {
            tileCoordi = coordi + areaList[areaIndex];

            resultList.Add(GetTileByCoordiOrNull(tileCoordi));
        }
    }

    /*생성*/
    public void CreateMapByMapData(MapData mapData)
    {
        ClearPuzzleBoard();

        CreateTileByTileStrInternal(mapData.tileStr, mapData.mapSize);
        SetTileStateByMapDataInternal(mapData);
        CreateBlockByBlockStrInternal(mapData.blockStr, mapData.mapSize);
        DrawBackGroundInternal(mapData.mapSize);
    }
    public void AllStopCoroutine()
    {
        StopAllCoroutines();
    }

    private void ClearPuzzleBoard()
    {
        int yCount = mAllTileList.Count;
        int xCount = 0;

        for (int yIndex = 0; yIndex < yCount; yIndex++)
        {
            xCount = mAllTileList[yIndex].Count;
            for (int xIndex = 0; xIndex < xCount; xIndex++)
            {
                mAllTileList[yIndex][xIndex].Dispose();
                GameObjectPool.Destroy(mAllTileList[yIndex][xIndex].gameObject);
            }
            mAllTileList[yIndex].Clear();
        }
        mAllTileList.Clear();
        mCoordiList.Clear();
        mTileList.Clear();
        mNormalTileList.Clear();
    }
    private void CreateTileByTileStrInternal(string tileStr, Vector2 mapSize)
    {
        float minX = mBoardTransform.anchorMin.x;
        float minY = mBoardTransform.anchorMin.y;
        float maxX = mBoardTransform.anchorMax.x;
        float maxY = mBoardTransform.anchorMax.y;
        float width = 720f; //Screen.width;
        float height = (Screen.height * (720f / Screen.width)); //Screen.height;

        float sizeX = (maxX - minX) * width / mapSize.x;
        float sizeY = (maxY - minY) * height / mapSize.y;

        mUiCellSize = sizeX * (720f / width);
        if (mUiCellSize > sizeY * (720f / width))
        {
            mUiCellSize = sizeY * (720f / width);
        }
        mRatioGap = (width / height) - (720f / 1280f);
        if (mRatioGap >= 0)
        {
            mUiCellSize += 15f;
            mUiCellSize += (mRatioGap / 0.03f) * 5f;
        }
        if (mUiCellSize > 100f)
        {
            mUiCellSize = 100f;
        }
        mCellSize = Vector2.one * mUiCellSize;

        mapSize += new Vector2(2, 2);
        Vector2 initPos = -mapSize / 2;

        initPos += new Vector2(0.5f, 0);
        initPos += new Vector2(0, 0.5f);
        Vector2 createPos = initPos;

        int tileStrIndex = 0;
        Tile instTile;

        for (int yIndex = 0; yIndex < mapSize.y; yIndex++)
        {
            mAllTileList.Add(new List<Tile>());
            for (int xIndex = 0; xIndex < mapSize.x; xIndex++)
            {
                //instTile;
                Vector2 coordi = new Vector2(xIndex, yIndex);

                if (yIndex == 0 || yIndex == (mapSize.y - 1) || xIndex == 0 || xIndex == (mapSize.x - 1))
                {
                    instTile = GameObjectPool.Instantiate<Tile>(edgeTilePrefab.gameObject, mTileParentTransform);
                }
                else
                {
                    switch (tileStr[tileStrIndex])
                    {
                        case 'o':
                            instTile = GameObjectPool.Instantiate<Tile>(normalTilePrefab.gameObject, mTileParentTransform);
                            break;
                        case 'b':
                            instTile = GameObjectPool.Instantiate<Tile>(blankTilePrefab.gameObject, mTileParentTransform);
                            break;
                        default:
                            instTile = GameObjectPool.Instantiate<Tile>(emptyTilePrefab.gameObject, mTileParentTransform);
                            break;
                    }
                    tileStrIndex += 1;
                }
                instTile.Coordi = coordi;
                instTile.transform.localPosition = createPos * new Vector2(1, -1);
                mAllTileList[yIndex].Add(instTile);
                createPos += Vector2.right;
            }
            createPos += Vector2.up;
            createPos.x = initPos.x;
        }

        int yCount = mAllTileList.Count;
        int xCount = 0;
        for (int yIndex = 0; yIndex < yCount; yIndex++)
        {
            xCount = mAllTileList[yIndex].Count;
            for (int xIndex = 0; xIndex < xCount; xIndex++)
            {
                mCoordiList.Add(mAllTileList[yIndex][xIndex].Coordi);
                mTileList.Add(mAllTileList[yIndex][xIndex]);
            }
        }
        for (int index = 0; index < mTileList.Count; index++)
        {
            if (mTileList[index] is NormalTile)
            {
                mNormalTileList.Add(mTileList[index] as NormalTile);
            }
        }
    }
    private void CreateBlockByBlockStrInternal(string blockStr, Vector2 mapSize)
    {
        int blockStrIndex = -1;
        int blockNumber = 0;
        bool bPhaseResult = false;
        Vector2 coordi = Vector2.one;
        Tile instTile;

        for (coordi.y = 1; coordi.y <= mapSize.y; coordi.y++)
        {
            for (coordi.x = 1; coordi.x <= mapSize.x; coordi.x++)
            {
                blockStrIndex++;
                instTile = GetTileByCoordiOrNull(coordi);
                if (instTile == null) { continue; }
                if (!(instTile is NormalTile)) { continue; }
                if (blockStr[blockStrIndex].Equals('x')) { continue; }

                bPhaseResult = int.TryParse(blockStr[blockStrIndex].ToString(), out blockNumber);
                if (!bPhaseResult) { continue; }

                BlockManager.Instance.CreateBlockByBlockDataInTile(instTile, typeof(NormalBlock), blockNumber, 1, mTileParentTransform);
            }
        }
    }
    private void SetTileStateByMapDataInternal(MapData mapData)
    {
        int loopCount;
        Vector2 gravity = mapData.gravity * Vector2.down;

        loopCount = mCoordiList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            mTileList[index].Coordi = mCoordiList[index];
            mTileList[index].Gravity = gravity;
        }

        loopCount = mTileList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            mTileList[index].CreateSendTileListByMapSize(mapData.mapSize);
        }

        for (int index = 0; index < loopCount; index++)
        {
            mTileList[index].SetRecieveTileListOfSendTileList();
        }

        for (int index = 0; index < loopCount; index++)
        {
            mTileList[index].AdjustRecieveTileListByMapSize(mapData.mapSize);
        }

        NormalTile normalTileOrNull;
        for (int index = 0; index < loopCount; index++)
        {
            normalTileOrNull = mTileList[index] as NormalTile;
            if (normalTileOrNull == null) { continue; }

            normalTileOrNull.CreateUpDownLinkAndAroundSquareList();
        }

        EdgeTile edgeTileOrNull;
        for (int index = 0; index < loopCount; index++)
        {
            mTileList[index].IsCreateTile = false;

            edgeTileOrNull = mTileList[index] as EdgeTile;
            if (edgeTileOrNull == null) { continue; }

            edgeTileOrNull.SetCreateTileBySendTileList();
        }

        int makerCount = mapData.blockMakerList.Count;
        Vector2 makerCoordi;
        for (int index = 0; index < loopCount; index++)
        {
            for (int makerIdx = 0; makerIdx < makerCount; makerIdx++)
            {
                makerCoordi = mapData.blockMakerList[makerIdx].Coordi;
                if (mTileList[index].Coordi != makerCoordi) { continue; }
                mTileList[index].IsCreateTile = true;

                if (mapData.blockMakerList[makerIdx].CreateBlockList.Count > 0)
                {
                    mTileList[index].BlockMaker = mapData.blockMakerList[makerIdx];
                }
            }
        }
    }
    private void DrawBackGroundInternal(Vector2Int mapSize)
    {
        mAllPuzzleTransform.position = mBoardTransform.position;

        mAllPuzzleTransform.localScale = mCellSize / 100;

        mapSize += new Vector2Int(2, 2);
        Texture2D drawTexture = new Texture2D(100 * mapSize.x, 100 * mapSize.y);
        Texture2D atlasTexture = mEdgeAtlas.GetSprite("Edge_1111_xxxx").texture;

        Sprite drawSprite = null;

        Vector2 tileCoordi = Vector2.zero;

        Vector2Int spritePos = Vector2Int.zero;
        Vector2Int drawPos = Vector2Int.zero;

        Color noneColor = new Color(0, 0, 0, 0);
        Color spriteColor = new Color(0, 0, 0, 0);
        Color pixelColor = new Color(0, 0, 0, 0);
        Color cellColor = Color.white;

        StringBuilder spriteNameStrBuilder = new StringBuilder();
        StringBuilder straightStrBuilder = new StringBuilder();
        StringBuilder diagonaStrBuilder = new StringBuilder();

        List<Tile> aroundTileList = new List<Tile>();

        char typeChar = '0';
        int loopCount;
        Tile instTile;

        for (int yIndex = 0; yIndex < mapSize.y; yIndex++)
        {
            tileCoordi.y = yIndex;
            for (int xIndex = 0; xIndex < mapSize.x; xIndex++)
            {
                tileCoordi.x = xIndex;

                drawPos.x = xIndex * 100;
                drawPos.y = (mapSize.y - yIndex - 1) * 100;

                cellColor = noneColor;
                pixelColor = noneColor;

                spriteNameStrBuilder.Clear();

                instTile = GetTileByCoordiOrNull(tileCoordi);
                if (instTile == null) { continue; }
                if (instTile is NormalTile)
                {
                    spriteNameStrBuilder.Append("Square");

                    if ((xIndex + yIndex) % 2 == 0) { cellColor = mEvenColor; }
                    else { cellColor = mOddColor; }
                }
                else
                {
                    straightStrBuilder.Clear();
                    diagonaStrBuilder.Clear();

                    CreateTileListIncludeNullByAreaList(aroundTileList, tileCoordi, TileArea.around);
                    loopCount = aroundTileList.Count;
                    for (int aroundIndex = 0; aroundIndex < loopCount; aroundIndex++)
                    {
                        if (aroundTileList[aroundIndex] is NormalTile)
                        {
                            typeChar = '1';
                        }
                        else
                        {
                            typeChar = '0';
                        }

                        if (aroundIndex % 2 == 0)
                        {
                            straightStrBuilder.Append(typeChar);
                        }
                        else
                        {
                            diagonaStrBuilder.Append(typeChar);
                        }
                    }
                    spriteNameStrBuilder = GetEdgeSpriteStrBuilderInternal(straightStrBuilder, diagonaStrBuilder);

                    cellColor = mEdgeColor;
                }

                drawSprite = mEdgeAtlas.GetSprite(spriteNameStrBuilder.ToString());
                spritePos.x = (int)drawSprite.textureRect.x;
                spritePos.y = (int)drawSprite.textureRect.y;

                for (int yPixelIndex = 0; yPixelIndex < 100; yPixelIndex++)
                {
                    for (int xPixelIndex = 0; xPixelIndex < 100; xPixelIndex++)
                    {
                        spriteColor = atlasTexture.GetPixel(spritePos.x + xPixelIndex, spritePos.y + yPixelIndex);
                        if (spriteColor.a == 0)
                        {
                            pixelColor = noneColor;
                        }
                        else
                        {
                            pixelColor = spriteColor.grayscale * cellColor;
                        }
                        drawTexture.SetPixel(xPixelIndex + drawPos.x, yPixelIndex + drawPos.y, pixelColor);
                    }
                }
            }
        }
        drawTexture.Apply();
        mBackgroundRenderer.sprite = Sprite.Create(drawTexture, new Rect(Vector2.zero, mapSize * 100), Vector2.one * 0.5f);
    }

    private StringBuilder GetEdgeSpriteStrBuilderInternal(StringBuilder straight, StringBuilder diagona)
    {
        StringBuilder result = new StringBuilder("Edge_");
        result.Append(straight);
        result.Append("_");
        string straightStr = straight.ToString();

        int loopCount = diagona.Length;
        switch (straightStr)
        {
            case "1111":
            case "1101":
            case "1110":
            case "0111":
            case "1011":
            case "1010":
            case "0101":
                diagona.Clear();
                diagona.Append("xxxx");
                break;
            case "0001":
                for (int cnt = 0; cnt < loopCount; cnt++)
                {
                    if (cnt == 2 || cnt == 3) { diagona[cnt] = 'x'; }
                }
                break;
            case "0010":
                for (int cnt = 0; cnt < loopCount; cnt++)
                {
                    if (cnt == 1 || cnt == 2) { diagona[cnt] = 'x'; }
                }
                break;
            case "0100":
                for (int cnt = 0; cnt < loopCount; cnt++)
                {
                    if (cnt == 0 || cnt == 1) { diagona[cnt] = 'x'; }
                }
                break;
            case "1000":
                for (int cnt = 0; cnt < loopCount; cnt++)
                {
                    if (cnt == 0 || cnt == 3) { diagona[cnt] = 'x'; }
                }
                break;
            case "0110":
                for (int cnt = 0; cnt < loopCount; cnt++)
                {
                    if (cnt != 3) { diagona[cnt] = 'x'; }
                }
                break;
            case "1001":
                for (int cnt = 0; cnt < loopCount; cnt++)
                {
                    if (cnt != 1) { diagona[cnt] = 'x'; }
                }
                break;
            case "1100":
                for (int cnt = 0; cnt < loopCount; cnt++)
                {
                    if (cnt != 2) { diagona[cnt] = 'x'; }
                }
                break;
            case "0011":
                for (int cnt = 0; cnt < loopCount; cnt++)
                {
                    if (cnt != 0) { diagona[cnt] = 'x'; }
                }
                break;
        }

        result.Append(diagona);

        return result;
    }
    private int SortHomingOrder(ReuseHomingOrderTileList aList, ReuseHomingOrderTileList bList)
    {
        return bList.HomingOrder.CompareTo(aList.HomingOrder);
    }

    /*기타*/
    /// 블록 : 전체 섞기
    public void ShuffleAllBlockContianer()
    {
        List<NormalTile> targetTileList = new List<NormalTile>();

        int loopCount = mNormalTileList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            if (mNormalTileList[index].BlockContainerOrNull == null) { continue; }
            if (!mNormalTileList[index].BlockContainerOrNull.IsMove) { continue; }
            targetTileList.Add(mNormalTileList[index]);
        }

        loopCount = targetTileList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            mNormalTileList[index].BlockContainerOrNull.transform.position = Vector3.zero;
        }

        NormalTile firstTile = null;
        NormalTile secondTile = null;
        BlockContainer tempBlockContainer = null;

        int rndIndex = 0;
        while (targetTileList.Count >= 1)
        {
            rndIndex = Random.Range(0, targetTileList.Count);
            firstTile = targetTileList[rndIndex];
            targetTileList.RemoveAt(rndIndex);

            rndIndex = Random.Range(0, targetTileList.Count);
            secondTile = targetTileList[rndIndex];
            targetTileList.RemoveAt(rndIndex);

            tempBlockContainer = firstTile.BlockContainerOrNull;
            firstTile.BlockContainerOrNull = secondTile.BlockContainerOrNull;
            secondTile.BlockContainerOrNull = tempBlockContainer;
        }

        loopCount = mNormalTileList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            if (mNormalTileList[index].BlockContainerOrNull == null) { continue; }
            if (!mNormalTileList[index].BlockContainerOrNull.IsMove) { continue; }

            mNormalTileList[index].BlockContainerOrNull.StartMovePositionByTile(mNormalTileList[index], GameConfig.DROP_DURATION);
        }

        targetTileList.Clear();

        PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.MatchCheck);
    }
}