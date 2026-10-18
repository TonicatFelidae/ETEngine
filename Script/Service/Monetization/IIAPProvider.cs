using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

namespace ET.Monetization
{
    /// <summary>
    /// Abstract provider interface for In-App Purchasing backends (Unity IAP, Steamworks, Mock Store, etc.).
    /// Encapsulates all store SDK logic away from ETEngine core systems.
    /// </summary>
    public interface IIAPProvider
    {
        string ProviderName { get; }
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
