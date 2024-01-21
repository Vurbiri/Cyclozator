using System.Collections;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class GameSFX : MonoBehaviour
{
    [SerializeField] private AudioClip _audioLevelUP;
    [SerializeField] private AudioClip _audioGameOver;

    private AudioSource _audioSource;

    private void Awake() =>_audioSource = GetComponent<AudioSource>();

    public IEnumerator PlayLevelUP()
    {
        _audioSource.PlayOneShot(_audioLevelUP);
        yield return new WaitWhile(() => _audioSource.isPlaying);
    }
    public void PlayGameOver() => _audioSource.PlayOneShot(_audioGameOver);


}
