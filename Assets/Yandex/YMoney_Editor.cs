#if UNITY_EDITOR

using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class YMoney
{
    public bool IsPayments => true;

    private async UniTask<bool> ShowAd(UniTaskCompletionSource<bool> taskCompletion, Action action, bool isOn = true)
    {

        IsShowAd = true;

        _audioMixer.SetFloat(_mixerGroup, _audioOffValue);

        PauseCallback();
        bool result = await taskCompletion.Task;
        Debug.Log(action.Method.Name + " - " + result);

        if (isOn)
        {
            _audioMixer.SetFloat(_mixerGroup, _audioOnValue);
            IsShowAd = false;
        }

        return result;

        async void PauseCallback()
        {
            await UniTask.Delay(500, true);
            taskCompletion.TrySetResult(true);
            await UniTask.Delay(100, true);
            taskCloseRewardedVideo?.TrySetResult(true);
        }
    }

    public UniTask<bool> Initialize()
    {
        return InitPayments();

        async UniTask<bool> InitPayments()
        {
            await UniTask.Delay(5, true);
            EventChangedPayments?.Invoke(true);
            return true;
        }
    }

    readonly List<Purchase> _purchase = new();
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
            await UniTask.Delay(5, true);
            _purchase.Add(new(pId, DevPayload.Create(), devPayload));
            return (true, _purchase.Last());
        }
    }

    private async UniTask<bool> SaveAndConsume(Purchase purchase, bool test = false)
    {
        if (Status == StatusPurchase.Validate)
        {
            if (purchase.ProductID != _productID || !DevPayload.Check(purchase.DeveloperPayload))
            {
                Message.Banner(Localization.Inst.GetText("ErrorPurchase"), MessageType.Error, 6000);
                Status = StatusPurchase.Consume;
                return await Consume(true);
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

        return await Consume(test);


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
                RemainingBlockMsg(5000);
                Message.AddCoins(Coins);
            }
            else
            {
                Message.Banner(Localization.Inst.GetText("ErrorSave"), MessageType.Error, 4000);
            }

            return isSave;
        }

        async UniTask<bool> Consume(bool test)
        {
            await UniTask.Delay(5, true);
            int index = _purchase.FindIndex((p) => p.PurchaseToken == purchase.PurchaseToken);
            if (index >= 0 && test)
            {
                _purchase.RemoveAt(index);
                Status = StatusPurchase.Ready;
                Message.Banner(Localization.Inst.GetText("CompletePurchase"));
            }
            else
            {
                Message.Banner(Localization.Inst.GetText("ErrorConsume"), MessageType.Error, 6000);
                MsgErrorBuy();
            }
            Storage.Save(_keyStatus, Status.ToInt());
            return index >= 0;
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
            Message.Banner(Localization.Inst.GetText("Incomplete"), time: 3000);
            if (Status == StatusPurchase.Ready)
                Status = StatusPurchase.Validate;

            if (!await SaveAndConsume(purchase, true))
                return false;
        }

        return true;

        async UniTask<(bool result, Purchase[] purchases)> GetPurchases()
        {
            await UniTask.Delay(5, true);
            if (Status != StatusPurchase.Ready && _purchase.Count == 0)
                _purchase.Add(new(_productID, DevPayload.Create(), DevPayload.Create()));

            return (true, _purchase.ToArray());
        }
    }
}
#endif