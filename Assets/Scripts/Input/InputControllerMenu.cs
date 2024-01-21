using UnityEngine;

[RequireComponent(typeof(InputControllerMenu))]
public class InputControllerMenu : InputController<InputEventsMenu>
{
    [SerializeField] protected GameObject _menu;
    [SerializeField] protected GameObject _leaderboard;

    protected override void Awake()
    {
        base.Awake();

        _inputEvent.EventHelp += () => _menu.SetActive(!_menu.activeSelf && !_leaderboard.activeSelf);
        _inputEvent.EventHelp += () => _leaderboard.SetActive(false);
    }
}
