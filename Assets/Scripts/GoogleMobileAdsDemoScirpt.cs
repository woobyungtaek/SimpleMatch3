using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

public class GoogleMobileAdsDemoScirpt : Singleton<GoogleMobileAdsDemoScirpt>
{
    private BannerView bannerView;
    private RewardedAd rewardedAd;

    private void Start()
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
        });
    }

    private void CreateRewardAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        var adRequest = new AdRequest.Builder().Build();

        RewardedAd.Load("ca-app-pub-4158526353894227/1047021968", adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    return;
                }

                rewardedAd = ad;
            });
    }

    public void ShowRewardAd()
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) => { Debug.Log($"{reward.Type},{reward.Amount}"); });
            rewardedAd.Destroy();
            rewardedAd = null;
        }
        else
        {
            CreateRewardAd();
        }
    }

    private void CreateBannerView()
    {
        DestroyBannerView();
        bannerView = new BannerView("ca-app-pub-3940256099942544/6300978111", AdSize.Banner, AdPosition.Top);
    }
    private void DestroyBannerView()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
        }
    }

    public void LoadAd()
    {
        if (bannerView == null)
        {
            CreateBannerView();
        }

        var adRequest = new AdRequest.Builder().AddKeyword("unity-admob-sample").Build();
        bannerView.LoadAd(adRequest);
    }
}
