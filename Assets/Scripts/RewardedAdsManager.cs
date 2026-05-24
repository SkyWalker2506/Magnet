using System;
using UnityEngine;
#if UNITY_WEBGL
using Playgama;
using Playgama.Modules.Advertisement;
#endif

public class RewardedAdsManager : MonoBehaviour
{
#if UNITY_WEBGL
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (FindFirstObjectByType<RewardedAdsManager>() != null) return;
        var go = new GameObject("RewardedAdsManager");
        DontDestroyOnLoad(go);
        go.AddComponent<RewardedAdsManager>();
    }
#endif

    private bool pendingReward;
    private bool subscribedBridgeEvents;

    private void OnEnable()
    {
        MagnetGameActionSystem.OnLevelFailed += OnLevelFailed;
#if UNITY_WEBGL
        try
        {
            if (Bridge.advertisement != null)
            {
                Bridge.advertisement.rewardedStateChanged += OnRewardedStateChanged;
                subscribedBridgeEvents = true;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[RewardedAds] subscribe failed: {e.Message}");
        }
#endif
    }

    private void OnDisable()
    {
        MagnetGameActionSystem.OnLevelFailed -= OnLevelFailed;
#if UNITY_WEBGL
        if (!subscribedBridgeEvents) return;
        try
        {
            if (Bridge.advertisement != null)
                Bridge.advertisement.rewardedStateChanged -= OnRewardedStateChanged;
        }
        catch { }
#endif
    }

    private void OnLevelFailed()
    {
        RequestRewardedRetry();
    }

    public void RequestRewardedRetry()
    {
#if UNITY_WEBGL
        try
        {
            if (Bridge.advertisement == null) return;
            if (!Bridge.advertisement.isRewardedSupported) return;
            Debug.Log("[RewardedAds] Bridge.advertisement.ShowRewarded(level_fail_retry)");
            pendingReward = true;
            Bridge.advertisement.ShowRewarded("level_fail_retry");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[RewardedAds] ShowRewarded failed: {e.Message}");
            pendingReward = false;
        }
#endif
    }

#if UNITY_WEBGL
    private void OnRewardedStateChanged(RewardedState state)
    {
        switch (state)
        {
            case RewardedState.Opened:
                AudioListener.pause = true;
                break;
            case RewardedState.Rewarded:
                if (pendingReward) GrantRetry();
                break;
            case RewardedState.Closed:
            case RewardedState.Failed:
                AudioListener.pause = false;
                pendingReward = false;
                break;
        }
    }
#endif

    private void GrantRetry()
    {
        pendingReward = false;
        if (LevelManager.IsInitialized)
            LevelManager.Instance.RestartLevel();
    }
}
