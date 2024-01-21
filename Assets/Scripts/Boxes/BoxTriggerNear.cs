using System;
using UnityEngine;

public class BoxTriggerNear : MonoBehaviour
{
    public event Action<Figure> EventTriggerEnter;
    public event Action<Figure> EventTriggerExit;

    private void OnTriggerEnter(Collider other) => EventTriggerEnter?.Invoke(other.GetComponentInParent<Figure>());
    private void OnTriggerExit(Collider other) => EventTriggerExit?.Invoke(other.GetComponentInParent<Figure>());
}
