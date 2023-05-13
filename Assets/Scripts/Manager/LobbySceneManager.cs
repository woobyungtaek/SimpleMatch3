using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;

public class LobbySceneManager : SceneSingleton<LobbySceneManager>
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


    public int[] UseBoosterItemArr = new int[3];

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

        PlayDataManager.Instance.ConceptName = "Concept_0";
        PlayDataManager.Instance.MapName = "DayMap_0";

        PlayDataManager.Instance.ChapterMapGimmickInfo = mChapterGimmickArr[mSelectedChapterNum];
        PlayDataManager.Instance.CurrentChapterData = mChapterDataArr[mSelectedChapterNum];

        PlayDataManager.Instance.InitPlayData();
        
        // 플레이어의 값 중 적용해야할덧 먼저 적용
        SetPlayDataByPlayerInfo();

        // 현재 설정된 Booster의 내용을 적용한다.
        for(int cnt = 0; cnt < UseBoosterItemArr.Length; ++cnt)
        {
            int index = UseBoosterItemArr[cnt];
            if (index < 0) { continue; }

            var data = DataManager.Instance.GetBoosterItemByIndex(index);
            data.InvokeAllEffect();
        }

        LoadGameScene();
    }

    private void SetPlayDataByPlayerInfo()
    {
        // 저장된 데이터가 적용된다.

        // StartCount
        PlayDataManager.Instance.StartCount += 10;

        // AdditoryMoveCount
        PlayDataManager.Instance.AdditoryMoveCount += 4;

        // ContinueCount
        // ColorChangeCount
        // BlockSwapCount
        // RandomBombBoxCount
    }
}
