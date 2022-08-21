using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : Singleton<ParticleManager>
{
    [SerializeField]
    GameObject confettiPrefab;

    void OnEnable()
    {
        MagnetGameActionSystem.OnLevelCompleted += OnLevelEnded;
    }

    void OnDisable()
    {
        MagnetGameActionSystem.OnLevelCompleted -= OnLevelEnded;
    }

    void OnLevelEnded()
    {
        GameObject co1 = Instantiate(confettiPrefab);
        GameObject co2 = Instantiate(confettiPrefab);
        co1.transform.position = new Vector3(25, 0, -4);
        co2.transform.position = new Vector3(25, 0, 16);
        co1.transform.rotation = Quaternion.Euler(new Vector3(0, 315, 0));
        co2.transform.rotation = Quaternion.Euler(new Vector3(0, 225, 0));
        Destroy(co1, LevelManager.LevelPassTime);
        Destroy(co2, LevelManager.LevelPassTime);
    }
}
