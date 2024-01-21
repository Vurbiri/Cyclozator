using UnityEngine;

public class DisableForMobile : MonoBehaviour
{
    private void Awake() => gameObject.SetActive(SettingsStorage.Inst.IsDesktop);

}
