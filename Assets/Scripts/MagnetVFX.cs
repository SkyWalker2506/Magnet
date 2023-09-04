using UnityEngine;

public class MagnetVFX : MonoBehaviour
{
    [SerializeField] private GameObject vfx;
    [SerializeField] private Transform pos1;
    [SerializeField] private Transform pos2;
    [SerializeField] private Transform pos3;
    [SerializeField] private Transform pos4;

    private Metal targetMetal;
    
    public void SetTarget(Metal target)
    {
        targetMetal = target;
        vfx.SetActive(true);
    }

    private void Update()
    {
        if (!(targetMetal && targetMetal.enabled&&targetMetal.IsMagnetized))
        {
            vfx.SetActive(false);
            return;
        }

        pos1.position = Player.CurrentPlayer.transform.position;
        pos4.position = targetMetal.transform.position;
    }
}
