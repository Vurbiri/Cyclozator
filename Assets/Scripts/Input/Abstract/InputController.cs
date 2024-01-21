using UnityEngine;

[RequireComponent(typeof(InputEvents))]
public abstract class InputController<T> : MonoBehaviour where T : InputEvents
{
    [SerializeField] protected GameObject _helpWindows;

    protected T _inputEvent;

    protected virtual void Awake()
    {
        _inputEvent = GetComponent<T>();

        _inputEvent.EventHelp += () => _helpWindows.SetActive(!_helpWindows.activeSelf);
    }
}
