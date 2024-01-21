using UnityEngine;

public class SliderSound : Sliders
{
    [SerializeField] private MixerGroup _audioMixerGroup;

    private void Start()
    {
        SettingsStorage profileSettings = SettingsStorage.Inst;

        _thisSlider.minValue = profileSettings.MinValue;
        _thisSlider.maxValue = profileSettings.MaxValue;
        _thisSlider.onValueChanged.AddListener((v) => profileSettings.SetVolume(_audioMixerGroup, v));
    }

    private void OnEnable() => _thisSlider.value = SettingsStorage.Inst.GetVolume(_audioMixerGroup);
}
