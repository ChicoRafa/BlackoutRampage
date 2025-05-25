using UnityEngine;
using UnityEngine.InputSystem;

public static class InputUtils
{
    public enum InputScheme
    {
        KeyboardMouse,
        Xbox,
        PlayStation,
        Unknown
    }

    public static InputScheme GetCurrentScheme()
    {
        var last = InputSystem.GetDevice<Keyboard>()?.lastUpdateTime ?? 0;
        var mouseLast = InputSystem.GetDevice<Mouse>()?.lastUpdateTime ?? 0;
        var gamepad = Gamepad.current;

        if (gamepad == null || !(gamepad.lastUpdateTime > Mathf.Max((float)last, (float)mouseLast)))
            return InputScheme.KeyboardMouse;
        // Identify gamepad
        var displayName = gamepad.displayName.ToLower();
        if (displayName.Contains("xbox")) return InputScheme.Xbox;
        if (displayName.Contains("dualshock") || displayName.Contains("dualSense") || displayName.Contains("playstation")) return InputScheme.PlayStation;

        return InputScheme.Xbox; // fallback
    }

    public static Sprite GetIcon(InputScheme scheme, PromptIconSet iconSet)
    {
        return scheme switch
        {
            InputScheme.KeyboardMouse => iconSet.keyboardIcon,
            InputScheme.Xbox => iconSet.xboxIcon,
            InputScheme.PlayStation => iconSet.psIcon,
            _ => iconSet.defaultIcon
        };
    }
}
