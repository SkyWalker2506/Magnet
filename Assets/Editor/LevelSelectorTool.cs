using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectorTool : EditorWindow
{
    [MenuItem("Tools/Game/LevelSelector")]
    public static void OpenTheWindow() => GetWindow<LevelSelectorTool>("Level Selector");

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        CreateElement("Open Boot Scene",0);
        for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            CreateElement($"Level {i}",i);
        }
        EditorGUILayout.EndVertical();
    }
    
    void CreateElement (string sceneLabel, int sceneIndex) {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(sceneLabel);
        if (GUILayout.Button("Open"))
        {
            if (SceneManager.GetActiveScene().isDirty)
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            }
            EditorCoroutineUtility.StartCoroutine(IEOpenScene(sceneIndex), this);
        }
        if (GUILayout.Button("Play"))
        {
            if (SceneManager.GetActiveScene().isDirty)
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            }
            EditorCoroutineUtility.StartCoroutine(IEPlayScene(sceneIndex), this);
        }
        EditorGUILayout.EndHorizontal();
    }

    private IEnumerator IEPlayScene(int sceneIndex)
    {
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
            yield return new WaitForEndOfFrame();
        }

        if (sceneIndex > 0)
        {
            LevelManager.CurrentLevel = sceneIndex;
        }

        EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(0));
        EditorApplication.isPlaying = true;
    }

    private IEnumerator IEOpenScene(int sceneIndex)
    {
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
            yield return new WaitForEndOfFrame();
        }
        EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(sceneIndex));
    }


}
