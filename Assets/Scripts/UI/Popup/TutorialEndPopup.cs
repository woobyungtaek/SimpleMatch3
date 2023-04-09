using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;

public class TutorialEndPopup : Popup
{
    private const string TUTO_FOLDER_NAME = "Tutorial";
    private const string TUTO_MAP_FILE_FORMAT = "TutorialMap_{0}";

    private int mCurrentIndex;
    private int mNextIndex;

    public override void Init()
    {
        base.Init();

        //현재 튜토리얼 번호에 따라서 마지막 튜토리얼인 경우 Next버튼은 안보여야 한다.
        mCurrentIndex = TutorialManager.Instance.TutoIndex;
        mNextIndex = mCurrentIndex + 1;
        if (mNextIndex >= TutorialManager.TUTORIAL_COUNT)
        {
            mNextIndex = -1;
        }
    }

    public void OnCancelButtonClicked()
    {
        OnTitleSceneButtonClicked();
    }

    public void OnNextTutorialButtonClicked()
    {
        if(mNextIndex < 0)
        {
            OnCancelButtonClicked();
            return;
        }
        MapDataInfoNotiArg data = new MapDataInfoNotiArg();
        data.ConceptName = TUTO_FOLDER_NAME;
        data.MapName = string.Format(TUTO_MAP_FILE_FORMAT, mNextIndex);
        ObserverCenter.Instance.SendNotification(Message.ChangeMapInfo, data);

        PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.Loading);
        ClosePopup();
    }
    public void OnRestartButtonClicked()
    {
        MapDataInfoNotiArg data = new MapDataInfoNotiArg();
        data.ConceptName = TUTO_FOLDER_NAME;
        data.MapName = string.Format(TUTO_MAP_FILE_FORMAT, mCurrentIndex);
        ObserverCenter.Instance.SendNotification(Message.ChangeMapInfo, data);

        PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.Loading);
        ClosePopup();
    }
    public void OnTitleSceneButtonClicked()
    {
        if (!SceneLoader.IsExist)
        {
            Instantiate(Resources.Load("Prefabs/SceneLoader"));
        }
        SceneLoader.Instance.LoadSceneByName("TitleScene");
    }
    public void OnQuitGameButtonClicked()
    {
        Application.Quit();
    }
}
