using UnityEngine;

public class MagnetVFX : MonoBehaviour
{
    [SerializeField] private GameObject vfx;
    [SerializeField] public Transform Pos1;
    [SerializeField] public Transform Pos2;
    [SerializeField] public Transform Pos3;
    [SerializeField] public Transform Pos4;
    public bool IsActive => vfx.activeSelf;
    private void Awake()
    {
        SetActive(false);
    }

    public void SetTargets(Transform target1, Transform target2)
    {
        Pos1.parent = target1;
        Pos1.localPosition = Vector3.zero;
        Pos4.parent = target2;
        Pos4.localPosition = Vector3.zero;
    }
    
    public void SetActive(bool isActive)
    {
        vfx.SetActive(isActive);
    }
}
