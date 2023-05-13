using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public static class RewardMethodBook
{
    public static MethodInfo GetRewardMethodInfo(string methodName)
    {
        return typeof(RewardMethodBook).GetMethod(methodName);
    }

    public static void MoveIncrease(RewardData data)
    {
        //Debug.Log("move increase");
        TileMapManager.Instance.MoveCount += data.RewardCount;
    }
    public static void HammerIncrease(RewardData data)
    {
        //Debug.Log("Hammer Increase");
        ItemManager.Instance.AddSkillCount(typeof(HammerSkill), data.RewardCount);

    }
    public static void RandBombBox(RewardData data)
    {
        //SDebug.Log("RandBombBox");
        ItemManager.Instance.AddSkillCount(typeof(RandomBoxSkill), data.RewardCount);
    }
    public static void BlockSwapIncrease(RewardData data)
    {
        //SDebug.Log("RandBombBox");
        ItemManager.Instance.AddSkillCount(typeof(BlockSwapSkill), data.RewardCount);
    }
    public static void ColorChangeIncrease(RewardData data)
    {
        //SDebug.Log("RandBombBox");
        ItemManager.Instance.AddSkillCount(typeof(ColorChangeSkill), data.RewardCount);
    }
}