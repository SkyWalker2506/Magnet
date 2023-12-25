using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private Renderer renderer;
    [SerializeField] private Material magnetismOnMaterial;
    [SerializeField] private Material magnetismOffMaterial;
    
    public void TogglePlayerVisual(bool isMagnetic)
    {
        renderer.material = isMagnetic?magnetismOnMaterial:magnetismOffMaterial;
    }
}