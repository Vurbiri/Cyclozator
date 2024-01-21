using System.Collections;
using UnityEngine;

public class GearsVarSound : SoundControl
{
    [Header("Громкость")]
    [SerializeField] private float _minVolume = 0.1f;
    [SerializeField] private float _maxVolume = 0.85f;

    [Header("Частота")]
    [SerializeField] private float _minPitch = 0.5f;
    [SerializeField] private float _maxPitch = 1.4f;

    private float _accelerationTime;

    private float _stepVolume;
    private float _stepPitch;

    private bool _isBraking;

    public void Initialize(int accelerationTime)
    {
        _accelerationTime = accelerationTime;
        _stepVolume = (_maxVolume - _minVolume) / _accelerationTime;
        _stepPitch = (_maxPitch - _minPitch) / _accelerationTime;
     
    }


    public override void Play()
    {
        base.Play();

        _isBraking = false;
        _thisAudioSource.volume = _minVolume;
        _thisAudioSource.pitch = _minPitch;
        _thisAudioSource.Play();
        StartCoroutine(SoundSetting());
    }

    private IEnumerator SoundSetting()
    {
        WaitWhile pause = new(() => !_isBraking);

        yield return StartCoroutine(SoundUp());

        yield return pause;

        yield return null;

        yield return StartCoroutine(SoundDown());

    }

    private IEnumerator SoundUp()
    {
        bool isQuietly = true;
        bool isSlowly = true;

        while (isQuietly || isSlowly)
        {

            if (isQuietly)
            {
                GetVolume(Time.deltaTime);
                isQuietly = _thisAudioSource.volume < _maxVolume;
            }

            if (isSlowly)
            {
                GetPitch(Time.deltaTime);
                isSlowly = _thisAudioSource.pitch < _maxPitch;
            }

            yield return null;
        }
    }

    private IEnumerator SoundDown()
    {
        bool isLoudly = true;
        bool isQuickly = true;

        while (isLoudly || isQuickly)
        {

            if (isLoudly)
            {
                GetVolume(-Time.deltaTime);
                isLoudly = _thisAudioSource.volume > _minVolume;
            }

            if (isQuickly)
            {
                GetPitch(-Time.deltaTime);
                isQuickly = _thisAudioSource.pitch > _minPitch;
            }

            yield return null;
        }
    }

    private void GetVolume(float delta) => _thisAudioSource.volume += _stepVolume * delta;
    private void GetPitch(float delta) => _thisAudioSource.pitch += _stepPitch * delta;

    public void Braking() => _isBraking = true;


}
