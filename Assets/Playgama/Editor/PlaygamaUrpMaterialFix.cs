#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Playgama.TemplateTools
{
    // Fixes magenta rendering in URP/WebGL builds:
    //   1) Converts project Standard/Legacy/error materials -> URP/Lit (color/texture remapped).
    //   2) Reassigns Renderers (in prefabs + scenes) that use the built-in Default-Material / null / error
    //      shader to a shared URP/Lit default material.
    //   Convert only:  -executeMethod Playgama.TemplateTools.PlaygamaUrpMaterialFix.FixMaterials
    //   Convert+build: -executeMethod Playgama.TemplateTools.PlaygamaUrpMaterialFix.FixAndBuildWebGL
    public static class PlaygamaUrpMaterialFix
    {
        const string DefaultMatPath = "Assets/Playgama/PlaygamaURPDefault.mat";
        static Shader _urpLit, _err;

        static bool NeedsFix(Material m)
        {
            if (m == null || m.shader == null) return true;
            if (m.shader == _err) return true;
            string n = m.shader.name;
            return n == "Standard" || n == "Standard (Specular setup)" || n.StartsWith("Legacy Shaders/");
        }

        public static int Convert()
        {
            _urpLit = Shader.Find("Universal Render Pipeline/Lit");
            _err = Shader.Find("Hidden/InternalErrorShader");
            if (_urpLit == null) { UnityEngine.Debug.LogError("[UrpFix] URP/Lit not found — not a URP project?"); return -1; }

            // 1) project materials Standard/Legacy -> URP/Lit
            int matConv = 0;
            foreach (var g in AssetDatabase.FindAssets("t:Material"))
            {
                var path = AssetDatabase.GUIDToAssetPath(g);
                if (path.StartsWith("Packages/")) continue;
                var m = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (m == null) continue;
                if (!NeedsFix(m)) continue;
                Color col = m.HasProperty("_Color") ? m.GetColor("_Color") : Color.white;
                Texture tex = m.HasProperty("_MainTex") ? m.GetTexture("_MainTex") : null;
                m.shader = _urpLit;
                if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", col);
                if (m.HasProperty("_BaseMap") && tex) m.SetTexture("_BaseMap", tex);
                EditorUtility.SetDirty(m);
                matConv++;
                UnityEngine.Debug.Log($"[UrpFix] mat {path}: -> URP/Lit");
            }
            AssetDatabase.SaveAssets();

            // shared URP default material for renderers that point at the built-in Default-Material
            var urpDefault = AssetDatabase.LoadAssetAtPath<Material>(DefaultMatPath);
            if (urpDefault == null)
            {
                urpDefault = new Material(_urpLit) { name = "PlaygamaURPDefault" };
                if (urpDefault.HasProperty("_BaseColor")) urpDefault.SetColor("_BaseColor", new Color(0.7f, 0.7f, 0.7f, 1f));
                Directory.CreateDirectory("Assets/Playgama");
                AssetDatabase.CreateAsset(urpDefault, DefaultMatPath);
                AssetDatabase.SaveAssets();
            }

            // 2a) prefabs
            int rendFixed = 0;
            foreach (var g in AssetDatabase.FindAssets("t:Prefab"))
            {
                var path = AssetDatabase.GUIDToAssetPath(g);
                if (path.StartsWith("Packages/")) continue;
                var root = PrefabUtility.LoadPrefabContents(path);
                bool changed = false;
                foreach (var r in root.GetComponentsInChildren<Renderer>(true))
                {
                    var mats = r.sharedMaterials; bool c = false;
                    for (int i = 0; i < mats.Length; i++)
                        if (NeedsFix(mats[i])) { mats[i] = urpDefault; c = true; }
                    if (c) { r.sharedMaterials = mats; changed = true; rendFixed++; }
                }
                if (changed) PrefabUtility.SaveAsPrefabAsset(root, path);
                PrefabUtility.UnloadPrefabContents(root);
            }

            // 2b) scenes
            foreach (var g in AssetDatabase.FindAssets("t:Scene"))
            {
                var path = AssetDatabase.GUIDToAssetPath(g);
                if (path.StartsWith("Packages/")) continue;
                var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                bool changed = false;
                foreach (var root in scene.GetRootGameObjects())
                    foreach (var r in root.GetComponentsInChildren<Renderer>(true))
                    {
                        var mats = r.sharedMaterials; bool c = false;
                        for (int i = 0; i < mats.Length; i++)
                            if (NeedsFix(mats[i])) { mats[i] = urpDefault; c = true; }
                        if (c) { r.sharedMaterials = mats; changed = true; rendFixed++; }
                    }
                if (changed) EditorSceneManager.SaveScene(scene);
            }

            UnityEngine.Debug.Log($"[UrpFix] materials converted={matConv} renderers reassigned={rendFixed}");
            return matConv + rendFixed;
        }

        public static void FixMaterials()
        {
            int n = Convert();
            EditorApplication.Exit(n < 0 ? 2 : 0);
        }

        public static void FixAndBuildWebGL()
        {
            int n = Convert();
            if (n < 0) { EditorApplication.Exit(2); return; }
            PlaygamaBuild.BuildWebGL(); // builds WebGL and exits
        }
    }
}
#endif
