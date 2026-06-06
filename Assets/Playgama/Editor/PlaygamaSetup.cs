#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Playgama.TemplateTools
{
    /// <summary>
    /// One-click finisher for the Playgama Template. The render-pipeline, package
    /// (com.playgama.bridge) and most WebGL player settings are already baked into
    /// the template; the few things that can only be set from an open Editor
    /// (active build target + selecting the Bridge WebGL template) are applied here.
    /// Menu: Playgama > Setup WebGL + Bridge
    /// </summary>
    public static class PlaygamaSetup
    {
        [MenuItem("Playgama/Setup WebGL + Bridge")]
        public static void Configure()
        {
            // 1) Switch active platform to WebGL.
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL)
            {
                UnityEngine.Debug.Log("[Playgama] Switching active build target to WebGL...");
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
            }

            // 2) Playgama-friendly WebGL player settings.
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;
            PlayerSettings.WebGL.decompressionFallback = true;
            PlayerSettings.WebGL.dataCaching = true;

            // 3) Select the Bridge WebGL template once the SDK has imported it.
            string bridgeDir = Path.Combine(Application.dataPath, "WebGLTemplates", "Bridge");
            if (Directory.Exists(bridgeDir))
            {
                PlayerSettings.WebGL.template = "PROJECT:Bridge";
                UnityEngine.Debug.Log("[Playgama] Bridge WebGL template selected.");
            }
            else
            {
                UnityEngine.Debug.LogWarning("[Playgama] Bridge WebGL template not found yet. " +
                    "Wait for the Playgama Bridge package to finish importing, then run " +
                    "'Playgama > Setup WebGL + Bridge' again.");
            }

            AssetDatabase.SaveAssets();
            UnityEngine.Debug.Log("[Playgama] Setup applied. WebGL template = " + PlayerSettings.WebGL.template);
        }
    }
}
#endif
