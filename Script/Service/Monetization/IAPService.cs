using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using VContainer;

namespace ET.Monetization
{
    /// <summary>
    /// Type L (Lifecycle Class) implementation of IIAPService.
    /// Coordinates store providers, product grant callbacks, ownership state notifications,
    /// and store-agnostic telemetry routing.
    /// Registered in VContainer via Lifetime.Singleton or Lifetime.Scoped.
    /// </summary>
    public class IAPService : IIAPService
    {
        //Field
        protected IIAPProvider _provider;
        private bool _isInitialized;
        private readonly Dictionary<string, Action<IAPPurchaseResult>> _productGrantHandlers = new Dictionary<string, Action<IAPPurchaseResult>>();

        public event Action OnInitialized;
        public event Action<string> OnInitializeFailed;
        public event Action<IReadOnlyList<IAPProductInfo>> OnProductsFetched;
        public event Action<IAPPurchaseResult> OnPurchaseSucceeded;
        public event Action<IAPPurchaseFailure> OnPurchaseFailed;
        public event Action<string> OnPurchaseDeferred;
        public event Action<bool, string> OnPurchasesRestored;
        public event Action<string, bool> OnOwnershipChanged;

        [Inject]
        public IAPService(IIAPProvider provider = null)
        {
            if (provider != null)
            {
                SetProvider(provider);
            }
        }

        public IAPService()
        {
        }

        //ReInit
        public virtual void ReInit()
        {
            _isInitialized = false;
        }

        //Init
        public virtual void Init()
        {
            ReInit();
            _isInitialized = true;
            if (!_provider.IsInitialized)
            {
                _provider.Initialize();
            }
        }

        public virtual void Init(IIAPProvider provider)
        {
            SetProvider(provider);
            Init();
        }

        public void Initialize() => Init();

        public virtual async UniTask<bool> InitializeAsync()
        {
            ValidateProvider();
            ReInit();
            _isInitialized = true;
            return await _provider.InitializeAsync();
        }

        public virtual UniTask<bool> WaitForInitializedAsync(float timeoutSeconds = 10f)
        {
            ValidateProvider();
            return _provider.WaitForInitializedAsync(timeoutSeconds);
        }

        //UpdateData()
        public virtual void UpdateData(Data data)
        {
            if (data == null) return;

            if (data.Provider != null)
            {
                SetProvider(data.Provider);
            }
        }

        //Get / Set
        public bool IsInitialized => _isInitialized && (_provider?.IsInitialized ?? false);
        public IIAPProvider Provider => _provider;

        public virtual void SetProvider(IIAPProvider provider)
        {
            if (_provider != null)
            {
                UnsubscribeProviderEvents(_provider);
            }

            _provider = provider ?? throw new ArgumentNullException(nameof(provider), "IAP provider cannot be null.");
            SubscribeProviderEvents(_provider);
        }

        //Functions

        #region Purchase & Restore Operations
        public virtual void Purchase(string productId, UnityAction<IAPPurchaseResult> onSuccess = null, UnityAction<IAPPurchaseFailure> onFailed = null, string placement = "")
        {
            ValidateProvider();
            _provider.Purchase(productId, onSuccess, onFailed, placement);
        }

        public virtual UniTask<(bool success, IAPPurchaseResult result, IAPPurchaseFailure failure)> PurchaseAsync(string productId, string placement = "")
        {
            ValidateProvider();
            return _provider.PurchaseAsync(productId, placement);
        }

        public virtual void RestorePurchases(UnityAction<bool, string> onCompleted = null)
        {
            ValidateProvider();
            _provider.RestorePurchases(onCompleted);
        }

        public virtual UniTask<bool> RestorePurchasesAsync()
        {
            ValidateProvider();
            return _provider.RestorePurchasesAsync();
        }
        #endregion

        #region Product & Ownership Queries
        public virtual bool IsProductOwned(string productId)
        {
            ValidateProvider();
            return _provider.IsProductOwned(productId);
        }

        public virtual bool IsProductAvailable(string productId)
        {
            ValidateProvider();
            return _provider.IsProductAvailable(productId);
        }

        public virtual IAPProductInfo GetProductInfo(string productId)
        {
            ValidateProvider();
            return _provider.GetProductInfo(productId);
        }

        public virtual IReadOnlyList<IAPProductInfo> GetAllProducts()
        {
            ValidateProvider();
            return _provider.GetAllProducts();
        }

        public virtual string GetLocalizedPriceString(string productId, string fallback = "")
        {
            ValidateProvider();
            return _provider.GetLocalizedPriceString(productId, fallback);
        }
        #endregion

        #region Product Grant Handlers
        public void RegisterProductGrantHandler(string productId, Action<IAPPurchaseResult> onGrant)
        {
            if (string.IsNullOrEmpty(productId))
            {
                throw new ArgumentException("Product ID cannot be null or empty.", nameof(productId));
            }

            if (_productGrantHandlers.TryGetValue(productId, out var existing))
            {
                _productGrantHandlers[productId] = existing + onGrant;
            }
            else
            {
                _productGrantHandlers[productId] = onGrant;
            }
        }

        public void UnregisterProductGrantHandler(string productId, Action<IAPPurchaseResult> onGrant)
        {
            if (string.IsNullOrEmpty(productId)) return;

            if (_productGrantHandlers.TryGetValue(productId, out var existing))
            {
                existing -= onGrant;
                if (existing == null)
                {
                    _productGrantHandlers.Remove(productId);
                }
                else
                {
                    _productGrantHandlers[productId] = existing;
                }
            }
        }

        private void InvokeProductGrantHandler(IAPPurchaseResult result)
        {
            if (!string.IsNullOrEmpty(result.ProductId) &&
                _productGrantHandlers.TryGetValue(result.ProductId, out var handler))
            {
                handler?.Invoke(result);
            }
        }
        #endregion

        #region Provider Event Subscriptions
        private void SubscribeProviderEvents(IIAPProvider provider)
        {
            provider.OnInitialized += ForwardInitialized;
            provider.OnInitializeFailed += ForwardInitializeFailed;
            provider.OnProductsFetched += ForwardProductsFetched;
            provider.OnPurchaseSucceeded += ForwardPurchaseSucceeded;
            provider.OnPurchaseFailed += ForwardPurchaseFailed;
            provider.OnPurchaseDeferred += ForwardPurchaseDeferred;
            provider.OnPurchasesRestored += ForwardPurchasesRestored;
            provider.OnOwnershipChanged += ForwardOwnershipChanged;
        }

        private void UnsubscribeProviderEvents(IIAPProvider provider)
        {
            provider.OnInitialized -= ForwardInitialized;
            provider.OnInitializeFailed -= ForwardInitializeFailed;
            provider.OnProductsFetched -= ForwardProductsFetched;
            provider.OnPurchaseSucceeded -= ForwardPurchaseSucceeded;
            provider.OnPurchaseFailed -= ForwardPurchaseFailed;
            provider.OnPurchaseDeferred -= ForwardPurchaseDeferred;
            provider.OnPurchasesRestored -= ForwardPurchasesRestored;
            provider.OnOwnershipChanged -= ForwardOwnershipChanged;
        }

        private void ForwardInitialized() => OnInitialized?.Invoke();
        private void ForwardInitializeFailed(string reason) => OnInitializeFailed?.Invoke(reason);
        private void ForwardProductsFetched(IReadOnlyList<IAPProductInfo> products) => OnProductsFetched?.Invoke(products);

        private void ForwardPurchaseSucceeded(IAPPurchaseResult result)
        {
            InvokeProductGrantHandler(result);
            OnPurchaseSucceeded?.Invoke(result);
        }

        private void ForwardPurchaseFailed(IAPPurchaseFailure failure) => OnPurchaseFailed?.Invoke(failure);
        private void ForwardPurchaseDeferred(string productId) => OnPurchaseDeferred?.Invoke(productId);
        private void ForwardPurchasesRestored(bool success, string error) => OnPurchasesRestored?.Invoke(success, error);
        private void ForwardOwnershipChanged(string productId, bool isOwned) => OnOwnershipChanged?.Invoke(productId, isOwned);

        private void ValidateProvider()
        {
            if (_provider == null)
            {
                throw new InvalidOperationException("No IIAPProvider configured in IAPService. Please register an IIAPProvider implementation.");
            }
        }
        #endregion

        [Serializable]
        public class Data
        {
            public IIAPProvider Provider;
        }
    }

    /// <summary>
    /// Configuration data used to update or initialize IAPService.
    /// </summary>
    [Serializable]
    public class IAPUpdateData : IAPService.Data
    {
    }
}
