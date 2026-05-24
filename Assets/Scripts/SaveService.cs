using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_WEBGL && !UNITY_EDITOR
using Playgama;
using Playgama.Modules.Storage;
#endif

public static class SaveService
{
    private const int MaxLevels = 30;
    private const string KeyLastPassedLevel = "LastPassedLevel";
    private const string KeyHighestUnlocked = "HighestUnlockedLevel";

    public static bool IsPreloadComplete { get; private set; }
    public static event Action PreloadCompleted;

    public static int GetInt(string key, int defaultValue = 0) => PlayerPrefs.GetInt(key, defaultValue);

    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        MirrorToCloud(key, value.ToString());
    }

    public static string GetString(string key, string defaultValue = "") => PlayerPrefs.GetString(key, defaultValue);

    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        MirrorToCloud(key, value);
    }

    public static void Save() => PlayerPrefs.Save();

    public static void StartPreload()
    {
        if (IsPreloadComplete) return;
#if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            if (Bridge.storage == null)
            {
                CompletePreload();
                return;
            }
            Bridge.storage.Get(BuildSyncKeys(), OnCloudReturned);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[SaveService] Cloud preload failed: {e.Message}");
            CompletePreload();
        }
#else
        CompletePreload();
#endif
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    private static void OnCloudReturned(bool success, List<string> values)
    {
        if (success && values != null)
        {
            var keys = BuildSyncKeys();
            int count = Math.Min(keys.Count, values.Count);
            for (int i = 0; i < count; i++)
            {
                if (string.IsNullOrEmpty(values[i])) continue;
                if (int.TryParse(values[i], out int v))
                    PlayerPrefs.SetInt(keys[i], v);
            }
            PlayerPrefs.Save();
        }
        CompletePreload();
    }

    private static void MirrorToCloud(string key, string value)
    {
        try
        {
            if (Bridge.storage == null) return;
            Debug.Log($"[SaveService] Bridge.storage.Set {key}={value}");
            Bridge.storage.Set(key, value);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[SaveService] Cloud mirror failed for {key}: {e.Message}");
        }
    }
#else
    private static void MirrorToCloud(string key, string value) { }
#endif

    private static List<string> BuildSyncKeys()
    {
        var keys = new List<string>(MaxLevels + 2) { KeyLastPassedLevel, KeyHighestUnlocked };
        for (int i = 1; i <= MaxLevels; i++) keys.Add($"Level {i} Stars");
        return keys;
    }

    private static void CompletePreload()
    {
        IsPreloadComplete = true;
        try { PreloadCompleted?.Invoke(); } catch { }
    }
}
