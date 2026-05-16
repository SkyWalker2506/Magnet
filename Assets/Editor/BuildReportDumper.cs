#if UNITY_EDITOR
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildReportDumper
{
    [MenuItem("Tools/Build/Dump Last Build Report")]
    public static void Dump()
    {
        const string src = "Library/LastBuild.buildreport";
        const string dst = "Assets/LastBuildTemp.buildreport";
        if (!File.Exists(src))
        {
            Debug.LogError("Library/LastBuild.buildreport not found. Run a build first.");
            return;
        }
        File.Copy(src, dst, true);
        AssetDatabase.ImportAsset(dst, ImportAssetOptions.ForceSynchronousImport);
        var report = AssetDatabase.LoadAssetAtPath<BuildReport>(dst);
        if (report == null)
        {
            AssetDatabase.DeleteAsset(dst);
            Debug.LogError("Failed to load build report after copying.");
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine("=== BUILD SUMMARY ===");
        sb.AppendLine($"Platform: {report.summary.platform}");
        sb.AppendLine($"Result: {report.summary.result}");
        sb.AppendLine($"Total Size: {report.summary.totalSize / 1024 / 1024} MB ({report.summary.totalSize} bytes)");
        sb.AppendLine($"Time: {report.summary.totalTime}");
        sb.AppendLine($"Errors: {report.summary.totalErrors}  Warnings: {report.summary.totalWarnings}");
        sb.AppendLine();

        // Aggregate per source asset
        var perAsset = new System.Collections.Generic.Dictionary<string, (ulong size, string type)>();
        foreach (var pa in report.packedAssets)
        {
            foreach (var entry in pa.contents)
            {
                var key = string.IsNullOrEmpty(entry.sourceAssetPath) ? $"<runtime:{entry.type.Name}>" : entry.sourceAssetPath;
                if (perAsset.TryGetValue(key, out var existing))
                {
                    perAsset[key] = (existing.size + entry.packedSize, existing.type);
                }
                else
                {
                    perAsset[key] = (entry.packedSize, entry.type != null ? entry.type.Name : "?");
                }
            }
        }

        sb.AppendLine("=== TOP 60 ASSETS BY UNCOMPRESSED PACKED SIZE ===");
        sb.AppendLine($"{"Size (KB)",10}  {"Type",-22}  Asset");
        foreach (var kv in perAsset.OrderByDescending(p => p.Value.size).Take(60))
        {
            double kb = kv.Value.size / 1024.0;
            sb.AppendLine($"{kb,10:N1}  {kv.Value.type,-22}  {kv.Key}");
        }

        sb.AppendLine();
        sb.AppendLine("=== AGGREGATED BY TOP-LEVEL FOLDER ===");
        var byFolder = perAsset
            .Where(p => p.Key.StartsWith("Assets/"))
            .GroupBy(p => p.Key.Split('/').Length > 1 ? p.Key.Split('/')[1] : p.Key)
            .Select(g => new { Folder = g.Key, Total = g.Sum(p => (long)p.Value.size), Count = g.Count() })
            .OrderByDescending(x => x.Total);
        foreach (var f in byFolder)
        {
            sb.AppendLine($"{f.Total / 1024 / 1024,8} MB  {f.Count,5} files  Assets/{f.Folder}");
        }

        sb.AppendLine();
        sb.AppendLine("=== AGGREGATED BY ASSET TYPE ===");
        var byType = perAsset
            .GroupBy(p => p.Value.type)
            .Select(g => new { Type = g.Key, Total = g.Sum(p => (long)p.Value.size), Count = g.Count() })
            .OrderByDescending(x => x.Total);
        foreach (var t in byType.Take(20))
        {
            sb.AppendLine($"{t.Total / 1024 / 1024,8} MB  {t.Count,5} files  {t.Type}");
        }

        var outPath = Path.Combine(Application.dataPath, "..", "BuildReportDump.txt");
        File.WriteAllText(outPath, sb.ToString());
        AssetDatabase.DeleteAsset(dst);
        Debug.Log($"Build report dumped to: {Path.GetFullPath(outPath)}");
        EditorUtility.RevealInFinder(outPath);
    }
}
#endif
