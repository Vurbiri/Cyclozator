using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuNavigation : MenuFirstSelect
{
    [Space]
    [SerializeField] private InputEvents _inputEvents;
    [Header("Настройка кнопок"), Space]
    [SerializeField] private Buttons[] _buttons;

    protected virtual void Awake()
    {
        foreach (var button in _buttons)
            button.Setup(gameObject);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        foreach (var button in _buttons)
            button.Subscribe(_inputEvents);
    }

    protected virtual void OnDisable()
    {
        foreach (var button in _buttons)
            button.Unsubscribe(_inputEvents);
    }

    [System.Serializable]
    private class Buttons
    {
        [SerializeField] private bool _isCancel;
        [SerializeField] private bool _isOk;
        [Space]
        [SerializeField] private Button _button;
        [Space]
        [SerializeField] private bool _closeCurrentMenu = true;
        [SerializeField] private GameObject _openMenu;
        [SerializeField] private UnityEvent _otherActions;

        public void Setup(GameObject closeMenu)
        {
            if (!_button) return;

            if (_otherActions != null)
                _button.onClick.AddListener(_otherActions.Invoke);

            _button.onClick.AddListener(() =>
            {
                if (closeMenu) closeMenu.SetActive(!_closeCurrentMenu);
                if (_openMenu) _openMenu.SetActive(true);
            });
   
        }

        public void Subscribe(InputEvents inputEvents)
        {
            if (_isOk)
                inputEvents.EventOk += ButtonOnClick;

            if (_isCancel)
                inputEvents.EventCancel += ButtonOnClick;
        }

        public void Unsubscribe(InputEvents inputEvents)
        {
            if (_isOk)
                inputEvents.EventOk -= ButtonOnClick;

            if (_isCancel)
                inputEvents.EventCancel -= ButtonOnClick;
        }

        private void ButtonOnClick() => _button.onClick?.Invoke();

    }
}
