using System;
using ColorPaletteTool.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace ColorPaletteTool.Editor
{

    public class ColorPaletteToolEditorWindow : EditorWindow
    {
        public ScriptableColorPalette ColorPalette;
        public MaterialDataCollection[] MaterialDataCollection;
        private SerializedObject so;
        private SerializedProperty materialDataProperty;
        
        
        [MenuItem("Tools/Game/Color Palette Tool")]
        public static void OpenTheWindow() => GetWindow<ColorPaletteToolEditorWindow>("Color Palette Tool");

        void OnEnable()
        {
            MaterialDataCollection = Array.Empty<MaterialDataCollection>();
            so = new SerializedObject(this);
            materialDataProperty = so.FindProperty("MaterialDataCollection");
        }
        
        void OnGUI()
        {
            CreateColorPalletArea();
            CreateMaterialsArea();
            ChangeColorsButton();

        }

        private void CreateColorPalletArea()
        {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty colorProperty = serializedObject.FindProperty("ColorPalette");

            EditorGUILayout.PropertyField(colorProperty, true);

            serializedObject.ApplyModifiedProperties();
        }
     
        private void CreateMaterialsArea()
        {
            so.Update();

            EditorGUILayout.PropertyField(materialDataProperty, true); 
            for (int i = 0; i < MaterialDataCollection.Length; i++)
            {
                MaterialDataCollection[i].Color=ColorPalette.Colors.Length>i? ColorPalette.Colors[i]:Color.clear;
            }
            so.ApplyModifiedProperties(); 
        }

        private void ChangeColorsButton()
        {
            if (GUILayout.Button("Change Palette!"))
            {
                var count = Mathf.Min(ColorPalette.Colors.Length, MaterialDataCollection.Length);
                for (int i = 0; i < count; i++)
                {
                    MaterialDataCollection[i].SetMaterialColors();
                }
            }
        }
        
        
    }
}