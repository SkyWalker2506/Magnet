using DG.Tweening;
using UnityEngine;

namespace DoorMechanic
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private Transform doorVisual;
        [SerializeField] private bool openOnEnterTrigger=true;
        [SerializeField] private float openHeight = 1;
        [SerializeField] private float closeHeight = 0;


        public void OnTrigger(bool isEntered,float moveDuration)
        {
            if (isEntered)
            {
                if (openOnEnterTrigger)
                {
                    doorVisual.DOLocalMoveY(openHeight, moveDuration);
                }
                else
                {
                    doorVisual.DOLocalMoveY(closeHeight, moveDuration);
                }
            }
            else
            {
                if (openOnEnterTrigger)
                {
                    doorVisual.DOLocalMoveY(closeHeight, moveDuration);
                }
                else
                {
                    doorVisual.DOLocalMoveY(openHeight, moveDuration);
                }
            }
        }
    }
}