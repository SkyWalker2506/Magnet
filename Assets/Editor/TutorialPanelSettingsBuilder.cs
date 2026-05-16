#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[InitializeOnLoad]
internal static class TutorialPanelSettingsBuilder
{
    private const string AssetPath = "Assets/UI/TutorialPanelSettings.asset";
    private const string DefaultThemePath = "Packages/com.unity.ui/PackageResources/StyleSheets/Generated/UnityDefaultRuntimeTheme.tss.asset";

    static TutorialPanelSettingsBuilder()
    {
        EditorApplication.delayCall += EnsureAsset;
    }

    private static void EnsureAsset()
    {
        if (AssetDatabase.LoadAssetAtPath<PanelSettings>(AssetPath) != null) return;
        if (!AssetDatabase.IsValidFolder("Assets/UI")) AssetDatabase.CreateFolder("Assets", "UI");

        var settings = ScriptableObject.CreateInstance<PanelSettings>();
        settings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
        settings.referenceResolution = new Vector2Int(1080, 1920);
        settings.match = 0.5f;
        settings.sortingOrder = 5000;
        settings.clearColor = false;

        var theme = AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(DefaultThemePath);
        if (theme != null) settings.themeStyleSheet = theme;

        AssetDatabase.CreateAsset(settings, AssetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif
