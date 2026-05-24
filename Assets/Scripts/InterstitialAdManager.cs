using System;
using UnityEngine;
#if UNITY_WEBGL
using Playgama;
using Playgama.Modules.Advertisement;
#endif

public class InterstitialAdManager : MonoBehaviour
{
    private const int LevelStartThreshold = 10;
    private const float TimeThresholdSeconds = 300f;

#if UNITY_WEBGL
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (FindFirstObjectByType<InterstitialAdManager>() != null) return;
        Debug.Log("[InterstitialAds] Bootstrap");
        var go = new GameObject("InterstitialAdManager");
        DontDestroyOnLoad(go);
        go.AddComponent<InterstitialAdManager>();
    }
#endif

    private int levelStartsSinceLastAd;
    private float lastAdRealtime;
    private float savedTimeScale = 1f;
    private bool gamePausedForAd;
    private bool subscribedBridgeEvents;

    private void Start()
    {
        lastAdRealtime = 0f;
    }

    private void OnEnable()
    {
        MagnetGameActionSystem.LevelStarted += OnLevelStarted;
#if UNITY_WEBGL
        try
        {
            if (Bridge.advertisement != null)
            {
                Bridge.advertisement.interstitialStateChanged += OnInterstitialStateChanged;
                subscribedBridgeEvents = true;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[InterstitialAds] subscribe failed: {e.Message}");
        }
#endif
    }

    private void OnDisable()
    {
        MagnetGameActionSystem.LevelStarted -= OnLevelStarted;
#if UNITY_WEBGL
        if (!subscribedBridgeEvents) return;
        try
        {
            if (Bridge.advertisement != null)
                Bridge.advertisement.interstitialStateChanged -= OnInterstitialStateChanged;
        }
        catch { }
#endif
    }

    private void OnLevelStarted(int level)
    {
        levelStartsSinceLastAd++;
        float elapsed = Time.realtimeSinceStartup - lastAdRealtime;
        bool countHit = levelStartsSinceLastAd >= LevelStartThreshold;
        bool timeHit = elapsed >= TimeThresholdSeconds;
        Debug.Log($"[InterstitialAds] LevelStarted lvl={level} count={levelStartsSinceLastAd}/{LevelStartThreshold} elapsed={elapsed:F1}s/{TimeThresholdSeconds:F0}s");
        if (countHit || timeHit)
            FireInterstitial();
    }

    private void FireInterstitial()
    {
        levelStartsSinceLastAd = 0;
        lastAdRealtime = Time.realtimeSinceStartup;
#if UNITY_WEBGL
        try
        {
            if (Bridge.advertisement == null) return;
            Debug.Log("[InterstitialAds] Bridge.advertisement.ShowInterstitial(level_start)");
            Bridge.advertisement.ShowInterstitial("level_start");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[InterstitialAds] ShowInterstitial failed: {e.Message}");
        }
#endif
    }

#if UNITY_WEBGL
    private void OnInterstitialStateChanged(InterstitialState state)
    {
        switch (state)
        {
            case InterstitialState.Opened:
                if (!gamePausedForAd)
                {
                    savedTimeScale = Time.timeScale;
                    Time.timeScale = 0f;
                    gamePausedForAd = true;
                }
                break;
            case InterstitialState.Closed:
            case InterstitialState.Failed:
                if (gamePausedForAd)
                {
                    Time.timeScale = savedTimeScale;
                    gamePausedForAd = false;
                }
                break;
        }
    }
#endif
}
