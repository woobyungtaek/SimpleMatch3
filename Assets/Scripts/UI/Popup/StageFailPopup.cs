using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageFailPopup : PopupObject
{
    public override void InitPopup()
    {
        base.InitPopup();
    }
    public override void OnCancelButtonClicked()
    {
        OnOkButtonClicked();
    }
    public override void OnOkButtonClicked()
    {
        StartCoroutine(DelayFadeInEffect());
        //PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.Loading);
        //base.OnOkButtonClicked();
    }
    public void OnQuitGameButtonClicked()
    {
        Application.Quit();
    }
    private IEnumerator DelayFadeInEffect()
    {
        Time.timeScale = 1;
        if (SceneLoader.IsExist)
        {
            SceneLoader.Instance.FadeInOutByExternal(true);
            yield return SceneLoader.Instance.FadeSecond;
        }
        yield return null;
        base.OnOkButtonClicked();
        PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.Loading);
    }
}
