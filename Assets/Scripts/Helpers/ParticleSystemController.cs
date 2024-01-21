using System;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemController : MonoBehaviour
{
    public Action EventParticleSystemStop;

    private ParticleSystem _thisParticleSystem;

    private void Awake() => _thisParticleSystem = GetComponent<ParticleSystem>();
    
    private void OnParticleSystemStopped() => EventParticleSystemStop?.Invoke();

    public void Play() => _thisParticleSystem.Play();
}
