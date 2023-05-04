using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GoogleMobileAds.Api;

public class PausePopup : Popup
{
    public override void Init()
    {
        base.Init();
        ObserverCenter.Instance.SendNotification(Message.PauseGame);
        Time.timeScale = 0;
    }

    public void TestBannerLoad()
    {
        AdsManager.Instance.LoadAd();
    }
    public void TestRewardLoad()
    {
        AdsManager.Instance.ShowRewardAd((Reward reward) => { Debug.Log($"{reward.Type} / {reward.Amount}"); });
    }

    public void OnCancelButtonClicked()
    {
        ObserverCenter.Instance.SendNotification(Message.ResumeGame);
        Time.timeScale = 1;
        ClosePopup(true);
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
        ClosePopup();

        SceneLoader.Instance.LoadSceneByName("LobbyScene");
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
        ClosePopup();
        PuzzleManager.Instance.ChangeCurrenGameStateForce(EGameState.Pause);
        PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.Loading);
    }
}
