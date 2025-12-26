#if YSDK
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
#endif
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ShortcutButton : MonoBehaviour
{
#if !YSDK
    private void Start() => Destroy(gameObject);
#else
	[SerializeField] private int _hoursShortcut = 50;
	[Range(0, 59)]
	[SerializeField] private int _minShortcut = 15;
	[Space]
	[SerializeField] private float _secPauseMsg = 8f;
	[Space]
	[SerializeField] private string _message = "ShortcutReward";
	[SerializeField] private int _coins = 8;

	private const string keyTimer = "ets";
	private DateTime _endRewardBlock = default;
	private readonly DateTime _originRewardBlock = new(2023, 11, 16);

	private Button _shortcutButton;
	private Timer _timer;
	private Coroutine _coroutine;

	private void Start()
	{
		_shortcutButton = GetComponent<Button>();
		Initialize().Forget();
	}

	private async UniTaskVoid Initialize()
	{
		
		if (!await YandexSDK.Inst.CanShortcut())
		{
			_shortcutButton.gameObject.SetActive(false);
			return;
		}

		bool isReward = false;

		var (result, minutes) = Storage.Load<int>(keyTimer);
		if (result)
			_endRewardBlock = _originRewardBlock + TimeSpan.FromMinutes(minutes);

		_timer = new();
		_timer.EventStop += () => { isReward = true; Msg(); };
		_timer.Setup(_endRewardBlock, 30f);
		isReward = !_timer.IsPlay;
		
		AddListener();
		if (isReward)
			_coroutine = StartCoroutine(MessageReward());

		Inventory.Inst.EventLoad += ReInitialize;

		IEnumerator MessageReward()
		{
			yield return new WaitForSeconds(_secPauseMsg);
			if (isReward)
				Msg();
			_coroutine = null;
		}

		void AddListener()
		{
			_shortcutButton.onClick.AddListener(UniTask.UnityAction(async () =>
			{
				if (await YandexSDK.Inst.CreateShortcut())
				{
					_shortcutButton.interactable = false;
					if (!isReward) return;

					_endRewardBlock = _timer.Start(new(_hoursShortcut, _minShortcut, 0));
					Storage.Save(keyTimer, (int)Math.Round((_endRewardBlock - _originRewardBlock).TotalMinutes), false);
					Inventory.Inst.TotalScore.AddCoins(_coins);
					Inventory.Inst.Save(true, (b) => { if (b) Message.AddCoins(_coins); });
				}
			}));
		}

		void Msg() => Message.Banner(String.Format(Localization.Inst.GetText(_message), _coins), time: 6500, isThrough: false);
	}

	private void ReInitialize()
	{
		if (_coroutine != null)
			StopCoroutine(_coroutine);
		_timer?.Break();
		_shortcutButton.onClick.RemoveAllListeners();

		Initialize().Forget();
	}

	private void OnDisable()
	{
		Inventory.Inst.EventLoad -= ReInitialize;

		_timer?.Break();
		_timer = null;
	}
#endif
}
