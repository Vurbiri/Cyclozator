using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MenuNavigation
{
    [Space]
    [SerializeField] private Text _captionText;
    [SerializeField] private RawImage _avatarTexture;
    [Space]
    [SerializeField] private GameObject _startGameMenu;
    [SerializeField] private GameObject _buffMenu;
    [Space]
    [SerializeField] private GameObject _authorizedButtonsPanel;
    [SerializeField] private GameObject _notAuthorizedButtonsPanel;

    private YandexSDK YSDK => YandexSDK.Inst;
    private YMoney YM => YMoney.Inst;

    public void Start()
    {
        SettingsStorage.Inst.Personalization(_captionText, _avatarTexture).Forget();
        Localization.Inst.EventSwitchLanguage += SetLocalizationName;

        _authorizedButtonsPanel.SetActive(false);
        _notAuthorizedButtonsPanel.SetActive(false);

        if (YSDK.IsLogOn)
            _authorizedButtonsPanel.SetActive(true);
        else if (YSDK.IsInitialize)
            _notAuthorizedButtonsPanel.SetActive(true);
           
    }
    
    public void OnPreStartGame()
    {
        GameModeStart mode = GameStorage.Inst.ModeStart;

        if (mode == GameModeStart.New)
            _buffMenu.SetActive(true);
        else if (mode == GameModeStart.Continue || mode == GameModeStart.ContinueSimple)
            _startGameMenu.SetActive(true);
    }

    public void OnClickAuthorization()
    {
        OnClickAuthorizationAsync().Forget();

        async UniTaskVoid OnClickAuthorizationAsync()
        {
            if (!await Authorization())
                Message.Banner(Localization.Inst.GetText("ErrorLogon"), MessageType.Error, 9750);
            else
                FirstSelect();
        }
    }

    public async UniTask<bool> Authorization()
    {
        if (!YSDK.IsPlayer)
            if (!await YSDK.InitPlayer())
                return false;

        if (!await YSDK.LogOn())
            return false;

        Banners.Inst.Clear();

        if (!YSDK.IsLeaderboard)
            await YSDK.InitLeaderboards();

        //if (!YM.IsPayments)
        //    await YM.Initialize();

        _authorizedButtonsPanel.SetActive(true);
        _notAuthorizedButtonsPanel.SetActive(false);

#if !UNITY_EDITOR
        if(!Storage.StoragesCreate())
            Message.Banner(Localization.Inst.GetText("ErrorStorage"), MessageType.Error, 7000);
#else
        Storage.Create<JsonToFile>();
#endif
        YM.IsFirstStart = !await Storage.Initialize();
        SettingsStorage.Inst.Personalization(_captionText, _avatarTexture).Forget();
        
        //if (YM.IsPayments)
        //    await YM.CheckPurchases();

        return true;
    }

    private void SetLocalizationName() => SettingsStorage.Inst.Personalization(_captionText);

    private void OnDestroy() => Localization.Inst.EventSwitchLanguage -= SetLocalizationName;
}
