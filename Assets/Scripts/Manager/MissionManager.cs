using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionManager : SceneSingleton<MissionManager>
{
    private readonly string MAP_DATA_FILE_FORMAT = "DayMap_{0}";
    private readonly string TUTO_MAP_FILE_FORMAT = "TutorialMap_{0}";

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
            return (mStageCount + 1) >= mChapterData.GetStageCount(mPartCount);
        }
    }
    public bool IsLastPart
    {
        get
        {
            return (mPartCount + 1) >= mChapterData.PartCount;
        }
    }

    [Header("Reward")]
    [SerializeField] private Transform mRewardCellUITransform;
    [SerializeField] private GameObject mRewardCellUIPrefab;
    public List<RewardData> SelectRewardDataList { get => mSelectRewardList; }


    [Header("Stage")]
    [SerializeField] private int mPartCount = 0;
    [SerializeField] private int mStageCount = 0;
    [SerializeField] private ChapterData mChapterData;

    [Header("Mission")]
    [SerializeField] private TextMeshProUGUI mDayCountText;
    [SerializeField] private TextMeshProUGUI mOrderCountText;

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
    private List<RewardData> mRewardDataList = new List<RewardData>();

    [SerializeField]
    private List<RewardData> mBasicRewardList = new List<RewardData>();
    [SerializeField]
    private List<RewardData> mSelectRewardList = new List<RewardData>();

    private List<RewardData> mInstRandomRewardList = new List<RewardData>();

    private void Awake()
    {
        mMissionDataListArr = DataManager.Instance.MissionDataListArr;
        mRewardDataList = DataManager.Instance.RewardDataList;
        CreateMissionCellUI();
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

    private void CreateMissionCollectEffect(Vector3 start, Vector3 end, System.Type missionType, int color, Sprite targetSprite)
    {
        MissionCollectEffect instCollectEffect = GameObjectPool.Instantiate<MissionCollectEffect>(mCollectEffectPrefab);
        start.z = 0;
        end.z = 0;

        if (!mAllCollectEffectList.Contains(instCollectEffect.gameObject))
        {
            mAllCollectEffectList.Add(instCollectEffect.gameObject);
        }
        instCollectEffect.SetEffectDataByData(start, end, targetSprite);
        instCollectEffect.PlayEffect();
    }

    private RewardData GetRewardDataByNameGrade(string name, int grade)
    {
        int loopCount = mRewardDataList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            if (mRewardDataList[index].RewardName != name) { continue; }
            if (mRewardDataList[index].Grade != grade) { continue; }
            return mRewardDataList[index];
        }
        return null;
    }
    public void CreateStageClearRewardDataList()
    {
        // 기본 움직임 횟수 충전
        mBasicRewardList.Clear();
        mBasicRewardList.Add(GetRewardDataByNameGrade("MoveCount", 0));

        mBasicRewardList[0].RewardCount = 5;
        if (PlayDataManager.IsExist)
        {
            mBasicRewardList[0].RewardCount = PlayDataManager.Instance.AdditoryMoveCount;
        }

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

        CreateSelectRewardList(grade, selectItemCount, 2);
    }
    private void CreateSelectRewardList(int grade, int selectCount, int provisionCount)
    {
        mSelectRewardList.Clear();
        if (selectCount <= 0) { return; }

        mInstRandomRewardList.Clear();
        int loopCount = mRewardDataList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            if (!mRewardDataList[index].RewardType.Equals("Select")) { continue; }
            if (mRewardDataList[index].Grade != grade) { continue; }//현재 등급의 보상만 추가 한다.
            mInstRandomRewardList.Add(mRewardDataList[index]);
        }

        int max = 0;
        int rand = 0;
        for (int index = 0; index < provisionCount; index++)
        {
            max = mInstRandomRewardList.Count;
            if (max <= 0) { Debug.Log("해당 등급의 추가 보상 수가 부족합니다."); return; }
            rand = Random.Range(0, max);
            mSelectRewardList.Add(mInstRandomRewardList[rand]);
            mInstRandomRewardList.RemoveAt(rand);
        }
    }

    // 게임 리셋 관련 > 안쓸 가능성이 크다.
    //public void ResetGameInfoByDay()
    //{
    //    mStageCount = 0;
    //    //ItemManager.Instance.AddSkillCount(typeof(HammerSkill), 1);
    //    ItemManager.Instance.AddSkillCount(typeof(RandomBoxSkill), 1);
    //    ItemManager.Instance.AddSkillCount(typeof(BlockSwapSkill), 1);
    //    ItemManager.Instance.AddSkillCount(typeof(ColorChangeSkill), 1);
    //}
    //public void ResetGameInfoByGameOver()
    //{
    //    mPartCount = 0;
    //    mStageCount = 0;
    //    //ItemManager.Instance.AddSkillCount(typeof(HammerSkill), 1);
    //    ItemManager.Instance.AddSkillCount(typeof(RandomBoxSkill), 1);
    //    ItemManager.Instance.AddSkillCount(typeof(BlockSwapSkill), 1);
    //    ItemManager.Instance.AddSkillCount(typeof(ColorChangeSkill), 1);
    //    MapDataInfoNotiArg data = new MapDataInfoNotiArg();
    //    if (PlayDataManager.IsExist)
    //    {
    //        data.ConceptName = PlayDataManager.Instance.ConceptName;
    //    }
    //    data.MapName = string.Format(MAP_DATA_FILE_FORMAT, mPartCount);
    //    if (data.ConceptName == "Tutorial")
    //    {
    //        data.MapName = string.Format(TUTO_MAP_FILE_FORMAT, mPartCount);
    //    }
    //    ObserverCenter.Instance.SendNotification(Message.ChangeMapInfo, data);
    //}

    public void SetMoveAndItemCount(MapData mapdata)
    {
        TileMapManager.Instance.MoveCount = mapdata.moveCount;
        if (PlayDataManager.IsExist)
        {
            TileMapManager.Instance.MoveCount = PlayDataManager.Instance.StartCount;
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
        mStageCount += 1;
        if (mStageCount >= mChapterData.GetStageCount(mPartCount))
        {
            mStageCount = 0;
            mPartCount += 1;
            if (mPartCount >= mChapterData.PartCount)
            {
                mPartCount = 0;
            }
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
        if (mSelectRewardList.Count > 0)
        {
            PopupManager.Instance.CreatePopupByName("StageSuccessPopup");
            return;
        }

        TakeBasicReward();
    }
    public void TakeBasicReward()
    {
        if (!IsLastStageInPart)
        {
            // 기본 보상 획득(무빙 추가)
            foreach (var basicReward in mBasicRewardList)
            {
                // 획득 이팩트를 만들어 보여준다.
                RewardCellUI inst =
                    GameObjectPool.Instantiate<RewardCellUI>(mRewardCellUIPrefab, mRewardCellUITransform);
                inst.InitCellUI(basicReward);
            }
        }
        ObserverCenter.Instance.SendNotification(Message.CharacterOut);
    }


    /// <summary>
    /// 1일 단위의 미션 데이터를 미리 생성합니다.
    /// 1일 단위로 호출이 되어야합니다.
    /// </summary>
    public void CreateDayStageInfo()
    {
        if (PlayDataManager.IsExist)
        {
            mChapterData = PlayDataManager.Instance.CurrentChapterData;
        }

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
        RefreshDayOrderUI();

        if (PuzzleManager.Instance.CurrentState == EGameState.StageSuccess)
        {
            PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.TileReadyCheck);
        }
    }

    public void RefreshDayOrderUI()
    {
        mDayCountText.text = $"{mPartCount + 1}";
        mOrderCountText.text = $"{mChapterData.GetStageCount(mPartCount) - mStageCount}";
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

            //CreateMissionCollectEffect(effectPos, TileMapManager.Instance.RandCollectPos, missionType, color, missionSprite);
            CreateMissionCollectEffect(effectPos, mMissionCellUIList[index].transform.position, missionType, color, missionSprite);
            mMissionCellUIList[index].CollectMissionTarget();

            if (mMissionCellUIList[index].IsComplete)
            {
                ObserverCenter.Instance.SendNotification(Message.RefreshMission);
            }
            return true;
        }
        return false;
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
}

public class BlockCollectNotiArgs : NotificationArgs
{
    public System.Type MissionType;
    public int MissionColor;
}
