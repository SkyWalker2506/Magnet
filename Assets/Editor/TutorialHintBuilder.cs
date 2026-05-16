#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[InitializeOnLoad]
internal static class TutorialHintBuilder
{
    private const string PrefabPath = "Assets/Prefabs/Managers/TutorialHint.prefab";
    private const string UxmlPath = "Assets/UI/TutorialOverlay.uxml";
    private const string PanelPath = "Assets/UI/TutorialPanelSettings.asset";
    private const string GameManagerPath = "Assets/Prefabs/Managers/GameManager.prefab";

    static TutorialHintBuilder()
    {
        EditorApplication.delayCall += Run;
    }

    private static void Run()
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        if (prefab == null)
        {
            prefab = CreatePrefab();
            if (prefab == null) return;
        }
        EnsureInGameManagerList(prefab);
    }

    private static GameObject CreatePrefab()
    {
        var panel = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelPath);
        var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);
        if (panel == null || uxml == null)
        {
            return null;
        }

        var folder = System.IO.Path.GetDirectoryName(PrefabPath);
        if (!AssetDatabase.IsValidFolder(folder)) AssetDatabase.CreateFolder("Assets/Prefabs", "Managers");

        var go = new GameObject("TutorialHint");
        var doc = go.AddComponent<UIDocument>();
        doc.panelSettings = panel;
        doc.visualTreeAsset = uxml;
        go.AddComponent<TutorialHint>();

        var saved = PrefabUtility.SaveAsPrefabAsset(go, PrefabPath);
        Object.DestroyImmediate(go);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return saved;
    }

    private static void EnsureInGameManagerList(GameObject hintPrefab)
    {
        var managerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameManagerPath);
        if (managerPrefab == null) return;
        var gm = managerPrefab.GetComponent<GameManager>();
        if (gm == null) return;
        var so = new SerializedObject(gm);
        var list = so.FindProperty("systemPrefabs");
        if (list == null || !list.isArray) return;

        for (int i = 0; i < list.arraySize; i++)
        {
            if (list.GetArrayElementAtIndex(i).objectReferenceValue == hintPrefab) return;
        }
        list.arraySize++;
        list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = hintPrefab;
        so.ApplyModifiedPropertiesWithoutUndo();
        PrefabUtility.SavePrefabAsset(managerPrefab);
        AssetDatabase.SaveAssets();
    }
}
#endif
