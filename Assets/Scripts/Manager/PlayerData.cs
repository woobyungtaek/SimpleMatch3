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

    private static MVC_Data<int> mGold_MVC = new MVC_Data<int>("PlayerData.Gold_MVC");

    public static int CurrentGold { get => mGold_MVC.Value; }

    public static void AddGold(int value)
    {
        // 추가 획득률 적용
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

    public static void UseGold(int useGold)
    {
        mGold_MVC.Value -= useGold;
    }


    #endregion

    #region 도감 적용 전 고정 스탯

    private  static readonly int MOVECOUNT_PARTSTART    = 10;
    private  static readonly int MOVECOUNT_STAGECLEAR   = 3;
    private  static readonly int MOVECOUNT_CONTINUE     = 5;
    
    #endregion

    #region 도감 능력치가 적용 된 스탯

    public static int MoveCount_PartStart;
    public static int MoveCount_StageClear;
    public static int MoveCount_Continue = MOVECOUNT_CONTINUE;

    public static int ItemCount_ColorChange;
    public static int ItemCount_BlockSwap;
    public static int ItemCount_RandomBombBox;

    public static int SelectRewardCount = 3;

    #endregion

    #region 도감

    /// <summary>
    /// 콜렉션의 효과를 적용합니다.
    /// </summary>
    public static void ApplyCollectionStat()
    {
        // 기본 상태로 돌리기
        MoveCount_PartStart = MOVECOUNT_PARTSTART;
        MoveCount_StageClear = MOVECOUNT_STAGECLEAR;
        MoveCount_Continue = MOVECOUNT_CONTINUE;

        ItemCount_ColorChange = 0;
        ItemCount_BlockSwap = 0;
        ItemCount_RandomBombBox = 0;

        // 콜렉션 해금 정보에 따라 능력치 추가
        foreach(var collection in mCollectionSaveDataDict)
        {
            CollectionManager.ExcuteCollectionEffect(collection.Key, collection.Value);
        }
    }

    /// <summary>
    /// 저장된 콜렉션 데이터
    /// </summary>
    private static Dictionary<int, int> mCollectionSaveDataDict = new Dictionary<int, int>(new IntComparer());

    /// <summary>
    /// 획득 시 해당 아이템 잠금 해제
    /// </summary>
    /// <param name="collectionName"> 도감 이름, 아이템이 가지고 있음 </param>
    /// <param name="index"> 도감 내 인덱스, 아이템이 가지고 있음 </param>
    public static void AddCollectionByIndex(int collectionIndex,int index)
    {
        if (!mCollectionSaveDataDict.ContainsKey(collectionIndex)) 
        {
            mCollectionSaveDataDict.Add(collectionIndex, 0);
        }
        mCollectionSaveDataDict[collectionIndex] = mCollectionSaveDataDict[collectionIndex] | (1 << index);
    }

    /// <summary>
    /// 저장 된 도감의 상태를 확인
    /// </summary>
    /// <param name="index">도감 번호</param>
    /// <returns></returns>
    public static int GetCollectionSaveValue(int index)
    {
        if (!mCollectionSaveDataDict.ContainsKey(index)) { return 0; }
        return mCollectionSaveDataDict[index];
    }

    public static void TestAddCollection(int collectionIndex, int value)
    {
        if (!mCollectionSaveDataDict.ContainsKey(collectionIndex))
        {
            mCollectionSaveDataDict.Add(collectionIndex, 0);
        }
        mCollectionSaveDataDict[collectionIndex] = (1 << value) - 1;
        Debug.Log($"{mCollectionSaveDataDict[collectionIndex]} / {mCollectionSaveDataDict[collectionIndex]}");
    }

    #endregion

    #region 부스터 아이템 인벤토리

    // 아이템 이름 기반으로 사용횟수가 저장된다.(가챠에 쓰인다)
    // 우선 PlayerPrefabs에

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

        Debug.Log($"Get Booster Item : {data.ItemName}");
    }

    #endregion

    #region 데코 아이템 인벤토리

    public static Dictionary<string, int> DecoItemInventory = new Dictionary<string, int>();

    public static void AddDecoItem(int itemIndex)
    {
        if(itemIndex >= DataManager.Instance.GetDecoItemDataCount) { return; }

        var data = DataManager.Instance.GetDecoItemByIndex(itemIndex);
        if (!DecoItemInventory.ContainsKey(data.ItemName))
        {
            DecoItemInventory.Add(data.ItemName, 0);
        }
        int count = DecoItemInventory[data.ItemName] + 1;
        if(count>= 10)
        {
            count = 10;
        }
        DecoItemInventory[data.ItemName]  = count;
        AddCollectionByIndex(data.CollectionInfoIndex, data.CollectionIndex);
        Debug.Log($"AddDecoItem : {data.ItemName}");
    }

    public static int GetAvrgCount_Deco()
    {
        if(DecoItemInventory.Count == 0) { return 0; }

        int total = 0;
        foreach(int count in DecoItemInventory.Values)
        {
            total += count;
        }

        return total / DecoItemInventory.Count;       
    }

    #endregion
}
