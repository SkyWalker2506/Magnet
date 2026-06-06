using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Playgama.TemplateTools
{
    /// <summary>
    /// Swaps the default (Arial) font on all legacy UI Text components to a nicer
    /// bundled font (Resources/Fonts/Poppins-SemiBold). Runs automatically; no per-Text
    /// editing or scene changes required. Re-applies for a while to catch runtime-created UI.
    /// </summary>
    public class FontBeautifier : MonoBehaviour
    {
        const string FontResourcePath = "Fonts/Poppins-SemiBold";
        static Font _font;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Boot()
        {
            _font = Resources.Load<Font>(FontResourcePath);
            if (_font == null) return;
            var go = new GameObject("~FontBeautifier");
            DontDestroyOnLoad(go);
            go.AddComponent<FontBeautifier>();
        }

        void Start()
        {
            Apply();
            SceneManager.sceneLoaded += (s, m) => Apply();
            InvokeRepeating(nameof(Apply), 0.5f, 1.0f); // catch UI created later (game-over, popups)
        }

        void Apply()
        {
            if (_font == null) return;
            foreach (var t in Resources.FindObjectsOfTypeAll<Text>())
            {
                if (t != null && t.gameObject.scene.IsValid() && t.font != _font)
                    t.font = _font;
            }
        }
    }
}
