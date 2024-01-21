using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class LanguageItem : BuffIcon
{
    [SerializeField] private Text _name;
    private int _id;
    private Toggle _thisToggle;

    public void Setup(Localization.LanguageType languageType, ToggleGroup toggleGroup) 
    {
        Icon = languageType.Sprite;
        _name.text = languageType.Name;
        _id = languageType.Id;

        _thisToggle = GetComponent<Toggle>();
        _thisToggle.isOn = Localization.Inst.CurrentIdLang == _id;
        _thisToggle.group = toggleGroup;
        _thisToggle.onValueChanged.AddListener(OnSelect);
        Localization.Inst.EventSwitchLanguage += OnSwitchLanguage;
    }

    private void OnSelect(bool isOn)
    {
        if(!isOn) return;

        if(Localization.Inst.SwitchLanguage(_id))
            SettingsStorage.Inst.Save(true, (b) => Message.Saving("GoodSaveSettings", b));
    }

    private void OnSwitchLanguage()
    {
        _thisToggle.onValueChanged.RemoveListener(OnSelect);
        _thisToggle.isOn = Localization.Inst.CurrentIdLang == _id;
        _thisToggle.onValueChanged.AddListener(OnSelect);
    }

    private void OnDestroy() =>Localization.Inst.EventSwitchLanguage -= OnSwitchLanguage;
}
