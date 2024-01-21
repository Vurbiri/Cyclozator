using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HelpButton : MonoBehaviour
{
    private void Start()
    {
        Button helpButton = GetComponent<Button>();

        if (YMoney.Inst.IsFirstStart && !SettingsStorage.Inst.IsDesktop) 
            helpButton.onClick?.Invoke();
    }
}
