using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class URL : MonoBehaviour
{
    [SerializeField] private string _link = "mailto:Vurbiri@yandex.ru";
    private void Awake() => GetComponent<Button>().onClick.AddListener(() => Application.OpenURL(_link));

}
