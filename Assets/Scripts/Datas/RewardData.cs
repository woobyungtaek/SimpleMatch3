using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ERewardType
{
    Basic,
    Select
}
[System.Serializable]
public class RewardData
{
    public int      Index;
    public string   RewardName;
    public string   SpriteName;
    public int      Grade;
    public ERewardType RewardType;

    private string mMethodName;

    private int mRewardCount;
    private System.Action<RewardData> mRewardFunc;

    private System.Func<int> mRewardCountFunc;

    public string MethodName
    {
        get => mMethodName;
        set
        {
            mMethodName = value;

            var info = RewardMethodBook.GetRewardMethodInfo(mMethodName);
            mRewardFunc = (System.Action<RewardData>)System.Delegate.CreateDelegate(typeof(System.Action<RewardData>), info);
        }
    }

    public int RewardCount
    {
        get
        {
            if(mRewardCountFunc== null)
            {
                if(mRewardCount != 0)
                {
                    mRewardCountFunc = RewardCountByData;
                }
                else
                {
                    var info = RewardMethodBook.GetRewardMethodInfo($"{mMethodName}_CountFunc");
                    mRewardCountFunc = (System.Func<int>)System.Delegate.CreateDelegate(typeof(System.Func<int>), info);
                }
            }

            return mRewardCountFunc.Invoke();
        }
        set
        {
            mRewardCount = value;
        }
    }

    public void ExcuteRewardFunc()
    {
        mRewardFunc?.Invoke(this);
    }

    private int RewardCountByData()
    {
        // 기본 상태의 반환 함수
        // 따로 처리되는 애들은 RewardMethodBook에 있다.
        return mRewardCount;
    }
}
