using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;

public class TitleSceneManager : SceneSingleton<TitleSceneManager>
{
    private int TitleDepth
    {
        get
        {
            return mTitleDepth;
        }
        set
        {
            mTitleDepth = value;

            int loopCount = mTitleMenus.Count;
            for(int index = 0; index< loopCount; index++)
            {
                mTitleMenus[index].SetActive(false);
                if(index == mTitleDepth)
                {
                    mTitleMenus[index].SetActive(true);
                }
            }
        }
    }

    [SerializeField] private int mTitleDepth;

    [SerializeField] private List<GameObject> mTitleMenus = new List<GameObject>();

    [SerializeField] private GameObject mTutoStageGrid;
    [SerializeField] private GameObject mTutoStageButtonPrefab;
    [SerializeField] private List<TutorialStageCellUI> mTutoStageButtonList = new List<TutorialStageCellUI>();

    private void Awake()
    {
        //Screen.SetResolution(720, 1280, true);
        DataManager.Instance.LoadTutoInfoList();
    }


    public void StartGame()
    {
        if(!SceneLoader.IsExist)
        {
            Instantiate(Resources.Load("Prefabs/SceneLoader"));
        }
        SceneLoader.Instance.LoadSceneByName("GameScene");
    }

    public void OnQuitGameButtonClicked()
    {
        Application.Quit();
    }
    public void OnBackButtonClicked()
    {
        TitleDepth -= 1;
    }
    public void OnTitleStartButtonClicked()
    {
        TitleDepth += 1;
    }
    public void OnStageStartButtonClicked()
    {
        PlayDataManager.Instance.ConceptName = "Concept_0";
        PlayDataManager.Instance.MapName = "DayMap_0";

        StartGame();
    }
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
        TitleDepth += 1;
    }

    public void OnEditButtonClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("CreateMissionEditScene");
    }
}
