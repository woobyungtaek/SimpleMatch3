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


    #region Effect Func

    public static void MoveIncrease(RewardData data)
    {

        TileMapManager.Instance.MoveCount += data.RewardCount;
    }

    public static void GoldIncrease(RewardData data)
    {
        // 난이도에 따라 차등 지급

        ItemManager.Instance.AddGold(data.RewardCount);
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

    #endregion

    #region Count Func

    public static int MoveIncrease_CountFunc()
    {
        //Debug.Log("move increase");
        int count = 5;
        if (PlayDataManager.IsExist)
        {
            count = PlayDataManager.Instance.AdditoryMoveCount;
        }
        return count;
    }

    public static int GoldIncrease_CountFunc()
    {
        int level = (int)MissionManager.Instance.CurrentMissionLevel;
        return level * level * 2 + 5;
    }

    #endregion
}