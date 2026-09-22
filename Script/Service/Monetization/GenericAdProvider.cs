using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace ET.Monetization
{
    /// <summary>
    /// Generic abstract base class for ad network providers.
    /// Bridges any third-party SDK manager instance (e.g. ApplovinMaxManager) to ETEngine's abstract ad architecture.
    /// </summary>
    /// <typeparam name="TManager">The underlying SDK manager instance type.</typeparam>
    public abstract class GenericAdProvider<TManager> : IAdProvider where TManager : class
    {
        protected TManager _manager;
        protected bool _isInitialized;
        protected bool _isBannerShowing;
        protected bool _isMRecShowing;

        public virtual string ProviderName => typeof(TManager).Name;
        public virtual bool IsInitialized => _isInitialized;
        public virtual bool IsRewardedReady => false;
        public virtual bool IsInterstitialReady => false;
        public virtual bool IsBannerShowing => _isBannerShowing;
        public virtual bool IsMRecShowing => _isMRecShowing;

        public TManager Manager => _manager;

        public virtual void SetManager(TManager manager)
        {
            _manager = manager;
        }

        public abstract void Initialize();

        // Banner
        public virtual void ShowBanner(string placement = "")
        {
            _isBannerShowing = true;
        }

        public virtual void HideBanner()
        {
            _isBannerShowing = false;
        }

        public virtual float GetBannerHeight(Rect canvasRect = default) => 0f;

        // Interstitial
        public abstract void ShowInterstitial(UnityAction onCompleted = null, UnityAction onFailed = null, string placement = "", string requestId = null);
        
        public virtual void LoadInterstitial() { }

        public virtual async UniTask<bool> WaitForInterstitialReadyAsync(float timeoutSeconds = 10f)
        {
            if (IsInterstitialReady) return true;
            float waited = 0f;
            while (waited < timeoutSeconds)
            {
                await UniTask.Yield();
                waited += Time.unscaledDeltaTime;
                if (IsInterstitialReady) return true;
            }
            return false;
        }

        // Rewarded
        public abstract void ShowRewarded(UnityAction onCompleted, UnityAction onFailed, string placement = "", string requestId = null, long waitMs = 0);
        
        public virtual void LoadRewarded() { }

        public virtual async UniTask<bool> WaitForRewardedReadyAsync(float timeoutSeconds = 10f)
        {
            if (IsRewardedReady) return true;
            float waited = 0f;
            while (waited < timeoutSeconds)
            {
                await UniTask.Yield();
                waited += Time.unscaledDeltaTime;
                if (IsRewardedReady) return true;
            }
            return false;
        }

        // MREC
        public virtual void ShowMRec(Vector2 unityPosition, Vector2 nativePosition)
        {
            _isMRecShowing = true;
        }

        public virtual void HideMRec()
        {
            _isMRecShowing = false;
        }

        public virtual void LoadMRec() { }

        // Events
        public event Action<AdImpressionData> OnAdImpression;
        public event Action<string> OnCountryCodeResolved;
        public event Action OnRewardedCompleted;
        public event Action OnRewardedFailed;
        public event Action OnRewardedShown;
        public event Action OnRewardedEnded;
        public event Action OnInterstitialCompleted;
        public event Action OnInterstitialFailed;
        public event Action OnInterstitialShown;
        public event Action OnInterstitialEnded;
        public event Action OnBannerLoaded;
        public event Action OnBannerFailed;
        public event Action OnMRecLoaded;
        public event Action OnMRecFailed;

        // Protected event invocation helpers
        protected void DispatchAdImpression(AdImpressionData data) => OnAdImpression?.Invoke(data);
        protected void DispatchCountryCodeResolved(string countryCode) => OnCountryCodeResolved?.Invoke(countryCode);
        protected void DispatchRewardedCompleted() => OnRewardedCompleted?.Invoke();
        protected void DispatchRewardedFailed() => OnRewardedFailed?.Invoke();
        protected void DispatchRewardedShown() => OnRewardedShown?.Invoke();
        protected void DispatchRewardedEnded() => OnRewardedEnded?.Invoke();
        protected void DispatchInterstitialCompleted() => OnInterstitialCompleted?.Invoke();
        protected void DispatchInterstitialFailed() => OnInterstitialFailed?.Invoke();
        protected void DispatchInterstitialShown() => OnInterstitialShown?.Invoke();
        protected void DispatchInterstitialEnded() => OnInterstitialEnded?.Invoke();
        protected void DispatchBannerLoaded() => OnBannerLoaded?.Invoke();
        protected void DispatchBannerFailed() => OnBannerFailed?.Invoke();
        protected void DispatchMRecLoaded() => OnMRecLoaded?.Invoke();
        protected void DispatchMRecFailed() => OnMRecFailed?.Invoke();
    }

    /// <summary>
    /// Generic abstract base class for ad network providers with dedicated configuration data.
    /// </summary>
    /// <typeparam name="TManager">The underlying SDK manager instance type.</typeparam>
    /// <typeparam name="TConfig">The configuration or settings data type.</typeparam>
    public abstract class GenericAdProvider<TManager, TConfig> : GenericAdProvider<TManager>
        where TManager : class
        where TConfig : class
    {
        protected TConfig _config;

        public TConfig Config => _config;

        public virtual void SetConfig(TConfig config)
        {
            _config = config;
        }
    }
}
