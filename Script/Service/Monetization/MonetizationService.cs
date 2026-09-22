using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace ET.Monetization
{
    /// <summary>
    /// Type L (Lifecycle Class) implementation of IMonetizationService and IAdsHelper.
    /// Manages ad providers, interstitial cooldown timers, and telemetry routing in an abstract manner.
    /// Registered in VContainer via Lifetime.Singleton or Lifetime.Scoped.
    /// </summary>
    public class MonetizationService : IMonetizationService, IAdsHelper
    {
        //Field
        [SerializeField] private float _interstitialCooldownSeconds = 30f;
        protected IAdProvider _provider;
        private float _lastInterstitialShowTime = -1f;
        private bool _isInitialized;

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

        // Constructor for VContainer DI
        [Inject]
        public MonetizationService(IAdProvider provider = null)
        {
            if (provider != null)
            {
                SetProvider(provider);
            }
        }

        public MonetizationService()
        {
        }

        //ReInit
        public virtual void ReInit()
        {
            _lastInterstitialShowTime = -1f;
            _isInitialized = false;
        }

        //Init
        public virtual void Init()
        {
            ReInit();
            _isInitialized = true;
            if (_provider != null && !_provider.IsInitialized)
            {
                _provider.Initialize();
            }
        }

        public virtual void Init(IAdProvider provider)
        {
            SetProvider(provider);
            Init();
        }

        public void Initialize() => Init();

        //UpdateData()
        public virtual void UpdateData(Data data)
        {
            if (data == null) return;

            if (data.InterstitialCooldownSeconds > 0f)
            {
                _interstitialCooldownSeconds = data.InterstitialCooldownSeconds;
            }

            if (data.Provider != null)
            {
                SetProvider(data.Provider);
            }
        }

        //Get / Set
        public bool IsInitialized => _isInitialized && (_provider?.IsInitialized ?? false);
        public bool IsRewardedReady => _provider?.IsRewardedReady ?? false;
        public bool IsInterstitialReady => _provider?.IsInterstitialReady ?? false;
        public bool IsBannerShowing => _provider?.IsBannerShowing ?? false;
        public bool IsMRecShowing => _provider?.IsMRecShowing ?? false;

        public float InterstitialCooldownSeconds
        {
            get => _interstitialCooldownSeconds;
            set => _interstitialCooldownSeconds = value;
        }

        public IAdProvider Provider => _provider;

        public virtual void SetProvider(IAdProvider provider)
        {
            if (_provider != null)
            {
                UnsubscribeProviderEvents(_provider);
            }

            _provider = provider ?? throw new ArgumentNullException(nameof(provider), "Ad provider cannot be null.");
            SubscribeProviderEvents(_provider);
        }

        //Functions

        #region Cooldown Tracking
        public bool HasInterstitialCooldownPassed()
        {
            if (_lastInterstitialShowTime < 0f) return true;
            return (Time.realtimeSinceStartup - _lastInterstitialShowTime) >= _interstitialCooldownSeconds;
        }

        public void ResetInterstitialCooldown()
        {
            _lastInterstitialShowTime = Time.realtimeSinceStartup;
        }
        #endregion

        #region Banner Operations
        public virtual void ShowBanner(string placement = "")
        {
            ValidateProvider();
            _provider.ShowBanner(placement);
        }

        public virtual void HideBanner()
        {
            ValidateProvider();
            _provider.HideBanner();
        }

        public virtual float GetBannerHeight(Rect canvasRect = default)
        {
            if (_provider == null) return 0f;
            return _provider.GetBannerHeight(canvasRect);
        }
        #endregion

        #region Interstitial Operations
        public virtual void ShowInterstitial(UnityAction onCompleted = null, UnityAction onFailed = null, string placement = "", string requestId = null)
        {
            ValidateProvider();
            _lastInterstitialShowTime = Time.realtimeSinceStartup;
            _provider.ShowInterstitial(onCompleted, onFailed, placement, requestId);
        }

        public virtual void LoadInterstitial()
        {
            ValidateProvider();
            _provider.LoadInterstitial();
        }

        public virtual UniTask<bool> WaitForInterstitialReadyAsync(float timeoutSeconds = 10f)
        {
            ValidateProvider();
            return _provider.WaitForInterstitialReadyAsync(timeoutSeconds);
        }

        public void ShowInterstitialAds(UnityAction onCompleted = null, UnityAction onFailed = null, AdCheckReason reason = AdCheckReason.None, string placement = "")
        {
            string resolvedPlacement = !string.IsNullOrEmpty(placement) ? placement : reason.ToString();
            ShowInterstitial(onCompleted, onFailed, resolvedPlacement);
        }

        public UniTask<bool> WaitForInterstitialAdReadyAsync(float timeoutSeconds = 10)
        {
            return WaitForInterstitialReadyAsync(timeoutSeconds);
        }
        #endregion

        #region Rewarded Operations
        public virtual void ShowRewarded(UnityAction onCompleted, UnityAction onFailed, string placement = "", string requestId = null, long waitMs = 0)
        {
            ValidateProvider();
            _provider.ShowRewarded(onCompleted, onFailed, placement, requestId, waitMs);
        }

        public virtual void LoadRewarded()
        {
            ValidateProvider();
            _provider.LoadRewarded();
        }

        public virtual UniTask<bool> WaitForRewardedReadyAsync(float timeoutSeconds = 10f)
        {
            ValidateProvider();
            return _provider.WaitForRewardedReadyAsync(timeoutSeconds);
        }

        public void ShowRewardAds(UnityAction onCompleted, UnityAction onFailed, AdCheckReason reason = AdCheckReason.None, string placement = "")
        {
            string resolvedPlacement = !string.IsNullOrEmpty(placement) ? placement : reason.ToString();
            ShowRewarded(onCompleted, onFailed, resolvedPlacement);
        }

        public UniTask<bool> WaitForRewardAdReadyAsync(AdCheckReason reason, float timeoutSeconds = 10)
        {
            return WaitForRewardedReadyAsync(timeoutSeconds);
        }
        #endregion

        #region MREC Operations
        public virtual void ShowMRec(Vector2 unityPosition, Vector2 nativePosition)
        {
            ValidateProvider();
            _provider.ShowMRec(unityPosition, nativePosition);
        }

        public virtual void HideMRec()
        {
            ValidateProvider();
            _provider.HideMRec();
        }

        public virtual void LoadMRec()
        {
            ValidateProvider();
            _provider.LoadMRec();
        }
        #endregion

        #region Provider Event Subscriptions
        private void SubscribeProviderEvents(IAdProvider provider)
        {
            provider.OnAdImpression += ForwardAdImpression;
            provider.OnCountryCodeResolved += ForwardCountryCodeResolved;
            provider.OnRewardedCompleted += ForwardRewardedCompleted;
            provider.OnRewardedFailed += ForwardRewardedFailed;
            provider.OnRewardedShown += ForwardRewardedShown;
            provider.OnRewardedEnded += ForwardRewardedEnded;
            provider.OnInterstitialCompleted += ForwardInterstitialCompleted;
            provider.OnInterstitialFailed += ForwardInterstitialFailed;
            provider.OnInterstitialShown += ForwardInterstitialShown;
            provider.OnInterstitialEnded += ForwardInterstitialEnded;
            provider.OnBannerLoaded += ForwardBannerLoaded;
            provider.OnBannerFailed += ForwardBannerFailed;
            provider.OnMRecLoaded += ForwardMRecLoaded;
            provider.OnMRecFailed += ForwardMRecFailed;
        }

        private void UnsubscribeProviderEvents(IAdProvider provider)
        {
            provider.OnAdImpression -= ForwardAdImpression;
            provider.OnCountryCodeResolved -= ForwardCountryCodeResolved;
            provider.OnRewardedCompleted -= ForwardRewardedCompleted;
            provider.OnRewardedFailed -= ForwardRewardedFailed;
            provider.OnRewardedShown -= ForwardRewardedShown;
            provider.OnRewardedEnded -= ForwardRewardedEnded;
            provider.OnInterstitialCompleted -= ForwardInterstitialCompleted;
            provider.OnInterstitialFailed -= ForwardInterstitialFailed;
            provider.OnInterstitialShown -= ForwardInterstitialShown;
            provider.OnInterstitialEnded -= ForwardInterstitialEnded;
            provider.OnBannerLoaded -= ForwardBannerLoaded;
            provider.OnBannerFailed -= ForwardBannerFailed;
            provider.OnMRecLoaded -= ForwardMRecLoaded;
            provider.OnMRecFailed -= ForwardMRecFailed;
        }

        private void ForwardAdImpression(AdImpressionData data) => OnAdImpression?.Invoke(data);
        private void ForwardCountryCodeResolved(string code) => OnCountryCodeResolved?.Invoke(code);
        private void ForwardRewardedCompleted() => OnRewardedCompleted?.Invoke();
        private void ForwardRewardedFailed() => OnRewardedFailed?.Invoke();
        private void ForwardRewardedShown() => OnRewardedShown?.Invoke();
        private void ForwardRewardedEnded() => OnRewardedEnded?.Invoke();
        private void ForwardInterstitialCompleted() => OnInterstitialCompleted?.Invoke();
        private void ForwardInterstitialFailed() => OnInterstitialFailed?.Invoke();
        private void ForwardInterstitialShown() => OnInterstitialShown?.Invoke();
        private void ForwardInterstitialEnded() => OnInterstitialEnded?.Invoke();
        private void ForwardBannerLoaded() => OnBannerLoaded?.Invoke();
        private void ForwardBannerFailed() => OnBannerFailed?.Invoke();
        private void ForwardMRecLoaded() => OnMRecLoaded?.Invoke();
        private void ForwardMRecFailed() => OnMRecFailed?.Invoke();

        private void ValidateProvider()
        {
            if (_provider == null)
            {
                throw new InvalidOperationException("No IAdProvider configured in MonetizationService. Please register an IAdProvider implementation.");
            }
        }
        #endregion

        // Nested class used to initialize or update data in the outer class
        [Serializable]
        public class Data
        {
            public float InterstitialCooldownSeconds;
            public IAdProvider Provider;
        }
    }

    /// <summary>
    /// Configuration data used to update or initialize MonetizationService.
    /// </summary>
    [Serializable]
    public class MonetizationUpdateData : MonetizationService.Data
    {
    }
}
