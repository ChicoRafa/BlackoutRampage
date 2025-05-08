using UnityEngine;

public class InteractionZone : MonoBehaviour
{
    private IInteractable interactableObject;
    private GameObject interactor;
    
    [SerializeField] private GameObject interactionPromptPrefab;
    [SerializeField] private PromptIconSet iconSet;
    private GameObject currentPromptInstance;
    private InteractionPromptUI currentPromptUI;

    private void Awake()
    {
        interactor = transform.parent.gameObject;
    }

    internal void TryInteract()
    {
        interactableObject?.Interact(interactor);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out IInteractable interactable)) return;
        interactableObject = interactable;

        if (interactionPromptPrefab == null) return;
        
        currentPromptInstance = Instantiate(interactionPromptPrefab);
        currentPromptUI = currentPromptInstance.GetComponent<InteractionPromptUI>();
        
        var scheme = InputUtils.GetCurrentScheme();
        Sprite icon = InputUtils.GetIcon(scheme, iconSet);
        string interactionPrompt = interactable.GetInteractionPrompt();

        currentPromptUI.SetPrompt(icon, interactionPrompt, iconSet);
        currentPromptUI.SetTarget(other.transform);
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out IInteractable interactable) || interactable != interactableObject) return;
        interactableObject = null;

        if (currentPromptInstance == null) return;
        Destroy(currentPromptInstance);
        currentPromptInstance = null;
    }
}