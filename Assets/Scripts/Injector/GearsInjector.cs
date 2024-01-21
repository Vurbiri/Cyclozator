using System;
using UnityEngine;

public class GearsInjector : Gears
{
    [SerializeField] private DirectRotation _direction = DirectRotation.Left;

    protected event Action EventBrakingAll;


    public void Initialize(int accelerationTime)
    {
        Initialize();

        foreach (GearVarRotation gear in _gearsRotation)
        {
            gear.Initialize(accelerationTime);
            EventBrakingAll += gear.Braking;
        }


        foreach (GearsVarSound audio in _gearsAudio)
        {
            audio.Initialize(accelerationTime);
            EventBrakingAll += audio.Braking;
        }
    }
    
    public void BrakingAllGears() => EventBrakingAll?.Invoke();
    public void StartAllGears() => StartAllGears(_direction);
}
