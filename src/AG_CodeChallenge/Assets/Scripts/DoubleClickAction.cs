using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoubleClickAction : MonoBehaviour, IPointerDownHandler
{
    const float doubleClickTime = 0.5f;
    private GameObject clickedObj;
    private Transform thisTransform;
    private float lastClickTime;

    private void Start()
    {
        clickedObj = this.gameObject;
        thisTransform = this.transform;
        lastClickTime = Time.unscaledTime - doubleClickTime;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Get top parent GameObject of clicked collider
        GameObject clickedParent = eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.gameObject;

        // If user didn't click this object, exit
        if (clickedParent != clickedObj) return;

        // If double-click
        if (Time.unscaledTime < (lastClickTime + doubleClickTime))
        {
            Debug.Log("DoubleClicked Object!");

            // Reset timer so can't triple-click
            lastClickTime = Time.unscaledTime - doubleClickTime;
        }
        // Record time as first click
        else lastClickTime = Time.unscaledTime;
    }

    void OnMouseDown()
    {
        print("Clicked Me");
    }

}
