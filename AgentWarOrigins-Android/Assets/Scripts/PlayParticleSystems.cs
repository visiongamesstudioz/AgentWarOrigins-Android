using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticleSystems : MonoBehaviour
{
    public float StartTime;
    private ParticleSystem particleSystem;


    private void OnEnable()
    {
        particleSystem = GetComponent<ParticleSystem>();
        Invoke("PlayParticles", StartTime);
    }

    void PlayParticles()
    {

        particleSystem.Play();
    }
}
