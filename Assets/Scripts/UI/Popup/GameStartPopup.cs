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

        // ��ư�� �ʱ�ȭ
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

        // Inventory Index�� Data ȹ��, Data�� Index�� �Ѱ��ش�.
        // Inventory Index�� ������ ���ش�.
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

        // BoosterItem�κ��丮�� �����Ѵ�.
        // View Booster Item Inventory
        // �÷��̾��� �κ��丮 ������ �ʿ�
    }
}
