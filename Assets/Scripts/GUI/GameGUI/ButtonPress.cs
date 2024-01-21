using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Coroutine _coroutine;

    public event Action EventPress;

    public void OnPointerDown(PointerEventData eventData) => _coroutine = StartCoroutine(Press());

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_coroutine == null) return;

        StopCoroutine(_coroutine );
        _coroutine = null;
    }

    private IEnumerator Press()
    {
        while (true) 
        {
            EventPress?.Invoke();

            yield return null;
        }
    }
}
