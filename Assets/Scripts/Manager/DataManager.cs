using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    private readonly string TUTO_INFO_DATA_FILE_NAME    = "TutoInfoDataList";
    private readonly string MISSION_DATA_FILE_NAME      = "MissionDataList";
    private readonly string REWARD_DATA_FILE_NAME       = "RewardDataList";

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

    private List<TutoInfoData> mTutoInfoList;
    [SerializeField]private List<MissionDataPreset>[] mMissionDataListArr = new List<MissionDataPreset>[(int)EMissionLevel.Max];
    private List<RewardData> mRewardDataList;

    [RuntimeInitializeOnLoadMethod]
    private static void init()
    {
        Instance.LoadTutoInfoList();
        Instance.LoadMissionDataPreset();
        Instance.LoadRewardDataList();
    }

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
