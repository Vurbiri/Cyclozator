using UnityEngine;
using UnityEngine.UI;

public class BuffIcon : MonoBehaviour
{
    [SerializeField] protected Image _icon;

    public Sprite Icon { get => _icon.sprite; set => _icon.sprite = value; }
}
