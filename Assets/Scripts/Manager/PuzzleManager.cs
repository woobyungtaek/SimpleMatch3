using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EGameState
{
    None,
    Loading,
    GameReady,
    GameStart,
    TileReadyCheck,
    Drop,
    MatchCheck,
    Match,
    ResultCheck,
    Input,
    MatchSwap,
    ReturnSwap,
    StageSuccess,
    StageFail,
    PlayerSkill,
    Pause,
    Max
}

public class PuzzleManager : SceneSingleton<PuzzleManager>
{
    [SerializeField] private EGameState mCurrentState = EGameState.None;
    private string mCurrentStateString;
    public EGameState CurrentState
    {
        get
        {
            return mCurrentState;
        }
        set
        {
            mCurrentState = value;
            mCurrentStateString = GetGameStateString(value);
        }
    }

    private Dictionary<string, List<EGameState>> gameStateConditionDict = new Dictionary<string, List<EGameState>>();
    private Dictionary<string, EGameState> stringGameStateDict = new Dictionary<string, EGameState>();

    private void Start()
    {
        ObserverCenter observerCenter = ObserverCenter.Instance;
        observerCenter.AddObserver(ExecuteGameReadyByNoti,   EGameState.GameReady.ToString());
        observerCenter.AddObserver(ExecuteGameStartByNoti,   EGameState.GameStart.ToString());
        observerCenter.AddObserver(ExcuteStageSuccessByNoti, EGameState.StageSuccess.ToString());
        observerCenter.AddObserver(ExcuteStageFailByNoti,    EGameState.StageFail.ToString());

        CreateGameStateConditionAndStringDictInternal();

        ChangeCurrentGameStateWithNoti(EGameState.Loading);
    }

#if UNITY_EDITOR
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    ChangeCurrentGameStateWithNoti(EGameState.TileReadyCheck);
        //}
        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    ChangeCurrentGameStateWithNoti(EGameState.Drop);
        //}
        if (Input.GetKeyDown(KeyCode.N))
        {
            TileMapManager.Instance.IsNextStep = true;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            TileMapManager.Instance.ShuffleAllBlockContianer();
        }
    }
#endif

    private void CreateGameStateConditionAndStringDictInternal()
    {
        gameStateConditionDict.Clear();

        foreach (EGameState gameState in System.Enum.GetValues(typeof(EGameState)))
        {
            if (stringGameStateDict.ContainsKey(gameState.ToString())) { continue; }
            stringGameStateDict.Add(gameState.ToString(), gameState);
        }

        AddGameStateInConditionDictListInternal(EGameState.None, EGameState.Loading);

        AddGameStateInConditionDictListInternal(EGameState.Loading, EGameState.GameReady);

        AddGameStateInConditionDictListInternal(EGameState.GameReady, EGameState.GameStart);

        AddGameStateInConditionDictListInternal(EGameState.GameStart, EGameState.TileReadyCheck);

        AddGameStateInConditionDictListInternal(EGameState.TileReadyCheck, EGameState.Drop);
        AddGameStateInConditionDictListInternal(EGameState.TileReadyCheck, EGameState.MatchCheck);

        AddGameStateInConditionDictListInternal(EGameState.Drop, EGameState.MatchCheck);
        AddGameStateInConditionDictListInternal(EGameState.MatchCheck, EGameState.Match);
        AddGameStateInConditionDictListInternal(EGameState.MatchCheck, EGameState.ResultCheck);

        AddGameStateInConditionDictListInternal(EGameState.Match, EGameState.TileReadyCheck);

        AddGameStateInConditionDictListInternal(EGameState.ResultCheck, EGameState.ReturnSwap);
        AddGameStateInConditionDictListInternal(EGameState.ResultCheck, EGameState.StageSuccess);
        AddGameStateInConditionDictListInternal(EGameState.ResultCheck, EGameState.StageFail);

        AddGameStateInConditionDictListInternal(EGameState.Input, EGameState.MatchSwap);
        AddGameStateInConditionDictListInternal(EGameState.Input, EGameState.MatchCheck);
        AddGameStateInConditionDictListInternal(EGameState.Input, EGameState.PlayerSkill);

        AddGameStateInConditionDictListInternal(EGameState.PlayerSkill, EGameState.Input);
        AddGameStateInConditionDictListInternal(EGameState.PlayerSkill, EGameState.Match);

        AddGameStateInConditionDictListInternal(EGameState.MatchSwap, EGameState.MatchCheck);

        AddGameStateInConditionDictListInternal(EGameState.StageSuccess, EGameState.TileReadyCheck);
        //AddGameStateInConditionDictListInternal(EGameState.StageSuccess, EGameState.Drop);
        AddGameStateInConditionDictListInternal(EGameState.StageSuccess, EGameState.Loading);
        AddGameStateInConditionDictListInternal(EGameState.StageFail, EGameState.Loading);

        AddGameStateInConditionDictListInternal(EGameState.Pause, EGameState.Loading);

        AddGameStateInConditionDictListInternal(EGameState.ReturnSwap, EGameState.Input);

    }
    private void AddGameStateInConditionDictListInternal(EGameState currentState, EGameState nextState)
    {
        mCurrentStateString = GetGameStateString(currentState);
        if (!(gameStateConditionDict.ContainsKey(mCurrentStateString)))
        {
            gameStateConditionDict.Add(mCurrentStateString, new List<EGameState>());
        }
        if (gameStateConditionDict[mCurrentStateString].Contains(nextState)) { return; }
        gameStateConditionDict[mCurrentStateString].Add(nextState);
    }
    private string GetGameStateString(EGameState gameState)
    {
        foreach (KeyValuePair<string, EGameState> checkState in stringGameStateDict)
        {
            if (checkState.Value != gameState) { continue; }
            return checkState.Key;
        }
        return null;
    }

    private void ExecuteGameReadyByNoti(Notification noti)
    {
        ChangeCurrentGameStateWithNoti(EGameState.GameStart);
        ChangeCurrentGameStateWithNoti(EGameState.TileReadyCheck);
    }
    private void ExecuteGameStartByNoti(Notification noti)
    {
        //ChangeCurrentGameStateWithNoti(EGameState.TileReadyCheck);
    }
    private void ExcuteStageSuccessByNoti(Notification noti)
    {
        StartCoroutine(StageSuccessCoroutine());
    }
    private IEnumerator StageSuccessCoroutine()
    {
        //공통 실행
        bool bLastStage = MissionManager.Instance.IsLastStageInPart;
        if (MissionManager.Instance.IsLastPart && bLastStage)
        {
            yield return GameConfig.yieldGameEndDuration;
            yield return null;

            MissionManager.Instance.ResetGameInfoByGameOver();
            PopupManager.Instance.CreatePopupByName("AllClearPopup");
           yield break;
        }
        MissionManager.Instance.SetNextStageInfo();


        #region DayClear
        if (bLastStage)
        {
            yield return GameConfig.yieldGameEndDuration;
            yield return null;
            PopupManager.Instance.CreatePopupByName("DayClearPopup");            
        }
        else
        {
            yield return null;
            ObserverCenter.Instance.SendNotification(Message.CharacterEnter);
        }
        #endregion
    }

    private void ExcuteStageFailByNoti(Notification noti)
    {
        //게임 패배 프로세스 실행
        //현재까지의 진행 수
        //사용한 아이템 수, 획득한 블록 개수 등등
        StartCoroutine(StageFailCoroutine());

        //MissionManager.Instance.ResetGameInfoByGameOver();
    }
    private IEnumerator StageFailCoroutine()
    {
        yield return GameConfig.yieldGameEndDuration;
        yield return null;

        // StageFailPopup이 떠야함
        PopupManager.Instance.CreatePopupByName("StageFailPopup");
    }

    public bool ChangeCurrentGameState(EGameState nextState)
    {
        mCurrentStateString = GetGameStateString(CurrentState);
        if (!gameStateConditionDict.ContainsKey(mCurrentStateString)) { Debug.LogWarningFormat("상태 변경 조건이 없는 게임 상태입니다.{0}", CurrentState); return false; }
        if (!gameStateConditionDict[mCurrentStateString].Contains(nextState)) 
        {
            Debug.LogWarningFormat("허용되지 않는 상태 변화입니다.{0} > {1}", CurrentState, nextState);
            return false; 
        }
        CurrentState = nextState;
        return true;
    }
    //public void ChangeCurrentGameStateWithNoti(EGameState nextState, Component senderOrNull = null)
    //{
    //    if (!ChangeCurrentGameState(nextState)) { return; }
    //    ObserverCenter.Instance.SendNotification(senderOrNull, mCurrentStateString);
    //}
    public void ChangeCurrentGameStateWithNoti(EGameState nextState, Component senderOrNull = null, NotificationArgs args = null)
    {
        if (!ChangeCurrentGameState(nextState)) { return; }
        ObserverCenter.Instance.SendNotification(senderOrNull, mCurrentStateString, args);
    }

    public void ChangeCurrenGameStateForce(EGameState nextState)
    {
        mCurrentState = nextState;
    }

    public void RemoveGameEndObseverByTutoManager()
    {
        ObserverCenter.Instance.RemoveObserver(EGameState.StageSuccess.ToString(), ExcuteStageSuccessByNoti);
        ObserverCenter.Instance.RemoveObserver(EGameState.StageFail.ToString(), ExcuteStageFailByNoti);
    }
}
