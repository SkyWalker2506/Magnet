using UnityEngine;
using VInspector;

[CreateAssetMenu(menuName = "Create GameColorSetter", fileName = "GameColorSetter", order = 0)]
public class GameColorSetter : ScriptableObject
{
    
    [SerializeField] private bool setAuto;
    [SerializeField] private Color playerColor;
    [SerializeField] private Color metalColor;
    [SerializeField] private Color woodColor;
    [SerializeField] private Color groundColor;
    [SerializeField] private Material playerMaterial;
    [SerializeField] private Material metalMaterial;
    [SerializeField] private Material woodMaterial;
    [SerializeField] private Material groundMaterial;

    private void OnValidate()
    {
        if (!setAuto)
        {
            return;
        }

        SetAll();
    }

     [Button]
    void SetPlayerColor()
    {
        if (playerMaterial)
        {
            playerMaterial.color = playerColor;
        }
    }
    
    [Button]
    void SetMetalColor()
    {
        if (metalMaterial)
        {
            metalMaterial.color = metalColor;
        }
    }
    
    [Button]
    void SetWoodColor()
    {
        if (woodMaterial)
        {
            woodMaterial.color = woodColor;
        }
    }
    
    [Button]
    void SetGroundColor()
    {
        if (groundMaterial)
        {
            groundMaterial.color = groundColor;
        }
    }
    
    
    [Button]
    void SetAll()
    {
        SetPlayerColor();
        SetMetalColor();
        SetWoodColor();
        SetGroundColor();
    }
}
