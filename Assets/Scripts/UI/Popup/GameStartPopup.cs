using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameStartPopup : Popup
{
    [Header("Chapter")]
    [SerializeField] private RecycleGridLayout mChapterGrid;
    [SerializeField] private TextMeshProUGUI mChapterNumText;

    [Header("Booster")]
    [SerializeField] private BoosterItemButton[] mBoosterButtons;

    public override void Init()
    {
        base.Init();

        mChapterGrid.Init();
        mChapterNumText.text = $"{LobbySceneManager.Instance.SelectedChapterNum}";

        // 버튼들 초기화
        foreach(var btn in mBoosterButtons)
        {
            btn.Init();
            btn.onClick.AddListener(()=> OnBoosterItemButtonClicked(btn));
        }
    }

    public void OnChapterSelectButtonClicked(int dir)
    {
        LobbySceneManager.Instance.SelectedChapterNum += dir;
        mChapterNumText.text = $"{LobbySceneManager.Instance.SelectedChapterNum}";
    }

    public void OnStartButtonClicked()
    {
        ClosePopup();

        // Inventory Index로 Data 획득, Data의 Index를 넘겨준다.
        // Inventory Index로 개수를 빼준다.
        for(int cnt =0; cnt < mBoosterButtons.Length; ++cnt)
        {
            LobbySceneManager.Instance.UseBoosterItemArr[cnt] = mBoosterButtons[cnt].GetInvenIndex;
        }

        LobbySceneManager.Instance.ChapterStart();
    }

    private void OnBoosterItemButtonClicked(BoosterItemButton btn)
    {
        int count = DataManager.Instance.GetBoosterDataCount;

        int idx = btn.GetInvenIndex;
        idx += 1;
        idx %= count;
        btn.SetButtonByIndex(idx);

        // BoosterItem인벤토리가 떠야한다.
        // View Booster Item Inventory
        // 플레이어의 인벤토리 정보가 필요
    }
}
