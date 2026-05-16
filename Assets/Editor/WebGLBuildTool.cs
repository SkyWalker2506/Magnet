using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EditorTools
{
    public static class WebGLBuildTool
    {
        private const string DefaultBuildPath = "Builds/WebGL";
        private const string ProbeVolumePerSceneDataName = "ProbeVolumePerSceneData";

        [MenuItem("Tools/Build/WebGL")]
        public static void BuildWebGL()
        {
            var buildPath = GetArgumentValue("-buildPath") ?? DefaultBuildPath;
            var scenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();

            if (scenes.Length == 0)
            {
                throw new InvalidOperationException("No enabled scenes found in Editor Build Settings.");
            }

            PrepareProjectForWebGL(scenes);
            Directory.CreateDirectory(buildPath);

            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = buildPath,
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            });

            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                throw new InvalidOperationException($"WebGL build failed: {report.summary.result}");
            }
        }

        [MenuItem("Tools/Build/Prepare WebGL")]
        public static void PrepareWebGL()
        {
            var scenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();

            PrepareProjectForWebGL(scenes);
        }

        private static void PrepareProjectForWebGL(string[] scenes)
        {
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                throw new InvalidOperationException("WebGL preparation was cancelled because open scene changes were not saved.");
            }

            DisableAdaptiveProbeVolumesInRenderPipelineAssets();
            RemoveAdaptiveProbeVolumeBakedData(scenes);
            AssetDatabase.SaveAssets();
        }

        private static void DisableAdaptiveProbeVolumesInRenderPipelineAssets()
        {
            var urpAssetGuids = AssetDatabase.FindAssets("t:UniversalRenderPipelineAsset");

            foreach (var guid in urpAssetGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadMainAssetAtPath(path);

                if (asset == null)
                {
                    continue;
                }

                var serializedAsset = new SerializedObject(asset);
                SetInt(serializedAsset, "m_LightProbeSystem", 0);
                SetBool(serializedAsset, "m_SupportProbeVolumeGPUStreaming", false);
                SetBool(serializedAsset, "m_SupportProbeVolumeDiskStreaming", false);
                SetBool(serializedAsset, "m_SupportProbeVolumeScenarios", false);
                SetBool(serializedAsset, "m_SupportProbeVolumeScenarioBlending", false);
                serializedAsset.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(asset);
            }
        }

        private static void RemoveAdaptiveProbeVolumeBakedData(string[] scenes)
        {
            foreach (var scenePath in scenes)
            {
                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                var changed = false;

                foreach (var gameObject in EnumerateSceneObjects(scene))
                {
                    if (gameObject.name != ProbeVolumePerSceneDataName)
                    {
                        continue;
                    }

                    UnityEngine.Object.DestroyImmediate(gameObject);
                    changed = true;
                }

                if (changed)
                {
                    EditorSceneManager.SaveScene(scene);
                    Debug.Log($"Removed WebGL-incompatible APV baked data from {scenePath}.");
                }
            }
        }

        private static IEnumerable<GameObject> EnumerateSceneObjects(Scene scene)
        {
            var stack = new Stack<Transform>();

            foreach (var root in scene.GetRootGameObjects())
            {
                stack.Push(root.transform);
            }

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                for (var i = 0; i < current.childCount; i++)
                {
                    stack.Push(current.GetChild(i));
                }

                yield return current.gameObject;
            }
        }

        private static void SetInt(SerializedObject serializedObject, string propertyName, int value)
        {
            var property = serializedObject.FindProperty(propertyName);

            if (property != null)
            {
                property.intValue = value;
            }
        }

        private static void SetBool(SerializedObject serializedObject, string propertyName, bool value)
        {
            var property = serializedObject.FindProperty(propertyName);

            if (property != null)
            {
                property.boolValue = value;
            }
        }

        private static string GetArgumentValue(string argumentName)
        {
            var args = Environment.GetCommandLineArgs();

            for (var i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == argumentName)
                {
                    return args[i + 1];
                }
            }

            return null;
        }
    }
}
