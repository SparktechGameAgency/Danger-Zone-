using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    // ── Replace with your real Ad Unit IDs from AdMob dashboard ──
    // For testing, use Google's test IDs below
    private const string BANNER_AD_UNIT_ANDROID   = "ca-app-pub-3940256099942544/6300978111"; // test
    private const string BANNER_AD_UNIT_IOS       = "ca-app-pub-3940256099942544/2934735716"; // test
    private const string INTERSTITIAL_UNIT_ANDROID = "ca-app-pub-3940256099942544/1033173712"; // test
    private const string INTERSTITIAL_UNIT_IOS     = "ca-app-pub-3940256099942544/4411468910"; // test

    private BannerView bannerView;
    private InterstitialAd interstitialAd;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize the SDK once
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("AdMob Initialized");
            LoadBanner();
            LoadInterstitial();
        });
    }

    // ── Banner ────────────────────────────────────────────────────

    public void LoadBanner()
    {
        string adUnitId;
#if UNITY_ANDROID
        adUnitId = BANNER_AD_UNIT_ANDROID;
#elif UNITY_IOS
        adUnitId = BANNER_AD_UNIT_IOS;
#else
        adUnitId = "unused";
#endif
        bannerView?.Destroy();
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
        var request = new AdRequest();
        bannerView.LoadAd(request);
    }

    public void ShowBanner()  => bannerView?.Show();
    public void HideBanner()  => bannerView?.Hide();

    // ── Interstitial ──────────────────────────────────────────────

    public void LoadInterstitial()
    {
        string adUnitId;
#if UNITY_ANDROID
        adUnitId = INTERSTITIAL_UNIT_ANDROID;
#elif UNITY_IOS
        adUnitId = INTERSTITIAL_UNIT_IOS;
#else
        adUnitId = "unused";
#endif
        InterstitialAd.Load(adUnitId, new AdRequest(), (ad, error) =>
        {
            if (error != null) { Debug.LogError("Interstitial load error: " + error); return; }
            interstitialAd = ad;
            Debug.Log("Interstitial loaded");
        });
    }

    public void ShowInterstitial(Action onClosed = null)
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                LoadInterstitial(); // preload next one
                onClosed?.Invoke();
            };
            interstitialAd.Show();
        }
        else
        {
            // Ad not ready — just proceed
            onClosed?.Invoke();
            LoadInterstitial();
        }
    }

    void OnDestroy()
    {
        bannerView?.Destroy();
        interstitialAd?.Destroy();
    }
}