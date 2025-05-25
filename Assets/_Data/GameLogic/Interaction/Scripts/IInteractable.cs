using UnityEngine;

public interface IInteractable
{
    void Interact(GameObject interactor);
    bool CanInteract(GameObject interactor);
    string GetInteractionPrompt();
    
    void ShowInteractPrompt(GameObject interactor);
    void HideInteractPrompt();
}
