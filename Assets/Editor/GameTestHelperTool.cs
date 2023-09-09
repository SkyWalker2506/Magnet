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
    
    }
}