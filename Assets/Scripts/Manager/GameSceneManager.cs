using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using WBTWeen;

public class GameSceneManager : MonoBehaviour
{
    private const string MAP_DATA_FOLDER_PATH = "MapData/";
    private const string TUTO_DATA_FORMAT = "TutoData/{0}";

    [Header("Stage Data")]
    [SerializeField] private string mConceptName;
    [SerializeField] private string mMapName;

    [SerializeField] private MapData mMapData;
    private TutorialData mTutoData;
    private StringBuilder mLoadMapStrBuilder = new StringBuilder();

    [Header("Game Scene Direction")]
    [SerializeField] private Camera mMainCamera;
    [SerializeField] private GameObject mCharacter;
    private Vector3 mGamePos = new Vector3(0, -5f, -10f);
    private Vector3 mUpPos = new Vector3(0, 0f, -10f);

    [Header("Animation Curve")]
    [SerializeField] private AnimationCurve mCurve_Order;

    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        ObserverCenter.Instance.AddObserver(ExecuteChagneMapDataInfoByNoti, Message.ChangeMapInfo);
        ObserverCenter.Instance.AddObserver(ExecuteLoadGameByNoti, EGameState.Loading.ToString());

        ObserverCenter.Instance.AddObserver(Excute_CameraUp, Message.CameraUp);
        ObserverCenter.Instance.AddObserver(Excute_CharacterEnter, Message.CharacterEnter);
        ObserverCenter.Instance.AddObserver(Excute_CharacterOut, Message.CharacterOut);

        mMainCamera = Camera.main;
        mMainCamera.transform.position = mUpPos;
    }

    // 데이터 로드
    private void ExecuteLoadGameByNoti(Notification noti)
    {
        Time.timeScale = 1;
        if (PlayDataManager.IsExist)
        {
            mConceptName = PlayDataManager.Instance.ConceptName;
            mMapName = PlayDataManager.Instance.MapName;
        }

        #region Restart시 선행 되어야 하는것들

        TileMapManager.Instance.AllStopCoroutine();
        MissionManager.Instance.ClearAllMissionCollectEffect();
        BlockManager.Instance.ClearAllBlockContainers();

        #endregion

        LoadMapDataInternal();
        LoadTutoDataInternal();

        TileMapManager.Instance.CreateMapByMapData(mMapData);
        MissionManager.Instance.ResetGameInfoByMapData(mMapData);
        MissionManager.Instance.CreateDayStageInfo();

        if (mTutoData != null)
        {
            TutorialManager.Instance.SetTutorialData(mTutoData);
            TutorialManager.Instance.StartTutorial();
        }
        else
        {
            MissionManager.Instance.RefreshDayOrderUI();
            //MissionManager.Instance.StartStage();
        }

        StartCoroutine(DelayStartByFadeOutEffect());
    }
    private IEnumerator DelayStartByFadeOutEffect()
    {
        Time.timeScale = 1;
        if (SceneLoader.IsExist)
        {
            SceneLoader.Instance.FadeInOutByExternal(false);
            yield return SceneLoader.Instance.FadeSecond;
        }
        yield return null;
        Sequence_CharacterEnter();
    }
    private void ExecuteChagneMapDataInfoByNoti(Notification noti)
    {
        MapDataInfoNotiArg data = noti.Data as MapDataInfoNotiArg;

        if (PlayDataManager.IsExist)
        {
            if (!string.IsNullOrEmpty(data.ConceptName))
            {
                PlayDataManager.Instance.ConceptName = data.ConceptName;
            }
            PlayDataManager.Instance.MapName = data.MapName;
        }
        if (!string.IsNullOrEmpty(data.ConceptName))
        {
            mConceptName = data.ConceptName;
        }
        mMapName = data.MapName;
    }

    private void LoadMapDataInternal()
    {
        mLoadMapStrBuilder.Clear();

        mLoadMapStrBuilder.Append(MAP_DATA_FOLDER_PATH);
        mLoadMapStrBuilder.Append(mConceptName);
        if (!string.IsNullOrEmpty(mConceptName)) { mLoadMapStrBuilder.Append('/'); }

        mLoadMapStrBuilder.Append(mMapName);

        mMapData = Utility.LoadJsonFile<MapData>(mLoadMapStrBuilder.ToString());
        if (mMapData == null)
        {
            Debug.Log("맵데이터가 제대로 로드되지 않았습니다.");
        }
    }
    private void LoadTutoDataInternal()
    {
        if (string.IsNullOrEmpty(mMapData.tutoName)) { return; }
        string loadTutoName = string.Format(TUTO_DATA_FORMAT, mMapData.tutoName);
        mTutoData = Utility.LoadJsonFile<TutorialData>(loadTutoName);
    }

    // 이벤트 콜백
    public void OnPauseButtonClicked()
    {
        PopupManager.Instance.CreatePopupByName("PausePopup");
    }


    // 연출
    private void Sequence_CharacterEnter()
    {
        // 등장 시퀀스
        mCharacter.transform.localPosition = new Vector3(0, 4f, 0);
        mCharacter.transform.localScale = Vector3.one * 0.5f;
        mCharacter.transform.Scale(Vector3.one, 1f);
        mCharacter.transform.MoveLocal(new Vector3(0, 3, 0), 1f)
            .OnComplete(() => { Sequence_CharacterOrder(); });
    }

    private void Sequence_CharacterOrder()
    {
        MissionManager.Instance.SetStageInfo();
        mCharacter.transform.Scale(Vector3.one * 1.2f, 1f).SetEase(mCurve_Order)
            .OnComplete(() =>
            {
                Sequence_CameraDown();
            });
    }

    private void Sequence_CharacterGetPotion()
    {
        // 보상 생성
        MissionManager.Instance.CreateStageClearRewardDataList();

        mCharacter.transform.Scale(Vector3.one * 1.2f, 1f).SetEase(mCurve_Order)
            .OnComplete(() =>
            {
                MissionManager.Instance.TakeStageClearReward();
            });
    }
    private void Sequence_CharacterOut()
    {
        mCharacter.transform.MoveLocal(mCharacter.transform.localPosition + new Vector3(-5, 0, 0), 1f)
            .OnComplete(() => {
                PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.StageSuccess);
            });
    }

    private void Sequence_CameraDown()
    {
        mMainCamera.transform.Move(mGamePos, 1f).OnComplete(() =>
            {
                if (PuzzleManager.Instance.CurrentState == EGameState.StageSuccess)
                {
                    MissionManager.Instance.StartStage();// 이후에는 
                }
                else
                {
                    MissionManager.Instance.StartStage();
                    PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.GameReady); // 1번째만 게임레디
                }
            });
    }

    private void Excute_CameraUp(Notification noti)
    {
        mMainCamera.transform.Move(mUpPos, 1f)
            .OnComplete(() =>
            {
                CheckGameOver();
            });
    }
    private void Excute_CharacterEnter(Notification noti)
    {
        Sequence_CharacterEnter();
    }

    private void Excute_CharacterOut(Notification noti)
    {
        Sequence_CharacterOut();
    }

    private void CheckGameOver()
    {
        MissionManager.Instance.ClearMissionCellUIList();
        if (MissionManager.Instance.IsMissionClear)
        {
            Sequence_CharacterGetPotion();
        }
        else
        {
            PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.StageFail);
        }
    }
}
public class MapDataInfoNotiArg : NotificationArgs
{
    public string ConceptName;
    public string MapName;

    public override void Dispose()
    {
        ConceptName = null;
        MapName = null;
    }
}
