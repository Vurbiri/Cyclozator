using UnityEngine;

public class HelpWindowGames : MenuNavigation
{
    [SerializeField] protected PauseMenu _menu;

    private bool _pause = false;


    protected override void OnEnable()
    {
        base.OnEnable();
        _pause = Game.Inst.IsPause;
        if(_pause) 
            _menu.NonShow = true;
        else
            Game.Inst.Pause(true);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _menu.NonShow = false;
        if (_pause)
            _menu.gameObject.SetActive(true);
        else
            Game.Inst.Pause(false);

    }
}
