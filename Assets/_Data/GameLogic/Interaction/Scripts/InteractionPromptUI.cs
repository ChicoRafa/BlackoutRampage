using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI actionText;
    private Transform followTarget;
    private InputUtils.InputScheme currentInputScheme;
    private PromptIconSet iconSet;

    public void SetPrompt(Sprite iconSprite, string text, PromptIconSet iconSetRef)
    {
        icon.sprite = iconSprite;
        actionText.text = text;
        iconSet = iconSetRef;
        currentInputScheme = InputUtils.GetCurrentScheme();
    }

    public void SetTarget(Transform target)
    {
        followTarget = target;
    }
    
    private void OnEnable()
    {
        InputDeviceWatcher.OnInputSchemeChanged += OnInputSchemeChanged;
    }
    
    private void OnDisable()
    {
        InputDeviceWatcher.OnInputSchemeChanged -= OnInputSchemeChanged;
    }
    
    private void OnInputSchemeChanged(InputUtils.InputScheme newScheme)
    {
        if (newScheme == currentInputScheme || iconSet == null) return;
        
        currentInputScheme = newScheme;
        icon.sprite = InputUtils.GetIcon(currentInputScheme, iconSet);
    }

    private void LateUpdate()
    {
        if (followTarget)
        {
            transform.position = followTarget.position + Vector3.up * 4f;
        }
    }
}
