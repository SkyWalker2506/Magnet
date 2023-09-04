using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Magnet : MonoBehaviour
{
    public Polarization PolarizationValue;
    [Range(10,2500)]
    public float MagneticCharge=1;//Manyetik g��
  
    public static float Permeability = 1;// Ortam�n Ge�irgenli�i

    public static List<Magnet> SceneMagnets=new List<Magnet>();
    
    public Rigidbody MagnetRB;
    [SerializeField]
    float maxDistance=10;

    Player player;
 
    public Vector3 CurrentPosition { get { return transform.position; } }

   
    private void Awake()
    {
        MagnetRB = GetComponent<Rigidbody>();
        player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        if (!SceneMagnets.Contains(this))
           SceneMagnets.Add(this);
      
    }

    private void OnDisable()
    {
        if (SceneMagnets.Contains(this))
            SceneMagnets.Remove(this);
        
    }

    private void FixedUpdate()
    {
            SceneMagnets.ForEach(ApplyMagneticForceToMagnet);
            Metal.SceneMetals.ForEach(ApplyMagneticForceToMetal);
    }

    void ApplyMagneticForceToMagnet(Magnet otherMagnet)
    {
        if (otherMagnet == this)
            return;
        var polarzationMultiplier = 1;
        if (PolarizationValue == otherMagnet.PolarizationValue)
            polarzationMultiplier = -1;
        var heading = CurrentPosition - otherMagnet.CurrentPosition;
        var distance = heading.magnitude;
        if (distance > maxDistance)
            return;
        var direction = heading / distance;
        var forceToApply= Permeability* MagneticCharge*otherMagnet.MagneticCharge/(4*Mathf.PI*Mathf.Pow(distance,2));
        otherMagnet.MagnetRB.AddForce(direction * polarzationMultiplier *forceToApply);
        MagnetRB.AddForce(-direction * polarzationMultiplier * forceToApply);

    }

    void ApplyMagneticForceToMetal(Metal metal)
    {
        if (!metal.UseMagnetism)
            return;
        var heading = CurrentPosition - metal.CurrentPosition;//(15,0,0)  10,10,10   5,5,5
     //   var heading = CurrentPosition+ preHeading*5 / preHeading.magnitude - metal.CurrentPosition ;
        var distance = heading.magnitude;//15
        if (distance > maxDistance)
            return;
        var direction = heading / distance;//1,0,0
        var forceToApply = Permeability * MagneticCharge * metal.MagneticCharge / (4 * Mathf.PI * Mathf.Pow(distance, 2));
        metal.MetalRB.AddForce( direction * forceToApply );
       // MagnetRB.AddForce( -direction * forceToApply);
    }
}

public enum Polarization
{
    Negative,
    Positive
}

