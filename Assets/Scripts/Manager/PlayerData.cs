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
    //??
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


    #region 부스터 아이템 인벤토리

    private static List<int> mBoosterItemInventory = new List<int>();

    public static void GetBoosterItem(int itemIndex)
    {
        if(itemIndex >= DataManager.Instance.GetBoosterDataCount) { return; }

        mBoosterItemInventory.Add(itemIndex);
    }

    #endregion
}
