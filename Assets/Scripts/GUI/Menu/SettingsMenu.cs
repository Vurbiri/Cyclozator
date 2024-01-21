public class SettingsMenu : MenuNavigation
{
    public void OnSave() =>
        SettingsStorage.Inst.Save(true, (b) => Message.Saving("GoodSaveSettings", b));

    public void OnCancel() => SettingsStorage.Inst.Apply();
}
