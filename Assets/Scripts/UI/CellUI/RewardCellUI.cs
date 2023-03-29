using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WBTWeen;

public class RewardCellUI : MonoBehaviour
{
    private const string TEXT_STR_FORMAT = "+{0}";

    private object[] mRewardParam = new object[1];

    public RewardData CurrentRewardData
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

        transform.MoveLocal(Vector3.up * 200f, 1f)
            .OnComplete(()=> {
                mRewardParam[0] = mRewardData;
                mRewardData.RewardMethodInfo.Invoke(null, mRewardParam);

                GameObjectPool.ReturnObject(gameObject);
            });
    }
}
