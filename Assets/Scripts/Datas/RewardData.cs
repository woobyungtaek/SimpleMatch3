using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RewardData
{
    public System.Reflection.MethodInfo RewardMethodInfo
    {
        get
        {
            if(mRewardMethodInfo == null)
            {
                mRewardMethodInfo = RewardMethodBook.GetRewardMethodInfo(MethodName);
            }
            return mRewardMethodInfo;
        }
    }

    public int      Index;
    public string   RewardName;
    public string   MethodName;
    public string   SpriteName;
    public int      Grade;
    public int      RewardCount;
    public string   RewardType;

    private System.Reflection.MethodInfo mRewardMethodInfo;
}
