using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Input/Input Reader")]
public class InputReader : ScriptableObject
{
    [Header("Input Actions Asset")]
    [SerializeField] private InputActionAsset inputActionAsset;
    
    private InputAction moveInputAction;
    private InputAction interactInputAction;
    private InputAction sprintInputAction;
    
    public UnityAction<Vector2> MoveEvent;
    public UnityAction MoveCanceledEvent;
    public UnityAction InteractEvent;
    public UnityAction<bool> SprintEvent;
    
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
    }

    private void OnDisable()
    {
        UnbindInputAction("Player/Move", OnMove, OnMove);
        UnbindInputAction("Player/Interact", OnInteract, OnInteract);
        UnbindInputAction("Player/Sprint", OnSprint, OnSprint);
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

}
