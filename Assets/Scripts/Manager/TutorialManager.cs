using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : SceneSingleton<TutorialManager>
{
    public const int     TUTORIAL_COUNT = 5;

    private const string CLICK_TYPE =    "Click";
    private const string SWAP_TYPE  =    "Swap";
    private const string ITEM_TYPE  =    "item";

    public int TutoIndex { get { return mTutoIndex; } }

    public bool     IsSwapMode;
    public Vector2 SelectCoordi;
    public Vector2 TargetCoordi;

    [SerializeField] private GameObject mTutoButtonPrefab;
    [SerializeField] private GameObject mTutoHighlightPrefab;
    [SerializeField] private RectTransform mTutoCanvas;

    private int mTutoIndex;

    private TutorialData mTutoData;
    private TutoStepData mCurrentStepData;
    private int mStepIndex;

    private TutoNextButton mNextButton;
    private TutoHighlight  mHighlight;

    private void Start()
    {
        ObserverCenter.Instance.AddObserver(ExcuteSwapComplete, EGameState.Input.ToString());
        ObserverCenter.Instance.AddObserver(ExcuteOffHighlight, EGameState.MatchSwap.ToString());
    }

    private void ExcuteSwapComplete(Notification noti)
    {
        if (!IsSwapMode) { return; }
        IsSwapMode = false;
        ProgressCurrentTutorial();
    }
    private void ExcuteOffHighlight(Notification noti)
    {
        mHighlight.gameObject.SetActive(false);
    }
    private void ExcuteTutorialEndByNoti(Notification noti)
    {
        PopupManager.Instance.CreatePopupByName("TutorialEndPopup");
    }

    private IEnumerator DelayStartTuto()
    {
        yield return null;
        ProgressCurrentTutorial();
    }
    public void SetTutorialData(TutorialData data)
    {
        mTutoData = data;
        mTutoIndex = mTutoData.TutoIndex;
        mStepIndex = 0;
        IsSwapMode = false;
        SelectCoordi = Vector2.zero;
        TargetCoordi = Vector2.zero;

        if (mTutoButtonPrefab == null) { mTutoButtonPrefab = Resources.Load("Prefabs/TutoNextButton") as GameObject; }
        if(mTutoHighlightPrefab == null) { mTutoHighlightPrefab = Resources.Load("Prefabs/TutoHighlight") as GameObject; }

        if (mNextButton == null)
        {
            mTutoCanvas = GameObject.Find("TutoCanvas").GetComponent<RectTransform>();
            mNextButton = GameObjectPool.Instantiate<TutoNextButton>(mTutoButtonPrefab, mTutoCanvas.transform);
            mNextButton.NextButton.onClick.AddListener(OnNextTutoButtonClicked);
            mNextButton.CanvasSize = mTutoCanvas.sizeDelta;
        }
        if (mHighlight == null)
        {
            mHighlight = GameObjectPool.Instantiate<TutoHighlight>(mTutoHighlightPrefab, mTutoCanvas.transform);
            mHighlight.CanvasSize = mTutoCanvas.sizeDelta;
        }        

        mNextButton.gameObject.SetActive(false);
        mHighlight.gameObject.SetActive(false);

        MissionManager.Instance.SetTutoMissionList(data.missionList);
        ItemManager.Instance.HammerCount = data.HammerCount;
        ItemManager.Instance.RandomBombBoxCount = data.RandBoxCount;

        PuzzleManager.Instance.RemoveGameEndObseverByTutoManager();

        ObserverCenter.Instance.RemoveObserver(EGameState.StageSuccess.ToString(), ExcuteTutorialEndByNoti);
        ObserverCenter.Instance.RemoveObserver(EGameState.StageFail.ToString(), ExcuteTutorialEndByNoti);

        ObserverCenter.Instance.AddObserver(ExcuteTutorialEndByNoti, EGameState.StageSuccess.ToString());
        ObserverCenter.Instance.AddObserver(ExcuteTutorialEndByNoti, EGameState.StageFail.ToString());

        ObserverCenter.Instance.SendNotification(Message.OnTutoMode);
    }
    public void StartTutorial()
    {
        MissionManager.Instance.StartStage();
        StartCoroutine(DelayStartTuto());
    }
    public void ProgressCurrentTutorial()
    {
        mHighlight.gameObject.SetActive(false);
        mNextButton.gameObject.SetActive(false);

        if (mStepIndex >= mTutoData.tutoStepList.Count) { return; }
        mCurrentStepData = mTutoData.tutoStepList[mStepIndex];
        if(mCurrentStepData == null) { return; }

        switch (mCurrentStepData.StepType)
        {
            case CLICK_TYPE:
                if(mNextButton == null) { return; }
                mNextButton.SetTutoInfo(mCurrentStepData);
                break;
            case SWAP_TYPE:
                SelectCoordi = mCurrentStepData.SelectCoordi;
                TargetCoordi = mCurrentStepData.TargetCoordi;

                mHighlight.SetMaskPositoin(mCurrentStepData, true);
                IsSwapMode = true;
                break;
            case ITEM_TYPE:
                //해당 아이템만 On, 아이템이 사용되면 원상 복귀 됩니다.
                //Item의 경우 지정 된 아이템 버튼만 활성화 시킵니다.
                break;
        }
        mStepIndex += 1;
    }
    public void OnNextTutoButtonClicked()
    {
        mNextButton.gameObject.SetActive(false);
        ProgressCurrentTutorial();
    }

}
