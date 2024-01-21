using System;
using System.Collections;
using UnityEngine;

public class GUIHideShow : MonoBehaviour
{
    [SerializeField] private GUITrigger[] _triggers;
    [SerializeField] GUIAnimation[] _controlButtons;
    [Space]
    [SerializeField] private float _pauseBeforeStart = 14f;
    [SerializeField] private float _timeToHide = 10f;
    [SerializeField] private float _motionTimeShow = 0.5f;
    [SerializeField] private float _motionTimeHide = 2f;

    private bool _isShow = true;
    Coroutine _hidingCoroutine = null;

    private ModeShowGUI ModeShow { get => SettingsStorage.Inst.ModeShow; set => SettingsStorage.Inst.ModeShow = value; }
    private readonly static ModeShowGUI[] _modes = Enum<ModeShowGUI>.GetValues();

    private Action actionHiding;
    private Action actionShowing;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(_pauseBeforeStart);

        actionHiding += () => _isShow = false;
        actionShowing += () => _isShow = true;

        foreach (var button in _controlButtons)
        {
            actionHiding += () => button.SetMotionTime(_motionTimeHide);
            actionShowing += () => button.SetMotionTime(_motionTimeShow);
            actionHiding += button.PlayRevers;
            actionShowing += button.Play;
        }

        bool isEnter = false;
        foreach (var trigger in _triggers)
        {
            isEnter = isEnter || trigger.IsEnter;
            trigger.EventEnter += OnEnter;
            trigger.EventExit += OnExit;
        }

        if (ModeShow == ModeShowGUI.AlwaysHide)
            actionHiding();
        else if (ModeShow == ModeShowGUI.Default && isEnter == false)
            _hidingCoroutine = StartCoroutine(TimerHide());

        SettingsStorage.Inst.EventModeShowChange += OnShowChange;
    }

    public void NextMode()
    {
        int nextIndex = _modes.Right(ModeShow.ToInt());
        ModeShow = _modes[nextIndex];
    }

    private void OnShowChange()
    {
        if (ModeShow == ModeShowGUI.AlwaysShow && !_isShow)
        {
            Show();
        }
        else if (ModeShow == ModeShowGUI.AlwaysHide && _isShow)
        {
            StopTimerHide();
            actionHiding();
        }
        else if (ModeShow == ModeShowGUI.Default)
        {
            bool isEnter = false;
            foreach (var trigger in _triggers)
                isEnter = isEnter || trigger.IsEnter;

            if (isEnter && !_isShow)
                Show();
            else if(!isEnter && _isShow)
                Hide();
        }
    }

    private void OnEnter()
    {
        if (ModeShow == ModeShowGUI.Default)
            Show();
    }
    private void OnExit()
    {
        if (ModeShow == ModeShowGUI.Default)
            Hide();
    }

    private void Show()
    {
        StopTimerHide();
        actionShowing();
    }
    private void Hide()
    {
        StopTimerHide();
        _hidingCoroutine = StartCoroutine(TimerHide());
    }

    private IEnumerator TimerHide()
    {
        float timer = 0f;

        while (timer <= _timeToHide) 
        {
            yield return null;
            timer += Time.deltaTime;
        }

        actionHiding();
        _hidingCoroutine = null;
    }

    private void StopTimerHide()
    {
        if (_hidingCoroutine == null) return;
        
        StopCoroutine(_hidingCoroutine);
        _hidingCoroutine = null;
    }

    private void OnDestroy()
    {
        if(SettingsStorage.Inst != null)
            SettingsStorage.Inst.EventModeShowChange -= OnShowChange;
    }
}
