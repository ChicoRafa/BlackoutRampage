using UnityEngine;

public class InteractableCube : InteractableBase
{
    public override void Interact(GameObject interactor)
    {
        Debug.Log(interactor.name + " interacted with " + gameObject.name);
    }

    public override bool CanInteract(GameObject interactor)
    {
        return true;
    }
    public override string GetInteractionPrompt()
    {
        return "Interact with Cube";
    }
    
    
}
