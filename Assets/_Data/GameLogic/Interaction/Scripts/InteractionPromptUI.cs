using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField] private InputSchemeEventChannel inputSchemeEventChannel;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI actionText;
    private Transform followTarget;
    private InputUtils.InputScheme currentInputScheme;
    private PromptIconSet iconSet;
    private Vector3 customOffset = Vector3.up * 4f;


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
    
    public void SetOffset(Vector3 offset) {
        customOffset = offset;
    }
    
    private void OnEnable()
    {
        inputSchemeEventChannel.OnInputSchemeChanged += OnInputSchemeChanged;
    }
    
    private void OnDisable()
    {
        inputSchemeEventChannel.OnInputSchemeChanged -= OnInputSchemeChanged;
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
            transform.position = followTarget.position + customOffset;
        }
    }
}
