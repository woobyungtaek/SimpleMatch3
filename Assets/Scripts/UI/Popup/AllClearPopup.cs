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

    [SerializeField] private LevelStateUI mLevelUI;

    [SerializeField] private Button mADButton;

    private int mAdBuff;
    private int mResultGold;
    private int mResultExp;

    public override void Init()
    {
        base.Init();

        mResultGold = 0;
        mResultExp = 0;
        mAdBuff = 1;

        // 골드 계산
        SetResultGold();

        // 획득 Exp 계산
        SetResultExp();
        // Exp 증가 애니메이션
        PlayExpIncrease();

        // 광고 버튼 갱신
        RefreshAdButton();

    }

    private void SetResultGold()
    {
        mStrBuilder.Clear();

        // 획득한 금화
        mResultGold = ItemManager.Instance.InstGold;
        mStrBuilder.AppendLine($"Gold +{ItemManager.Instance.InstGold}");

        // 추가 금화 %
        if (InGameUseDataManager.IsExist)
        {
            int allClearGold = InGameUseDataManager.Instance.CurrentChapterData.AllClearGold;
            mResultGold += allClearGold;
            // 완료 보상 금화
            mStrBuilder.AppendLine($"All Clear bonus +{allClearGold}");

            float per = InGameUseDataManager.Instance.AdditoryGoldPer;
            if (per != 0)
            {
                // -일수도 있으므로
                mStrBuilder.AppendLine($"Additory Gold +{per}%");
            }
        }

        // 광고 보고 2배 효과
        if (mAdBuff > 1)
        {
            mStrBuilder.AppendLine($"Ad Double x{mAdBuff}");
        }

        // 최종 획득 표시
        mStrBuilder.AppendLine("-------------------------");
        mResultGold = PlayerData.CalResultGold(mResultGold) * mAdBuff;
        mStrBuilder.AppendLine($"Total : {mResultGold}");

        mResultText.text = mStrBuilder.ToString();
    }

    private void SetResultExp()
    {
        mResultExp = ItemManager.Instance.InstExp;

        // 숫자만 계산 되면 된다.
        if (InGameUseDataManager.IsExist)
        {
            // 경험치 획득량 증가 있는지 확인
        }
    }

    private void PlayExpIncrease()
    {
        int start = PlayerData.CurrentExp;
        int end = start + mResultExp;

        mLevelUI.PlayExpIncrease(start, end);
    }


    #region  광고
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

        // 결과 저장
        PlayerData.AddGold(mResultGold);
        PlayerData.AddExp(mResultExp);

        SceneLoader.Instance.LoadSceneByName("LobbyScene");
    }

}
