using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "InputSchemeEventChannel", menuName = "Input/InputSchemeEventChannel")]
public class InputSchemeEventChannel : ScriptableObject
{
    public UnityAction<InputUtils.InputScheme> OnInputSchemeChanged;

    public void RaiseEvent(InputUtils.InputScheme scheme)
    {
        OnInputSchemeChanged?.Invoke(scheme);
    }
}