using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EChapterLevel
{
    Normal = 0,
    Hard,
    Max
}

public static class PlayerData
{
    #region 챕터 해금

    // 난이도 별, 최대 31 Chapter 가능 ( 0 ~ 30 )
    private static int[] mUnlockChapterArr = new int[(int)EChapterLevel.Max];

    public static void ChapterUnlock(EChapterLevel level, int chapter)
    {
        if( chapter >= sizeof(int)) { return; }

        int current = mUnlockChapterArr[(int)level];
        int unlock = 1 << chapter;

        mUnlockChapterArr[(int)level] = current | unlock;
    }

    public static bool IsUnlockChapter(EChapterLevel level, int chapter)
    {
        if (chapter >= sizeof(int)) { return false; }

        int current = mUnlockChapterArr[(int)level];
        int check = 1 << chapter;

        int result = current & check;

        return result != 0;
    }

    #endregion

    #region 재화

    public static MVC_Data<int> mGold_MVC = new MVC_Data<int>("PlayerData.Gold_MVC");

    public static void GetGold(int value)
    {
        // 추가 획득률 적용
        float additory = 0f;
        if (PlayDataManager.IsExist)
        {
            additory = PlayDataManager.Instance.AdditoryGoldPer;
        }
        additory /= 100f;

        int gold = (int)(value * (1f + additory));

        mGold_MVC.Value += gold;

        Debug.Log($"Current Gold : {mGold_MVC.Value}");
    }

    #endregion

    #region 보상

    public static int SelectRewardCount = 3;

    #endregion

    #region 활동력

    public static int ContinueMoveCount = 5;

    #endregion

    #region 부스터 아이템 인벤토리

    // 아이템 이름 기반으로 개수가 저장 된다.
    public static Dictionary<string, int> BoosterItemInventory = new Dictionary<string, int>();
    
    public static void AddBoosterItem(int itemIndex)
    {
        if(itemIndex >= DataManager.Instance.GetBoosterDataCount) { return; }

        var data = DataManager.Instance.GetBoosterItemByIndex(itemIndex);
        if(!BoosterItemInventory.ContainsKey(data.ItemName))
        {
            BoosterItemInventory.Add(data.ItemName, 0);
        }
        BoosterItemInventory[data.ItemName] += 1;
    }

    #endregion
}
