using UnityEngine;

public class ActiveFromQuality : MonoBehaviour
{
    [SerializeField] int _qualityOff = 0;

    void Start()
    {
        OnOff(QualitySettings.GetQualityLevel());
        QualitySettings.activeQualityLevelChanged += OnQualityLevelChanged;
    }

    private void OnQualityLevelChanged(int previousQuality, int currentQuality) => OnOff(currentQuality);

    void OnDestroy() => QualitySettings.activeQualityLevelChanged -= OnQualityLevelChanged;

    private void OnOff(int quality)
    {
        if (quality <= _qualityOff)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }
    
}
