using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WBTWeen;
using TMPro;

public class RewardCellUI : MonoBehaviour
{
    private const string TEXT_STR_FORMAT = "+{0}";


    public RewardData CurrentRewardData
    {
        get => mRewardData;
    }
    [SerializeField] private RewardData mRewardData;
    [SerializeField] private Image mRewardImage;
    [SerializeField] private TextMeshProUGUI mRewardText;

    public void InitCellUI(RewardData rewardData)
    {
        mRewardData = rewardData;
        mRewardImage.sprite = SpriteManager.Instance.GetUISpriteByName(mRewardData.SpriteName);

        //reward의 카운트가 아니라 개수를 받아와야할 때도 있다...

        mRewardText.text = string.Format(TEXT_STR_FORMAT, mRewardData.RewardCount);

        transform.MoveLocal(Vector3.up * 200f, 1f)
            .OnComplete(()=> {
                mRewardData.ExcuteRewardFunc();
                GameObjectPool.ReturnObject(gameObject);
            });
    }
}
