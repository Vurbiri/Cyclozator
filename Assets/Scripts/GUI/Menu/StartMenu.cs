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


    public void Start()
    {
        SettingsStorage.Inst.Personalization(_captionText, _avatarTexture).Forget();
        Localization.Inst.EventSwitchLanguage += SetLocalizationName;

        _authorizedButtonsPanel.SetActive(false);
        _notAuthorizedButtonsPanel.SetActive(false);

#if YSDK
        if (YandexSDK.Inst.IsLogOn)
            _authorizedButtonsPanel.SetActive(true);
        else if (YandexSDK.Inst.IsInitialize)
            _notAuthorizedButtonsPanel.SetActive(true);
#endif
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
#if YSDK

        OnClickAuthorizationAsync().Forget();

        async UniTaskVoid OnClickAuthorizationAsync()
        {
            if (!await Authorization())
                Message.Banner(Localization.Inst.GetText("ErrorLogon"), MessageType.Error, 9750);
            else
                FirstSelect();
        }
#endif
    }

#if YSDK
    public async UniTask<bool> Authorization()
    {
        if (!YandexSDK.Inst.IsPlayer)
            if (!await YandexSDK.Inst.InitPlayer())
                return false;

        if (!await YandexSDK.Inst.LogOn())
            return false;

        Banners.Inst.Clear();

        if (!YandexSDK.Inst.IsLeaderboard)
            await YandexSDK.Inst.InitLeaderboards();


        _authorizedButtonsPanel.SetActive(true);
        _notAuthorizedButtonsPanel.SetActive(false);

        if (!Storage.StoragesCreate())
            Message.Banner(Localization.Inst.GetText("ErrorStorage"), MessageType.Error, 7000);

        YMoney.Inst.IsFirstStart = !await Storage.Initialize();
        SettingsStorage.Inst.Personalization(_captionText, _avatarTexture).Forget();
        
        //if (YM.IsPayments)
        //    await YM.CheckPurchases();

        return true;
    }
#endif

    private void SetLocalizationName() => SettingsStorage.Inst.Personalization(_captionText);

    private void OnDestroy() => Localization.Inst.EventSwitchLanguage -= SetLocalizationName;
}
