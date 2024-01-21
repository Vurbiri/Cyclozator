using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextLocalization : MonoBehaviour
{
    public Text Text {get; protected set;}
    private string _keyString;    

    private void Awake() => Text = GetComponent<Text>();

    public void Setup(string keyString)
    {
        _keyString = keyString;
        SetText();
        Localization.Inst.EventSwitchLanguage += SetText;
    }
    private void OnDestroy()
    {
        if(Localization.Inst != null)
            Localization.Inst.EventSwitchLanguage -= SetText;
    }

    protected void SetText() => Text.text = Localization.Inst.GetText(_keyString);

    
}
