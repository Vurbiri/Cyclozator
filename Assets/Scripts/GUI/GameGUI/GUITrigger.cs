using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class GUITrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private float _timeToUp = 10f;
    private float _tick = 0.15f;

    private SettingsStorage Sett => SettingsStorage.Inst;

    public event Action EventEnter;
    public event Action EventExit;

    public bool IsEnter { get; private set; } = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsEnter || !Sett.IsDesktop) return;

        IsEnter = true;
        EventEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsEnter || !Sett.IsDesktop) return;

        IsEnter = false;
        EventExit?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsEnter || Sett.IsDesktop) return;

        IsEnter = true;
        EventEnter?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!IsEnter || Sett.IsDesktop) return;

        IsEnter = false;
        PointerUp().Forget();
    }

    private async UniTaskVoid PointerUp()
    {
        float timer = _timeToUp;
        int tick = (int)(_tick * 1000);
        while (timer > 0)
        {
            await UniTask.Delay(tick);
            timer -= _tick;
            if (IsEnter) return;
        }

        EventExit?.Invoke();
    }

    private void OnDisable() => IsEnter = true;
    private void OnDestroy() => EventExit = null;
}
