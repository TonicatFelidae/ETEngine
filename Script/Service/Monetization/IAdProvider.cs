using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace ET.Monetization
{
    /// <summary>
    /// Abstract provider interface for ad networks (AppLovin, Google Ads, Unity Ads, etc.).
    /// Encapsulates all SDK-specific logic away from ETEngine core systems.
    /// </summary>
    public interface IAdProvider
    {
        string ProviderName { get; }
        bool IsInitialized { get; }
        bool IsRewardedReady { get; }
        bool IsInterstitialReady { get; }
        bool IsBannerShowing { get; }
        bool IsMRecShowing { get; }

        void Initialize();

        // Banner
        void ShowBanner(string placement = "");
        void HideBanner();
        float GetBannerHeight(Rect canvasRect = default);

        // Interstitial
        void ShowInterstitial(UnityAction onCompleted = null, UnityAction onFailed = null, string placement = "", string requestId = null);
        void LoadInterstitial();
        UniTask<bool> WaitForInterstitialReadyAsync(float timeoutSeconds = 10f);

        // Rewarded
        void ShowRewarded(UnityAction onCompleted, UnityAction onFailed, string placement = "", string requestId = null, long waitMs = 0);
        void LoadRewarded();
        UniTask<bool> WaitForRewardedReadyAsync(float timeoutSeconds = 10f);

        // MREC
        void ShowMRec(Vector2 unityPosition, Vector2 nativePosition);
        void HideMRec();
        void LoadMRec();

        // Events
        event Action<AdImpressionData> OnAdImpression;
        event Action<string> OnCountryCodeResolved;
        event Action OnRewardedCompleted;
        event Action OnRewardedFailed;
        event Action OnRewardedShown;
        event Action OnRewardedEnded;
        event Action OnInterstitialCompleted;
        event Action OnInterstitialFailed;
        event Action OnInterstitialShown;
        event Action OnInterstitialEnded;
        event Action OnBannerLoaded;
        event Action OnBannerFailed;
        event Action OnMRecLoaded;
        event Action OnMRecFailed;
    }
}
