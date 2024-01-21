using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class OffPauseSound : MonoBehaviour
{
    protected AudioSource _thisAudioSource;

    protected virtual void Awake() =>
        _thisAudioSource = GetComponent<AudioSource>();

    protected virtual void Start() => Game.Inst.EventPause += Pause;
    protected virtual void OnDisable() => Game.Inst.EventPause -= Pause;

    public virtual void Pause(bool paused)
    {
        if (paused)
            _thisAudioSource.Pause();
        else
            _thisAudioSource.UnPause();
    }
}
