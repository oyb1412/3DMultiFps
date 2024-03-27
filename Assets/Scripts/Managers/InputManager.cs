using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    public Action OnKeyboardEvent;
    public Action OnKeyboardUpEvent;
    public Action<Define.MouseEventType> OnMouseEvent;
    public Action<Define.MouseEventType> OnMouseUpEvent;
    
    public void OnUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;



        if (Input.anyKey)
        {
            OnKeyboardEvent?.Invoke();
        }
        else {
            OnKeyboardUpEvent?.Invoke();
        }

        if(Input.GetMouseButtonUp(0)) {
            OnMouseUpEvent?.Invoke(Define.MouseEventType.ButtonUpLeft);
        }  
        if(Input.GetMouseButtonUp(1)) {
            OnMouseUpEvent?.Invoke(Define.MouseEventType.ButtonUpRight);
        }

        if (Input.GetMouseButtonDown(0))
        {
            OnMouseEvent?.Invoke(Define.MouseEventType.ButtonDownLeft);
            OnMouseEvent?.Invoke(Define.MouseEventType.ButtonLeft);
        }

        if (Input.GetMouseButtonDown(1))
        {
            OnMouseEvent?.Invoke(Define.MouseEventType.ButtonDownRight);
            OnMouseEvent?.Invoke(Define.MouseEventType.ButtonRight);
        }

        if (Input.GetMouseButton(0)) {
            OnMouseEvent?.Invoke(Define.MouseEventType.ButtonLeft);
        }
        
        if (Input.GetMouseButton(1)) {
            OnMouseEvent?.Invoke(Define.MouseEventType.ButtonRight);
        }



    }

}
