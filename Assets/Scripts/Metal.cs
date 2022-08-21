using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Metal : MonoBehaviour
{
    public MetalType Type = MetalType.Black;

    [Range(0, 1)]
    public float MagneticCharge = 1;//Manyetik güç
    public bool UseMagnetism;
    public float Permeability = 1;//Geçirgenlik

    public static List<Metal> SceneMetals = new List<Metal>();

    public Rigidbody MetalRB;
    public Vector3 CurrentPosition { get { return transform.position; } }



    private void Awake()
    {
        UseMagnetism = true;
        MetalRB = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        if (!SceneMetals.Contains(this))
            SceneMetals.Add(this);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            AudioManager.PlayMetalHitClip();
        }
    }


    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            UseMagnetism = false;
            MetalRB.velocity = Vector3.zero;
            collision.rigidbody.velocity = Vector3.zero;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        UseMagnetism = true;

    }
    private void OnDisable()
    {
        if (SceneMetals.Contains(this))
            SceneMetals.Remove(this);

    }


}
public enum MetalType
{
    Black,
    Red
}

