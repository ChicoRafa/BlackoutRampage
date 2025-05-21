using UnityEngine;

public abstract class InteractableBase : MonoBehaviour, IInteractable {

    [Header("Interaction Prompt")]
    [SerializeField] private GameObject interactionPromptPrefab;
    [SerializeField] private PromptIconSet iconSet;

    private GameObject promptInstance;
    private InteractionPromptUI promptUI;

    public virtual void ShowInteractPrompt(GameObject interactor) {
        if (promptInstance != null) return;

        promptInstance = Instantiate(interactionPromptPrefab);
        promptUI = promptInstance.GetComponent<InteractionPromptUI>();

        var scheme = InputUtils.GetCurrentScheme();
        Sprite icon = InputUtils.GetIcon(scheme, iconSet);
        promptUI.SetPrompt(icon, GetInteractionPrompt(), iconSet);
        promptUI.SetTarget(interactor.transform);
    }

    public virtual void HideInteractPrompt() {
        if (promptInstance != null)
            Destroy(promptInstance);
    }

    public abstract void Interact(GameObject interactor);
    public abstract bool CanInteract(GameObject interactor);
    public abstract string GetInteractionPrompt();
}