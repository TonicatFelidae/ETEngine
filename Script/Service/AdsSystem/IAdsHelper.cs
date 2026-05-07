using Cysharp.Threading.Tasks;

namespace ET.Monetization
{
    public interface IAdsHelper
    {
        bool HasInterstitialCooldownPassed();
        void Init();
        void ShowInterstitialAds(UnityEngine.Events.UnityAction onCompleted = null, UnityEngine.Events.UnityAction onFailed = null, AdCheckReason reason = AdCheckReason.None, string placement = "");
        void ShowRewardAds(UnityEngine.Events.UnityAction onCompleted, UnityEngine.Events.UnityAction onFailed, AdCheckReason reason = AdCheckReason.None, string placement = "");
        UniTask<bool> WaitForInterstitialAdReadyAsync(float timeoutSeconds = 10);
        UniTask<bool> WaitForRewardAdReadyAsync(AdCheckReason reason, float timeoutSeconds = 10);
    }
}