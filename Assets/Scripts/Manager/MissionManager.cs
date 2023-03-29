using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionManager : SceneSingleton<MissionManager>
{
    private readonly string MAP_DATA_FILE_FORMAT = "DayMap_{0}";
    private readonly string TUTO_MAP_FILE_FORMAT = "TutorialMap_{0}";
    private readonly string MISSION_DATA_FILE_NAME = "MissionDataList";
    private readonly string REWARD_DATA_FILE_NAME = "RewardDataList";
            
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
    public bool IsLastStageInDay
    {
        get
        {
            //현재 스테이지가 Day의 가장 마지막 스테이지 인지 확인합니다.
            return (mStageCount + 1) >= mTestStageList[mDayCount].Count;
        }
    }
    public bool IsLastDay
    {
        get
        {
            return (mDayCount+1) >= mTestStageList.Count;
        }
    }

    public int DayCount { get => mDayCount; }

    [Header("Reward")]
    [SerializeField] private Transform mRewardCellUITransform;
    [SerializeField] private GameObject mRewardCellUIPrefab;
    public List<MissionInfo> MissionInfoList { get => mMissionInfoList; }
    public List<RewardData> SelectRewardDataList { get => mSelectRewardList; }


    [Header("Stage")]
    [SerializeField] private int mDayCount = 0;
    [SerializeField] private int mStageCount = 0;
    private int mRewardMoveCount = 0;
    private List<List<EMissionLevel>> mTestStageList
        = new List<List<EMissionLevel>>()
        {
            new List<EMissionLevel>()
            {
                EMissionLevel.VeryEasy, EMissionLevel.Easy,
                EMissionLevel.VeryEasy,EMissionLevel.Easy,EMissionLevel.Normal
            },
            new List<EMissionLevel>()
            {
                EMissionLevel.VeryEasy, EMissionLevel.Easy,
                EMissionLevel.VeryEasy,EMissionLevel.Easy,EMissionLevel.Normal,
                EMissionLevel.Easy,EMissionLevel.Easy,EMissionLevel.Hard,
            },
            new List<EMissionLevel>()
            {
                EMissionLevel.VeryEasy, EMissionLevel.Easy, EMissionLevel.Easy,
                EMissionLevel.VeryEasy,EMissionLevel.Easy,EMissionLevel.Normal,
                EMissionLevel.Easy,EMissionLevel.Easy,EMissionLevel.Hard,
                EMissionLevel.VeryEasy,EMissionLevel.Normal,EMissionLevel.VeryHard
            }
        };

    [Header("Mission")]
    [SerializeField] private Text mDayCountText;
    [SerializeField] private Text mOrderCountText;

    [SerializeField]
    private Transform mMissionCellGridTransform;

    [SerializeField]
    private GameObject mCollectEffectPrefab;

    [SerializeField]
    private GameObject mMissionCellUIPrefab;
    private List<MissionCellUI> mMissionCellUIList = new List<MissionCellUI>();
    private List<MissionInfo> mMissionInfoList = new List<MissionInfo>();
    private List<GameObject> mAllCollectEffectList = new List<GameObject>();

    [SerializeField]
    private List<MissionData>[] mMissionDataListArr = new List<MissionData>[(int)EMissionLevel.Max];
    private Queue<MissionData> mDayMissionDataQueue = new Queue<MissionData>();
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
        //얘는 1번만 하면 되는디....
        var missionDataList = Utility.LoadCSVFile<MissionData>(MISSION_DATA_FILE_NAME);
        foreach(var data in missionDataList)
        {
            // Area Check

            // level Check
            int level = (int)data.Level;
            if (mMissionDataListArr[level] == null) { mMissionDataListArr[level] = new List<MissionData>(); }

            mMissionDataListArr[level].Add(data);
        }

        mRewardDataList = Utility.LoadCSVFile<RewardData>(REWARD_DATA_FILE_NAME);
        CreateMissionCellUI();
    }

    private void CreateMissionCellUI()
    {
        for (int index = 0; index < 5; index++)
        {
            mMissionCellUIList.Add(GameObjectPool.Instantiate<MissionCellUI>(mMissionCellUIPrefab, mMissionCellGridTransform));
            mMissionCellUIList[index].transform.SetAsLastSibling();
            mMissionCellUIList[index].gameObject.SetActive(false);
        }
    }

    private MissionData CreateMissionDataList(EMissionLevel level)
    {
        if (mMissionDataListArr[(int)level] == null) { return null; }
        int count = mMissionDataListArr[(int)level].Count;
        int randIdx = Random.Range(0, count);

        return mMissionDataListArr[(int)level][randIdx];
    }
    private void CreateMissionInfoListByDataList(MissionData data)
    {
        ClearMissionInfoList();

        foreach(var element in data.MissionList)
        {
            var info = MissionInfo.Instantiate();
            info.InitMissionInfo(element);
            mMissionInfoList.Add(info);
        }
    }
    private void ClearMissionInfoList()
    {
        int loopCount = mMissionInfoList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            MissionInfo.Destroy(mMissionInfoList[index]);
        }
        mMissionInfoList.Clear();
    }
    public void ClearMissionCellUIList()
    {
        int loopCount = mMissionCellUIList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            mMissionCellUIList[index].gameObject.SetActive(false);
        }
    }

    private void CreateMissionCollectEffect(Vector3 start, Vector3 end, System.Type missionType, int color ,Sprite targetSprite)
    {
        MissionCollectEffect instCollectEffect = GameObjectPool.Instantiate<MissionCollectEffect>(mCollectEffectPrefab);
        start.z = 0;
        end.z = 0;

        if(!mAllCollectEffectList.Contains(instCollectEffect.gameObject))
        {
            mAllCollectEffectList.Add(instCollectEffect.gameObject);
        }
        instCollectEffect.SetEffectDataByData(start, end,  targetSprite);
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
        //CostCalculator는 이제 사용하지 않음.

        // 기본 움직임 횟수 충전
        mBasicRewardList.Clear();
        mBasicRewardList.Add(GetRewardDataByNameGrade("MoveCount", 0));
        mBasicRewardList[0].RewardCount = 5;

        EMissionLevel level = mCurrentMissionData.Level;


        //최하, 하 : 무브 카운트
        //중 : 무브카운트 + 아이템 1개
        //상 : 무브 카운트 + 아이템 1개+ 선택 아이템 1개
        //최상 : 무브 카운트 + 아이템 2개 + 선택 아이템 1개
        int grade;
        int provisionCount = 0;  // 선택 아이템 보기 갯수 
        int selectItemCount = 0; // 선택 아이템

        switch (level)
        {
            case EMissionLevel.VeryEasy:
                grade = 0;
                selectItemCount = 0;
                break;
            case EMissionLevel.Easy:
                grade = 1;
                selectItemCount = 0;
                provisionCount = 2;
                break;
            case EMissionLevel.Normal:
                grade = 2;
                mBasicRewardList[0].RewardCount += 1;
                selectItemCount = 1;
                provisionCount = 2;
                break;
            case EMissionLevel.Hard:
                grade = 3;
                mBasicRewardList[0].RewardCount += 2;
                selectItemCount = 1;
                provisionCount = 2;
                break;
            case EMissionLevel.VeryHard:
                grade = 4;
                mBasicRewardList[0].RewardCount += 3;
                selectItemCount = 1;
                provisionCount = 3;
                break;
            default:
                grade = -1;
                selectItemCount = 0;
                break;
        }
        CreateSelectRewardList(grade, selectItemCount, provisionCount);
    }
    private void CreateSelectRewardList(int grade, int selectCount, int provisionCount)
    {
        mSelectRewardList.Clear();
        if(selectCount <= 0) { return; }

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
        mDayCount   = 0;
        mStageCount = 0;
        //ItemManager.Instance.AddSkillCount(typeof(HammerSkill), 1);
        ItemManager.Instance.AddSkillCount(typeof(RandomBoxSkill), 1);
        ItemManager.Instance.AddSkillCount(typeof(BlockSwapSkill), 1);
        ItemManager.Instance.AddSkillCount(typeof(ColorChangeSkill), 1);

        MapDataInfoNotiArg data = new MapDataInfoNotiArg();
        if(PlayDataManager.IsExist)
        {
            data.ConceptName = PlayDataManager.Instance.ConceptName;
        }
        data.MapName = string.Format(MAP_DATA_FILE_FORMAT, mDayCount);
        if(data.ConceptName == "Tutorial")
        {
            data.MapName = string.Format(TUTO_MAP_FILE_FORMAT, mDayCount);
        }

        ObserverCenter.Instance.SendNotification(Message.ChangeMapInfo, data);
    }
    public void ResetGameInfoByMapData(MapData mapdata)
    {
        TileMapManager.Instance.MoveCount = mapdata.moveCount;

        //게임 시작을 체크하는 부분을 따로 정확하게 만들어야한다.
        if(mDayCount == 0)
        {
            //ItemManager.Instance.AddSkillCount(typeof(HammerSkill), 1);
            ItemManager.Instance.AddSkillCount(typeof(RandomBoxSkill), 1);
            ItemManager.Instance.AddSkillCount(typeof(BlockSwapSkill), 1);
            ItemManager.Instance.AddSkillCount(typeof(ColorChangeSkill), 1);
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
        if(tutoMissionList == null) { return; }
        if(tutoMissionList.Count <= 0) { return; }

        var cost = 0;
        //CreateMissionInfoListByDataList(tutoMissionList, out cost);
    }

    public void SetNextStageInfo()
    {
        mStageCount += 1;
        if (mStageCount >= mTestStageList[mDayCount].Count)
        {
            mStageCount = 0;
            mDayCount += 1;
            if (mDayCount >= mTestStageList.Count)
            {
                mDayCount = 0;
            }
        }
    }

    /// <summary>
    /// 스테이지 단위로 생성합니다.
    /// </summary>
    public void SetStageInfo()
    {
        mCurrentMissionData = mDayMissionDataQueue.Dequeue();
        CreateMissionInfoListByDataList(mCurrentMissionData);
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

        // 선택 보상이 없음
        TakeBasicReward();
    }
    public void TakeBasicReward()
    {
        // 기본 보상 획득(무빙 추가)
        foreach (var basicReward in mBasicRewardList)
        {
            // 획득 이팩트를 만들어 보여준다.
            RewardCellUI inst =
                GameObjectPool.Instantiate<RewardCellUI>(mRewardCellUIPrefab, mRewardCellUITransform);
            inst.InitCellUI(basicReward);
        }
        ObserverCenter.Instance.SendNotification(Message.CharacterOut);
    }

    
    /// <summary>
    /// 1일 단위의 미션 데이터를 미리 생성합니다.
    /// 1일 단위로 호출이 되어야합니다.
    /// </summary>
    public void CreateDayStageInfo()
    {
        mDayMissionDataQueue.Clear();

        foreach (var stageLevel in mTestStageList[mDayCount])
        {
            // 사용할 미션을 담아둔다.
            mDayMissionDataQueue.Enqueue(CreateMissionDataList(stageLevel));
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
        mDayCountText.text = $"{mDayCount + 1}";
        mOrderCountText.text = $"{mTestStageList[mDayCount].Count - mStageCount}";
    }

    public void RefreshMissionCellUI()
    {
        //ClearMissionCellUIList();
        
        int missionCount = mMissionInfoList.Count;

        for (int index = 0; index < 5; index++)
        {
            if(index >= missionCount)
            {
                mMissionCellUIList[index].gameObject.SetActive(false); 
                continue; 
            }

           // mMissionCellUIList.Add(GameObjectPool.Instantiate<MissionCellUI>(mMissionCellUIPrefab, mMissionCellGridTransform));
            mMissionCellUIList[index].InitCellUI(mMissionInfoList[index]);
            mMissionCellUIList[index].gameObject.SetActive(true);
            //mMissionCellUIList[index].transform.SetAsLastSibling();
        }
        ObserverCenter.Instance.SendNotification(Message.RefreshMission);
    }
    public bool CheckMissionTargetByInfo(Vector3 effectPos, System.Type missionType, int color, Sprite missionSprite)
    {
        if(color <= -100) { return false; }

        int loopCount = mMissionCellUIList.Count;
        MissionInfo instMissionInfo;
        for (int index = 0; index < loopCount; index++)
        {
            if(!mMissionCellUIList[index].gameObject.activeInHierarchy) { continue; }
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
        int loopCount = mMissionInfoList.Count;
        for(int index =0; index< loopCount; index++)
        {
            if (mMissionInfoList[index].MissionType != checkType) { continue; }
            if (mMissionInfoList[index].MissionCount <= 0) { continue; }
            return mMissionInfoList[index];
        }
        return null;
    }
    public bool IsMissionBlock(System.Type checkType, int blockColor)
    {
        int loopCount = mMissionInfoList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            if (mMissionInfoList[index].MissionType != checkType) { continue; }
            if (mMissionInfoList[index].MissionCount <= 0) { continue; }
            if(mMissionInfoList[index].MissionColor >= 0)
            {
                if(mMissionInfoList[index].MissionColor != blockColor) { continue; }
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
