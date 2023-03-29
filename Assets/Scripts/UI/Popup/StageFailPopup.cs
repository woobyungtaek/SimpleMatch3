using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageFailPopup : Popup
{
    public void OnCancelButtonClicked()
    {
        OnOkButtonClicked();
    }
    public void OnOkButtonClicked()
    {
        ClosePopup(false);

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
