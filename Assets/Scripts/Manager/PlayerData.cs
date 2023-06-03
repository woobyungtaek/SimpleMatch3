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

    // 골드
    private static MVC_Data<int> mGold_MVC = new MVC_Data<int>("PlayerData.Gold_MVC");
    public static int CurrentGold { get => mGold_MVC.Value; }
    public static void AddGold(int value)
    {
        mGold_MVC.Value += value;
        SaveCurrentGold();

        Debug.Log($"Current Gold : {mGold_MVC.Value}");
    }
    public static void UseGold(int useGold)
    {
        mGold_MVC.Value -= useGold;
        SaveCurrentGold();
    }
    public static int CalResultGold(int value)
    {
        float additory = 0f;
        if (InGameUseDataManager.IsExist)
        {
            additory = InGameUseDataManager.Instance.AdditoryGoldPer;
        }
        additory /= 100f;

        int gold = (int)(value * (1f + additory));
        return gold;
    }
    private static void SaveCurrentGold()
    {
        // 우선 PlayerPref에...
        PlayerPrefs.SetInt("PlayerGold", mGold_MVC.Value);
    }
    public static void LoadCurrentGold()
    {
        if(PlayerPrefs.HasKey("PlayerGold"))
        {
            mGold_MVC.Value = PlayerPrefs.GetInt("PlayerGold");
        }
    }

    // 유료 재화
    private static MVC_Data<int> mGem_MVC = new MVC_Data<int>("PlayerData.Gem_MVC");
    public static int CurrentGem { get => mGem_MVC.Value; }
    public static void AddGem(int value)
    {
        mGem_MVC.Value += value;
        SaveCurrentGem();
        Debug.Log($"Current Gold : {mGem_MVC.Value}");
    }
    public static bool UseGem(int value)
    {
        if(CurrentGem >= value)
        {
            mGem_MVC.Value -= value;
            SaveCurrentGem();
            return true;
        }
        return false;
    }
    private static void SaveCurrentGem()
    {
        // 우선 PlayerPref에...
        PlayerPrefs.SetInt("PlayerGem", mGem_MVC.Value);
    }
    public static void LoadCurrentGem()
    {
        if (PlayerPrefs.HasKey("PlayerGem"))
        {
            mGem_MVC.Value = PlayerPrefs.GetInt("PlayerGem");
        }
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
        SaveCollectionData();
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

    /// <summary>
    /// 도감 해금 데이터 파일 저장 / 바꿔야함
    /// </summary>
    public static void SaveCollectionData()
    {
        // JsonUtility를 사용하려면 저장용 구조체 생성 후 List > 저장 해야함
        // & Dictionary안됨

        System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();

        // 따로 Pasing을 해서 저장
        foreach (var pair in mCollectionSaveDataDict)
        {
            uint value = 0;
            value = (uint)pair.Key << 4; // index
            value |= (uint)pair.Value; // count

            strBuilder.Append($"{value},");
        }
        Debug.Log(strBuilder.ToString());
        PlayerPrefs.SetString("CollectionSaveStr", strBuilder.ToString());
    }

    /// <summary>
    /// 도금 해금 데이터 파일 불러오기 / 바꿔야함
    /// </summary>
    public static void LoadCollectionData()
    {
        if(mCollectionSaveDataDict.Count > 0) { return; }
        // 불러와서 데이터 셋
        if (!PlayerPrefs.HasKey("CollectionSaveStr")) { return; }
        var dataArr = PlayerPrefs.GetString("CollectionSaveStr").Split(',');

        foreach (var strValue in dataArr)
        {
            if (string.IsNullOrEmpty(strValue)) { return; }

            uint value;
            if(!uint.TryParse(strValue, out value)) { continue; }

            int infoIndex = (int)(value >> 4);
            int index = (int)(value & 15);

            mCollectionSaveDataDict.Add(infoIndex, index);
        }
    }

#if UNITY_EDITOR
    public static void TestAddCollection(int collectionIndex, int value)
    {
        if (!mCollectionSaveDataDict.ContainsKey(collectionIndex))
        {
            mCollectionSaveDataDict.Add(collectionIndex, 0);
        }
        mCollectionSaveDataDict[collectionIndex] = (1 << value) - 1;
        Debug.Log($"{mCollectionSaveDataDict[collectionIndex]} / {mCollectionSaveDataDict[collectionIndex]}");
    }
#endif

#endregion

    #region 부스터 아이템 인벤토리

    // 아이템 이름 기반으로 사용횟수가 저장된다.(가챠에 쓰인다)
    // 우선 PlayerPref에 string으로 저장 바꿔야함

    // 아이템 이름 기반으로 개수가 저장 된다.
    public static Dictionary<int, int> BoosterItemInventory = new Dictionary<int, int>(new IntComparer());
    public static void AddBoosterItem(int itemIndex)
    {
        if(itemIndex >= DataManager.Instance.GetBoosterDataCount) { return; }

        if(!BoosterItemInventory.ContainsKey(itemIndex))
        {
            BoosterItemInventory.Add(itemIndex, 0);
        }
        BoosterItemInventory[itemIndex] += 1;

        Debug.Log($"Get Booster Item : {itemIndex}");
        SaveBoosterInventory();
    }

    public static void SaveBoosterInventory()
    {
        // uint에 16밀고 16당겨서

        System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();

        // 따로 Pasing을 해서 저장
        foreach (var pair in BoosterItemInventory)
        {
            uint value = 0;
            value = (uint)pair.Key << 16; // index
            value |= (uint)pair.Value; // count

            strBuilder.Append($"{value},");
        }
        Debug.Log(strBuilder.ToString());
        PlayerPrefs.SetString("BoosterInvenStr", strBuilder.ToString());
    }
    public static void LoadBoosterInventory()
    {
        if(BoosterItemInventory.Count > 0) { return; }
        // 불러와서 데이터 셋
        if (!PlayerPrefs.HasKey("BoosterInvenStr")) { return; }
        var dataArr = PlayerPrefs.GetString("BoosterInvenStr").Split(',');

        foreach (var strValue in dataArr)
        {
            if (string.IsNullOrEmpty(strValue)) { return; }

            uint value;
            if (!uint.TryParse(strValue, out value)) { continue; }

            int index = (int)(value >> 16);
            int count = (int)(value & 65535);

            BoosterItemInventory.Add(index, count);
        }
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
