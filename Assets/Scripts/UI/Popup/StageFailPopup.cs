using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageFailPopup : Popup
{
    public void OnOkButtonClicked()
    {
        ClosePopup();

        PlayerData.AddGold(ItemManager.Instance.InstGold);

        SceneLoader.Instance.LoadSceneByName("LobbyScene");
    }
}
