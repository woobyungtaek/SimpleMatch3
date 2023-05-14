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


        PlayerData.BoosterItemInventory.Clear();
        for(int cnt = 0; cnt < 100; ++cnt)
        {
            int rnd = Random.Range(0, DataManager.Instance.GetBoosterDataCount);
            PlayerData.AddBoosterItem(rnd);
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
            LobbySceneManager.Instance.UseBoosterItemArr[cnt] = mBoosterButtons[cnt].GetInvenIndex;
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
        BoosterItemCellUI.ItemList = mBoosterItemList;
        mBoosterInvenGrid.TotalDataCount = mBoosterItemList.Count;

        mBoosterInvenGrid.Init();

        mBoosterInvenInnerPopup.SetActive(true);
    }
}
