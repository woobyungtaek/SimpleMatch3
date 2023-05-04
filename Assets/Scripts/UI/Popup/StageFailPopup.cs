using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageFailPopup : Popup
{
    public void OnOkButtonClicked()
    {
        ClosePopup();
        SceneLoader.Instance.LoadSceneByName("LobbyScene");
    }
}
