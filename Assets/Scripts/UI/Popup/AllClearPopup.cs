using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllClearPopup : Popup
{
    public void OnCancelButtonClicked()
    {
        OnOkButtonClicked();
    }
    public void OnOkButtonClicked()
    {
        PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.Loading);
        ClosePopup();
    }
    public void OnQuitGameButtonClicked()
    {
        ClosePopup();

        if (!SceneLoader.IsExist)
        {
            Instantiate(Resources.Load("Prefabs/SceneLoader"));
        }
        SceneLoader.Instance.LoadSceneByName("TitleScene");
        //Application.Quit();
    }
}
