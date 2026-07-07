using UnityEngine;
using GoogleMobileAds.Api;
using System;
using PlayFab;
using PlayFab.ClientModels;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    // ── Replace with your real Ad Unit IDs from AdMob dashboard ──
    // For testing, use Google's test IDs below
    private const string BANNER_AD_UNIT_ANDROID   = "ca-app-pub-3940256099942544/6300978111"; // test
    private const string BANNER_AD_UNIT_IOS       = "ca-app-pub-3940256099942544/2934735716"; // test
    private const string INTERSTITIAL_UNIT_ANDROID = "ca-app-pub-3940256099942544/1033173712"; // test
    private const string INTERSTITIAL_UNIT_IOS     = "ca-app-pub-3940256099942544/4411468910"; // test

    private const string RemoveAdsKey = "RemoveAds";

    private BannerView bannerView;
    private InterstitialAd interstitialAd;

    /// <summary>True once this player has "purchased" ad removal. Synced with PlayFab Player Data.</summary>
    public bool AdsRemoved { get; private set; } = false;

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
        if (AdsRemoved) return; // don't even load banners for a paying player

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

    public void ShowBanner()
    {
        if (AdsRemoved) return;
        bannerView?.Show();
    }

    public void HideBanner() => bannerView?.Hide();

    // ── Interstitial ──────────────────────────────────────────────

    public void LoadInterstitial()
    {
        if (AdsRemoved) return; // don't preload interstitials for a paying player

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
        if (AdsRemoved)
        {
            // Paying player — skip straight through
            onClosed?.Invoke();
            return;
        }

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

    // ── Remove Ads: PlayFab sync ─────────────────────────────────

    /// <summary>
    /// Applies the ads-removed state locally (hides banner if needed).
    /// Call this once right after login, using the value read from PlayFab.
    /// </summary>
    public void SetAdsRemoved(bool removed)
    {
        AdsRemoved = removed;
        Debug.Log("[AdManager] AdsRemoved set to: " + removed);

        if (removed)
        {
            HideBanner();
        }
    }

    /// <summary>
    /// Reads the RemoveAds flag from this player's PlayFab account.
    /// Call this right after the player logs in / signs up / auto-logs in.
    /// </summary>
    public void FetchAdsRemovedStatus()
    {
        var request = new GetUserDataRequest
        {
            Keys = new System.Collections.Generic.List<string> { RemoveAdsKey }
        };

        PlayFabClientAPI.GetUserData(request, result =>
        {
            bool removed = result.Data != null
                           && result.Data.ContainsKey(RemoveAdsKey)
                           && result.Data[RemoveAdsKey].Value == "true";

            SetAdsRemoved(removed);
        },
        error =>
        {
            Debug.LogError("Failed to fetch RemoveAds status: " + error.GenerateErrorReport());
        });
    }

    /// <summary>
    /// DUMMY purchase — no real transaction/IAP yet. Wire this directly to your
    /// "Remove Ads" button. Immediately marks the account as ads-removed both
    /// locally and on PlayFab.
    /// </summary>
    public void OnRemoveAdsButtonClicked()
    {
        var request = new UpdateUserDataRequest
        {
            Data = new System.Collections.Generic.Dictionary<string, string>
            {
                { RemoveAdsKey, "true" }
            },
            Permission = UserDataPermission.Private
        };

        PlayFabClientAPI.UpdateUserData(request, result =>
        {
            Debug.Log("RemoveAds flag saved to PlayFab.");
            SetAdsRemoved(true);
        },
        error =>
        {
            Debug.LogError("Failed to save RemoveAds flag: " + error.GenerateErrorReport());
        });
    }

    void OnDestroy()
    {
        bannerView?.Destroy();
        interstitialAd?.Destroy();
    }
}