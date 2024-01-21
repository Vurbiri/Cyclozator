using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SmoothStartEndMusic : SmoothStartEnd<SmoothStartEndMusic>
{
    [Space]
    [SerializeField] private float _maxVolume = 0.8f;
    [SerializeField] private float _minVolume = 0f;

    AudioSource _thisAudioSource;

    protected override void Awake()
    {
         _thisAudioSource = GetComponent<AudioSource>();
        if (_startRun == ChangingType.Disappearances)
            _thisAudioSource.volume = _maxVolume;
        else if (_startRun == ChangingType.Appearances)
            _thisAudioSource.volume = _minVolume;

        _thisAudioSource.Play();

        base.Awake();
    }


    protected override IEnumerator Changing(ChangingType type, float speed)
    {
        if (type == ChangingType.None) yield break;

        float start = _minVolume;
        float end = _maxVolume;

        Func<bool> isWork = () => _thisAudioSource.volume < end;

        if (type == ChangingType.Disappearances)
        {
            start = _maxVolume;
            end = _minVolume;
            speed = -speed;
            isWork = () => _thisAudioSource.volume > _minVolume;
        }

        _thisAudioSource.volume = start;
        while (isWork())
        {
            yield return null;
            _thisAudioSource.volume += speed * Time.unscaledDeltaTime;
        }
    }
}
