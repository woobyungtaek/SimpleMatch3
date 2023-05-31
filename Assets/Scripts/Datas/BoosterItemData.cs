using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class BoosterItemData
{
    private static readonly string EFFECT_FORMAT = "{0}[{1}]\n";
    private static readonly System.Text.StringBuilder mStrBuilder = new System.Text.StringBuilder();

    public int Index;

    public int Grade;

    public string ItemName
    {
        get
        {
            return mItemName;
        }
        set
        {
            mItemName = value;
            if (mEffectMethodList == null) { mEffectMethodList = new List<Action<float>>(); }
            if (mEffectValueList == null) { mEffectValueList = new List<float>(); }
            mEffectMethodList.Clear();
            mEffectValueList.Clear();

            mUseCount = 0;
            if (PlayerPrefs.HasKey(mItemName))
            {
                mUseCount = PlayerPrefs.GetInt(mItemName);
            }
        }
    }

    public string EffectName
    {
        set
        {
            // value로 부터 Method찾기
            // List에 추가하기
            var method = BoosterEffectBook.GetBoosterMethodInfo(value);
            if (method == null) { return; }
            var action = (Action<float>)Delegate.CreateDelegate(typeof(Action<float>), method);
            mEffectMethodList.Add(action);
        }
    }

    public float EffectValue
    {
        set
        {
            mEffectValueList.Add(value);
        }
    }

    public int UseCount { get => mUseCount; }

    public string EffectDesc
    {
        get
        {
            mStrBuilder.Clear();

            for(int cnt = 0; cnt < mEffectMethodList.Count; ++cnt)
            {
                Localization.GetString(mEffectMethodList[0].Method.Name);
                mStrBuilder.AppendFormat(EFFECT_FORMAT, Localization.GetString(mEffectMethodList[cnt].Method.Name), mEffectValueList[cnt]);
            }

            return mStrBuilder.ToString();
        }
    }

    private string mItemName;
    private List<Action<float>> mEffectMethodList;
    private List<float> mEffectValueList;

    private int mUseCount;
    public void InvokeAllEffect()
    {
        mUseCount++;
        for (int idx = 0; idx < mEffectMethodList.Count; ++idx)
        {
            mEffectMethodList[idx].Invoke(mEffectValueList[idx]);
        }
    }
}

public static class BoosterEffectBook
{
    public static MethodInfo GetBoosterMethodInfo(string methodName)
    {
        return typeof(BoosterEffectBook).GetMethod(methodName);
    }

    public static void AdditoryMove_Start(float value)
    {
        Debug.Log($"AdditoryMove_Start  {value}");
        if (!InGameUseDataManager.IsExist) { return; }
        InGameUseDataManager.Instance.StartCount += (int)value;
    }
    public static void AdditoryMove_StageClear(float value)
    {
        Debug.Log($"AdditoryMove_StageClear  {value}");
        if (!InGameUseDataManager.IsExist) { return; }
        InGameUseDataManager.Instance.MoveCount_StageClear += (int)value;
    }
    public static void ContinueCount(float value)
    {
        Debug.Log($"ContinueCount  {value}");
        if (!InGameUseDataManager.IsExist) { return; }
        InGameUseDataManager.Instance.MoveCount_Continue += (int)value;
    }
    public static void StartItem_ColorChange(float value)
    {
        Debug.Log($"StartItem_ColorChange  {value}");
        if (!InGameUseDataManager.IsExist) { return; }
        InGameUseDataManager.Instance.ItemCount_ColorChange += (int)value;
    }
    public static void StartItem_BlockSwap(float value)
    {
        Debug.Log($"StartItem_BlockSwap  {value}");
        if (!InGameUseDataManager.IsExist) { return; }
        InGameUseDataManager.Instance.ItemCount_BlockSwap += (int)value;
    }
    public static void StartItem_RandomBombBox(float value)
    {
        Debug.Log($"StartItem_RandomBombBox  {value}");
        if (!InGameUseDataManager.IsExist) { return; }
        InGameUseDataManager.Instance.ItemCount_RandomBombBox += (int)value;
    }
    public static void AdditoryRewardItem(float value)
    {
        Debug.Log($"AdditoryRewardItem  {value}");
        if (!InGameUseDataManager.IsExist) { return; }
        InGameUseDataManager.Instance.DoubleChancePer += value;
    }
    public static void DoubleChance(float value)
    {
        Debug.Log($"DoubleChance  {value}");
        if (!InGameUseDataManager.IsExist) { return; }
        InGameUseDataManager.Instance.DoubleChancePer += value;
    }
    public static void AdditoryGoldPer(float value)
    {
        Debug.Log($"AdditoryGoldPer  {value}");
        if (!InGameUseDataManager.IsExist) { return; }
        InGameUseDataManager.Instance.AdditoryGoldPer += value;
    }
    public static void LockItems(float value)
    {
        Debug.Log($"LockItems  {value}");
        if (!InGameUseDataManager.IsExist) { return; }
        InGameUseDataManager.Instance.IsLockItem = System.Convert.ToBoolean(value);
    }


}
