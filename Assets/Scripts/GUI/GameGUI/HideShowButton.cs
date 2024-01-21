using UnityEngine;
using UnityEngine.UI;

public class HideShowButton : MonoBehaviour
{
    [SerializeField] Text _textCaption;
    [Space]
    [SerializeField] string _captionDefault = "-";
    [SerializeField] string _captionShow = "^";
    [SerializeField] string _captionHide = "*";

    private void Start()
    {
        GetCaption();
        SettingsStorage.Inst.EventModeShowChange += GetCaption;
    }

    private void GetCaption() =>
        _textCaption.text = SettingsStorage.Inst.ModeShow switch
        {
            ModeShowGUI.AlwaysShow => _captionShow,
            ModeShowGUI.AlwaysHide => _captionHide,
            ModeShowGUI.Default => _captionDefault,
            _ => ""
        };

    private void OnDestroy()
    {
        if (SettingsStorage.Inst != null)
            SettingsStorage.Inst.EventModeShowChange -= GetCaption;
    }
}
