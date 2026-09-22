using System;

namespace ET.Monetization
{
    /// <summary>
    /// Represents ad impression and revenue attribution data delivered by an ad network.
    /// Used for telemetry, attribution, and analytics reporting.
    /// </summary>
    [Serializable]
    public class AdImpressionData
    {
        public string NetworkName;
        public string AdUnitIdentifier;
        public AdFormat Format;
        public string Placement;
        public double Revenue;
        public string Currency = "USD";
        public string CountryCode;

        public AdImpressionData() { }

        public AdImpressionData(
            string networkName,
            string adUnitIdentifier,
            AdFormat format,
            double revenue,
            string placement = null,
            string currency = "USD",
            string countryCode = null)
        {
            NetworkName = networkName;
            AdUnitIdentifier = adUnitIdentifier;
            Format = format;
            Revenue = revenue;
            Placement = placement;
            Currency = currency;
            CountryCode = countryCode;
        }
    }
}
