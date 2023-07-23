using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionManager : SceneSingleton<MissionManager>
{
    private readonly string MAP_DATA_FILE_FORMAT = "DayMap_{0}";
    private readonly string TUTO_MAP_FILE_FORMAT = "TutorialMap_{0}";
    private readonly WaitForSeconds REWARD_CREATE_DELAY = new WaitForSeconds(0.5f);

    public bool IsMissionClear
    {
        get
        {
            int loopCount = mMissionCellUIList.Count;
            for (int index = 0; index < loopCount; index++)
            {
                if (!mMissionCellUIList[index].gameObject.activeInHierarchy) { continue; }
                if (mMissionCellUIList[index].IsComplete) { continue; }
                return false;
            }
            return true;
        }
    }
    public bool IsLastStageInPart
    {
        get
        {
            //현재 스테이지가 Day의 가장 마지막 스테이지 인지 확인합니다.
            return mStageCount >= mChapterData.GetStageCount(mPartCount) - 1;
        }
    }
    public bool IsLastPart
    {
        get
        {
            return (mPartCount + 1) >= mChapterData.PartCount;
        }
    }


    public EMissionLevel CurrentMissionLevel
    {
        get => mCurrentMissionData.Level;
    }

    [Header("Reward")]
    [SerializeField] private Transform mRewardCellUITransform;
    [SerializeField] private GameObject mRewardCellUIPrefab;
    public List<RewardData> SelectRewardDataList { get => mSelectRewardList_Grade; }


    [Header("Stage")]
    [SerializeField] private int mPartCount = 0;
    [SerializeField] private int mStageCount = 0;
    [SerializeField] private ChapterData mChapterData;

    private MVC_Data<int> mRemainStage = new MVC_Data<int>("MissionManager.mRemainStage");
    private MVC_Data<int> mPart = new MVC_Data<int>("MissionManager.mPart");
    private int StageCount
    {
        get => mStageCount;
        set
        {
            mStageCount = value;
            mRemainStage.Value = mChapterData.GetStageCount(mPartCount) - mStageCount;
        }
    }
    private int PartCount
    {
        get => mPartCount;
        set
        {
            mPartCount = value;
            mPart.Value = mPartCount + 1;
        }
    }


    [Header("Mission")]
    [SerializeField]
    private Transform mMissionCellGridTransform;

    [SerializeField]
    private GameObject mCollectEffectPrefab;

    [SerializeField]
    private GameObject mMissionCellUIPrefab;
    private List<MissionCellUI> mMissionCellUIList = new List<MissionCellUI>();
    private List<GameObject> mAllCollectEffectList = new List<GameObject>();

    [SerializeField]
    private List<MissionDataPreset>[] mMissionDataListArr;
    private List<MissionData> mDayMissionDataList = new List<MissionData>();
    private MissionData mCurrentMissionData;

    [Header("Reward")]
    [SerializeField]
    private List<RewardData> mSelectRewardList_Grade = new List<RewardData>();


    [SerializeField]
    private List<RewardData> mBasicRewardList = new List<RewardData>();
    [SerializeField]
    private List<RewardData> mSelectRewardList = new List<RewardData>();
    [SerializeField]
    private List<RewardData> mAdRewardList = new List<RewardData>();



    private List<RewardData> mInstRandomRewardList = new List<RewardData>();


    [Header("DoubleChance")]
    [SerializeField] private float mDoubleChancePer;
    private System.Func<System.Type, int> mDoubleChanceFunc;


    private void Awake()
    {
        mMissionDataListArr = DataManager.Instance.MissionDataListArr;

        var totalRewardList = DataManager.Instance.RewardDataList;

        mBasicRewardList.Clear();
        mSelectRewardList.Clear();
        mAdRewardList.Clear();
        for (int idx = 0; idx < totalRewardList.Count; ++idx)
        {
            switch (totalRewardList[idx].RewardType)
            {
                case ERewardType.Basic:
                    mBasicRewardList.Add(totalRewardList[idx]);
                    break;

                case ERewardType.AD:
                    mAdRewardList.Add(totalRewardList[idx]);
                    break;

                default:
                    mSelectRewardList.Add(totalRewardList[idx]);
                    break;
            }
        }

        CreateMissionCellUI();

        mDoubleChanceFunc = null;
        if (InGameUseDataManager.IsExist)
        {
            if (InGameUseDataManager.Instance.DoubleChancePer > 0f)
            {
                mDoubleChancePer = InGameUseDataManager.Instance.DoubleChancePer;
                mDoubleChanceFunc = DoubleChanceFunc;
            }
        }

    }

    private void CreateMissionCellUI()
    {
        for (int index = 0; index < 5; index++)
        {
            var cellUI = GameObjectPool.Instantiate<MissionCellUI>(mMissionCellUIPrefab, mMissionCellGridTransform);
            mMissionCellUIList.Add(cellUI);
            mMissionCellUIList[index].transform.SetAsLastSibling();
            mMissionCellUIList[index].gameObject.SetActive(false);
        }
    }

    private MissionDataPreset GetMissionDataPreset(EMissionLevel level)
    {
        if (mMissionDataListArr[(int)level] == null) { return null; }
        int count = mMissionDataListArr[(int)level].Count;
        int randIdx = Random.Range(0, count);

        return mMissionDataListArr[(int)level][randIdx];
    }


    public void ClearMissionCellUIList()
    {
        int loopCount = mMissionCellUIList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            mMissionCellUIList[index].gameObject.SetActive(false);
        }
    }

    private void CreateMissionCollectEffect(Vector3 start, Vector3 end, System.Type missionType, int color, Sprite targetSprite, int count)
    {
        MissionCollectEffect instCollectEffect = GameObjectPool.Instantiate<MissionCollectEffect>(mCollectEffectPrefab);
        start.z = 0;
        end.z = 0;

        if (!mAllCollectEffectList.Contains(instCollectEffect.gameObject))
        {
            mAllCollectEffectList.Add(instCollectEffect.gameObject);
        }
        instCollectEffect.SetEffectDataByData(start, end, targetSprite, count);
        instCollectEffect.PlayEffect();
    }

    public void CreateStageClearRewardDataList()
    {
        if (mCurrentMissionData == null) { return; }

        int grade;
        int selectItemCount = 0; // 선택 아이템

        switch (mCurrentMissionData.Level)
        {
            case EMissionLevel.Normal:
                grade = 2;
                selectItemCount = 1;
                break;
            case EMissionLevel.Hard:
                grade = 3;
                selectItemCount = 1;
                break;
            case EMissionLevel.VeryHard:
                grade = 4;
                selectItemCount = 1;
                break;
            default:
                grade = 0;
                selectItemCount = 0;
                break;
        }

        if (IsLastStageInPart && IsLastPart)
        {
            selectItemCount = 0;
        }

        // 등급, 선택 횟수, 보여지는 개수
        CreateSelectRewardList(grade, selectItemCount, PlayerData.SelectRewardCount);
    }
    private void CreateSelectRewardList(int grade, int selectCount, int provisionCount)
    {
        mSelectRewardList_Grade.Clear();
        if (selectCount <= 0) { return; }

        mInstRandomRewardList.Clear();

        int loopCount = mSelectRewardList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            if (mSelectRewardList[index].Grade != grade) { continue; }//현재 등급의 보상만 추가 한다.
            mInstRandomRewardList.Add(mSelectRewardList[index]);
        }

        int max = 0;
        int rand = 0;
        for (int index = 0; index < provisionCount; index++)
        {
            max = mInstRandomRewardList.Count;
            if (max <= 0) { Debug.Log("해당 등급의 추가 보상 수가 부족합니다."); return; }
            rand = Random.Range(0, max);
            mSelectRewardList_Grade.Add(mInstRandomRewardList[rand]);
            mInstRandomRewardList.RemoveAt(rand);
        }

        // AD 리워드 추가 / Select 마지막에 넣는다.
        if (!AdsManager.IsExist) { return; }
        if (AdsManager.Instance.IsRewardAdReady && mAdRewardList.Count > 0)
        {
            int rndAdRewardIdx = Random.Range(0, mAdRewardList.Count);
            mSelectRewardList_Grade.Add(mAdRewardList[rndAdRewardIdx]);
        }
    }

    /*
        게임 리셋 관련 > 안쓸 가능성이 크다.
        public void ResetGameInfoByDay()
        {
            mStageCount = 0;
            //ItemManager.Instance.AddSkillCount(typeof(HammerSkill), 1);
            ItemManager.Instance.AddSkillCount(typeof(RandomBoxSkill), 1);
            ItemManager.Instance.AddSkillCount(typeof(BlockSwapSkill), 1);
            ItemManager.Instance.AddSkillCount(typeof(ColorChangeSkill), 1);
        }
        public void ResetGameInfoByGameOver()
        {
            mPartCount = 0;
            mStageCount = 0;
            //ItemManager.Instance.AddSkillCount(typeof(HammerSkill), 1);
            ItemManager.Instance.AddSkillCount(typeof(RandomBoxSkill), 1);
            ItemManager.Instance.AddSkillCount(typeof(BlockSwapSkill), 1);
            ItemManager.Instance.AddSkillCount(typeof(ColorChangeSkill), 1);
            MapDataInfoNotiArg data = new MapDataInfoNotiArg();
            if (PlayDataManager.IsExist)
            {
                data.ConceptName = PlayDataManager.Instance.ConceptName;
            }
            data.MapName = string.Format(MAP_DATA_FILE_FORMAT, mPartCount);
            if (data.ConceptName == "Tutorial")
            {
                data.MapName = string.Format(TUTO_MAP_FILE_FORMAT, mPartCount);
            }
            ObserverCenter.Instance.SendNotification(Message.ChangeMapInfo, data);
        }
     */

    public void SetMoveAndItemCount(MapData mapdata)
    {
        if (InGameUseDataManager.IsExist)
        {
            // 테스트용 / 수집한 카운트가 더 높으면 초기화 안함
            int currentCount = TileMapManager.Instance.MoveCount;
            int settingCount = InGameUseDataManager.Instance.StartCount;
            int result = currentCount >= settingCount ? currentCount : settingCount;
            Debug.Log($"MoveCount Init : {currentCount} / {settingCount} / {result}");
            TileMapManager.Instance.MoveCount = result;

            //TileMapManager.Instance.MoveCount = InGameUseDataManager.Instance.StartCount;
        }
        else
        {
            TileMapManager.Instance.MoveCount = mapdata.moveCount;
        }

        //게임 시작을 체크하는 부분을 따로 정확하게 만들어야한다.
        if (mPartCount == 0)
        {
            ItemManager.Instance.SetSkillCountByData();
        }
    }
    public void ClearAllMissionCollectEffect()
    {
        int loopCount = mAllCollectEffectList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            GameObjectPool.ReturnObject(mAllCollectEffectList[index]);
        }
    }

    public void SetTutoMissionList(List<MissionData_Element> tutoMissionList)
    {
        if (tutoMissionList == null) { return; }
        if (tutoMissionList.Count <= 0) { return; }

        var cost = 0;
        //CreateMissionInfoListByDataList(tutoMissionList, out cost);
    }

    public void SetNextStageInfo()
    {
        StageCount += 1;

        if (mStageCount >= mChapterData.GetStageCount(mPartCount))
        {
            PartCount += 1;
            if (mPartCount >= mChapterData.PartCount)
            {
                PartCount = 0;
            }
            StageCount = 0;
        }
    }

    /// <summary>
    /// 스테이지 단위로 생성합니다.
    /// </summary>
    public void SetStageInfo()
    {
        if (mCurrentMissionData != null)
        {
            MissionData.Destroy(mCurrentMissionData);
        }
        mCurrentMissionData = mDayMissionDataList[0];
        mDayMissionDataList.RemoveAt(0);
    }

    public MissionData GetStageInfoByIndex(int index)
    {
        if (mDayMissionDataList == null) { return null; }
        if (mDayMissionDataList.Count == 0) { return null; }
        if (mDayMissionDataList.Count <= index) { return null; }

        return mDayMissionDataList[index];
    }

    public void OnRemainCustomerButtonClicked()
    {
        PopupManager.Instance.CreatePopupByName("RemainCustomerPopup");
    }

    public void TakeStageClearReward()
    {
        // 생성 하기
        CreateStageClearRewardDataList();

        // 선택 보상이 있음
        if (mSelectRewardList_Grade.Count > 0)
        {
            PopupManager.Instance.CreatePopupByName("StageSuccessPopup");
            return;
        }

        TakeBasicReward();
    }
    public void TakeBasicReward()
    {
        //// 이건 매번 호출 되는 함수
        ////if (!IsLastStageInPart)
        //{
        //    // 기본 보상 획득(무빙 추가)
        //    foreach (var basicReward in mBasicRewardList)
        //    {
        //        // 획득 이팩트를 만들어 보여준다.
        //        RewardCellUI inst =
        //            GameObjectPool.Instantiate<RewardCellUI>(mRewardCellUIPrefab, mRewardCellUITransform);
        //        inst.InitCellUI(basicReward);
        //    }
        //}
        //ObserverCenter.Instance.SendNotification(Message.CharacterOut);

        StartCoroutine(DelayCreateRewardEffect());
    }
    private System.Collections.IEnumerator DelayCreateRewardEffect()
    {
        foreach (var basicReward in mBasicRewardList)
        {
            // 획득 이팩트를 만들어 보여준다.
            RewardCellUI inst =
                GameObjectPool.Instantiate<RewardCellUI>(mRewardCellUIPrefab, mRewardCellUITransform);
            inst.InitCellUI(basicReward);
            yield return REWARD_CREATE_DELAY;
        }

        //EXP는 따로 처리 (UI를 안만들어도 되므로)
        ItemManager.Instance.AddExp(CurrentMissionLevel);

        ObserverCenter.Instance.SendNotification(Message.CharacterOut);
    }


    /// <summary>
    /// 1일 단위의 미션 데이터를 미리 생성합니다.
    /// 1일 단위로 호출이 되어야합니다.
    /// </summary>
    public void CreateDayStageInfo()
    {
        if (InGameUseDataManager.IsExist)
        {
            mChapterData = InGameUseDataManager.Instance.CurrentChapterData;
        }

        StageCount = mStageCount;
        PartCount = mPartCount;

        mDayMissionDataList.Clear();

        foreach (var stageLevel in mChapterData.MissionLevelList[mPartCount].list)
        {
            // 사용할 미션을 담아둔다.
            var missionDataPreset = GetMissionDataPreset(stageLevel);
            var missionData = MissionData.Instantiate();
            missionData.Init(missionDataPreset);

            mDayMissionDataList.Add(missionData);
        }
    }

    public void StartStage()
    {
        RefreshMissionCellUI();

        if (PuzzleManager.Instance.CurrentState == EGameState.StageSuccess)
        {
            PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.TileReadyCheck);
        }
    }

    public void RefreshMissionCellUI()
    {
        if (mCurrentMissionData == null) { return; }
        int missionCount = mCurrentMissionData.MissionInfoList.Count;

        for (int index = 0; index < 5; index++)
        {
            if (index >= missionCount)
            {
                mMissionCellUIList[index].gameObject.SetActive(false);
                continue;
            }

            mMissionCellUIList[index].InitCellUI(mCurrentMissionData.MissionInfoList[index]);
            mMissionCellUIList[index].gameObject.SetActive(true);

            // 미션 타겟을 강제로 생성해야하는 경우 추가
            // Type을 TileGimmick, Block으로 변경 후 CreateForece 실행? 난이도를 전달 해줘야함
            var createType = mCurrentMissionData.MissionInfoList[index].MissionType;
            int createCount = mCurrentMissionData.MissionInfoList[index].MissionCount;
            int createColor = mCurrentMissionData.MissionInfoList[index].MissionColor;
            if (createType.GetInterface("IForceCreateOnBoard") != null)
            {
                Debug.Log("강제 생성");

                int alreadyCount = TileMapManager.Instance.GetAlreadyContainCount_TileGimmick(createType);
                int gapCount = createCount - alreadyCount;
                if (gapCount <= 0) { continue; }

                Debug.Log($"생성 개수 (타일기믹) : {gapCount}");

                TileMapManager.Instance.CreateMissionTargetOnTile(createType, gapCount, createColor, 1);
            }
            else if (createType.GetInterface("IReserveBlockMaker") != null)
            {

                Debug.Log("생성 예약");
                int alreadyCount = TileMapManager.Instance.GetAlreadyContainCount_Block(createType, createColor);
                int gapCount = createCount - alreadyCount;
                if (gapCount <= 0) { continue; }

                Debug.Log($"생성 개수 (블럭) : {gapCount}");

                BlockData reserveData = new BlockData();
                reserveData.BlockType = createType;
                if (createColor < 0)
                {
                    createColor = Random.Range(0, BlockMaker.MaxColor);
                }
                reserveData.BlockColor = createColor;
                reserveData.BlockHP = 1;
                BlockMaker.SetRandomDelayCreateCount();
                for (int cnt = 0; cnt < gapCount; ++cnt)
                {
                    BlockMaker.AddReserveBlockData(reserveData);
                }
            }

        }
        ObserverCenter.Instance.SendNotification(Message.RefreshMission);
    }
    public bool CheckMissionTargetByInfo(Vector3 effectPos, System.Type missionType, int color, Sprite missionSprite)
    {
        if (color <= -100) { return false; }

        int loopCount = mMissionCellUIList.Count;
        MissionInfo instMissionInfo;
        for (int index = 0; index < loopCount; index++)
        {
            if (!mMissionCellUIList[index].gameObject.activeInHierarchy) { continue; }
            if (mMissionCellUIList[index].IsComplete) { continue; }

            instMissionInfo = mMissionCellUIList[index].CurrentMission;
            if (instMissionInfo == null) { continue; }
            if (instMissionInfo.MissionType != missionType) { continue; }
            if (instMissionInfo.MissionColor != color)
            {
                if (instMissionInfo.MissionColor != -1) { continue; }
            }

            int collectCount = 1;
            if (mDoubleChanceFunc != null)
            {
                collectCount = mDoubleChanceFunc.Invoke(missionType);
            }

            //CreateMissionCollectEffect(effectPos, TileMapManager.Instance.RandCollectPos, missionType, color, missionSprite);
            CreateMissionCollectEffect(effectPos, mMissionCellUIList[index].transform.position, missionType, color, missionSprite, collectCount);

            mMissionCellUIList[index].CollectMissionTarget(collectCount);

            if (mMissionCellUIList[index].IsComplete)
            {
                ObserverCenter.Instance.SendNotification(Message.RefreshMission);
            }

            return true;
        }
        return false;
    }

    private int DoubleChanceFunc(System.Type checkType)
    {
        if (checkType.BaseType == typeof(BombBlock))
        {
            // 1. 일단 여기서 하면 기본 틀을 해치지 않고 기능이 구현
            // 2. 통합으로 관리할 수 있어서
            float chance = Random.Range(0f, 100f);
            if (chance <= mDoubleChancePer)
            {
                Debug.Log("DoubleChance Activate");
                return 2;
            }
        }
        return 1;
    }

    public MissionInfo GetMissionInfoByType(System.Type checkType)
    {
        if (mCurrentMissionData == null) { return null; }
        int loopCount = mCurrentMissionData.MissionInfoList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            if (mCurrentMissionData.MissionInfoList[index].MissionType != checkType) { continue; }
            if (mCurrentMissionData.MissionInfoList[index].MissionCount <= 0) { continue; }
            return mCurrentMissionData.MissionInfoList[index];
        }
        return null;
    }
    public bool IsMissionBlock(System.Type checkType, int blockColor)
    {
        if (mCurrentMissionData == null) { return false; }
        int loopCount = mCurrentMissionData.MissionInfoList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            if (mCurrentMissionData.MissionInfoList[index].MissionType != checkType) { continue; }
            if (mCurrentMissionData.MissionInfoList[index].MissionCount <= 0) { continue; }
            if (mCurrentMissionData.MissionInfoList[index].MissionColor >= 0)
            {
                if (mCurrentMissionData.MissionInfoList[index].MissionColor != blockColor) { continue; }
            }
            return true;
        }
        return false;
    }

    public GameObject GetMissionCellUIByIndex(int index)
    {
        return mMissionCellUIList[index].gameObject;
    }

    // 강제로 추가
    public void SetMissionTargetByForce(string type, int slot, int color, int amount)
    {
        if (mCurrentMissionData == null)
        {
            mCurrentMissionData = MissionData.Instantiate();
            mCurrentMissionData.ClearMissionInfoList();
        }

        var missionInfo = MissionInfo.Instantiate();
        missionInfo.InitMissionInfo(System.Type.GetType(type), color, amount);
        mCurrentMissionData.AddMissionInfoForce(slot, missionInfo);

        RefreshMissionCellUI();
    }
}

public class BlockCollectNotiArgs : NotificationArgs
{
    public System.Type MissionType;
    public int MissionColor;
}

public interface IForceCreateOnBoard { }
public interface IReserveBlockMaker { }