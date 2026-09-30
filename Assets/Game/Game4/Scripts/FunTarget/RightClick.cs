using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class RightClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Action OnRightClicked;
    public Action OnHold;
    private float pointerDownTime;
    private bool pointerDown;
    private Coroutine holdRoutine;


    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClicked?.Invoke();
            return;
        }
        pointerDownTime = Time.time;
        pointerDown = true;
        holdRoutine = StartCoroutine(IncrementVal());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerDown = false;
        pointerDownTime = 0;
        if (holdRoutine != null) { 
            StopCoroutine(holdRoutine);
        }
    }

    IEnumerator IncrementVal()
    {
        while (pointerDown)
        {
            OnHold?.Invoke();
            yield return new WaitForSeconds(0.15f);
        }
    }
}
