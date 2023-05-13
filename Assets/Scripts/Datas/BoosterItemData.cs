using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class BoosterItemData
{
    public int Index;

    public string ItemName
    {
        get
        {
            return mItemName;
        }
        set
        {
            if (mEffectMethodList == null) { mEffectMethodList = new List<Action<float>>(); }
            if (mEffectValueList == null) { mEffectValueList = new List<float>(); }
            mEffectMethodList.Clear();
            mEffectValueList.Clear();
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

    private string mItemName;
    private List<Action<float>> mEffectMethodList;
    private List<float> mEffectValueList;

    public void InvokeAllEffect()
    {
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
        if (!PlayDataManager.IsExist) { return; }
        PlayDataManager.Instance.StartCount += (int)value;
    }
    public static void AdditoryMove_StageClear(float value)
    {
        Debug.Log($"AdditoryMove_StageClear  {value}");
        if (!PlayDataManager.IsExist) { return; }
        PlayDataManager.Instance.AdditoryMoveCount += (int)value;
    }
    public static void ContinueCount(float value)
    {
        Debug.Log($"ContinueCount  {value}");
        if (!PlayDataManager.IsExist) { return; }
        PlayDataManager.Instance.ContinueCount += (int)value;
    }
    public static void StartItem_ColorChange(float value)
    {
        Debug.Log($"StartItem_ColorChange  {value}");
        if (!PlayDataManager.IsExist) { return; }
        PlayDataManager.Instance.ColorChangeCount += (int)value;
    }
    public static void StartItem_BlockSwap(float value)
    {
        Debug.Log($"StartItem_BlockSwap  {value}");
        if (!PlayDataManager.IsExist) { return; }
        PlayDataManager.Instance.BlockSwapCount += (int)value;
    }
    public static void StartItem_RandomBombBox(float value)
    {
        Debug.Log($"StartItem_RandomBombBox  {value}");
        if (!PlayDataManager.IsExist) { return; }
        PlayDataManager.Instance.RandomBombBoxCount += (int)value;
    }
    public static void AdditoryRewardItem(float value)
    {
        Debug.Log($"AdditoryRewardItem  {value}");
        if (!PlayDataManager.IsExist) { return; }
        PlayDataManager.Instance.DoubleChancePer += value;
    }
    public static void DoubleChance(float value)
    {
        Debug.Log($"DoubleChance  {value}");
        if (!PlayDataManager.IsExist) { return; }
        PlayDataManager.Instance.DoubleChancePer += value;
    }
    public static void AdditoryGoldPer(float value)
    {
        Debug.Log($"AdditoryGoldPer  {value}");
        if (!PlayDataManager.IsExist) { return; }
        PlayDataManager.Instance.AdditoryGoldPer += value;
    }
    public static void LockItems(float value)
    {
        Debug.Log($"LockItems  {value}");
        if (!PlayDataManager.IsExist) { return; }
        PlayDataManager.Instance.IsLockItem = System.Convert.ToBoolean(value);
    }


}
