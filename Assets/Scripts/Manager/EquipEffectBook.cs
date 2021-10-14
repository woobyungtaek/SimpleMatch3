using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class RewardMethodBook
{
    public static MethodInfo GetRewardMethodInfo(string methodName)
    {
        return typeof(RewardMethodBook).GetMethod(methodName);
    }

    public static void TestEffect(int count)
    {
    }
    public static void MoveIncrease(RewardData data)
    {
        //Debug.Log("move increase");
        TileMapManager.Instance.MoveCount += data.RewardCount;
    }
    public static void HammerIncrease(RewardData data)
    {
        //Debug.Log("Hammer Increase");
        ItemManager.Instance.HammerCount += data.RewardCount;

    }
    public static void RandBombBox(RewardData data)
    {
        //SDebug.Log("RandBombBox");
        ItemManager.Instance.RandomBombBoxCount += data.RewardCount;
    }
}
