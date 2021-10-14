using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePopup : PopupObject
{
    public override void InitPopup()
    {
        ObserverCenter.Instance.SendNotification(Message.PauseGame);
        Time.timeScale = 0;
        base.InitPopup();
    }
    public override void OnCancelButtonClicked()
    {
        ObserverCenter.Instance.SendNotification(Message.ResumeGame);
        Time.timeScale = 1;
        base.OnCancelButtonClicked();
    }

    public void OnDayRestartButtonClicked()
    {
        MissionManager.Instance.ResetGameInfoByDay();
        StartCoroutine(DelayFadeInEffect());
    }
    public void OnRestartButtonClicked()
    {
        MissionManager.Instance.ResetGameInfoByGameOver();
        StartCoroutine(DelayFadeInEffect());
    }
    public void OnTitleButtonClicked()
    {
        base.OnOkButtonClicked();

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

    private IEnumerator DelayFadeInEffect()
    {
        if (SceneLoader.IsExist)
        {
            SceneLoader.Instance.FadeInOutByExternal(true);
            yield return SceneLoader.Instance.FadeSecond;
        }
        yield return null;
        base.OnOkButtonClicked();
        PuzzleManager.Instance.ChangeCurrenGameStateForce(EGameState.Pause);
        PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.Loading);
    }
}
