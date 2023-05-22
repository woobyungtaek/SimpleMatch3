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
    #region é�� �ر�

    // ���̵� ��, �ִ� 31 Chapter ���� ( 0 ~ 30 )
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

    #region ��ȭ

    public static MVC_Data<int> mGold_MVC = new MVC_Data<int>("PlayerData.Gold_MVC");

    public static void GetGold(int value)
    {
        // �߰� ȹ��� ����
        float additory = 0f;
        if (InGameUseDataManager.IsExist)
        {
            additory = InGameUseDataManager.Instance.AdditoryGoldPer;
        }
        additory /= 100f;

        int gold = (int)(value * (1f + additory));

        mGold_MVC.Value += gold;

        Debug.Log($"Current Gold : {mGold_MVC.Value}");
    }

    #endregion

    #region ���� ���� �� ���� ����

    private  static readonly int MOVECOUNT_PARTSTART    = 10;
    private  static readonly int MOVECOUNT_STAGECLEAR   = 3;
    private  static readonly int MOVECOUNT_CONTINUE     = 5;
    
    #endregion

    #region ���� �ɷ�ġ�� ���� �� ����

    public static int MoveCount_PartStart;
    public static int MoveCount_StageClear;
    public static int MoveCount_Continue;

    public static int ItemCount_ColorChange;
    public static int ItemCount_BlockSwap;
    public static int ItemCount_RandomBombBox;

    public static int SelectRewardCount = 3;

    #endregion

    #region ����

    /// <summary>
    /// �ݷ����� ȿ���� �����մϴ�.
    /// </summary>
    public static void ApplyCollectionStat()
    {
        // �⺻ ���·� ������
        MoveCount_PartStart = MOVECOUNT_PARTSTART;
        MoveCount_StageClear = MOVECOUNT_STAGECLEAR;
        MoveCount_Continue = MOVECOUNT_CONTINUE;

        ItemCount_ColorChange = 0;
        ItemCount_BlockSwap = 0;
        ItemCount_RandomBombBox = 0;

        // �ݷ��� �ر� ������ ���� �ɷ�ġ �߰�
        foreach(var collection in mCollectionSaveDataDict)
        {
            CollectionManager.ExcuteCollectionEffect(collection.Key, collection.Value);
        }
    }

    /// <summary>
    /// ����� �ݷ��� ������
    /// </summary>
    private static Dictionary<int, int> mCollectionSaveDataDict = new Dictionary<int, int>(new IntComparer());

    /// <summary>
    /// ȹ�� �� �ش� ������ ��� ����
    /// </summary>
    /// <param name="collectionName"> ���� �̸�, �������� ������ ���� </param>
    /// <param name="index"> ���� �� �ε���, �������� ������ ���� </param>
    public static void AddCollectionByIndex(int collectionIndex,int index)
    {
        if (!mCollectionSaveDataDict.ContainsKey(collectionIndex)) 
        {
            mCollectionSaveDataDict.Add(collectionIndex, 0);
        }
        mCollectionSaveDataDict[collectionIndex] = mCollectionSaveDataDict[collectionIndex] | (1 << index);
    }

    public static void TestAddCollection(int collectionIndex, int value)
    {
        if (!mCollectionSaveDataDict.ContainsKey(collectionIndex))
        {
            mCollectionSaveDataDict.Add(collectionIndex, 0);
        }
        mCollectionSaveDataDict[collectionIndex] = (1 << value);
        Debug.Log($"{mCollectionSaveDataDict[collectionIndex]} / {mCollectionSaveDataDict[collectionIndex]}");
    }

    #endregion

    #region �ν��� ������ �κ��丮

    // ������ �̸� ������� ������ ���� �ȴ�.
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
