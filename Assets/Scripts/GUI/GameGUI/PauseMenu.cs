using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private SettingsMenu _settingsMenu;

    public bool NonShow
    {
        get => _nonShow; set
        {
            _nonShow = value;
            if (_nonShow)
                gameObject.SetActive(false);
        }
    }
    private bool _nonShow = false;

    private void Start()
    {
        Game.Inst.EventGameOver += () => NonShow = true;
        gameObject.SetActive(false);
    }

    public void PauseContinue()
    {
        if (NonShow) return;

        Game.Inst.Pause(!Game.Inst.IsPause);
        if (_settingsMenu.gameObject.activeSelf)
        {
            _settingsMenu.OnCancel();
            _settingsMenu.gameObject.SetActive(false);
            
        }
        gameObject.SetActive(Game.Inst.IsPause);
        _mainMenu.SetActive(Game.Inst.IsPause);

    }
}
