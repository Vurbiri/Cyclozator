using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Slider))]
public class Sliders : MonoBehaviour
{
    protected Slider _thisSlider;
    protected virtual void Awake() => _thisSlider = GetComponent<Slider>();
}
