using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    private const string TutoInfoFileName = "TutoInfoDataList";

    public List<TutoInfoData> TutoInfoList
    {
        get
        {
            if(mTutoInfoList == null)
            {
                mTutoInfoList = Utility.LoadCSVFile<TutoInfoData>(TutoInfoFileName);
            }
            return mTutoInfoList;
        }
    }

    private List<TutoInfoData> mTutoInfoList;

    public void LoadTutoInfoList()
    {
        mTutoInfoList = Utility.LoadCSVFile<TutoInfoData>(TutoInfoFileName);
    }
}
