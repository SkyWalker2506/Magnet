using System;
using UnityEngine;

public class MagnetVFX : MonoBehaviour
{
    [SerializeField] private GameObject vfx;
    [SerializeField] private Transform pos1;
    [SerializeField] private Transform pos2;
    [SerializeField] private Transform pos3;
    [SerializeField] private Transform pos4;

    private void Awake()
    {
        SetActive(false);
    }

    public void SetTargets(Transform target1, Transform target2)
    {
        pos1.parent = target1;
        pos1.localPosition = Vector3.zero;
        pos4.parent = target2;
        pos4.localPosition = Vector3.zero;
    }
    
    public void SetActive(bool isActive)
    {
        vfx.SetActive(isActive);
    }


    private void Update()
    {
        if (!vfx.activeSelf)
        {
            return;
        }

        pos2.position = Vector3.Lerp(pos1.position, pos4.position, .33f)+Vector3.down+Vector3.right;
        pos3.position = Vector3.Lerp(pos1.position, pos4.position, .66f)+Vector3.up*3+Vector3.left;

    }
}
