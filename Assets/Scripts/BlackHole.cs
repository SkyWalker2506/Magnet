﻿using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    List<GameObject> dropObjects=new List<GameObject>();
    public MetalType Type = MetalType.Black;
    [SerializeField]
    bool isForCollecting =true;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Metal metal))
        {
            Vector3 targetPos = (transform.position+ Vector3.down*10);
            other.enabled = false;
            metal.transform.DOMove(targetPos, 2).OnComplete(() =>
            {
                if (isForCollecting)
                {
                    if (other.TryGetComponent(out ICollectable collectable))
                    {
                        collectable.Collect();
                    }
                }
            });
            dropObjects.Add(other.gameObject);

                other.isTrigger = true;
            if (Type == metal.Type && isForCollecting)
            {
                MagnetGameActionSystem.ObjectCollected?.Invoke(dropObjects.Count);
                //AudioManager.PlayDropToHoleClip();
                SoundManager.Instance.PlaySfx("DropToHoleClip");
            }
            else if (Type != metal.Type)
            {
                MagnetGameActionSystem.OnLevelFailed?.Invoke();
                //AudioManager.PlayDropToLavaClip();
                SoundManager.Instance.PlaySfx("DropToLavaClip");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(dropObjects.Contains(other.gameObject))
        {

        }
    }
}