using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Shading : SmoothStartEnd<Shading>
{
    
    [Space]
    [SerializeField] private Color _colorOn = new(0, 0, 0, 1);
    [SerializeField] private Color _colorOff = new(0, 0, 0, 0);
    
    [SerializeField] private float _pauseForOff = 0.3f;

    private Image _image;
    protected WaitForSecondsRealtime _pause;

    protected override void Awake()
    {
        _image = GetComponent<Image>();
        _pause = new(_pauseForOff);

        if (_startRun == ChangingType.Disappearances)
            _image.color = _colorOn;
        else if(_startRun == ChangingType.Appearances)
            _image.color = _colorOff;

        base.Awake();
    }

    protected override IEnumerator Changing(ChangingType type, float speed)
    {
        if (type == ChangingType.None) yield break;

        _image.raycastTarget = true;

        Color start = _colorOff;
        Color end = _colorOn;

        if (type == ChangingType.Disappearances)
        {
            start = _colorOn;
            end = _colorOff;
            yield return Pause();
        }        

        float step = 0f;
        _image.color = start;
        
        while (step <= 1f)
        {
            yield return null;
            step += speed * Time.unscaledDeltaTime;
            _image.color = Color.Lerp(start, end, step);
        }

        if (type == ChangingType.Appearances)
            yield return Pause();

    }

    IEnumerator Pause()
    {
        yield return _pause;
        _image.raycastTarget = false;
    }
}
