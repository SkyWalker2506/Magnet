#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Playgama.TemplateTools
{
    // Batchmode WebGL builder for Playgama games.
    // Run: Unity -batchmode -quit -projectPath <p> -buildTarget WebGL -executeMethod Playgama.TemplateTools.PlaygamaBuild.BuildWebGL
    public static class PlaygamaBuild
    {
        public static void BuildWebGL()
        {
            string scene = FindMainScene();
            UnityEngine.Debug.Log("[PlaygamaBuild] main scene = " + scene);

            // WebGL-friendly settings (most already baked in the template)
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;
            PlayerSettings.WebGL.decompressionFallback = true;
            PlayerSettings.WebGL.dataCaching = true;
            var bridge = Path.Combine(Application.dataPath, "WebGLTemplates", "Bridge");
            if (Directory.Exists(bridge)) PlayerSettings.WebGL.template = "PROJECT:Bridge";

            string outDir = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Builds", "WebGL");
            Directory.CreateDirectory(outDir);

            var opts = new BuildPlayerOptions
            {
                scenes = new[] { scene },
                locationPathName = outDir,
                target = BuildTarget.WebGL,
                targetGroup = BuildTargetGroup.WebGL,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(opts);
            var s = report.summary;
            UnityEngine.Debug.Log($"[PlaygamaBuild] result={s.result} sizeMB={s.totalSize / (1024f * 1024f):F1} errors={s.totalErrors} time={s.totalTime}");
            if (s.result != BuildResult.Succeeded)
            {
                UnityEngine.Debug.LogError("[PlaygamaBuild] BUILD FAILED");
                EditorApplication.Exit(1);
            }
            EditorApplication.Exit(0);
        }

        static string FindMainScene()
        {
            var scenes = AssetDatabase.FindAssets("t:Scene")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(p => p.StartsWith("Assets/") && !p.Contains("SampleScene"))
                .OrderBy(p => p.Length)
                .ToList();
            if (scenes.Count > 0) return scenes[0];
            return "Assets/Scenes/SampleScene.unity";
        }
    }
}
#endif
