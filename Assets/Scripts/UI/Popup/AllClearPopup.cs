using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllClearPopup : PopupObject
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
        PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.Loading);
        base.OnOkButtonClicked();
    }
    public void OnQuitGameButtonClicked()
    {
        Application.Quit();
    }
}
