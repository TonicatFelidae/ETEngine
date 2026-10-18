using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

namespace ET.Monetization
{
    /// <summary>
    /// Core In-App Purchasing service interface for ETEngine.
    /// Provides a store-agnostic API for product queries, purchasing, restoration,
    /// entitlement verification, and reward fulfillment callbacks without referencing any store SDK.
    /// </summary>
    public interface IIAPService
    {
        bool IsInitialized { get; }

        void Initialize();
        UniTask<bool> InitializeAsync();
        UniTask<bool> WaitForInitializedAsync(float timeoutSeconds = 10f);

        // Purchase & Restore Operations
        void Purchase(string productId, UnityAction<IAPPurchaseResult> onSuccess = null, UnityAction<IAPPurchaseFailure> onFailed = null, string placement = "");
        UniTask<(bool success, IAPPurchaseResult result, IAPPurchaseFailure failure)> PurchaseAsync(string productId, string placement = "");
        void RestorePurchases(UnityAction<bool, string> onCompleted = null);
        UniTask<bool> RestorePurchasesAsync();

        // Product & Ownership Queries
        bool IsProductOwned(string productId);
        bool IsProductAvailable(string productId);
        IAPProductInfo GetProductInfo(string productId);
        IReadOnlyList<IAPProductInfo> GetAllProducts();
        string GetLocalizedPriceString(string productId, string fallback = "");

        // Reward Grant Handlers
        void RegisterProductGrantHandler(string productId, Action<IAPPurchaseResult> onGrant);
        void UnregisterProductGrantHandler(string productId, Action<IAPPurchaseResult> onGrant);

        // Events
        event Action OnInitialized;
        event Action<string> OnInitializeFailed;
        event Action<IReadOnlyList<IAPProductInfo>> OnProductsFetched;
        event Action<IAPPurchaseResult> OnPurchaseSucceeded;
        event Action<IAPPurchaseFailure> OnPurchaseFailed;
        event Action<string> OnPurchaseDeferred;
        event Action<bool, string> OnPurchasesRestored;
        event Action<string, bool> OnOwnershipChanged;
    }
}
