using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Metal : MonoBehaviour, ICollectable,IRespawnable
{
    public MetalType Type = MetalType.Black;

    [Range(0, 1)]
    public float MagneticCharge = 1;//Manyetik güç
    public bool UseMagnetism;
    public float Permeability = 1;//Geçirgenlik

    public Rigidbody MetalRB;
    public bool IsMagnetized;
    public Vector3 InitialSpawnPosition { get; private set; }

    public Action OnCollected { get; }

    public Vector3 CurrentPosition { get { return transform.position; } }

    private void Awake()
    {
        UseMagnetism = true;
        MetalRB = GetComponent<Rigidbody>();
        MetalRB.useGravity = true;
        MetalRB.constraints = RigidbodyConstraints.None;
        InitialSpawnPosition = transform.position;
    }

    private void OnEnable()
    {
        if (MagnetismManager.Instance&&!MagnetismManager.Instance.SceneMetals.Contains(this))
        {
            MagnetismManager.Instance.SceneMetals.Add(this);
        }
    }
    
    private void OnDisable()
    {
        if (MagnetismManager.Instance&&MagnetismManager.Instance.SceneMetals.Contains(this))
        {
            MagnetismManager.Instance.SceneMetals.Remove(this);
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            //AudioManager.PlayMetalHitClip();
            SoundManager.Instance.PlaySfx("MetalHitClip");
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            UseMagnetism = false;
            MetalRB.linearVelocity = Vector3.zero;
            collision.rigidbody.linearVelocity = Vector3.zero;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        UseMagnetism = true;
    }

    public void Collect()
    {
        GetComponent<MeshRenderer>().enabled = false;
        MetalRB.isKinematic = true;
        OnCollected?.Invoke();
        UseMagnetism= false;
        MagnetGameActionSystem.OnMetalCollected?.Invoke(this);
    }

    public void ApplyMagneticForce(Vector3 forceToApply)
    {
        MetalRB.AddForce(forceToApply);
        IsMagnetized = true;
    }

    public void Respawn()
    {
        transform.position = InitialSpawnPosition;
        MetalRB.linearVelocity = Vector3.zero;
    }
}

public enum MetalType
{
    Black,
    Red
}

