using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using GoogleMobileAds.Api;

public class RewardAdPopup : Popup
{
    [Header("Reward")]
    [SerializeField] private TextMeshProUGUI mAdButtonText;

    private int mMoveCount;

    public override void Init()
    {
        int mMoveCount = 5;
        if (InGameUseDataManager.IsExist)
        {
            mMoveCount = InGameUseDataManager.Instance.MoveCount_Continue;
        }

        mAdButtonText.text = $"광고보고 이어하기\nMove Count + {mMoveCount}";
        base.Init();
    }

    // 광고 보기 버튼 > 
    public void OnShowRewardAdButtonClicked()
    {
        AdsManager.Instance.ShowRewardAd(ContinueRewardFunc);
    }

    private void ContinueRewardFunc(Reward reward)
    {
        //이어하기 시 MoveCount 추가
        TileMapManager.Instance.MoveCount += mMoveCount;

        // 게임 재개 해야함
        PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.ReturnSwap);

        // 파업을 닫아야함
        ClosePopup();
    }

    public void OnCancelButtonClicked()
    {
        // 게임 오버시 카메라 올라가는 부분 부터 되어야함
        ClosePopup();
        ObserverCenter.Instance.SendNotification(Message.CameraUp);
    }
}
