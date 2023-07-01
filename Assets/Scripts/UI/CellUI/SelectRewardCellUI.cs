using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectRewardCellUI : MonoBehaviour
{
    private static string CountStr = "+{0}";

    public RewardData CellRewardData { get => mRewardData; }

    [SerializeField] private Image      mSelectBackImage;
    [SerializeField] private Image      mRewardImage;
    [SerializeField] private TextMeshProUGUI mRewardCountText;
    [SerializeField] private TextMeshProUGUI mItemNameText;
    [SerializeField] private TextMeshProUGUI mItemDescText;
    [SerializeField] private RewardData mRewardData;

    [SerializeField] private GameObject mAdInfoObj;


    public delegate void SelectRewardEvent(SelectRewardCellUI cellIUI);
    public SelectRewardEvent EventSelectReward;

    public virtual void InitCellUI(RewardData rewardData)
    {
        mRewardData = rewardData;
        mRewardImage.sprite = SpriteManager.Instance.GetUISpriteByName(mRewardData.SpriteName);
        mItemNameText.text = Localization.GetString($"{mRewardData.SpriteName}_Name");
        mItemDescText.text = Localization.GetString($"{mRewardData.SpriteName}_Desc");

        RefreshRewardCountText();

        mAdInfoObj.SetActive(mRewardData.RewardType == ERewardType.AD);
    }

    public void OnSelectButtonClicked()
    {      
        EventSelectReward(this);
    }

    protected void RefreshRewardCountText()
    {
        mRewardCountText.text = string.Format(CountStr, mRewardData.RewardCount);
    }
}

