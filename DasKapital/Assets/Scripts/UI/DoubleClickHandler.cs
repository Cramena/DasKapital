using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoubleClickHandler : MonoBehaviour, IPointerClickHandler
{
    public float doubleClickDuration = 0.2f;
    private float doubleClickTimer;
    private bool clickedOnce;
    public System.Action onDoubleClicked;

    void Update()
    {
        if (clickedOnce)
        {
            if (doubleClickTimer > 0)
            {
                doubleClickTimer -= Time.deltaTime;
            }
            else
            {
                clickedOnce = false;
            }
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (!clickedOnce)
        {
            clickedOnce = true;
            doubleClickTimer = doubleClickDuration;
        }
        else 
        {
            clickedOnce = false;
            onDoubleClicked?.Invoke();
        }
    }
}
