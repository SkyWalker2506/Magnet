using UnityEngine;

public class Respawner : MonoBehaviour
{
   private void OnTriggerEnter(Collider other)
   {
      if(other.TryGetComponent(out IRespawnable respawnable))
      {
         respawnable.Respawn();
      }
   }
}
