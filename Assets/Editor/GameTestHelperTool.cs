using UnityEditor;
using UnityEngine;

namespace EditorTools
{
    public class GameTestHelperTool : EditorWindow
    {
        [MenuItem("Tools/Game/GameTestHelper")]
        public static void OpenTheWindow() => GetWindow<GameTestHelperTool>("Game Test Helper");

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            WinLevelButton();
            LoseLevelButton();
            EditorGUILayout.EndHorizontal();
            SetFrameRate();
            ReplaceMetals();
            EditorGUILayout.EndVertical();

        }

        void WinLevelButton()
        {
            if (GUILayout.Button("Win Level"))
            {
                MagnetGameActionSystem.OnLevelCompleted?.Invoke();
            }
        }
    
        void LoseLevelButton()
        {
            if (GUILayout.Button("Lose Level"))
            {
                MagnetGameActionSystem.OnLevelFailed?.Invoke();
            }
        }

        private int frameRate;
        void SetFrameRate()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 70;   

            frameRate = EditorGUILayout.IntField("Frame rate:", frameRate);

            if (GUILayout.Button("Set Frame Rate"))
            {
                Application.targetFrameRate = frameRate;
            } 
            EditorGUILayout.EndHorizontal();

        }
        
        private Object metalPrefab;
        void ReplaceMetals()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 70;   

            metalPrefab = EditorGUILayout.ObjectField( metalPrefab,typeof(GameObject),false);
            var sceneMetals = FindObjectsOfType<Metal>();
            if (GUILayout.Button("Replace to Metal Prefabs"))
            {
                foreach (var metal in sceneMetals)
                {
                    GameObject createdMetal =(GameObject)PrefabUtility.InstantiatePrefab(metalPrefab,metal.transform.parent);
                    createdMetal.transform.position = metal.transform.position;
                    DestroyImmediate(metal.gameObject);
                }
            } 
            EditorGUILayout.EndHorizontal();

        }
    
    }
}