using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class ChangeLightBridge : MonoBehaviour
{
    [SerializeField] private Color _colorOpen = Color.cyan;
    [SerializeField] private Color _colorClose = Color.red;

    [SerializeField] private float _speedChange = 0.25f;

    private Light _light;


    void Awake()
    {
        _light = GetComponent<Light>();
        _light.color = _colorClose;
    }

    public void LightClose() => StartCoroutine(ChangingLight(_colorOpen, _colorClose));

    public void LightOpen() => StartCoroutine(ChangingLight(_colorClose, _colorOpen));

    private IEnumerator ChangingLight(Color start, Color end)
    {
        float step = 0f;
        while (step <= 1f)
        {
            _light.color = Color.Lerp(start, end, step);
            step += _speedChange * Time.deltaTime;
            yield return null;
        }
    }
}

