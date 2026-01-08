using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputHandler : MonoBehaviour, Control.IPlayerActions
{
    public event Action ClickEvent;
    public event Action PositionEvent;

    private Control control;

    private bool isClick;
    private bool released;
    private Vector2 position;
    private bool wasClickedThisFrame = false;

    private void Awake()
    {
        control = new Control();
        control.Player.SetCallbacks(this);
        control.Player.Enable();
    }

    private void OnDestroy()
    {
        control.Player.Disable();
    }

    public bool isClicked()
    {
        return isClick;
    }

    public bool releaseButton()
    {
        bool result = released;
        released = false;
        return result;
    }

    public Vector2 getPosition()
    {
        return position;
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isClick = true;
            ClickEvent?.Invoke();
            wasClickedThisFrame = true;
            //Debug.Log("Click started");
        }
        else if (context.canceled)
        {
            released = true;
            isClick = false;
            //Debug.Log("Click canceled");
        }
    }

    public void OnPosition(InputAction.CallbackContext context)
    {
        position = context.ReadValue<Vector2>();

    }

    public bool WasClickedThisFrame()
    {
        bool result = wasClickedThisFrame;
        wasClickedThisFrame = false; 
        return result;
    }

}
