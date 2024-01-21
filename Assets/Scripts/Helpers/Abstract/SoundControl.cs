using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundControl : OffPauseSound
{
    public virtual void Play()
    {
        if (_thisAudioSource.isPlaying)
            return;
    }

    public virtual void Stop() 
        => _thisAudioSource.Stop();
}
