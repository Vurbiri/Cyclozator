using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class InputEvents : MonoBehaviour
{
    [Space]
    [SerializeField] private Button _help;

    public event Action EventHelp;
    public event Action EventCancel;
    public event Action EventOk;


    protected virtual void Awake() => _help.onClick.AddListener(() => EventHelp?.Invoke());

    protected virtual void Update() => ButtonMonitoring();

    protected virtual void ButtonMonitoring()
    {
        if (Input.GetButtonDown("Help")) EventHelp?.Invoke();
        if (Input.GetButtonDown("Cancel")) EventCancel?.Invoke();
        if (Input.GetButtonDown("Ok")) EventOk?.Invoke();
    }
}
