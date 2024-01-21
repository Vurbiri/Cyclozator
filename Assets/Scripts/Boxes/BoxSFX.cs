using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class BoxSFX : MonoBehaviour
{
    [SerializeField] private BoxSettings _startSettings;

    private FigureType _currentType;
    public FigureType Type => _currentType;

    [Header("Ёффекты")]
    [SerializeField] private ParticleSystem _particleOff;
    [SerializeField] private ParticleSystem _particleLightningLeft;
    [SerializeField] private ParticleSystem _particleLightningRight;
    [SerializeField] private ParticleSystem _particleScoreUP;
    [SerializeField] private ParticleSystem _particleCatch;
    private ParticleSystem.MainModule _scoreMain;
    private ParticleSystem.MainModule _catchMain;

    [Header("«вуки")]
    [SerializeField] private AudioClip _audioOff;
    [SerializeField] private AudioClip _audioLightning;
    [SerializeField] private AudioClip _audioScoreUP;
    [Space]
    [SerializeField] private AudioClip _audioCatchFar;
    [SerializeField] private AudioClip _audioCatchNear;
    private AudioSource _thisAudioSource;

    [Header("¬нутренн€€ часть бокса")]
    [SerializeField] private Renderer _rendererInner;

    private Renderer _thisRenderer;
    private List<Material> _thisMaterials;

    private void Start()
    {
        _thisRenderer = GetComponent<Renderer>();
        _thisAudioSource = GetComponent<AudioSource>();
        _thisMaterials = new List<Material>(_thisRenderer.sharedMaterials);

        _scoreMain = _particleScoreUP.main;
        _catchMain = _particleCatch.main;


        SetVisualize(_startSettings);
    }

    public void OnPauseSound(bool paused)
    {
        if (paused)
            _thisAudioSource.Pause();
        else
            _thisAudioSource.UnPause();
    }

    public IEnumerator SelfOff(BoxSettings offSetting, float volumeScale = 1f)
    {
        SetVisualize(offSetting);
        EffectPlay(_particleOff, _audioOff, volumeScale);

        yield return new WaitWhile(() => _particleOff.isPlaying);
    }

    public IEnumerator LeftRightOff(bool isLeft, bool isRight)
    {
        if (isLeft) EffectPlay(_particleLightningLeft, _audioLightning);

        if (isRight) EffectPlay(_particleLightningRight, _audioLightning);

        yield return new WaitWhile(() => _particleLightningLeft.isPlaying || _particleLightningRight.isPlaying);
    }

    public void ScoreUP() => EffectPlay(_particleScoreUP, _audioScoreUP, 1.25f);
    public void SetVisualize(BoxSettings boxSettings)
    {
        SimpleSetVisualize(boxSettings);

        _currentType = boxSettings.Type;

        Color color = boxSettings.Color;
        _scoreMain.startColor = color;
        _catchMain.startColor = color;
    }

    public void SimpleSetVisualize(BoxSettings boxSettings)
    {
        _rendererInner.sharedMaterial = boxSettings.MaterialChange;

        if (SettingsStorage.Inst.IsDesktop || QualitySettings.GetQualityLevel() > 1)
            _thisMaterials[2] = boxSettings.MaterialDisplay;
        else
            _thisMaterials[2] = boxSettings.MaterialDisplayMobile;
        _thisRenderer.SetSharedMaterials(_thisMaterials);

        _particleCatch.Clear();
    }

    public void CatchStart(ParticleSystemForceField particleSystemForceField, bool isFar)
    {
        _particleCatch.externalForces.AddInfluence(particleSystemForceField);

        if (isFar) _thisAudioSource.PlayOneShot(_audioCatchFar);
        else _thisAudioSource.PlayOneShot(_audioCatchNear, 1.35f);
    }
    public void CatchEnd(ParticleSystemForceField particleSystemForceField)
    {
        if(_currentType == FigureType.Off)
            _particleCatch.Clear();
        _particleCatch.externalForces.RemoveInfluence(particleSystemForceField);
    }

    private void EffectPlay(ParticleSystem particleSystem, AudioClip audioClip, float volumeScale = 1f)
    {
        _thisAudioSource.PlayOneShot(audioClip, volumeScale);
        particleSystem.Play();
    }
}
