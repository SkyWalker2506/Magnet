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
            EditorGUILayout.BeginHorizontal();
            WinLevelButton();
            LoseLevelButton();
            EditorGUILayout.EndHorizontal();
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
    
    }
}