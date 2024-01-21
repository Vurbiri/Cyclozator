using UnityEngine;

[RequireComponent(typeof(InputControllerGame))]
public class InputControllerGame : InputController<InputEventsGame>
{
    [SerializeField] private GUIHideShow _guiHideShow;
    [SerializeField] private PauseMenu _menu;


    protected override void Awake()
    {
        base.Awake();

        _inputEvent.EventLeft += Game.Inst.RotationLeft;
        _inputEvent.EventRight += Game.Inst.RotationRight;

        _inputEvent.EventSelect += _guiHideShow.NextMode;
        _inputEvent.EventMenu += _menu.PauseContinue;

        //if (Input.GetKeyDown(KeyCode.G))
        //    Game.Inst.OnGameOver();
    }
}
