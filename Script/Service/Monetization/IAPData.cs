using System;

namespace ET.Monetization
{
    /// <summary>
    /// Store-agnostic product type enumeration for ETEngine IAP services.
    /// </summary>
    public enum IAPProductType
    {
        Consumable = 0,
        NonConsumable = 1,
        Subscription = 2,
        Unknown = 3
    }

    /// <summary>
    /// Standardized failure reasons for in-app purchase operations.
    /// </summary>
    public enum IAPPurchaseFailureReason
    {
        PurchasingUnavailable = 0,
        ExistingPurchasePending = 1,
        ProductUnavailable = 2,
        SignatureInvalid = 3,
        UserCancelled = 4,
        PaymentDeclined = 5,
        DuplicateTransaction = 6,
        Unknown = 7
    }

    /// <summary>
    /// Configuration definition for an IAP product, supporting store-specific overrides
    /// and cross-product bundle ownership rules.
    /// </summary>
    [Serializable]
    public class IAPProductDefinition
    {
        public string ProductId;
        public IAPProductType ProductType = IAPProductType.Consumable;
        public string GooglePlayId;
        public string AppleAppStoreId;
        public string DefaultPriceString = "$0.99";
        public string DefaultTitle;
        public string DefaultDescription;
        public string[] RefPlayerPrefs;
        public string[] OwnedIfOwnedProducts;
    }

    /// <summary>
    /// Store-agnostic runtime metadata and ownership state for an IAP product.
    /// </summary>
    [Serializable]
    public struct IAPProductInfo
    {
        public string ProductId;
        public IAPProductType ProductType;
        public string LocalizedPriceString;
        public decimal LocalizedPrice;
        public string IsoCurrencyCode;
        public string LocalizedTitle;
        public string LocalizedDescription;
        public bool IsAvailableToPurchase;
        public bool IsOwned;
    }

    /// <summary>
    /// Standardized result payload emitted when a purchase or restoration succeeds.
    /// </summary>
    [Serializable]
    public struct IAPPurchaseResult
    {
        public string ProductId;
        public IAPProductType ProductType;
        public string TransactionId;
        public string Receipt;
        public decimal LocalizedPrice;
        public string IsoCurrencyCode;
        public bool IsRestored;
        public string Placement;
    }

    /// <summary>
    /// Standardized failure payload emitted when a purchase attempt fails.
    /// </summary>
    [Serializable]
    public struct IAPPurchaseFailure
    {
        public string ProductId;
        public IAPPurchaseFailureReason Reason;
        public string Message;
        public string Placement;
    }
}
