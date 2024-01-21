using UnityEngine;

public class SliderGraphics : Sliders
{
    void Start()
    {
        _thisSlider.minValue = 0f;
        _thisSlider.maxValue = QualitySettings.count - 1;
        _thisSlider.onValueChanged.AddListener((v) => QualitySettings.SetQualityLevel((int)v, true));
    }

    private void OnEnable() => _thisSlider.value = QualitySettings.GetQualityLevel();
}
