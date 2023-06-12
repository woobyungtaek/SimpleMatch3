using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageFailPopup : Popup
{
    [SerializeField] private TMPro.TextMeshProUGUI mResultText;

    [SerializeField] private LevelStateUI mLevelUI;

    public override void Init()
    {
        base.Init();

        // È¹µæ Exp °è»ê
        int start = PlayerData.CurrentExp;
        int end = start + ItemManager.Instance.InstExp;

        mLevelUI.PlayExpIncrease(start, end);
        // Exp Áõ°¡

        //È¹µæ ÇÒ ±ÝÈ­ Ç¥½Ã
        mResultText.text = $"Gold +{ItemManager.Instance.InstGold}";
    }

    public void OnOkButtonClicked()
    {
        ClosePopup();

        // °á°ú ÀúÀå
        PlayerData.AddGold(ItemManager.Instance.InstGold);
        PlayerData.AddExp(ItemManager.Instance.InstExp);

        SceneLoader.Instance.LoadSceneByName("LobbyScene");
    }
}
