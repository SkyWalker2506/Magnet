using System;
using UnityEngine;

namespace ColorPaletteTool.Runtime
{
    [Serializable]
    public class MaterialDataCollection
    {
        public Color Color;
        public MaterialData[] MaterialDatas;

        public void SetMaterialColors()
        {
            foreach (var materialData in MaterialDatas)
            {
                materialData.SetMaterialColor(Color);
            }
        }
    }
    
    [Serializable]
    public class MaterialData
    {
        public Material Material;
        public string ShaderProperty;
        
        public void SetMaterialColor(Color color)
        {
            Material.SetColor(ShaderProperty, color);
        }
    }
    
    

}