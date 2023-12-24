using System;
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
        private void Awake()
        {
            SetVisual(false, 0);
        }

        private void OnTriggerEnter(Collider other)
        {
            IsTriggered = true;
            OnTriggerUpdated?.Invoke();
            SetVisual(true, 0.25f);

        }
    
        private void OnTriggerExit(Collider other)
        {
            IsTriggered = false;
            OnTriggerUpdated?.Invoke();
            SetVisual(false,  0.25f);

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