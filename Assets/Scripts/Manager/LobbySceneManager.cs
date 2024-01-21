using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;

public class LobbySceneManager : SceneSingleton<LobbySceneManager>
{
    public int SelectedChapterNum
    {
        get 
        {
            return mSelectedChapterNum; 
        }
        set
        {
            mSelectedChapterNum = value;
            if (mSelectedChapterNum < 0) { mSelectedChapterNum = 0; }
            PlayerPrefs.SetInt("LastPlayChapterNum", mSelectedChapterNum);
        }
    }
    [SerializeField] private int mSelectedChapterNum;

    [SerializeField] private GameObject mTutoStageGrid;
    [SerializeField] private GameObject mTutoStageButtonPrefab;
    [SerializeField] private List<TutorialStageCellUI> mTutoStageButtonList = new List<TutorialStageCellUI>();

    [Header("Chapter")]
    [SerializeField] private ChapterData[] mChapterDataArr = new ChapterData[0];
    public int GetChapterCount
    {
        get => mChapterDataArr.Length;
    }


    public BoosterItemData[] UseBoosterItemArr = new BoosterItemData[3];


    private void Awake()
    {
#if UNITY_ANDROID || UNITY_IOS

        if (!IAPManager.IsExist)
        {
            IAPManager.Instance.InitUnityIAP();
        }
        if (!AdsManager.IsExist)
        {
            AdsManager.Instance.Init();
        }

#endif

        if (PlayerPrefs.HasKey("LastPlayChapterNum"))
        {
            SelectedChapterNum  = PlayerPrefs.GetInt("LastPlayChapterNum");
        }

        // 플레이어 정보 로드
        PlayerData.LoadCurrentChapter();
        PlayerData.LoadCurrentGold();
        PlayerData.LoadCurrentGem();
        PlayerData.LoadExp();
        PlayerData.LoadLv();
        PlayerData.LoadCollectionData();
        PlayerData.LoadBoosterInventory();
    }


#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) { PlayerData.AddGold(100); }
        if (Input.GetKeyDown(KeyCode.O)) { PlayerData.AddGold(-100); }
        if (Input.GetKeyDown(KeyCode.K)) { PlayerPrefs.DeleteAll(); }
        if (Input.GetKeyDown(KeyCode.G)) { PlayerData.AddExp(20); }
    }

#endif

    public void LoadGameScene()
    {
        SceneLoader.Instance.LoadSceneByName("GameScene");
    }

    // 튜툐리얼
    public void OnTutorialButtonClicked()
    {
        List<TutoInfoData> instList = DataManager.Instance.TutoInfoList;

        int loopCount = mTutoStageButtonList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            GameObjectPool.ReturnObject(mTutoStageButtonList[index].gameObject);
        }
        mTutoStageButtonList.Clear();


        loopCount = instList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            mTutoStageButtonList.Add(GameObjectPool.Instantiate<TutorialStageCellUI>(mTutoStageButtonPrefab, mTutoStageGrid.transform));
            mTutoStageButtonList[index].transform.SetAsLastSibling();
            mTutoStageButtonList[index].SetCellInfo(instList[index].Info, instList[index].Index);
        }
    }

    // 게임 스타트 관련
    public void OnStartButtonClicked()
    {
        PopupManager.Instance.CreatePopupByName("GameStartPopup");
    }
    public void ChapterStart()
    {
        InGameUseDataManager.Instance.CurrentChapterData = mChapterDataArr[mSelectedChapterNum];

        InGameUseDataManager.Instance.ChapterNumber = mSelectedChapterNum;

        InGameUseDataManager.Instance.InitInGameUseData();

        // 현재 설정된 Booster의 내용을 적용한다.
        for (int cnt = 0; cnt < UseBoosterItemArr.Length; ++cnt)
        {
            var data = UseBoosterItemArr[cnt];
            if (data == null) { continue; }

            Debug.Log($"Data Index : {data.Index}");
            if (PlayerData.BoosterItemInventory[data.Index] <= 0) { continue; }

            data.InvokeAllEffect();
            PlayerData.BoosterItemInventory[data.Index] -= 1;
        }
        PlayerData.SaveBoosterInventory();

        LoadGameScene();
    }


    // 도감 관련
    public void OnCollectionViewButtonClicked()
    {
        PopupManager.Instance.CreatePopupByName("CollectionViewPopup");
    }

    // 상점 관련
    public void OnShopButtonClicked()
    {
        PopupManager.Instance.CreatePopupByName("ShopPopup");
    }

}
