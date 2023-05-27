using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

/*
    Google Ads Mob�� �̿��� ���� ���
    #�̵��̼� :  Unity Ads
 */

public class AdsManager : Singleton<AdsManager>
{
    private BannerView mBannserView;
    private RewardedAd mRewardedAd;

#if UNITY_ANDROID
    private string mRewardAdUnitId = "ca-app-pub-4158526353894227/1047021968";
    private string mBannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IOS
    private string mRewardAdUnitId = "ca-app-pub-4158526353894227/1047021968";
    private string mBannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";
#else
    private string mRewardAdUnitId = "ca-app-pub-4158526353894227/1047021968";
    private string mBannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";
#endif

    public void Init()
    {
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            var map = initStatus.getAdapterStatusMap();
            foreach (var pair in map)
            {
                string className = pair.Key;
                AdapterStatus status = pair.Value;
                switch (status.InitializationState)
                {
                    case AdapterState.NotReady:
                        Debug.Log("Adapter: " + className + " not ready.");
                        break;
                    case AdapterState.Ready:
                        Debug.Log("Adapter: " + className + " is initialized.");
                        break;
                }
            }

            CreateRewardAd();
        });
    }

    #region ������ ����
    public bool IsRewardAdReady
    {
        get
        {
            if(mRewardedAd == null) { return false; }
            return mRewardedAd.CanShowAd();
        }

    }

    private void CreateRewardAd()
    {
        if (mRewardedAd != null)
        {
            mRewardedAd.Destroy();
            mRewardedAd = null;
        }

        var adRequest = new AdRequest.Builder().Build();

        RewardedAd.Load(mRewardAdUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    return;
                }

                mRewardedAd = ad;
            });
    }

    public void ShowRewardAd(System.Action<Reward> rewardFunc, System.Action closeFunc = null)
    {
        if (mRewardedAd == null)
        {
            Debug.Log("�˼��մϴ�. �غ�� ���� �����ϴ�.");
            CreateRewardAd();
            return;
        }
        if (!mRewardedAd.CanShowAd())
        {
            Debug.Log("�˼��մϴ�. ���� ���� ǥ�� �� �� �����ϴ�.");
            return;
        }

        // ���� ���� �� �� ���� �ε带 ���� �̺�Ʈ �߰�
        mRewardedAd.OnAdFullScreenContentClosed += ReloadReqedAd_Close;
        if (closeFunc != null)
        {
            mRewardedAd.OnAdFullScreenContentClosed += closeFunc;
        }
        mRewardedAd.OnAdFullScreenContentFailed += ReloadReqedAd_Fail;

        // ���� ���� ��� �� ����
        mRewardedAd.Show(rewardFunc);
        mRewardedAd.Destroy();
        mRewardedAd = null;
    }


    private void ReloadReqedAd_Close()
    {
        Debug.Log("Rewarded Ad full screen content closed.");
        CreateRewardAd();
    }
    private void ReloadReqedAd_Fail(AdError error)
    {
        Debug.LogError($"Rewarded ad failed to open full screen content with error : {error}");
        CreateRewardAd();
    }

    #endregion

    #region ��ʱ���
    private void CreateBannerView()
    {
        DestroyBannerView();
        mBannserView = new BannerView(mBannerAdUnitId, AdSize.Banner, AdPosition.Top);
    }
    private void DestroyBannerView()
    {
        if (mBannserView != null)
        {
            mBannserView.Destroy();
            mBannserView = null;
        }
    }

    public void LoadAd()
    {
        if (mBannserView == null)
        {
            CreateBannerView();
        }

        var adRequest = new AdRequest.Builder().AddKeyword("unity-admob-sample").Build();
        mBannserView.LoadAd(adRequest);
    }
    #endregion
}
