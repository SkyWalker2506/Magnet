using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace DoorMechanic
{
    [RequireComponent(typeof(Collider))]
    public class DoorTrigger : MonoBehaviour
    {
        [SerializeField] private Transform triggerVisual;
        public Action OnTriggerUpdated;
        [SerializeField] private float triggeredHeight = 0;
        [SerializeField] private float normalHeight = .1f;
        public bool IsTriggered { get; private set; }
        private HashSet<Collider> inTriggerColliders=new HashSet<Collider>();
        private void Awake()
        {
            SetVisual(false, 0);
        }

        private void OnTriggerEnter(Collider other)
        {
            
            inTriggerColliders.Add(other);
            if (inTriggerColliders.Count == 1)
            {
                TriggerEntered();
            }

        }
    
        private void OnTriggerExit(Collider other)
        {
            inTriggerColliders.Remove(other);
            if (inTriggerColliders.Count == 0)
            {
                TriggerExited();
            }
        }

        private void TriggerExited()
        {
            IsTriggered = false;
            OnTriggerUpdated?.Invoke();
            SetVisual(false, 0.25f);
        }

        private void TriggerEntered()
        {
            IsTriggered = true;
            OnTriggerUpdated?.Invoke();
            SetVisual(true, 0.25f);
        }

        void SetVisual(bool isTriggered,float duration)
        {
            if (isTriggered)
            {
                triggerVisual.DOLocalMoveY(triggeredHeight, duration);
            }
            else
            {
                triggerVisual.DOLocalMoveY(normalHeight, duration);
            }
        }
        
    }
}