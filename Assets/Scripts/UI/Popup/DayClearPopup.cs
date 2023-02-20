using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayClearPopup : Popup
{
    public void OnCancelButtonClicked()
    {
        OnOkButtonClicked();
    }
    public void OnOkButtonClicked()
    {
        StartCoroutine(DelayFadeInEffect());
        //PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.Loading);
        //base.OnOkButtonClicked();
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
        ClosePopup(false);
        PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.Loading);
    }
}
