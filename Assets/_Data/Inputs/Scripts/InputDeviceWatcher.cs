using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class InputDeviceWatcher : MonoBehaviour
{
    public static event Action<InputUtils.InputScheme> OnInputSchemeChanged;
    private static InputUtils.InputScheme currentInputScheme;
    private static InputDeviceWatcher instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        InputSystem.onAnyButtonPress.Call(OnAnyInput);
        currentInputScheme = InputUtils.GetCurrentScheme();
    }

    private void OnAnyInput(InputControl control)
    {
        var newScheme = InputUtils.GetCurrentScheme();

        if (newScheme == currentInputScheme) return;
        currentInputScheme = newScheme;
        OnInputSchemeChanged?.Invoke(currentInputScheme);
    }

    public static InputUtils.InputScheme CurrentInputScheme() => currentInputScheme;
}