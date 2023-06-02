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


    // Out Game
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

        foreach(var data in mBoosterItemList)
        {
            if(data.Grade == grade) { list.Add(data); }
        }
    }

    // In Game
    private List<TutoInfoData> mTutoInfoList;
    [SerializeField] private List<MissionDataPreset>[] mMissionDataListArr = new List<MissionDataPreset>[(int)EMissionLevel.Max];
    private List<RewardData> mRewardDataList;

    [RuntimeInitializeOnLoadMethod]
    private static void init()
    {
        // 콜렉션 정보가 먼저 설정되어야함
        CollectionManager.Init();

        // Collectable 데이터 로드
        Instance.LoadDecoItemList();

        //Instance.LoadTutoInfoList();
        Instance.LoadMissionDataPreset();
        Instance.LoadRewardDataList();
        Instance.LoadBoosterItmeList();
    }

    // Out Game
    public void LoadDecoItemList()
    {
        mDecoItemList = Utility.LoadCSVFile<DecoItemData>(DECOITEM_DATA_FILE_NAME);
        foreach(var data in mDecoItemList)
        {
            data.InitData();
        }
    }
    public void LoadBoosterItmeList()
    {
        mBoosterItemList = Utility.LoadCSVFile<BoosterItemData>(BOOSTER_DATA_FILE_NAME);
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
