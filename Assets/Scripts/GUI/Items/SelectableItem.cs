using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableItem : MonoBehaviour, ISelectHandler
{
    public event Action EventSelect;
    
    public virtual void OnSelect(BaseEventData eventData) =>  EventSelect?.Invoke();
    
}
