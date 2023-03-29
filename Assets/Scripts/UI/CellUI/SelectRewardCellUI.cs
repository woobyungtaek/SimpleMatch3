using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectRewardCellUI : MonoBehaviour
{
    private static string CountStr = "+{0}";

    public RewardData CellRewardData { get => mRewardData; }

    [SerializeField] private Image      mSelectBackImage;
    [SerializeField] private Image      mRewardImage;
    [SerializeField] private Text       mRewardCountText;
    [SerializeField] private RewardData mRewardData;

    public delegate void SelectRewardEvent(SelectRewardCellUI cellIUI);
    public SelectRewardEvent EventSelectReward;

    public void InitCellUI(RewardData rewardData)
    {
        mRewardData = rewardData;
        mRewardImage.sprite = SpriteManager.Instance.GetUISpriteByName(mRewardData.SpriteName);
        mRewardCountText.text = string.Format(CountStr, mRewardData.RewardCount);
    }
    public void UnSelectCell()
    {
        //mSelectBackImage.color = Color.black;
    }

    public void OnSelectButtonClicked()
    {
       // mSelectBackImage.color = Color.red;
        EventSelectReward(this);
    }
}

