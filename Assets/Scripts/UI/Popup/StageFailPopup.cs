using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageFailPopup : Popup
{
    [SerializeField] private TMPro.TextMeshProUGUI mResultText;

    public override void Init()
    {
        base.Init();

        //È¹µæ ÇÒ ±ÝÈ­ Ç¥½Ã
        mResultText.text = $"Gold +{ItemManager.Instance.InstGold}";
    }

    public void OnOkButtonClicked()
    {
        ClosePopup();

        PlayerData.AddGold(ItemManager.Instance.InstGold);

        SceneLoader.Instance.LoadSceneByName("LobbyScene");
    }
}
