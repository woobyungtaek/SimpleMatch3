using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    private readonly string TUTO_INFO_DATA_FILE_NAME = "TutoInfoDataList";
    private readonly string MISSION_DATA_FILE_NAME = "MissionDataList";
    private readonly string REWARD_DATA_FILE_NAME = "RewardDataList";
    private readonly string BOOSTER_DATA_FILE_NAME = "BoosterItemList";
    private readonly string DECOITEM_DATA_FILE_NAME = "DecoItemList";
    private readonly string COLLECTION_INFO_DATA_FILE_NAME = "CollectionInfoList";
    private readonly string LEVEL_INFO_DATA_FILE_NAME = "LevelInfoList";

    // Out Game
    #region 콜렉션 관련
    private Dictionary<int, CollectionInfoData> mCollectionInfoDataDict
        = new Dictionary<int, CollectionInfoData>(new IntComparer());
    public int InfoDataCount { get => mCollectionInfoDataDict.Count; }
    public CollectionInfoData GetInfoDataByIndex(int index)
    {
        if (!mCollectionInfoDataDict.ContainsKey(index)) { return null; }
        return mCollectionInfoDataDict[index];
    }
    #endregion

    #region 데코레이션 아이템 관련
    private List<DecoItemData> mDecoItemList;
    public int GetDecoItemDataCount
    {
        get => mDecoItemList.Count;
    }
    public DecoItemData GetDecoItemByIndex(int index)
    {
        if (index < 0 || index >= mDecoItemList.Count) { return null; }
        return mDecoItemList[index];
    }
    public void GetGradeDataList_Deco(int grade, ref List<DecoItemData> list)
    {
        list.Clear();

        foreach (var data in mDecoItemList)
        {
            if (data.Grade == grade) { list.Add(data); }
        }
    }
    #endregion

    #region 부스터 관련
    private List<BoosterItemData> mBoosterItemList;
    public int GetBoosterDataCount
    {
        get => mBoosterItemList.Count;
    }
    public BoosterItemData GetBoosterItemByName(string name)
    {
        for (int idx = 0; idx < mBoosterItemList.Count; ++idx)
        {
            if (mBoosterItemList[idx].ItemName != name) { continue; }
            return mBoosterItemList[idx];
        }
        return null;
    }
    public BoosterItemData GetBoosterItemByIndex(int index)
    {
        if (index < 0 || index >= mBoosterItemList.Count) { return null; }
        return mBoosterItemList[index];
    }
    public void GetGradeDataList_Booster(int grade, ref List<BoosterItemData> list)
    {
        list.Clear();

        foreach (var data in mBoosterItemList)
        {
            if (data.Grade == grade) { list.Add(data); }
        }
    }
    #endregion

    #region 레벨 관련

    private List<LevelInfoData> mLevelInfoList;

    public int MaxLevel { get => mLevelInfoList.Count; }

    #endregion

    // In Game
    private List<TutoInfoData> mTutoInfoList;
    public List<TutoInfoData> TutoInfoList
    {
        get
        {
            if (mTutoInfoList == null)
            {
                LoadTutoInfoList();
            }
            return mTutoInfoList;
        }
    }

    [SerializeField] private List<MissionDataPreset>[] mMissionDataListArr = new List<MissionDataPreset>[(int)EMissionLevel.Max];
    public List<MissionDataPreset>[] MissionDataListArr
    {
        get
        {
            if (mMissionDataListArr == null)
            {
                LoadMissionDataPreset();
            }
            return mMissionDataListArr;
        }
    }

    private List<RewardData> mRewardDataList;
    public List<RewardData> RewardDataList
    {
        get
        {
            if (mTutoInfoList == null)
            {
                LoadRewardDataList();
            }
            return mRewardDataList;
        }
    }

    [RuntimeInitializeOnLoadMethod]
    private static void init()
    {
        // 콜렉션 정보가 먼저 설정되어야함
        Instance.LoadCollectionInfoData();

        // Collectable 데이터 로드
        Instance.LoadDecoItemList();

        //Instance.LoadTutoInfoList();
        Instance.LoadMissionDataPreset();
        Instance.LoadRewardDataList();
        Instance.LoadBoosterItemList();

        // 레벨 데이터 로드
        Instance.LoadLevelInfoList();
    }

    // Out Game
    public void LoadDecoItemList()
    {
        mDecoItemList = Utility.LoadCSVFile<DecoItemData>(DECOITEM_DATA_FILE_NAME);

        foreach (var data in mDecoItemList)
        {
            AddCollectionItem(data);
        }
    }
    public void LoadBoosterItemList()
    {
        mBoosterItemList = Utility.LoadCSVFile<BoosterItemData>(BOOSTER_DATA_FILE_NAME);
    }
    public void LoadCollectionInfoData()
    {
        var collectionInfoDataList = Utility.LoadCSVFile<CollectionInfoData>(COLLECTION_INFO_DATA_FILE_NAME);
        mCollectionInfoDataDict.Clear();
        foreach (var info in collectionInfoDataList)
        {
            mCollectionInfoDataDict.Add(info.Index, info);
        }

    }
    public void LoadLevelInfoList()
    {
        mLevelInfoList = Utility.LoadCSVFile<LevelInfoData>(LEVEL_INFO_DATA_FILE_NAME);
    }

    // Out Game Call
    public void AddCollectionItem(ICollectableItem item)
    {
        if (!mCollectionInfoDataDict.ContainsKey(item.CollectionInfoIndex)) { return; }

        mCollectionInfoDataDict[item.CollectionInfoIndex].CollectableItemDataArr[item.CollectionIndex] = item;
    }
    public void ExcuteCollectionEffect(int index, int currentValue)
    {
        if (!mCollectionInfoDataDict.ContainsKey(index)) { return; }

        // UnlockCheck

        // Excute
        mCollectionInfoDataDict[index].ExcuteEffect(currentValue);
    }

    public void ExcuteLevelEffect(int level)
    {
        level = Mathf.Min(level, mLevelInfoList.Count - 1);
        for(int cnt = 1; cnt <= level; ++cnt)
        {
            mLevelInfoList[cnt].ExcuteEffect();
        }
    }

    public LevelInfoData GetLevelInfoByIndex(int index)
    {
        return mLevelInfoList[index];
    }

    // In Game
    public void LoadTutoInfoList()
    {
        mTutoInfoList = Utility.LoadCSVFile<TutoInfoData>(TUTO_INFO_DATA_FILE_NAME);
    }
    public void LoadMissionDataPreset()
    {
        var missionDataList = Utility.LoadCSVFile<MissionDataPreset>(MISSION_DATA_FILE_NAME);
        foreach (var data in missionDataList)
        {
            // Area Check

            // level Check
            int level = (int)data.Level;
            if (mMissionDataListArr[level] == null) { mMissionDataListArr[level] = new List<MissionDataPreset>(); }

            mMissionDataListArr[level].Add(data);
        }
    }
    public void LoadRewardDataList()
    {
        mRewardDataList = Utility.LoadCSVFile<RewardData>(REWARD_DATA_FILE_NAME);
    }

}
