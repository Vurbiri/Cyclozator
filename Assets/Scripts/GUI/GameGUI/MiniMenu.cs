using UnityEngine;

[RequireComponent(typeof(GUIAnimation))]
public class MiniMenu : MonoBehaviour
{
    [SerializeField] private float _motionTime = 2f;
    private GUIAnimation _thisGUIAnimator;
    private bool _isShow = false;

    private void Start()
    {
        _thisGUIAnimator = GetComponent<GUIAnimation>();
        _thisGUIAnimator.SetMotionTime(_motionTime);
        OpenCloseMenu();
        SettingsStorage.Inst.EventModeShowChange += OpenCloseMenu;
    }

    private void OpenCloseMenu()
    {
        if (SettingsStorage.Inst.ModeShow == ModeShowGUI.AlwaysHide && !_isShow)
        {
            _thisGUIAnimator.Play();
            _isShow = true;
        }
        else if (_isShow)
        {
            _thisGUIAnimator.PlayRevers();
            _isShow = false;
        }
    }

    private void OnDestroy()
    {
        if (SettingsStorage.Inst != null)
            SettingsStorage.Inst.EventModeShowChange -= OpenCloseMenu;
    }
}
