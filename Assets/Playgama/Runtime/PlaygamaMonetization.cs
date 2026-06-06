using System;
using UnityEngine;

namespace Playgama.TemplateTools
{
    /// <summary>
    /// Generic monetization facade over the Playgama Bridge SDK (ads + IAP).
    /// Games call these methods; platform/editor differences are handled here so
    /// game code never needs #if UNITY_WEBGL guards.
    ///   - Editor / unsupported platform: ads are no-ops, rewarded grants instantly,
    ///     purchases succeed (so the game stays testable).
    ///   - WebGL build on Playgama: real Bridge ads + payments.
    /// </summary>
    public static class PlaygamaMonetization
    {
        // Show an interstitial automatically every N game-overs.
        public static int interstitialEveryN = 2;
        static int _gameOverCount;

        public static bool AdsRemoved { get; private set; }

#if UNITY_WEBGL && !UNITY_EDITOR
        static Action _onReward;
        static bool _subscribed;

        static void EnsureSubscribed()
        {
            if (_subscribed) return;
            Playgama.Bridge.advertisement.rewardedStateChanged += OnRewardedState;
            _subscribed = true;
        }

        static void OnRewardedState(Playgama.Modules.Advertisement.RewardedState state)
        {
            if (state == Playgama.Modules.Advertisement.RewardedState.Rewarded)
            {
                var cb = _onReward; _onReward = null;
                cb?.Invoke();
            }
        }
#endif

        /// <summary>Show a rewarded ad; invokes onReward only if the reward is granted.</summary>
        public static void ShowRewarded(Action onReward)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (Playgama.Bridge.advertisement.isRewardedSupported)
            {
                EnsureSubscribed();
                _onReward = onReward;
                Playgama.Bridge.advertisement.ShowRewarded();
                return;
            }
#endif
            onReward?.Invoke(); // editor / unsupported: grant instantly
        }

        /// <summary>Show an interstitial ad (skipped if ads removed via IAP).</summary>
        public static void ShowInterstitial()
        {
            if (AdsRemoved) return;
#if UNITY_WEBGL && !UNITY_EDITOR
            if (Playgama.Bridge.advertisement.isInterstitialSupported)
                Playgama.Bridge.advertisement.ShowInterstitial();
#endif
        }

        /// <summary>Call on every game-over; shows an interstitial every interstitialEveryN deaths.</summary>
        public static void GameOver()
        {
            _gameOverCount++;
            if (AdsRemoved) return;
            if (_gameOverCount % Mathf.Max(1, interstitialEveryN) == 0)
                ShowInterstitial();
        }

        public static void ShowBanner()
        {
            if (AdsRemoved) return;
#if UNITY_WEBGL && !UNITY_EDITOR
            if (Playgama.Bridge.advertisement.isBannerSupported)
                Playgama.Bridge.advertisement.ShowBanner();
#endif
        }

        public static void HideBanner()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            Playgama.Bridge.advertisement.HideBanner();
#endif
        }

        /// <summary>Buy an IAP product. onDone(true) on success. "remove_ads" toggles AdsRemoved.</summary>
        public static void Purchase(string productId, Action<bool> onDone = null)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (Playgama.Bridge.payments.isSupported)
            {
                Playgama.Bridge.payments.Purchase(productId, (ok, data) =>
                {
                    if (ok && productId == "remove_ads") AdsRemoved = true;
                    onDone?.Invoke(ok);
                });
                return;
            }
#endif
            if (productId == "remove_ads") AdsRemoved = true;
            onDone?.Invoke(true); // editor / unsupported: assume success
        }
    }
}
