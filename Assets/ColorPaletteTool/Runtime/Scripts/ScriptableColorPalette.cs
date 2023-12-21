using UnityEngine;

namespace ColorPaletteTool.Runtime
{
    [CreateAssetMenu(menuName = "Create ScriptableColorPalette", fileName = "Color Palette", order = 0)]
    public class ScriptableColorPalette : ScriptableObject
    {
        [field:SerializeField] public Color[] Colors { get; set; }
    }
}
