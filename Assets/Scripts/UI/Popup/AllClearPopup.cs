using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllClearPopup : Popup
{
    public void OnQuitGameButtonClicked()
    {
        ClosePopup();
        PlayerData.GetGold(ItemManager.Instance.InstGold);
        SceneLoader.Instance.LoadSceneByName("LobbyScene");
        //Application.Quit();
    }
}
