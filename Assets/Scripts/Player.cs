using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player CurrentPlayer;
    [SerializeField]
    ParticleSystem magnetParticle;
    int maxParticle;
    void Awake()
    {
        CurrentPlayer = this;
        maxParticle = magnetParticle.main.maxParticles;
    }

    public void SetParticleSize(float size)
    {
        var shape = magnetParticle.shape;
        shape.radius = size+1;
        var main = magnetParticle.main;
        if(size>0)
            main.maxParticles= maxParticle;
        else
            main.maxParticles = 0;

    }


}