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
        Application.Quit();
    }
}
