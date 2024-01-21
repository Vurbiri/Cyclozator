using UnityEngine;
using UnityEngine.UI;

public class LanguageSwatch : ToggleGroup
{
    [SerializeField] private LanguageItem langPrefab;
    
    protected override void Awake()
    {
        this.allowSwitchOff = false;

        foreach (var item in Localization.Inst.Languages)
            Instantiate(langPrefab, transform).Setup(item, this);
    }
}
