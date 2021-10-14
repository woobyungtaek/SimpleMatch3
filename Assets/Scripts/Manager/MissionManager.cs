using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum EStageCost
{
    VeryLow = 10,
    Low = 30,
    Middle = 50,
    High = 80,
    VeryHigh = 100
}

public class MissionManager : SceneSingleton<MissionManager>
{
    private const string MAP_DATA_FILE_FORMAT = "DayMap_{0}";
    private const string TUTO_MAP_FILE_FORMAT = "TutorialMap_{0}";
    private const string MISSION_DATA_FILE_NAME = "MissionDataList";
    private const string REWARD_DATA_FILE_NAME = "RewardDataList";
    private const string STAGE_TEXT_FORMAT = "{0}일 {1}명 남음 / 난이도 : {2}";
            
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

    public int DayCount         { get => mDayCount; }
    public int RewardMoveCount  { get => mRewardMoveCount; }

    public List<MissionInfo> MissionInfoList { get => mMissionInfoList; }
    public List<RewardData> BasicRewardDataList { get => mBasicRewardList; }
    public List<RewardData> SelectRewardDataList { get => mSelectRewardList; }


    [Header("Stage")]
    [SerializeField] private int mDayCount = 0;
    [SerializeField] private int mStageCount = 0;
    private int mRewardMoveCount = 0;
    private List<List<EStageCost>> mTestStageList
        = new List<List<EStageCost>>()
        {
            new List<EStageCost>()
            {
                EStageCost.VeryLow, EStageCost.Low,
                EStageCost.VeryLow,EStageCost.Low,EStageCost.Middle
            },
            new List<EStageCost>()
            {
                EStageCost.VeryLow, EStageCost.Low,
                EStageCost.VeryLow,EStageCost.Low,EStageCost.Middle,
                EStageCost.Low,EStageCost.Low,EStageCost.High,
            },
            new List<EStageCost>()
            {
                EStageCost.VeryLow, EStageCost.Low, EStageCost.Low,
                EStageCost.VeryLow,EStageCost.Low,EStageCost.Middle,
                EStageCost.Low,EStageCost.Low,EStageCost.High,
                EStageCost.VeryLow,EStageCost.Middle,EStageCost.VeryHigh
            }
        };

    [Header("Mission")]
    [SerializeField] private Text mDayCountText;
    [SerializeField] private Text mOrderCountText;


    [SerializeField]
    private EStageCost mStageCost;
    private int mTotalCost;
    private int mCurrentTotalCost;

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
    private List<MissionData> mMissionDataList = new List<MissionData>();
    private List<MissionData> mSelectPool = new List<MissionData>();
    private List<MissionData> mStageMissionDataList = new List<MissionData>();


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
        mMissionDataList = Utility.LoadCSVFile<MissionData>(MISSION_DATA_FILE_NAME);
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
    private void CreateMissionDataList()
    {
        mStageMissionDataList.Clear();

        int selectCount;
        int maxLimit;
        int minLimit;
        int missionCost;
        int randIndex;
        int loopCount = mMissionDataList.Count;

        mCurrentTotalCost = 0;
        selectCount = 5;
        maxLimit = mTotalCost;

        for (int cnt = 0; cnt < 5; cnt++)
        {
            mSelectPool.Clear();
            minLimit = maxLimit / selectCount;

            for (int index = 0; index < loopCount; index++)
            {
                missionCost = mMissionDataList[index].MissionCost;
                if (missionCost < minLimit) { continue; }
                if (missionCost > maxLimit) { continue; }

                mSelectPool.Add(mMissionDataList[index]);
            }
            if (mSelectPool.Count <= 0)
            {
                //최소부터 올라가는 공식 수행
                continue;
            }
            randIndex = Random.Range(0, mSelectPool.Count);
            selectCount -= 1;
            maxLimit -= mSelectPool[randIndex].MissionCost;
            mCurrentTotalCost += mSelectPool[randIndex].MissionCost;
            mStageMissionDataList.Add(mSelectPool[randIndex]);
        }
    }
    private void CreateMissionInfoListByDataList(List<MissionData> missionDataList)
    {
        ClearMissionInfoList();
        int loopCount = missionDataList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            mMissionInfoList.Add(MissionInfo.Instantiate());
            mMissionInfoList[mMissionInfoList.Count - 1]
                .InitMissionInfo(missionDataList[index].MissionName, missionDataList[index].MissionColor, missionDataList[index].MissionCount);
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
    private void ClearMissionCellUIList()
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
    private void CreateStageClearRewardDataList()
    {
        //최하, 하 : 무브 카운트
        //중 : 무브카운트 + 아이템 1개
        //상 : 무브 카운트 + 아이템 1개+ 선택 아이템 1개
        //최상 : 무브 카운트 + 아이템 2개 + 선택 아이템 1개
        int grade;
        int basicItemCount = 0;//추가 기본 아이템
        int selectItemCount = 0;//선택 아이템
        int provisionCount = 0;

        switch (mStageCost)
        {
            case EStageCost.VeryLow:
                grade = 0;
                basicItemCount = 0;
                selectItemCount = 0;
                break;
            case EStageCost.Low:
                grade = 1;
                basicItemCount = 0;
                selectItemCount = 0;
                provisionCount = 2;
                break;
            case EStageCost.Middle:
                grade = 2;
                basicItemCount = 0;
                selectItemCount = 1;
                provisionCount = 2;
                break;
            case EStageCost.High:
                grade = 3;
                basicItemCount = 1;
                selectItemCount = 1;
                provisionCount = 2;
                break;
            case EStageCost.VeryHigh:
                grade = 4;
                basicItemCount = 2;
                selectItemCount = 1;
                provisionCount = 3;
                break;
            default:
                grade = -1;
                basicItemCount = 0;
                selectItemCount = 0;
                break;
        }

        CreateBasicRewardList(grade, basicItemCount);
        CreateSelectRewardList(grade, selectItemCount, provisionCount);
    }
    private void CreateBasicRewardList(int grade, int rewardCount)
    {
        mBasicRewardList.Clear();
        mBasicRewardList.Add(GetRewardDataByNameGrade("MoveCount", 0));
        mBasicRewardList[0].RewardCount = CostCalculator.GetBasicRewardMoveCount(mCurrentTotalCost);

        if (rewardCount <= 0) { return; }

        mInstRandomRewardList.Clear();
        int loopCount = mRewardDataList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            if (!mRewardDataList[index].RewardType.Equals("AddBasic")) { continue; }
            if (mRewardDataList[index].Grade != grade) { continue; }//현재 등급의 보상만 추가 한다.
            mInstRandomRewardList.Add(mRewardDataList[index]);
        }

        int max = 0;
        int rand = 0;
        for(int index =0; index < rewardCount; index++)
        {
            max = mInstRandomRewardList.Count;
            if(max<= 0) { Debug.Log("해당 등급의 추가 보상 수가 부족합니다."); return; }
            rand = Random.Range(0, max);
            mBasicRewardList.Add(mInstRandomRewardList[rand]);
            mInstRandomRewardList.RemoveAt(rand);
        }
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
        ItemManager.Instance.HammerCount = 1;
        ItemManager.Instance.RandomBombBoxCount = 1;
    }
    public void ResetGameInfoByGameOver()
    {
        mDayCount   = 0;
        mStageCount = 0;
        ItemManager.Instance.HammerCount        = 1;
        ItemManager.Instance.RandomBombBoxCount = 1;

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
            ItemManager.Instance.HammerCount = 1;
            ItemManager.Instance.RandomBombBoxCount = 1;
        }
    }
    public void ClearAllMissionCollectEffect()
    {
        int loopCount = mAllCollectEffectList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            GameObjectPool.Destroy(mAllCollectEffectList[index]);
        }
    }

    public void SetTutoMissionList(List<MissionData> tutoMissionList)
    {
        if(tutoMissionList == null) { return; }
        if(tutoMissionList.Count <= 0) { return; }
        mStageMissionDataList = tutoMissionList;
        CreateMissionInfoListByDataList(mStageMissionDataList);
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
    public void CreateCurrentStageInfo()
    {
        mStageCost = mTestStageList[mDayCount][mStageCount];
        mTotalCost = (int)mStageCost;
        CreateMissionDataList();
        CreateMissionInfoListByDataList(mStageMissionDataList);

        CreateStageClearRewardDataList();
    }

    public void StartStage()
    {
        TileMapManager.Instance.MoveCount += mRewardMoveCount;
        RefreshMissionCellUI();
        mDayCountText.text = (mDayCount + 1).ToString();
        mOrderCountText.text = (mTestStageList[mDayCount].Count - mStageCount).ToString();
       // mTestStageText.text = string.Format(STAGE_TEXT_FORMAT, mDayCount + 1, mTestStageList[mDayCount].Count - mStageCount, mTestStageList[mDayCount][mStageCount].ToString());
        
        if (PuzzleManager.Instance.CurrentState == EGameState.StageSuccess)
        {
            PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.Drop);
        }
    }

    public void RefreshMissionCellUI()
    {
        ClearMissionCellUIList();
        
        //int loopCount = mStageMissionDataList.Count;
        int missionCount = mStageMissionDataList.Count;

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
    public void CheckMissionTargetByInfo(Vector3 effectPos, System.Type missionType, int color, Sprite missionSprite)
    {
        if(color <= -100) { return; }

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

            CreateMissionCollectEffect(effectPos, TileMapManager.Instance.RandCollectPos, missionType, color, missionSprite);
            mMissionCellUIList[index].CollectMissionTarget();

            if (mMissionCellUIList[index].IsComplete)
            {
                ObserverCenter.Instance.SendNotification(Message.RefreshMission);
            }
            break;
        }
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
