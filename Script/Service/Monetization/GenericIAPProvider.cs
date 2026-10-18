using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace ET.Monetization
{
    /// <summary>
    /// Generic abstract base class for In-App Purchasing providers.
    /// Bridges any store SDK manager instance (e.g. UnityIAPManager) to ETEngine's abstract IAP architecture.
    /// </summary>
    /// <typeparam name="TManager">The underlying store SDK manager instance type.</typeparam>
    public abstract class GenericIAPProvider<TManager> : IIAPProvider where TManager : class
    {
        protected TManager _manager;
        protected bool _isInitialized;

        public virtual string ProviderName => typeof(TManager).Name;
        public virtual bool IsInitialized => _isInitialized;

        public TManager Manager => _manager;

        protected GenericIAPProvider(TManager manager = null)
        {
            _manager = manager;
        }

        public virtual void SetManager(TManager manager)
        {
            _manager = manager;
        }

        public abstract void Initialize();

        public virtual async UniTask<bool> InitializeAsync()
        {
            if (IsInitialized) return true;
            Initialize();
            return await WaitForInitializedAsync();
        }

        public virtual async UniTask<bool> WaitForInitializedAsync(float timeoutSeconds = 10f)
        {
            if (IsInitialized) return true;
            float waited = 0f;
            while (waited < timeoutSeconds)
            {
                await UniTask.Yield();
                waited += Time.unscaledDeltaTime;
                if (IsInitialized) return true;
            }
            return false;
        }

        // Purchase & Restore Operations
        public abstract void Purchase(string productId, UnityAction<IAPPurchaseResult> onSuccess = null, UnityAction<IAPPurchaseFailure> onFailed = null, string placement = "");

        public virtual async UniTask<(bool success, IAPPurchaseResult result, IAPPurchaseFailure failure)> PurchaseAsync(string productId, string placement = "")
        {
            var tcs = new UniTaskCompletionSource<(bool, IAPPurchaseResult, IAPPurchaseFailure)>();
            Purchase(
                productId,
                onSuccess: res => tcs.TrySetResult((true, res, default)),
                onFailed: fail => tcs.TrySetResult((false, default, fail)),
                placement: placement
            );
            return await tcs.Task;
        }

        public abstract void RestorePurchases(UnityAction<bool, string> onCompleted = null);

        public virtual async UniTask<bool> RestorePurchasesAsync()
        {
            var tcs = new UniTaskCompletionSource<bool>();
            RestorePurchases((success, _) => tcs.TrySetResult(success));
            return await tcs.Task;
        }

        // Product & Ownership Queries
        public abstract bool IsProductOwned(string productId);
        public abstract bool IsProductAvailable(string productId);
        public abstract IAPProductInfo GetProductInfo(string productId);
        public abstract IReadOnlyList<IAPProductInfo> GetAllProducts();

        public virtual string GetLocalizedPriceString(string productId, string fallback = "")
        {
            var info = GetProductInfo(productId);
            return !string.IsNullOrEmpty(info.LocalizedPriceString) ? info.LocalizedPriceString : fallback;
        }

        // Events
        public event Action OnInitialized;
        public event Action<string> OnInitializeFailed;
        public event Action<IReadOnlyList<IAPProductInfo>> OnProductsFetched;
        public event Action<IAPPurchaseResult> OnPurchaseSucceeded;
        public event Action<IAPPurchaseFailure> OnPurchaseFailed;
        public event Action<string> OnPurchaseDeferred;
        public event Action<bool, string> OnPurchasesRestored;
        public event Action<string, bool> OnOwnershipChanged;

        // Protected event invocation helpers
        protected void DispatchInitialized() => OnInitialized?.Invoke();
        protected void DispatchInitializeFailed(string reason) => OnInitializeFailed?.Invoke(reason);
        protected void DispatchProductsFetched(IReadOnlyList<IAPProductInfo> products) => OnProductsFetched?.Invoke(products);
        protected void DispatchPurchaseSucceeded(IAPPurchaseResult result) => OnPurchaseSucceeded?.Invoke(result);
        protected void DispatchPurchaseFailed(IAPPurchaseFailure failure) => OnPurchaseFailed?.Invoke(failure);
        protected void DispatchPurchaseDeferred(string productId) => OnPurchaseDeferred?.Invoke(productId);
        protected void DispatchPurchasesRestored(bool success, string error) => OnPurchasesRestored?.Invoke(success, error);
        protected void DispatchOwnershipChanged(string productId, bool isOwned) => OnOwnershipChanged?.Invoke(productId, isOwned);
    }

    /// <summary>
    /// Generic abstract base class for IAP providers with dedicated configuration data.
    /// </summary>
    /// <typeparam name="TManager">The underlying store SDK manager instance type.</typeparam>
    /// <typeparam name="TConfig">The configuration or settings data type.</typeparam>
    public abstract class GenericIAPProvider<TManager, TConfig> : GenericIAPProvider<TManager>
        where TManager : class
        where TConfig : class
    {
        protected TConfig _config;

        public TConfig Config => _config;

        protected GenericIAPProvider(TManager manager = null, TConfig config = null) : base(manager)
        {
            _config = config;
        }

        public virtual void SetConfig(TConfig config)
        {
            _config = config;
        }
    }
}
