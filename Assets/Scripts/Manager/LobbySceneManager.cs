using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;

public class LobbySceneManager : Singleton<LobbySceneManager>
{
    public int SelectedChapterNum
    {
        get { return mSelectedChapterNum; }
        set
        {
            mSelectedChapterNum = value;
            if(mSelectedChapterNum < 0) { mSelectedChapterNum = 0; }
            if(mSelectedChapterNum >= mChapterGimmickArr.Length) { mSelectedChapterNum = mChapterGimmickArr.Length - 1; }
        }
    }
    [SerializeField]private int mSelectedChapterNum;
    
    [SerializeField] private GameObject mTutoStageGrid;
    [SerializeField] private GameObject mTutoStageButtonPrefab;
    [SerializeField] private List<TutorialStageCellUI> mTutoStageButtonList = new List<TutorialStageCellUI>();

    [Header("Chapter")] 
    [SerializeField] private MapGimmickInfo[] mChapterGimmickArr = new MapGimmickInfo[2];
    [SerializeField] private ChapterData[] mChapterDataArr = new ChapterData[2];


    public string[] UseBoosterItemArr = new string[3];

    private void Awake()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (!AdsManager.IsExist)
        {
            AdsManager.Instance.Init();
        }
#endif
    }


#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) { PlayerData.AddGold(100); }    
        if (Input.GetKeyDown(KeyCode.O)) { PlayerData.AddGold(-100); }
        if (Input.GetKeyDown(KeyCode.L)) { CollectionManager.TestUnlockCollection(); }
        if (Input.GetKeyDown(KeyCode.K)) { PlayerPrefs.DeleteAll(); }
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
        for(int index =0; index< loopCount; index++)
        {
            GameObjectPool.ReturnObject(mTutoStageButtonList[index].gameObject);
        }
        mTutoStageButtonList.Clear();


        loopCount = instList.Count;
        for (int index= 0; index< loopCount; index++)
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
        InGameUseDataManager.Instance.ConceptName = "Concept_0";
        InGameUseDataManager.Instance.MapName = "DayMap_0";

        InGameUseDataManager.Instance.ChapterMapGimmickInfo = mChapterGimmickArr[mSelectedChapterNum];
        InGameUseDataManager.Instance.CurrentChapterData = mChapterDataArr[mSelectedChapterNum];

        InGameUseDataManager.Instance.InitInGameUseData();
        
        // 현재 설정된 Booster의 내용을 적용한다.
        for(int cnt = 0; cnt < UseBoosterItemArr.Length; ++cnt)
        {
            var data = DataManager.Instance.GetBoosterItemByName(UseBoosterItemArr[cnt]);
            if(data == null) { continue; }
            if(PlayerData.BoosterItemInventory[data.ItemName] <= 0) { continue; }

            data.InvokeAllEffect();
            PlayerData.BoosterItemInventory[data.ItemName] -= 1;
        }

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
