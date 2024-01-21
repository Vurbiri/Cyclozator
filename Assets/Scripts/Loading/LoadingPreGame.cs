using Cysharp.Threading.Tasks;

public class LoadingPreGame : LoadingScreen
{
    private YandexSDK YSDK => YandexSDK.Inst;
    private Localization Loc => Localization.Inst;
    private YMoney YM => YMoney.Inst;

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

        if (await YSDK.InitYsdk())
            await Initialize();
        else
            Message.Log("YandexSDK - initialization error!");

#if !UNITY_EDITOR
        if(!Storage.StoragesCreate())
            Message.Banner(Loc.GetText("ErrorStorage"), MessageType.Error, 7000);
#else
        if (YSDK.IsLogOn)
            Storage.Create<JsonToFile>();
        else
            Storage.Create<JsonToPlayerPrefs>();
#endif
        YM.IsFirstStart = !await Storage.Initialize();

        //if (YM.IsPayments)
        //    await YM.CheckPurchases();

        Message.Log("End LoadingPreGame");
        EndLoadScene();

        //============== local func ===============================
        bool LoadFromResources() => Loc.LoadFromResources() && Inventory.Inst.LoadFromResources();
        
        async UniTask Initialize()
        {
            if (!await YSDK.InitPlayer())
                Message.Log("Player - initialization error!");

            if (!await YSDK.InitLeaderboards())
                Message.Log("Leaderboards - initialization error!");

            //if (!await YM.Initialize())
            //    Message.Log("Payments - initialization error!");
        }
    }

    private void OnDisable() => YSDK.LoadingAPI_Ready();

}
