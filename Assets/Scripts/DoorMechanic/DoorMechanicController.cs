using System;
using System.Linq;
using UnityEngine;

namespace DoorMechanic
{
    public class DoorMechanicController : MonoBehaviour
    {
        [SerializeField] private DoorTrigger[] doorTriggers;
        [SerializeField] private Door[] doors;
        [SerializeField] float doorsMoveDuration = 1;
        private void Awake()
        {
            foreach (var door in doors)
            {
                door.OnTrigger(false, 0);
            }
        }

        private void OnEnable()
        {
            foreach (var doorTrigger in doorTriggers)
            {
                doorTrigger.OnTriggerUpdated += SetDoors;
            }
        }

        private void OnDisable()
        {
            foreach (var doorTrigger in doorTriggers)
            {
                doorTrigger.OnTriggerUpdated -= SetDoors;
            }
        }

        private void SetDoors()
        {
            SetDoors(doorTriggers.Any(dt=>dt.IsTriggered), doorsMoveDuration);
        }
        
        private void SetDoors(bool isTriggered, float duration = 0)
        {
            foreach (var door in doors)
            {
                door.OnTrigger(isTriggered, duration);
            }
        }
    }
}