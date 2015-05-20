using UnityEngine;
using System.Collections;

public class fadeParticles : MonoBehaviour
{
    public AnimationCurve particleFade;
    public Color color;
    public float minSize;
    public float maxSize;

    public void Update()
    {
        Particle[] particles = particleEmitter.particles;
        for (var i = 0; i < particles.Length; i++)
        {
            // Translation add
            color.a = particleFade.Evaluate(1 - (particles[i].energy / particles[i].startEnergy));
            particles[i].color = color;
            //particles[i].color.a = particleFade.Evaluate(1 - (particles[i].energy / particles[i].startEnergy));
            float energyVariation = particleEmitter.maxEnergy - particleEmitter.minEnergy;
            float particleEnergyVariation = particles[i].startEnergy - particleEmitter.minEnergy;
            float makeSize = particleEnergyVariation / energyVariation;
            particles[i].size = Mathf.Lerp(minSize, maxSize, makeSize);
        }
        particleEmitter.particles = particles;

    }
}
