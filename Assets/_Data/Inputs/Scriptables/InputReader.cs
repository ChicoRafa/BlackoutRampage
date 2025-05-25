using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Input/Input Reader")]
public class InputReader : ScriptableObject
{
    [Header("Input Actions Asset")]
    [SerializeField] private InputActionAsset inputActionAsset;

    [Header("Events")]
    public UnityAction<Vector2> MoveEvent;
    public UnityAction MoveCanceledEvent;
    public UnityAction InteractEvent;
    public UnityAction<bool> SprintEvent;
    public UnityAction DropEvent;
    public UnityAction<int> SelectItemEvent;
    public UnityAction PauseEvent;
    
    private int currentSelectedIndex = 0;

    private void BindInputAction(string actionName, Action<InputAction.CallbackContext> performed, Action<InputAction.CallbackContext> canceled)
    {
        var action = inputActionAsset.FindAction(actionName);
        action.Enable();
        if (performed != null) action.performed += performed;
        if (canceled != null) action.canceled += canceled;
    }

    private void UnbindInputAction(string actionName, Action<InputAction.CallbackContext> performed, Action<InputAction.CallbackContext> canceled)
    {
        var action = inputActionAsset.FindAction(actionName);
        if (performed != null) action.performed -= performed;
        if (canceled != null) action.canceled -= canceled;
        action.Disable();
    }

    private void OnEnable()
    {
        BindInputAction("Player/Move", OnMove, OnMove);
        BindInputAction("Player/Interact", OnInteract, OnInteract);
        BindInputAction("Player/Sprint", OnSprint, OnSprint);
        BindInputAction("Player/Drop", OnDrop, OnDrop);
        BindInputAction("Player/SelectItem", OnSelectItem, OnSelectItem);
        BindInputAction("Player/Pause", OnPause, OnPause);
    }

    private void OnDisable()
    {
        UnbindInputAction("Player/Move", OnMove, OnMove);
        UnbindInputAction("Player/Interact", OnInteract, OnInteract);
        UnbindInputAction("Player/Sprint", OnSprint, OnSprint);
        UnbindInputAction("Player/Drop", OnDrop, OnDrop);
        UnbindInputAction("Player/SelectItem", OnSelectItem, OnSelectItem);
        UnbindInputAction("Player/Pause", OnPause, OnPause);
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (!context.canceled)
            MoveEvent?.Invoke(context.ReadValue<Vector2>());
        else
            MoveCanceledEvent?.Invoke();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
            InteractEvent?.Invoke();
    }

    private void OnSprint(InputAction.CallbackContext context)
    {
        SprintEvent?.Invoke(context.performed);
    }

    private void OnDrop(InputAction.CallbackContext context)
    {
        if (context.performed)
            DropEvent?.Invoke();
    }
    
    private void OnSelectItem(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // Keyboard number keys
        for (int i = 0; i < 9; i++)
        {
            var key = (Key)((int)Key.Digit1 + i);
            if (!Keyboard.current[key].wasPressedThisFrame) continue;
            
            currentSelectedIndex = i;
            SelectItemEvent?.Invoke(currentSelectedIndex);
            return;
        }
        
        float scrollValue = context.ReadValue<float>();

        // Mouse scroll and gamepad input
        if (!(Mathf.Abs(scrollValue) > 0.01f)) return;
        
        int direction = scrollValue > 0 ? 1 : -1; // Up is previous, down is next
        currentSelectedIndex = (currentSelectedIndex + direction + 9) % 9;
        SelectItemEvent?.Invoke(currentSelectedIndex);
    }
    
    private void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
            PauseEvent?.Invoke();
    }
}
