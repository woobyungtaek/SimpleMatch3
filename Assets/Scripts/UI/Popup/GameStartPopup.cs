using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameStartPopup : Popup
{
    private readonly System.Text.StringBuilder mStrBulider = new System.Text.StringBuilder();

    [Header("Chapter")]
    [SerializeField] private RecycleGridLayout mChapterGrid;
    [SerializeField] private TextMeshProUGUI mChapterNumText;

    [Header("Booster")]
    [SerializeField] private BoosterItemButton[] mBoosterButtons;

    [SerializeField] private GameObject mBoosterInvenInnerPopup;
    [SerializeField] private RecycleGridLayout mBoosterInvenGrid;
    [SerializeField] private TextMeshProUGUI mBoosterEffectText;
    private List<int> mBoosterItemList = new List<int>();

    private BoosterItemButton mCurrentActiveButton;

    public override void Init()
    {
        base.Init();

        //mChapterGrid.Init();
        mChapterNumText.text = $"{LobbySceneManager.Instance.SelectedChapterNum}";

        // 버튼들 초기화
        foreach (var btn in mBoosterButtons)
        {
            btn.Init();
            btn.onClick.AddListener(() => OnBoosterItemButtonClicked(btn));
        }

        // 팝업이 뜰 때 부스터 아이템 List를 만들자
        CreateBoosterItemListByPlayerInven();
    }

    public void OnChapterSelectButtonClicked(int dir)
    {
        LobbySceneManager.Instance.SelectedChapterNum += dir;
        mChapterNumText.text = $"{LobbySceneManager.Instance.SelectedChapterNum}";
    }

    public void OnCloseButtonClicked()
    {
        mChapterGrid.Clear();
        mBoosterInvenGrid.Clear();
        mBoosterInvenInnerPopup.SetActive(false);
        ClosePopup();
    }

    public void OnStartButtonClicked()
    {
        ClosePopup();

        // Inventory Index로 Data 획득, Data의 Index를 넘겨준다.
        // Inventory Index로 개수를 빼준다.
        for (int cnt = 0; cnt < mBoosterButtons.Length; ++cnt)
        {
            LobbySceneManager.Instance.UseBoosterItemArr[cnt] = mBoosterButtons[cnt].CurrentData;
        }

        LobbySceneManager.Instance.ChapterStart();
    }

    private void CreateBoosterItemListByPlayerInven()
    {
        mBoosterItemList.Clear();

        var inven = PlayerData.BoosterItemInventory;

        foreach (var data in inven)
        {
            int count = data.Value;
            for (int cnt = 0; cnt < count; ++cnt)
            {
                mBoosterItemList.Add(data.Key);
            }
        }
    }

    private void OnBoosterItemButtonClicked(BoosterItemButton btn)
    {
        if (mBoosterInvenInnerPopup.activeInHierarchy)
        {
            if (mCurrentActiveButton != btn)
            {
                mCurrentActiveButton.SetButtonByItemIndex(mCurrentActiveButton.CurrentData);
            }
            else
            {
                mCurrentActiveButton.SetButtonByItemIndex(null);
            }
            mCurrentActiveButton = null;

            RefreshBoosterEffectText();
            mBoosterInvenInnerPopup.SetActive(false);
            return;
        }

        mCurrentActiveButton = btn;

        BoosterItemCellUI.ItemList = mBoosterItemList;
        BoosterItemCellUI.ButtonClickFunc = OnBoosterCellUIClicked;

        mBoosterInvenGrid.TotalDataCount = mBoosterItemList.Count;
        mBoosterInvenGrid.Init();

        if (mCurrentActiveButton.CurrentData != null)
        {
            mCurrentActiveButton.SetButtonText("해제하기");
        }

        mBoosterInvenInnerPopup.SetActive(true);
    }
    private void OnBoosterCellUIClicked(BoosterItemData data)
    {
        if (mCurrentActiveButton == null) { return; }

        mCurrentActiveButton.SetButtonByItemIndex(data);
        mCurrentActiveButton = null;

        RefreshBoosterEffectText();
        mBoosterInvenInnerPopup.SetActive(false);
    }

    private void RefreshBoosterEffectText()
    {
        mStrBulider.Clear();
        foreach (var btn in mBoosterButtons)
        {
            var data = btn.CurrentData;
            if (data == null) { continue; }

            mStrBulider.AppendLine(data.EffectDesc);
        }

        mBoosterEffectText.text = mStrBulider.ToString();
    }

    public void OnBoosterInvenCloseButtonClicked()
    {
        RefreshBoosterEffectText();
        mBoosterInvenInnerPopup.SetActive(false);
    }

}
