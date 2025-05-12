using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class InputDeviceWatcher : MonoBehaviour
{
    [SerializeField] private InputSchemeEventChannel inputSchemeEventChannel;
    private InputUtils.InputScheme currentInputScheme;
    
    private void Awake()
    {
        InputSystem.onAnyButtonPress.Call(OnAnyInput);
        currentInputScheme = InputUtils.GetCurrentScheme();
    }

    private void OnAnyInput(InputControl control)
    {
        var newScheme = InputUtils.GetCurrentScheme();
        if (newScheme == currentInputScheme) return;

        currentInputScheme = newScheme;
        inputSchemeEventChannel.RaiseEvent(currentInputScheme);
    }
}