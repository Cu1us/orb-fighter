using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbVFX : MonoBehaviour
{
    public ParticleSystem[] particleSystems;

    public void SetEmitting(bool emitting)
    {
        foreach (ParticleSystem particleSystem in particleSystems)
        {
            var emissionModule = particleSystem.emission;
            emissionModule.enabled = emitting;
        }
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
