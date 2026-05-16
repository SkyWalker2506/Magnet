using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class TutorialHint : MonoBehaviour
{
    private static bool dismissed;

    [SerializeField] private float fadeDuration = 0.35f;

    private UIDocument doc;
    private bool fading;
    private float fadeStart;

    private void Awake()
    {
        doc = GetComponent<UIDocument>();
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
        PlayerController.OnPlayerFirstMoved += HandlePlayerMoved;
        TryShow();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        PlayerController.OnPlayerFirstMoved -= HandlePlayerMoved;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryShow();
    }

    private void TryShow()
    {
        if (doc == null) return;
        var root = doc.rootVisualElement;
        if (root == null) return;
        root.pickingMode = PickingMode.Ignore;
        bool isLevel = int.TryParse(SceneManager.GetActiveScene().name, out _);
        bool show = isLevel && !dismissed;
        root.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        root.style.opacity = 1f;
        fading = false;
    }

    private void HandlePlayerMoved()
    {
        if (dismissed) return;
        dismissed = true;
        fading = true;
        fadeStart = Time.unscaledTime;
    }

    private void Update()
    {
        if (!fading || doc == null) return;
        var root = doc.rootVisualElement;
        if (root == null) return;
        float t = (Time.unscaledTime - fadeStart) / Mathf.Max(0.01f, fadeDuration);
        if (t >= 1f)
        {
            root.style.display = DisplayStyle.None;
            fading = false;
            return;
        }
        root.style.opacity = 1f - t;
    }
}
