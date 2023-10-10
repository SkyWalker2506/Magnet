using UnityEngine;

public interface IRespawnable
{
    Vector3 InitialSpawnPosition { get; }
    void Respawn();
}
