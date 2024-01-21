using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SectorSound : OffPauseSound
{
    [Header("Звуки")]
    [SerializeField] private AudioClip _audioMove;
    [SerializeField] private AudioClip _audioSnap;
    [SerializeField] private AudioClip _audioKnock;

    [Header("Громкость")]
    [SerializeField] private float _volume = 0.9f;
    [SerializeField] private float _volumeScaleEnd = 1.5f;

    public float VolumeScale { get; set; }

    protected override void Awake()
    {
        base.Awake();
        _thisAudioSource.loop = true;
    }

    public void GetVolume(float volumeScale) => _thisAudioSource.volume = _volume * volumeScale;

    public void MovePlay()
    {
        _thisAudioSource.Stop();
        _thisAudioSource.clip = _audioMove;
        _thisAudioSource.Play();
    }

    public void SnapPlay() => PlayOneShot(_audioSnap);
    public void KnockPlay() => PlayOneShot(_audioKnock);

    private void PlayOneShot(AudioClip audioClip)
    {
        _thisAudioSource.Stop();
        _thisAudioSource.PlayOneShot(audioClip, _volumeScaleEnd);
    }


}
