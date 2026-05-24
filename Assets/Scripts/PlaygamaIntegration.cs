using System;
using System.Collections;
using UnityEngine;
#if UNITY_WEBGL
using Playgama;
using Playgama.Modules.Platform;
using Playgama.Modules.Game;
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
    private bool subscribedBridgeEvents;

    private IEnumerator Start()
    {
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
            if (Bridge.game != null)
            {
                Bridge.game.visibilityStateChanged += OnVisibilityChanged;
                subscribedBridgeEvents = true;
            }
            if (Bridge.advertisement != null)
            {
                Bridge.advertisement.interstitialStateChanged += OnInterstitialChanged;
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
            if (Bridge.game != null) Bridge.game.visibilityStateChanged -= OnVisibilityChanged;
            if (Bridge.advertisement != null) Bridge.advertisement.interstitialStateChanged -= OnInterstitialChanged;
        }
        catch { }
#endif
    }

    private void OnLevelStarted(int level) => TrySendMessage(PlatformMessageProxy.LevelStarted);

    private void OnLevelCompleted()
    {
        TrySendMessage(PlatformMessageProxy.LevelCompleted);
        if (LevelManager.IsInitialized && LevelManager.CurrentLevel > 1)
            TryShowInterstitial();
    }

    private void OnLevelFailed() => TrySendMessage(PlatformMessageProxy.LevelFailed);

#if UNITY_WEBGL
    private void OnVisibilityChanged(VisibilityState state)
    {
        bool hidden = state == VisibilityState.Hidden;
        AudioListener.pause = hidden;
    }

    private void OnInterstitialChanged(InterstitialState state)
    {
        switch (state)
        {
            case InterstitialState.Opened:
                AudioListener.pause = true;
                break;
            case InterstitialState.Closed:
            case InterstitialState.Failed:
                AudioListener.pause = false;
                break;
        }
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

    private static void TryShowInterstitial()
    {
#if UNITY_WEBGL
        try
        {
            if (Bridge.advertisement == null) return;
            if (!Bridge.advertisement.isInterstitialSupported) return;
            Bridge.advertisement.ShowInterstitial("level_complete");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[Playgama] ShowInterstitial failed: {e.Message}");
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
