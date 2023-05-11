using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    private readonly string TUTO_INFO_DATA_FILE_NAME = "TutoInfoDataList";
    private readonly string MISSION_DATA_FILE_NAME   = "MissionDataList";
    private readonly string REWARD_DATA_FILE_NAME    = "RewardDataList";
    private readonly string BOOSTER_DATA_FILE_NAME   = "BoosterItemList";

    public List<TutoInfoData> TutoInfoList
    {
        get
        {
            if(mTutoInfoList == null)
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

    public BoosterItemData GetBoosterItemByIndex(int index)
    {
        return mBoosterItemList[index];
    }

    private List<TutoInfoData> mTutoInfoList;
    [SerializeField]private List<MissionDataPreset>[] mMissionDataListArr = new List<MissionDataPreset>[(int)EMissionLevel.Max];
    private List<RewardData> mRewardDataList;
    private List<BoosterItemData> mBoosterItemList;

    [RuntimeInitializeOnLoadMethod]
    private static void init()
    {
        //Instance.LoadTutoInfoList();
        Instance.LoadMissionDataPreset();
        Instance.LoadRewardDataList();
        Instance.LoadBoosterItmeList();
    }

    // Out Game

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

    public void LoadBoosterItmeList()
    {
        mBoosterItemList = Utility.LoadCSVFile<BoosterItemData>(BOOSTER_DATA_FILE_NAME);
    }
}
