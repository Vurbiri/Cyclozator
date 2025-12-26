#if YSDK

using Cysharp.Threading.Tasks;
using System;
using UnityEngine.SceneManagement;

public partial class YMoney : ILoading
{
    private static YMoney s_instance;
    public static YMoney Inst => s_instance;


    private string _productID = "100coins";
    private readonly CInt _priceYan = 44;
    private readonly CInt _dayOffAd = 3;
    private readonly CInt _coins = 100;

    private readonly AudioMixerController _audioMixer;
    private readonly Chance _chanceReward;
    private readonly int _rewardAdFull;
    private readonly int _minOffFirstAdFull;

    #region Full
    private readonly TimeSpan _timeCDAdFull = default;
    private DateTime _endTimeAdFull = default;
    public bool IsTimeAdFull => CheckDate(_endTimeAdFull);
    #endregion

    #region Rewarded
    public TimeSpan CooldownAdReward { get; private set; }
    public DateTime EndTimeAdReward { get; set; }
    #endregion

    public bool IsShowAd { get; private set; } = false;
    
    #region Purchase
    private const string _keyTime = "etb";
    private TimeSpan _deltaTimeBlock = default;
    private readonly DateTime _originTimeBlock = new(2023, 11, 16);
    private DateTime _endTimeBlock = default;
    private int EndTotalMinutes => (int)Math.Round((_endTimeBlock - _originTimeBlock).TotalMinutes);
    private const string _keyStatus = "kst";
    public StatusPurchase Status { get; private set; }

    public bool IsBlockAd => CheckDate(_endTimeBlock);
    public int PriceYan => _priceYan;
    public int DayOffAd => _dayOffAd;
    public int Coins => _coins;
    #endregion

    public bool IsFirstStart { get; set; } = true;
    public event Action<bool> EventChangedPayments;


    public static void Create(Settings settings) => s_instance ??= new(settings);

    private YMoney(Settings stt)
    {
        _audioMixer = stt.audioMixer;

        CooldownAdReward = new(0, stt.minCDAdReward, stt.secCDAdReward);
        _timeCDAdFull = new(0, stt.minCDAdFull, stt.secCDAdFull);
        _deltaTimeBlock = new(_dayOffAd, 0, 1, 0);
        _chanceReward = new(stt.chanceRewardAdFull);

        _rewardAdFull = stt.rewardAdFull;
        _minOffFirstAdFull = stt.minOffFirstAdFull;

        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnSceneUnloaded(Scene scene) => EventChangedPayments = null;

    public bool Load()
    {
        var (resultTime, minutes) = Storage.Load<int>(_keyTime);
        if (resultTime)
        {
            _endTimeBlock = _originTimeBlock + TimeSpan.FromMinutes(minutes);
            if (IsBlockAd)
                RemainingBlockMsg();
        }
        var (resultStatus, status) = Storage.Load<int>(_keyStatus);
        if (resultStatus)
            Status = Enum<StatusPurchase>.FromInt(status);
        return resultTime || resultStatus;
    }

    public void Create()
    {
        _endTimeBlock = default;
        Status = StatusPurchase.Ready;
    }

    public async UniTask ShowFullscreenAdv()
    {
        if (!YandexSDK.Inst.IsInitialize)
            return;


        if (IsFirstStart)
        {
            IsFirstStart = false;

            if (!YandexSDK.Inst.IsLogOn)
                return;
            
            TimeSpan span = TimeSpan.FromMinutes(_minOffFirstAdFull);
            if (IsBlockAd)
                _endTimeBlock += span;
            else
                _endTimeBlock = DateTime.Now + span;
        }

        if (IsBlockAd)
        {
            RemainingBlockMsg(3000);
            if (_chanceReward.Next)
                AddCoins();
            return;
        }

        if (IsTimeAdFull)
            return;

        taskEndShowFullscreenAdv?.TrySetResult(default);
        taskEndShowFullscreenAdv = new();
        if (await ShowAd(taskEndShowFullscreenAdv, ShowFullscreenAdvJS))
        {
            _endTimeAdFull = DateTime.Now + _timeCDAdFull;
            if (_chanceReward.Next)
                AddCoins("AdReward");

        }
        taskEndShowFullscreenAdv = null;

        void AddCoins(string msg = null) 
        {
            Inventory.Inst.TotalScore.AddCoins(_rewardAdFull);
            Inventory.Inst.Save(false, (b) => { if (b) Message.AddCoins(_rewardAdFull, msg, true, 3000); });
        }

    }

    public async UniTask<bool> ShowRewardedVideo()
    {
        if ((EndTimeAdReward - DateTime.Now) > TimeSpan.Zero)
            return false;

        taskRewardRewardedVideo?.TrySetResult(default);
        taskCloseRewardedVideo?.TrySetResult(default);
        taskRewardRewardedVideo = new();
        taskCloseRewardedVideo = new();

        bool result = await ShowAd(taskRewardRewardedVideo, ShowRewardedVideoJS, false);
        taskRewardRewardedVideo = null;

        return result;
    }
    public async UniTask<bool> AwaitCloseRewardedVideo()
    {
        bool result = await taskCloseRewardedVideo.Task;
        taskCloseRewardedVideo = null;

        _audioMixer.On();
        IsShowAd = false;

        return result;
    }

#if !UNITY_EDITOR
    public bool IsPayments => IsPaymentsJS();

    private async UniTask<bool> ShowAd(UniTaskCompletionSource<bool> taskCompletion, Action action, bool isOn = true)
    {
        IsShowAd = true;
        _audioMixer.Off();

        action();
        bool result = await taskCompletion.Task;

        if (isOn)
        {
            _audioMixer.On();
            IsShowAd = false;
        }
        return result;
    }

    public async UniTask<bool> Initialize()
    {
        bool result = await YandexSDK.WaitTask(ref taskEndInitPayments, InitPaymentsJS);
        taskEndInitPayments = null;
        EventChangedPayments?.Invoke(result);
        return result;
    }

    public async UniTask Buy()
    {
        if (Status == StatusPurchase.Ready)
        {
            var (result, purchase) = await Purchasing(_productID, DevPayload.Create());
            if (!result)
                return;
            Status = StatusPurchase.Validate;
            await SaveAndConsume(purchase);
        }
        else
        {
            await CheckPurchases();
        }

        async UniTask<(bool result, Purchase purchase)> Purchasing(string pId, string devPayload = "")
        {
            string json = await YandexSDK.WaitTask(ref taskEndPurchase, PurchaseJS, pId, devPayload);
            taskEndPurchase = null;
            if (string.IsNullOrEmpty(json))
                return (false, null);
            else
                return Storage.Deserialize<Purchase>(json);
        }
    }

    private async UniTask<bool> SaveAndConsume(Purchase purchase)
    {
        if (Status == StatusPurchase.Validate)
        {
            if (purchase.ProductID != _productID || !DevPayload.Check(purchase.DeveloperPayload))
            {
                Message.Banner(Localization.Inst.GetText("ErrorPurchase"), MessageType.Error, 6500);
                Status = StatusPurchase.Consume;
                return await Consume();
            }
            Status = StatusPurchase.AddAndSave;
        }

        if (Status == StatusPurchase.AddAndSave)
        {
            Add();
            if (!await Save())
            {
                Remove();
                return MsgErrorBuy();
            }
            
            Status = StatusPurchase.Consume;
        }
        
        return await Consume();

        //============== local func ===============================
        void Add()
        {
            if (IsBlockAd)
                _endTimeBlock += _deltaTimeBlock;
            else
                _endTimeBlock = DateTime.Now + _deltaTimeBlock;
            Inventory.Inst.TotalScore.AddCoins(_coins);
        }
        void Remove()
        {
            _endTimeBlock -= _deltaTimeBlock;
            Inventory.Inst.TotalScore.AddCoins(-_coins);
            Storage.Save(_keyTime, EndTotalMinutes, false);
            Inventory.Inst.Save(true);
        }
        async UniTask<bool> Save()
        {
            bool isSave = true;
            isSave = isSave && await Storage.SaveAsync(_keyTime, EndTotalMinutes, false);
            isSave = isSave && await Inventory.Inst.SaveAsync(true);

            if (isSave)
            {
                RemainingBlockMsg();
                Message.AddCoins(Coins);
            }
            else
            {
                Message.Banner(Localization.Inst.GetText("ErrorSave"), MessageType.Error, 4000);
            }

            return isSave;
        }
        async UniTask<bool> Consume()
        {
            bool result;
            if (result = await YandexSDK.WaitTask(ref taskEndConsumePurchase, ConsumePurchaseJS, purchase.PurchaseToken))
            {
                Status = StatusPurchase.Ready;
                Message.Banner(Localization.Inst.GetText("CompletePurchase"));
            }
            else
            { 
                Message.Banner(Localization.Inst.GetText("ErrorConsume"), MessageType.Error, 6000);
                MsgErrorBuy();
            }

            Storage.Save(_keyStatus, Status.ToInt());
            return result;
        }

        bool MsgErrorBuy()
        {
            Message.Banner(Localization.Inst.GetText("ErrorCompletePurchase"), MessageType.Warning, 8000);
            return false;
        }
    }

    public async UniTask<bool> CheckPurchases()
    {
        var (result, purchases) = await GetPurchases();
        if (!result)
            return false;

        foreach (var purchase in purchases)
        {
            Message.Banner(Localization.Inst.GetText("Incomplete"));
            if (Status == StatusPurchase.Ready)
                Status = StatusPurchase.Validate;

            if (!await SaveAndConsume(purchase))
                return false;
        }

        return true;
    
        async UniTask<(bool result, Purchase[] purchases)> GetPurchases()
        {
            string json = await YandexSDK.WaitTask(ref taskEndGetPurchases, GetPurchasesJS);
            taskEndGetPurchases = null;
            if (string.IsNullOrEmpty(json))
                return (false, null);
            else if(json == "Empty")
                return (true, new Purchase[0]);
            else
                return Storage.Deserialize<Purchase[]>(json);
        }
    }
#endif


    private bool CheckDate(DateTime date) => date - DateTime.Now > TimeSpan.Zero;

    private void RemainingBlockMsg(int time = 5000)
    {
        TimeSpan remain = _endTimeBlock - DateTime.Now;
        string msg = Localization.Inst.GetText("OffAds");
        if (remain.Days > 0)
        {
            msg += remain.Days.ToString() + Localization.Inst.GetText("Days");
        }
        msg += remain.ToString(@"hh\:mm\:ss");

        Message.Banner(msg, time: time);
    }
}
#endif