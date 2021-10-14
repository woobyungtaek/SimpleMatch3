using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    private const string MAP_DATA_FOLDER_PATH = "MapData/";
    private const string TUTO_DATA_FORMAT = "TutoData/{0}";

    [SerializeField] private string mConceptName;
    [SerializeField] private string mMapName;

    [SerializeField] private MapData mMapData;
    private TutorialData mTutoData;
    private StringBuilder mLoadMapStrBuilder = new StringBuilder();

    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        ObserverCenter.Instance.AddObserver(ExecuteChagneMapDataInfoByNoti, Message.ChangeMapInfo);
        ObserverCenter.Instance.AddObserver(ExecuteLoadGameByNoti, EGameState.Loading.ToString());   
    }

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
        MissionManager.Instance.CreateCurrentStageInfo();

        if (mTutoData != null)
        {
            TutorialManager.Instance.SetTutorialData(mTutoData);
            TutorialManager.Instance.StartTutorial();
        }
        else
        {
            MissionManager.Instance.StartStage();
        }

        //PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.GameReady);
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
        PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.GameReady);
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
        if(mMapData == null)
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

    public void OnPauseButtonClicked()
    {
        PausePopup instPausePopup =
            GameObjectPool.Instantiate<PausePopup>(PopupManager.Instance.PausePopup, PopupManager.Instance.PopupTransform);
        instPausePopup.InitPopup();
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
