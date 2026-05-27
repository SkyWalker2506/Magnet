using System;
using System.Collections;
using UnityEngine;
#if UNITY_WEBGL
using Playgama;
using Playgama.Modules.Platform;
using Playgama.Modules.Advertisement;
#endif

public class PlaygamaIntegration : MonoBehaviour
{
#if UNITY_WEBGL
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (FindFirstObjectByType<PlaygamaIntegration>() != null) return;
        var go = new GameObject("PlaygamaIntegration");
        DontDestroyOnLoad(go);
        go.AddComponent<PlaygamaIntegration>();
    }
#endif

    private const float PreloadTimeoutSeconds = 2f;
    private const int LevelStartThreshold = 10;
    private const float TimeThresholdSeconds = 300f;

    private bool subscribedBridgeEvents;
    private int levelStartsSinceLastAd;
    private float lastAdRealtime;
    private float savedTimeScale = 1f;
    private bool gamePausedForAd;

    private IEnumerator Start()
    {
        lastAdRealtime = 0f;
        SaveService.StartPreload();
        float waited = 0f;
        while (!SaveService.IsPreloadComplete && waited < PreloadTimeoutSeconds)
        {
            waited += Time.unscaledDeltaTime;
            yield return null;
        }

        TrySendMessage(PlatformMessageProxy.GameReady);

        MagnetGameActionSystem.LevelStarted += OnLevelStarted;
        MagnetGameActionSystem.OnLevelCompleted += OnLevelCompleted;
        MagnetGameActionSystem.OnLevelFailed += OnLevelFailed;

#if UNITY_WEBGL
        try
        {
            if (Bridge.platform != null)
            {
                Bridge.platform.pauseStateChanged += OnPlatformPauseChanged;
                Bridge.platform.audioStateChanged += OnPlatformAudioChanged;
                subscribedBridgeEvents = true;
            }
            if (Bridge.advertisement != null)
            {
                Bridge.advertisement.SetMinimumDelayBetweenInterstitial(0);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[Playgama] Bridge event subscribe failed: {e.Message}");
        }
#endif
    }

    private void OnDestroy()
    {
        MagnetGameActionSystem.LevelStarted -= OnLevelStarted;
        MagnetGameActionSystem.OnLevelCompleted -= OnLevelCompleted;
        MagnetGameActionSystem.OnLevelFailed -= OnLevelFailed;

#if UNITY_WEBGL
        if (!subscribedBridgeEvents) return;
        try
        {
            if (Bridge.platform != null)
            {
                Bridge.platform.pauseStateChanged -= OnPlatformPauseChanged;
                Bridge.platform.audioStateChanged -= OnPlatformAudioChanged;
            }
        }
        catch { }
#endif
    }

    private void OnLevelStarted(int level)
    {
        TrySendMessage(PlatformMessageProxy.LevelStarted);

        levelStartsSinceLastAd++;
        float elapsed = Time.realtimeSinceStartup - lastAdRealtime;
        bool countHit = levelStartsSinceLastAd >= LevelStartThreshold;
        bool timeHit = elapsed >= TimeThresholdSeconds;
        Debug.Log($"[InterstitialAds] lvl={level} count={levelStartsSinceLastAd}/{LevelStartThreshold} elapsed={elapsed:F1}s");
        if (countHit || timeHit)
            FireInterstitial();
    }

    private void OnLevelCompleted() => TrySendMessage(PlatformMessageProxy.LevelCompleted);

    private void OnLevelFailed() => TrySendMessage(PlatformMessageProxy.LevelFailed);

    private void FireInterstitial()
    {
        levelStartsSinceLastAd = 0;
        lastAdRealtime = Time.realtimeSinceStartup;
#if UNITY_WEBGL
        try
        {
            if (Bridge.advertisement == null) return;
            Debug.Log("[InterstitialAds] ShowInterstitial(level_start)");
            Bridge.advertisement.ShowInterstitial("level_start");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[InterstitialAds] ShowInterstitial failed: {e.Message}");
        }
#endif
    }

#if UNITY_WEBGL
    private void OnPlatformPauseChanged(bool isPaused)
    {
        if (isPaused)
        {
            if (!gamePausedForAd)
            {
                savedTimeScale = Time.timeScale;
                Time.timeScale = 0f;
                gamePausedForAd = true;
            }
        }
        else
        {
            if (gamePausedForAd)
            {
                Time.timeScale = savedTimeScale;
                gamePausedForAd = false;
            }
        }
    }

    private void OnPlatformAudioChanged(bool isEnabled)
    {
        AudioListener.pause = !isEnabled;
    }
#endif

    private static void TrySendMessage(PlatformMessageProxy msg)
    {
#if UNITY_WEBGL
        try
        {
            if (Bridge.platform == null) return;
            Bridge.platform.SendMessage(ToBridge(msg));
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[Playgama] SendMessage({msg}) failed: {e.Message}");
        }
#endif
    }

#if UNITY_WEBGL
    private static PlatformMessage ToBridge(PlatformMessageProxy msg)
    {
        switch (msg)
        {
            case PlatformMessageProxy.GameReady: return PlatformMessage.GameReady;
            case PlatformMessageProxy.LevelStarted: return PlatformMessage.LevelStarted;
            case PlatformMessageProxy.LevelCompleted: return PlatformMessage.LevelCompleted;
            case PlatformMessageProxy.LevelFailed: return PlatformMessage.LevelFailed;
            default: return PlatformMessage.GameReady;
        }
    }
#endif

    private enum PlatformMessageProxy
    {
        GameReady,
        LevelStarted,
        LevelCompleted,
        LevelFailed
    }
}
