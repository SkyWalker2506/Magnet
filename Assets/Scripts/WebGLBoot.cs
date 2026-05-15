using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public static class WebGLBoot
{
    private const string BuildVersionKey = "MagnetBuildVersion";
    private const string CurrentBuildVersion = "2026-05-15-v2";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetSavesOnNewBuild()
    {
        if (PlayerPrefs.GetString(BuildVersionKey, string.Empty) == CurrentBuildVersion)
        {
            return;
        }

        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetString(BuildVersionKey, CurrentBuildVersion);
        PlayerPrefs.Save();
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InstallAmbientFallback()
    {
        ApplyAmbient();
        SceneManager.sceneLoaded += (scene, mode) => ApplyAmbient();
    }

    private static void ApplyAmbient()
    {
        RenderSettings.ambientMode = AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(0.85f, 0.90f, 1.00f, 1f);
        RenderSettings.ambientEquatorColor = new Color(0.50f, 0.55f, 0.65f, 1f);
        RenderSettings.ambientGroundColor = new Color(0.25f, 0.27f, 0.30f, 1f);
        RenderSettings.ambientIntensity = 1.3f;
    }
#endif
}
