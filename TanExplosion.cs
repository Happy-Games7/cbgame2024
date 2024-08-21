using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TanExplosion : MonoBehaviour
{
    public ParticleSystem tanExplosion_Particle;

    // Start is called before the first frame update
    void Start()
    {
        tanExplosion_Particle.Play();
        Invoke("ParticleStopAndDisable", 0.8f);
    }

    private void OnEnable()
    {
        tanExplosion_Particle.Play();
        Invoke("ParticleStopAndDisable", 0.8f);
    }

    void ParticleStopAndDisable()
    {
        gameObject.SetActive(false);
    }
}
