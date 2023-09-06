using System;
using System.Collections.Generic;
using System.Timers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling;

public class MagnetismManager : MonoBehaviour
{
    public static List<Magnet> SceneMagnets = new List<Magnet>();
    public static List<Metal> SceneMetals = new List<Metal>();
    [SerializeField] private MagnetVFX vfxPrefab; 
    private static float permeability = 1;// Ortam�n Ge�irgenli�i
    private Dictionary<string, MagnetVFX> vfxDictionary = new Dictionary<string, MagnetVFX>(); 

    private void OnEnable()
    {
        MagnetGameActionSystem.LevelStarted += (x) =>WaitAndSetVFXs();
        MagnetGameActionSystem.LevelUnloadedStarted += DestroyVFXs;
        MagnetGameActionSystem.OnMetalCollected += OnMetalCollected;
    }
    
    private void OnDisable()
    {
        MagnetGameActionSystem.LevelStarted -= (x) =>WaitAndSetVFXs();
        MagnetGameActionSystem.LevelUnloadedStarted -= DestroyVFXs;
        MagnetGameActionSystem.OnMetalCollected -= OnMetalCollected;
    }

    private void DestroyVFXs()
    {
        foreach (string key in vfxDictionary.Keys)
        {
            DestroyVfx(key).Forget();
        }
    }
    
    string GetKey(GameObject go1, GameObject go2) => $"{go1.GetInstanceID()%1000}{go2.GetInstanceID()%1000}";

    private async void WaitAndSetVFXs()
    {
        await UniTask.Delay(1000);
        vfxDictionary = new Dictionary<string, MagnetVFX>();
        MagnetVFX vfx;
        foreach (Magnet magnet1 in SceneMagnets)
        {
            foreach (Magnet magnet2 in SceneMagnets)
            {
                if (magnet1 != magnet2)
                {
                    vfx=Instantiate(vfxPrefab);
                    vfx.SetTargets(magnet1.transform,magnet2.transform);
                    vfxDictionary.Add(GetKey(magnet1.gameObject,magnet2.gameObject),vfx);
                }
            }   
            
            foreach (Metal metal in SceneMetals)
            {
                vfx=Instantiate(vfxPrefab);
                vfx.SetTargets(magnet1.transform, metal.transform);
                vfxDictionary.Add(GetKey(magnet1.gameObject,metal.gameObject),vfx);
            }   
        }
    }


    
    private void OnMetalCollected(Metal metal)
    {
        foreach (var magnet in SceneMagnets)
        {
            string key = GetKey(magnet.gameObject, metal.gameObject);
            if (vfxDictionary.ContainsKey(key))
            {
                DestroyVfx(key).Forget();
            }
            else
            {
                Debug.Log($"{magnet} and {metal} vfx not found");
            }
        }
    }

    private async UniTaskVoid DestroyVfx(string key)
    {
        if (vfxDictionary.ContainsKey(key))
        {
            vfxDictionary[key].SetActive(false);
            await UniTask.Yield(PlayerLoopTiming.LastFixedUpdate);
            Destroy(vfxDictionary[key].gameObject);
            vfxDictionary.Remove(key); 
        }
    }

    private void SetVFXActive(string key, bool isActive)
    {
        if (vfxDictionary.ContainsKey(key))
        {
            vfxDictionary[key].SetActive(isActive);
        }
    }
    private void FixedUpdate()
    {
        Profiler.BeginSample("Magnetism Logic");
        for (int i = 0; i < SceneMagnets.Count; i++)
        {
            for (int j = i+1; j < SceneMagnets.Count; j++)
            {
                ApplyMagneticForceToMagnet(SceneMagnets[i], SceneMagnets[j]);
            }

            foreach (Metal metal in SceneMetals)
            {
                ApplyMagneticForceToMetal(SceneMagnets[i], metal);
            }
        }
        Profiler.EndSample();
    }
    
    void ApplyMagneticForceToMagnet(Magnet magnet1, Magnet magnet2)
    {
        if (magnet1 == magnet2)
            return;
        var polarzationMultiplier = 1;
        if (magnet1.PolarizationValue == magnet2.PolarizationValue)
            polarzationMultiplier = -1;
        var heading = magnet1.CurrentPosition - magnet2.CurrentPosition;
        var distance = heading.magnitude;
        var direction = heading / distance;
        var forceToApply= permeability* magnet1.MagneticCharge*magnet2.MagneticCharge/(4*Mathf.PI*Mathf.Pow(distance,2))*Time.fixedDeltaTime*100;;
        var directedForce = direction * (polarzationMultiplier * forceToApply);
        string key = GetKey(magnet1.gameObject, magnet2.gameObject);

        if (distance < magnet1.MaxDistance )
        {
            magnet2.ApplyMagneticForce(directedForce);
            SetVFXActive(key, true);
        }
        else
        {
            SetVFXActive(key, false);
        }
        
        key = GetKey(magnet2.gameObject, magnet1.gameObject);

        if (distance < magnet2.MaxDistance )
        {
            magnet1.ApplyMagneticForce(-directedForce);
            SetVFXActive(key, true);
        }
        else
        {
            SetVFXActive(key, false);
        }
    }

    void ApplyMagneticForceToMetal(Magnet magnet, Metal metal)
    {
        string key = GetKey(magnet.gameObject, metal.gameObject);
        if (!metal.UseMagnetism)
        {
            SetVFXActive(key, false);
            return;
        }
        var heading = magnet.CurrentPosition - metal.CurrentPosition;//(15,0,0)  10,10,10   5,5,5
        var distance = heading.magnitude;//15
        if (distance > magnet.MaxDistance)
        {
            metal.IsMagnetized = false;
            SetVFXActive(key, false);
            return;
        }
        var direction = heading / distance;
        var forceToApply = permeability * magnet.MagneticCharge * metal.MagneticCharge / (4 * Mathf.PI * Mathf.Pow(distance, 2))*Time.fixedDeltaTime*100;;
        metal.ApplyMagneticForce(direction* forceToApply);
        SetVFXActive(key, true);
    }

    
    
}

