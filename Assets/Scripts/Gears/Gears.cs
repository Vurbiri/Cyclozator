using System;
using UnityEngine;

public class Gears : MonoBehaviour
{
    protected event Action<DirectRotation> EventStartAllDirect;
    protected event Action EventStartAll;
    protected event Action EventStopAll;

    protected GearRotation[] _gearsRotation;
    protected SoundControl[] _gearsAudio;

    private void Awake() => Initialize();

    protected virtual void Initialize()
    {
        if (_gearsRotation != null && _gearsAudio != null)
            return;
        
        _gearsRotation = GetComponentsInChildren<GearRotation>();
        _gearsAudio = GetComponentsInChildren<SoundControl>();

        foreach (GearRotation gear in _gearsRotation)
        {
            EventStartAllDirect += gear.StartRotation;
            EventStopAll += gear.StopRotation;
        }

        foreach (SoundControl audio in _gearsAudio)
        {
            EventStartAll += audio.Play;
            EventStopAll += audio.Stop;
        }
    }

    public void StartAllGears(DirectRotation direction)
    {
        EventStartAllDirect?.Invoke(direction);
        EventStartAll?.Invoke();

    }
    public void StopAllGears() => EventStopAll?.Invoke();
}
