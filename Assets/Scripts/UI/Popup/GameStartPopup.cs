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

    [SerializeField] private GameObject mBoosterInvenInnerPopup;
    [SerializeField] private RecycleGridLayout mBoosterInvenGrid;
    private List<string> mBoosterItemList = new List<string>();

    private BoosterItemButton mCurrentActiveButton;

    public override void Init()
    {
        base.Init();

        //mChapterGrid.Init();
        mChapterNumText.text = $"{LobbySceneManager.Instance.SelectedChapterNum}";

        // ��ư�� �ʱ�ȭ
        foreach(var btn in mBoosterButtons)
        {
            btn.Init();
            btn.onClick.AddListener(()=> OnBoosterItemButtonClicked(btn));
        }

        // �˾��� �� �� �ν��� ������ List�� ������
        CreateBoosterItemListByPlayerInven();
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
            LobbySceneManager.Instance.UseBoosterItemArr[cnt] = mBoosterButtons[cnt].ItemName;
        }

        LobbySceneManager.Instance.ChapterStart();
    }

    private void CreateBoosterItemListByPlayerInven()
    {
        mBoosterItemList.Clear();

        var inven = PlayerData.BoosterItemInventory;

        foreach(var data in inven)
        {
            int count = data.Value;
            for(int cnt = 0; cnt < count; ++cnt)
            {
                mBoosterItemList.Add(data.Key);
            }
        }
    }

    private void OnBoosterItemButtonClicked(BoosterItemButton btn)
    {
        mCurrentActiveButton = btn;

        BoosterItemCellUI.ItemList = mBoosterItemList;
        BoosterItemCellUI.ButtonClickFunc = OnBoosterCellUIClicked;

        mBoosterInvenGrid.TotalDataCount = mBoosterItemList.Count;
        mBoosterInvenGrid.Init();

        mBoosterInvenInnerPopup.SetActive(true);
    }

    private void OnBoosterCellUIClicked(int itemIndex)
    {
        if(mCurrentActiveButton == null) { return; }

        mCurrentActiveButton.SetButtonByIndex(mBoosterItemList[itemIndex]);
        mCurrentActiveButton = null;

        mBoosterInvenInnerPopup.SetActive(false);
    }
}
