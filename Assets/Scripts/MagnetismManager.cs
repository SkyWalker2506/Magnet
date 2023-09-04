using System.Collections.Generic;
using UnityEngine;

public class MagnetismManager : MonoBehaviour
{
    public static float Permeability = 1;// Ortam�n Ge�irgenli�i
    public static List<Magnet> SceneMagnets=new List<Magnet>();
    public static List<Metal> SceneMetals = new List<Metal>();

    private void FixedUpdate()
    {
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
        var forceToApply= Permeability* magnet1.MagneticCharge*magnet2.MagneticCharge/(4*Mathf.PI*Mathf.Pow(distance,2));
        var directedForce = direction * (polarzationMultiplier * forceToApply);

        if (distance < magnet1.MaxDistance )
        {
            magnet2.ApplyMagneticForce(directedForce);
        }

        if (distance < magnet2.MaxDistance )
        {
            magnet1.ApplyMagneticForce(-directedForce);
        }

    }

    void ApplyMagneticForceToMetal(Magnet magnet, Metal metal)
    {
        if (!metal.UseMagnetism)
            return;
        var heading = magnet.CurrentPosition - metal.CurrentPosition;//(15,0,0)  10,10,10   5,5,5
        var distance = heading.magnitude;//15
        if (distance > magnet.MaxDistance)
        {
            metal.IsMagnetized = false;
            return;
        }
        var direction = heading / distance;
        var forceToApply = Permeability * magnet.MagneticCharge * metal.MagneticCharge / (4 * Mathf.PI * Mathf.Pow(distance, 2));
        metal.ApplyMagneticForce(direction* forceToApply);
    }

    
    
}
