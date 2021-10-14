using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardCellUI : MonoBehaviour
{
    private const string TEXT_STR_FORMAT = "+{0}";

    public RewardData CurrenRewardData
    {
        get => mRewardData;
    }
    [SerializeField] private RewardData mRewardData;
    [SerializeField] private Image mRewardImage;
    [SerializeField] private Text mRewardText;

    public void InitCellUI(RewardData rewardData)
    {
        mRewardData = rewardData;
        mRewardImage.sprite = SpriteManager.Instance.GetUISpriteByName(mRewardData.SpriteName);
        mRewardText.text = string.Format(TEXT_STR_FORMAT, mRewardData.RewardCount);
    }
}
