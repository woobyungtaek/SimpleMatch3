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

    public delegate void SelectRewardEvent(SelectRewardCellUI cellIUI);
    public SelectRewardEvent EventSelectReward;

    public void InitCellUI(RewardData rewardData)
    {
        mRewardData = rewardData;
        mRewardImage.sprite = SpriteManager.Instance.GetUISpriteByName(mRewardData.SpriteName);
        mItemNameText.text = Localization.GetString($"{mRewardData.SpriteName}_Name");
        mItemDescText.text = Localization.GetString($"{mRewardData.SpriteName}_Desc");
        mRewardCountText.text = string.Format(CountStr, mRewardData.RewardCount);
    }
    public void OnSelectButtonClicked()
    {
        EventSelectReward(this);
    }
}

