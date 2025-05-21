using UnityEngine;

public interface IInteractable
{
    void Interact(GameObject interactor);
    bool CanInteract(GameObject interactor);
    string GetInteractionPrompt(); //Interaction message. I.E. "Press E to open door"
    
    void ShowInteractPrompt(GameObject interactor);
    void HideInteractPrompt();
    
}
