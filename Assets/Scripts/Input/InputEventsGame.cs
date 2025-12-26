using System;
using UnityEngine;
using UnityEngine.UI;

public class InputEventsGame : InputEvents
{
	[SerializeField] private Button[] _switches;
	[SerializeField] private Button[] _menus;
	[Space]
	[SerializeField] private ButtonPress _leftRotation;
	[SerializeField] private ButtonPress _rightRotation;
	[Space]
	[SerializeField] private GUITrigger[] _triggers;

	public event Action EventLeft;
	public event Action EventRight;

	public event Action EventMenu;
	public event Action EventSelect;

	private bool _blocked = false;

	protected override void Awake()
	{
		base.Awake();

		foreach (var sw in _switches)
			sw.onClick.AddListener(() => EventSelect?.Invoke());

		foreach (var m in _menus)
			m.onClick.AddListener(() => EventMenu?.Invoke());

		_leftRotation.EventPress += () => EventLeft?.Invoke();
		_rightRotation.EventPress += () => EventRight?.Invoke();

		foreach (var t in _triggers)
		{
			t.EventEnter += () => _blocked = true;
			t.EventExit += () => _blocked = false;
		}

	}

	protected override void ButtonMonitoring()
	{
		base.ButtonMonitoring();
		
		if (Input.GetButtonDown("Select")) EventSelect?.Invoke();

#if YSDK
		if (!YMoney.Inst.IsShowAd && Input.GetButtonDown("Menu")) EventMenu?.Invoke();
#endif

		if (Input.GetButton("Left")) EventLeft?.Invoke();
		if (Input.GetButton("Right")) EventRight?.Invoke();

		if (!_blocked && !Game.Inst.IsPause && SettingsStorage.Inst.IsDesktop)
		{
			if (Input.GetButton("LeftM")) EventLeft?.Invoke();
			if (Input.GetButton("RightM")) EventRight?.Invoke();
		}
	}


}
