using UnityEngine;
using UnityEngine.Serialization;


[RequireComponent(typeof(Rigidbody))]
public class Magnet : MonoBehaviour
{
    public Polarization PolarizationValue;
    [Range(10,2500)]
    public float MagneticCharge=1;//Manyetik g��
  
    
    public Rigidbody MagnetRB;
    [FormerlySerializedAs("maxDistance")] [SerializeField]
    public float MaxDistance=10;
 
    public Vector3 CurrentPosition => transform.position;


    private void Awake()
    {
        MagnetRB = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        if (!MagnetismManager.Instance.SceneMagnets.Contains(this))
        {
            MagnetismManager.Instance.AddMagnet(this);
        }
    }

    private void OnDisable()
    {
        if (MagnetismManager.Instance&&MagnetismManager.Instance.SceneMagnets.Contains(this))
        {
            MagnetismManager.Instance.RemoveMagnet(this);
        }
    }
    
    public void ApplyMagneticForce(Vector3 forceToApply)
    {
        MagnetRB.AddForce(forceToApply);
    }

}

public enum Polarization
{
    Negative,
    Positive
}

