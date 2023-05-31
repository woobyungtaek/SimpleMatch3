using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;

public class AllClearPopup : Popup
{
    [Header("AllClearPopup")]
    private System.Text.StringBuilder mStrBuilder = new System.Text.StringBuilder();
    [SerializeField] private TMPro.TextMeshProUGUI mResultText;

    [SerializeField] private Button mADButton;

    private int mAdBuff;
    private int mResultGold;

    public override void Init()
    {
        base.Init();

        mResultGold = 0;
        mAdBuff = 1;

        SetResultGold();
        RefreshAdButton();
    }

    private void SetResultGold()
    {
        mStrBuilder.Clear();

        mResultGold = ItemManager.Instance.InstGold;
        // 획득한 금화
        mStrBuilder.AppendLine($"Stage Clear Gold +{ItemManager.Instance.InstGold}");

        // 추가 금화 %
        if (InGameUseDataManager.IsExist)
        {
            mResultGold += InGameUseDataManager.Instance.CurrentChapterData.AllClearGold;
            // 완료 보상 금화
            mStrBuilder.AppendLine($"All Clear Gold +{InGameUseDataManager.Instance.CurrentChapterData.AllClearGold}");

            float per = InGameUseDataManager.Instance.AdditoryGoldPer;
            if (per != 0)
            {
                // -일수도 있으므로
                mStrBuilder.AppendLine($"Additory Gold +{per}%");
            }
        }

        if (mAdBuff > 1)
        {
            mStrBuilder.AppendLine($"Ad Double x{mAdBuff}");
        }
        mStrBuilder.AppendLine("-------------------------");
        mResultGold = PlayerData.CalResultGold(mResultGold) * mAdBuff;
        mStrBuilder.AppendLine($"Total : {mResultGold}");

        mResultText.text = mStrBuilder.ToString();
    }
    private void RefreshAdButton()
    {
        if (mADButton == null) { return; }
        mADButton.onClick.RemoveAllListeners();
        mADButton.onClick.AddListener(OnAdButtonClicked);

        //광고 있는지 확인하고 2배 보상 버튼 켜기
        mADButton.gameObject.SetActive(false);
        if (!AdsManager.IsExist) { return; }
        if (!AdsManager.Instance.IsRewardAdReady) { return; }
        mADButton.gameObject.SetActive(true);
    }

    #region  광고
    private void OnAdButtonClicked()
    {
        AdsManager.Instance.ShowRewardAd(GetDoubleGoldRewardFunc, AdEndFunc);
    }
    private void GetDoubleGoldRewardFunc(Reward reward)
    {
        // 2배로 적용하기
        mAdBuff = 2;
    }
    private void AdEndFunc()
    {
        Invoke("DelayRefresh", 0.2f);
    }
    private void DelayRefresh()
    {
        mADButton.gameObject.SetActive(false);
        SetResultGold();
    }
    #endregion

    public void OnQuitGameButtonClicked()
    {
        ClosePopup();

        mADButton.gameObject.SetActive(false);
        PlayerData.AddGold(mResultGold);

        SceneLoader.Instance.LoadSceneByName("LobbyScene");
    }

}
