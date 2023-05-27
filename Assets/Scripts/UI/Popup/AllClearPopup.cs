using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllClearPopup : Popup
{
    public void OnQuitGameButtonClicked()
    {
        ClosePopup();

        PlayerData.AddGold(ItemManager.Instance.InstGold);

        SceneLoader.Instance.LoadSceneByName("LobbyScene");
    }
}
