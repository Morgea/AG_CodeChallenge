using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DoubleClickActionOMD : MonoBehaviour
{
    const float doubleClickTime = 0.5f;
    private float lastClickTime;

    [System.Serializable]
    public class DblClickEvent : UnityEvent { }

    public DblClickEvent OnEvent;

    private void Start()
    {
        // Prevent edge case of user being able to immediately single-click to trigger
        lastClickTime = Time.unscaledTime - doubleClickTime;
    }

    void OnMouseDown()
    {
        // If double-click
        if (Time.unscaledTime < (lastClickTime + doubleClickTime))
        {
            OnEvent.Invoke();

            // Reset timer so can't triple-click
            lastClickTime = Time.unscaledTime - doubleClickTime;
        }
        // Record time as first click
        else lastClickTime = Time.unscaledTime;
    }

}
