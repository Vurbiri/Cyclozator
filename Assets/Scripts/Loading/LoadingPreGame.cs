using Cysharp.Threading.Tasks;

public class LoadingPreGame : LoadingScreen
{
#if YSDK
    [UnityEngine.SerializeField] private YMoney.Settings _settingsYMoney;
#endif

    private Localization Loc => Localization.Inst;

    private void Start() => Loading().Forget();

    private async UniTaskVoid Loading()
    {
        StartLoadScene();

        Message.Log("Start LoadingPreGame");

        if (!LoadFromResources())
        {
            Message.Banner("Error loading game resources!", MessageType.FatalError);
            return;
        }

#if YSDK
        if (await YandexSDK.Inst.InitYsdk())
            await Initialize();
        else
            Message.Log("YandexSDK - initialization error!");
#endif

        if (!Storage.StoragesCreate())
            Message.Banner(Loc.GetText("ErrorStorage"), MessageType.Error, 7000);

        Message.Log(Storage.TypeStorage?.Name);

#if YSDK
        YMoney.Inst.IsFirstStart = SettingsStorage.Inst.IsFirstStart =!await Storage.Initialize();
#else
        SettingsStorage.Inst.IsFirstStart = !await Storage.Initialize();
#endif

        Message.Log("End LoadingPreGame");
        EndLoadScene();

        //============== local func ===============================
        bool LoadFromResources() => Loc.LoadFromResources() && Inventory.Inst.LoadFromResources();

#if YSDK
        async UniTask Initialize()
        {
            if (!await YandexSDK.Inst.InitPlayer())
                Message.Log("Player - initialization error!");

            if (!await YandexSDK.Inst.InitLeaderboards())
                Message.Log("Leaderboards - initialization error!");

            YMoney.Create(_settingsYMoney);
        }
#endif
    }

#if YSDK
    private void OnDisable() => YandexSDK.Inst.LoadingAPI_Ready();
#endif

}
