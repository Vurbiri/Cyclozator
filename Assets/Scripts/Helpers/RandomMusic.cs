using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomMusic : MonoBehaviour
{
    [Header("Музыкальные треки")]
    [SerializeField] private AudioClip[] _tracks;

    private AudioSource _thisAudioSource;
    
    private int _oldIndexTrack = 0;

    private AudioClip RandomTrack
    {
        get
        {
            int length = _tracks.Length;
            int trackIndex = _oldIndexTrack + Random.Range(1, length);
            trackIndex = trackIndex >= length ? trackIndex - length : trackIndex;
            _oldIndexTrack = trackIndex;
            return _tracks[trackIndex];
        }
    }


    private void Start()
    {
        Random.InitState(Helper.Seed);
        _thisAudioSource = GetComponent<AudioSource>();
        StartCoroutine(PlayTack());
    }

    private IEnumerator PlayTack()
    {
        WaitWhile pause = new(() => _thisAudioSource.isPlaying);
        while (true)
        {
            _thisAudioSource.clip = RandomTrack;
            _thisAudioSource.Play();
            yield return pause;
        }
    }

    
}
