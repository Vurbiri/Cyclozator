using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class Dealer : MonoBehaviour
{
	[SerializeField] private StartMenu _startMenu;

	public Item TargetItem { get; set; }

	private ScoreTotal Score => Inventory.Inst.TotalScore;

#if YSDK
	private Timer _timer;
	private TimeSpan CooldownAd => YMoney.Inst.CooldownAdReward;
	public event Action<string, bool> EventAdvertisement;

	private void Awake()
	{
		TimerSetup();
		Inventory.Inst.EventLoad += TimerReSetup;
	}

	private void OnDestroy()
	{
		Inventory.Inst.EventLoad -= TimerReSetup;

		_timer?.Break();
		_timer = null;
	}
#endif

    public bool IsPossibleBuy()
	{
		if (TargetItem == null) return false;

		return TargetItem.Currency switch
		{
			Currency.Coins => TargetItem.Price <= Score.PossibleMaxCoins(),
			Currency.Points => TargetItem.Price <= Score.Points,
#if YSDK
			Currency.Ad => _timer != null && !_timer.IsPlay,
			Currency.Yan => YMoney.Inst.IsPayments,
#endif

			_ => false
		};
	}

	public async UniTask Buy()
	{
		if (TargetItem == null) return;

		Currency currency = TargetItem.Currency;
		bool result = false;

		if (currency == Currency.Coins)
		{
			if (TargetItem.IsBuff)
			{
				if (result = Score.TrySpendCoins(TargetItem.Price))
				{
					TargetItem.Count++;
					Message.Banner("+1: " + TargetItem.Description, time: 3000);
				}
			}
			else if (TargetItem.SubType == ItemSubType.Cell)
			{
				if(result = await BuyCell())
					Message.Banner(Localization.Inst.GetText("GoodsCell"), time: 3000);
			}
		}
		else if (currency == Currency.Points)
		{
			int coins = (int)Math.Round(TargetItem.Value);
			if (result = Score.TryPointsToCoins(coins))
				Message.AddCoins(coins);
		}
		
		if (result)
		{
			Inventory.Inst.Save(true, (b) => Message.Saving("GoodSaveInventory", b));
			return;
		}

#if YSDK
		if (currency == Currency.Ad)
			await BuyAdvertisement();
		else if (currency == Currency.Yan)
			await BuyYan();
#endif
	}

	private async UniTask<bool> BuyCell()
	{
		if (BuffStorage.Inst.IsMaxCellBuff) return false;

		if (Score.TrySpendCoins(TargetItem.Price))
			return await BuffStorage.Inst.MaxBuffOut();

		return false;
	}

#if YSDK
	private async UniTask<bool> BuyAdvertisement()
	{
		if (_timer.IsPlay) return false;

		bool result = false;
		if (await YMoney.Inst.ShowRewardedVideo())
		{
			int coins = (int)Math.Round(TargetItem.Value);
			Score.AddCoins(coins);
			result = await Inventory.Inst.SaveAsync();
		}

		if (await YMoney.Inst.AwaitCloseRewardedVideo() && result)
		{
			Message.Banner(Localization.Inst.GetText("GoodsCoinsAd"));
			Message.Saving("GoodSaveInventory", result);
			YMoney.Inst.EndTimeAdReward = _timer.Start(CooldownAd);
			return true;
		}

		return false;
	}
	private async UniTask BuyYan()
	{
		if (!YandexSDK.Inst.IsLogOn)
		{
			if (!await _startMenu.Authorization())
			{
				Message.Banner(Localization.Inst.GetText("ErrorLogon"), MessageType.Error, 2000);
				return;
			}
		}

		await YMoney.Inst.Buy();
	}


	private void TimerSetup()
	{
		if (_timer == null)
		{
			_timer = new();
			_timer.EventUpdate += (t) => EventAdvertisement?.Invoke(t.ToString(@"%m\:ss"), false);
			_timer.EventStop += () => EventAdvertisement?.Invoke("1", true);
		}

		_timer.Setup(YMoney.Inst.EndTimeAdReward, 0.5f);
	}

   private void TimerReSetup()
	{
		_timer?.Stop();
		YMoney.Inst.EndTimeAdReward = DateTime.Now;
	}
#endif
}
