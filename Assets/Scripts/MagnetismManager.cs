using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MagnetismManager : Singleton<MagnetismManager>
{
    public List<Magnet> SceneMagnets { get; private set; } = new List<Magnet>();
    public List<Metal> SceneMetals = new List<Metal>();
    [SerializeField] private MagnetVFX vfxPrefab; 
    private float permeability = 1;
    private Dictionary<string, MagnetVFX> vfxDictionary = new Dictionary<string, MagnetVFX>(); 

    private void OnEnable()
    {
        MagnetGameActionSystem.LevelUnloadedStarted += DestroyVFXs;
        MagnetGameActionSystem.OnMetalCollected += OnMetalCollected;
    }
    
    private void OnDisable()
    {
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

    private void OnMetalCollected(Metal metal)
    {
        foreach (Magnet magnet in SceneMagnets)
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
            if (vfxDictionary.ContainsKey(key))
            {
                if (vfxDictionary[key].gameObject)
                {
                    Destroy(vfxDictionary[key].gameObject);
                }
                vfxDictionary.Remove(key); 
            }
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
        ApplyMagneticForce();
        SetVFXPositions();
    }

    private void ApplyMagneticForce()
    {
        for (int i = 0; i < SceneMagnets.Count; i++)
        {
            for (int j = i + 1; j < SceneMagnets.Count; j++)
            {
                ApplyMagneticForceToMagnet(SceneMagnets[i], SceneMagnets[j]);
            }

            foreach (Metal metal in SceneMetals)
            {
                ApplyMagneticForceToMetal(SceneMagnets[i], metal);
            }
        }
    }

    private void SetVFXPositions()
    {
        foreach (MagnetVFX magnetVFX in vfxDictionary.Values)
        {
            SetVFXNewPositions(magnetVFX);
        }
    }
    
    private void SetVFXNewPositions(MagnetVFX magnetVFX)
    {
        if (!magnetVFX.IsActive)
        {
            return;
        }

        var position1 = magnetVFX.Pos1.position;
        var position2 = magnetVFX.Pos4.position;
        magnetVFX.Pos2.position = Vector3.Lerp(position1, position2, .33f) + Vector3.down + Vector3.right;
        magnetVFX.Pos3.position = Vector3.Lerp(position1, position2, .66f) + Vector3.up * 5 + Vector3.left;
    }
    
    void ApplyMagneticForceToMagnet(Magnet magnet1, Magnet magnet2)
    {
        if (magnet1 == magnet2)
            return;
        int polarizationMultiplier = 1;
        if (magnet1.PolarizationValue == magnet2.PolarizationValue)
            polarizationMultiplier = -1;
        Vector3 heading = magnet1.CurrentPosition - magnet2.CurrentPosition;
        float distance = Mathf.Max(heading.magnitude,1);
        Vector3 direction = heading / distance;
        float forceToApply= permeability* magnet1.MagneticCharge*magnet2.MagneticCharge/(4*Mathf.PI*Mathf.Pow(distance,2))*Time.fixedDeltaTime*100;
        Vector3 directedForce = direction * (polarizationMultiplier * forceToApply);
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
        Vector3 heading = magnet.CurrentPosition - metal.CurrentPosition;
        float distance = Mathf.Max(heading.magnitude,1);

        if (distance > magnet.MaxDistance)
        {
            metal.IsMagnetized = false;
            SetVFXActive(key, false);
            return;
        }
        Vector3 direction = heading / distance;
        float forceToApply = permeability * magnet.MagneticCharge * metal.MagneticCharge / (4 * Mathf.PI * Mathf.Pow(distance, 2))*Time.fixedDeltaTime*100;;
        metal.ApplyMagneticForce(direction* forceToApply);
        SetVFXActive(key, true);
    }

    public void AddMagnet(Magnet addedMagnet)
    {
        SceneMagnets.Add(addedMagnet);
        MagnetVFX vfx;

        foreach (Magnet magnet in SceneMagnets)
        {
            string key = GetKey(addedMagnet.gameObject, magnet.gameObject);
            if (vfxDictionary.ContainsKey(key))
            {
                continue;
            }
            key = GetKey(magnet.gameObject, addedMagnet.gameObject);
            if (vfxDictionary.ContainsKey(key))
            {
                continue;
            }
            
            if (addedMagnet != magnet)
            {
                vfx=Instantiate(vfxPrefab);
                vfx.SetTargets(addedMagnet.transform,magnet.transform);
                vfxDictionary.Add(GetKey(addedMagnet.gameObject,magnet.gameObject),vfx);
            }
        }   
            
        foreach (Metal metal in SceneMetals)
        {
            vfx=Instantiate(vfxPrefab);
            vfx.SetTargets(addedMagnet.transform, metal.transform);
            vfxDictionary.Add(GetKey(addedMagnet.gameObject,metal.gameObject),vfx);
        }  

    }
    
    public void RemoveMagnet(Magnet removedMagnet)
    {
        SceneMagnets.Remove(removedMagnet);
        foreach (Magnet magnet in SceneMagnets)
        {
            string key = GetKey(removedMagnet.gameObject, magnet.gameObject);
            if (vfxDictionary.ContainsKey(key))
            {
                DestroyVfx(key).Forget();
            }
            else
            {
                Debug.Log($"{removedMagnet} and {magnet} vfx not found");
            }
            key = GetKey(magnet.gameObject, removedMagnet.gameObject);
            if (vfxDictionary.ContainsKey(key))
            {
                DestroyVfx(key).Forget();
            }
            else
            {
                Debug.Log($"{magnet} and {removedMagnet} vfx not found");
            }
        }
        
        foreach (var metal in SceneMetals)
        {
            string key = GetKey(removedMagnet.gameObject, metal.gameObject);
            if (vfxDictionary.ContainsKey(key))
            {
                DestroyVfx(key).Forget();
            }
            else
            {
                Debug.Log($"{removedMagnet} and {metal} vfx not found");
            }
        }
    }
}

