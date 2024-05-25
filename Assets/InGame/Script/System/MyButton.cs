using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using UniRx;

public class MyButton : Selectable, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
{
    public IObservable<MyButton> OnClickDown => _onClickDown.AsObservable().Where(x => IsInteractable()).Select(_ => this);
    public IObservable<MyButton> OnClickUp => _onClickUp.AsObservable().Where(x => IsInteractable()).Select(_ => this);

    private UnityEvent _onClickDown = new();
    private UnityEvent _onClickUp = new();


    public void OnPointerClick(PointerEventData eventData)
    {
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        _onClickDown?.Invoke();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        _onClickUp?.Invoke();
    }

}
