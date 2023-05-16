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
        PlayerData.GetGold(ItemManager.Instance.InstGold);

        PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.Loading);
        ClosePopup();
    }
    public void OnQuitGameButtonClicked()
    {
        ClosePopup();
        SceneLoader.Instance.LoadSceneByName("LobbyScene");
        //Application.Quit();
    }
}
