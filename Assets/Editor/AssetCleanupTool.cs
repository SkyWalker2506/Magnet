#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class AssetCleanupTool
{
    [MenuItem("Tools/Build/Clear All Lightmaps (every build scene)")]
    public static void ClearAllLightmaps()
    {
        if (!EditorUtility.DisplayDialog(
                "Clear All Lightmaps",
                "This will open each scene listed in Build Settings, clear baked lightmap data, save, and close it.\n\nMake sure all scenes are saved first.\n\nContinue?",
                "Yes, clear",
                "Cancel"))
        {
            return;
        }

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
        int cleared = 0;
        int total = scenes.Length;

        for (int i = 0; i < total; i++)
        {
            string path = scenes[i];
            EditorUtility.DisplayProgressBar("Clearing lightmaps", path, (float)i / total);
            try
            {
                var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                Lightmapping.ClearLightingDataAsset();
                Lightmapping.Clear();
                EditorSceneManager.SaveScene(scene);
                cleared++;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Could not clear lightmap for {path}: {e.Message}");
            }
        }

        EditorUtility.ClearProgressBar();
        Debug.Log($"Cleared baked lightmap data in {cleared}/{total} scenes. Now delete the per-scene Lightmap-*.png files manually or via the cleanup helper.");
        DeleteOrphanedLightmapPngs();
    }

    private static void DeleteOrphanedLightmapPngs()
    {
        string scenesRoot = "Assets/Scenes";
        if (!Directory.Exists(scenesRoot)) return;

        var lightmapPngs = Directory
            .GetFiles(scenesRoot, "Lightmap-*.png", SearchOption.AllDirectories)
            .ToList();

        var lightingAssets = Directory
            .GetFiles(scenesRoot, "*.lighting", SearchOption.AllDirectories)
            .ToList();

        long total = 0;
        int n = 0;
        foreach (var p in lightmapPngs)
        {
            var info = new FileInfo(p);
            total += info.Length;
            AssetDatabase.DeleteAsset(p.Replace("\\", "/"));
            n++;
        }
        foreach (var p in lightingAssets)
        {
            AssetDatabase.DeleteAsset(p.Replace("\\", "/"));
        }
        // Also remove now-empty per-scene Lightmap folders (Scenes/Levels/<n>/ which only held lightmap pngs)
        foreach (var dir in Directory.GetDirectories(scenesRoot, "*", SearchOption.AllDirectories))
        {
            if (!Directory.Exists(dir)) continue;
            if (Directory.GetFileSystemEntries(dir).Length == 0)
            {
                AssetDatabase.DeleteAsset(dir.Replace("\\", "/"));
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Deleted {n} Lightmap-*.png files (~{total / 1024 / 1024} MB) and orphan .lighting assets.");
    }

    [MenuItem("Tools/Build/Scan Unused TMP Fonts")]
    public static void ScanUnusedTmpFonts()
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== TMP FONT USAGE SCAN ===\n");

        var fontGuids = AssetDatabase.FindAssets("t:Object", new[] { "Assets" })
            .Select(g => new { Guid = g, Path = AssetDatabase.GUIDToAssetPath(g) })
            .Where(p => p.Path.Contains("SDF") && (p.Path.EndsWith(".asset") || p.Path.EndsWith(".png")))
            .ToList();

        var fontAssets = fontGuids.Where(p => p.Path.EndsWith("SDF.asset")).ToList();
        var fontAtlases = fontGuids.Where(p => p.Path.EndsWith("SDF Atlas.png") || p.Path.EndsWith("SDF Atlas.asset")).ToList();
        var fontMaterials = fontGuids
            .Where(p => p.Path.EndsWith(".asset") && !p.Path.EndsWith("SDF.asset"))
            .ToList();

        sb.AppendLine($"SDF Font Assets found: {fontAssets.Count}");
        sb.AppendLine($"SDF Atlas textures found: {fontAtlases.Count}");
        sb.AppendLine();

        // Build searchable text from all scenes/prefabs/materials/uxml/uss
        sb.AppendLine("=== USAGE PER FONT ASSET ===");
        sb.AppendLine();
        foreach (var f in fontAssets)
        {
            int refs = CountReferences(f.Guid);
            long size = new FileInfo(f.Path).Length;
            string flag = refs == 0 ? "[UNUSED]" : "        ";
            sb.AppendLine($"{flag}  refs={refs,3}  {size / 1024,8} KB  {f.Path}");
        }

        sb.AppendLine();
        sb.AppendLine("=== USAGE PER ATLAS TEXTURE ===");
        sb.AppendLine();
        foreach (var f in fontAtlases.OrderByDescending(p => new FileInfo(p.Path).Length))
        {
            int refs = CountReferences(f.Guid);
            long size = new FileInfo(f.Path).Length;
            string flag = refs == 0 ? "[UNUSED]" : "        ";
            sb.AppendLine($"{flag}  refs={refs,3}  {size / 1024,8} KB  {f.Path}");
        }

        string outPath = Path.Combine(Application.dataPath, "..", "UnusedFontsReport.txt");
        File.WriteAllText(outPath, sb.ToString());
        Debug.Log($"Font usage report written to: {Path.GetFullPath(outPath)}");
        EditorUtility.RevealInFinder(outPath);
    }

    private static int CountReferences(string guid)
    {
        int count = 0;
        string[] searchRoots = { "Assets/Scenes", "Assets/Prefabs", "Assets/UI", "Assets/Materials", "Assets/Scripts", "Assets/Resources" };
        var exts = new[] { ".unity", ".prefab", ".asset", ".mat", ".controller", ".uxml", ".uss", ".cs" };

        foreach (var root in searchRoots)
        {
            if (!Directory.Exists(root)) continue;
            foreach (var file in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
            {
                if (!exts.Any(e => file.EndsWith(e))) continue;
                try
                {
                    var text = File.ReadAllText(file);
                    if (text.Contains(guid)) count++;
                }
                catch { }
            }
        }
        return count;
    }
}
#endif
