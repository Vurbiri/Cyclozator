using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HelpButton : MonoBehaviour
{
    private void Start()
    {
        var helpButton = GetComponent<Button>();
        var settings = SettingsStorage.Inst;

        if (settings.IsFirstStart && !settings.IsDesktop) 
            helpButton.onClick?.Invoke();
    }
}
