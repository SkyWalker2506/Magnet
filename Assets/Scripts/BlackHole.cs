using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [SerializeField]
    LayerMask metalLayer;
    [SerializeField]
    LayerMask otherLayers;
    List<GameObject> dropObjects=new List<GameObject>();
    public MetalType Type = MetalType.Black;
    [SerializeField]
    bool isForCollecting =true;
    GameObject collectedObject;
    private void OnTriggerEnter(Collider other)
    {
        var metal = other.GetComponent<Metal>();
        if (metal)
        {
            if(dropObjects.Contains(other.gameObject))
              metal.UseMagnetism= false;
          
            metal.MetalRB.constraints = RigidbodyConstraints.None;
            var direction = (other.transform.position- Vector3.up * 10).normalized;
            metal.MetalRB.velocity = direction * 1;
            dropObjects.Add(other.gameObject);

                other.isTrigger = true;
            collectedObject = other.gameObject;
            if (Type == metal.Type && isForCollecting)
            {
                MagnetGameActionSystem.ObjectCollected?.Invoke(dropObjects.Count);
                AudioManager.PlayDropToHoleClip();
            }
            else if (Type != metal.Type)
            {
                MagnetGameActionSystem.OnLevelFailed?.Invoke();
                AudioManager.PlayDropToLavaClip();

            }
                //Invoke("DeactivateCollected",.12f);
        }
    }

    void DeactivateCollected(GameObject go)
    {
        go.GetComponent<MeshRenderer>().enabled = false;
        go.GetComponent<Collider>().enabled = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if(dropObjects.Contains(other.gameObject))
        {
            if (isForCollecting)
            {
                DeactivateCollected(other.gameObject);
            }
                //other.transform.position = transform.position + Vector3.down*2;
            //other.attachedRigidbody.velocity = Vector3.zero;
            //other.attachedRigidbody.useGravity = true;
            //other.isTrigger = false;

        }
    }
}