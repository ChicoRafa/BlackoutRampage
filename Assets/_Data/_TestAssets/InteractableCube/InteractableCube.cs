using UnityEngine;

public class InteractableCube : MonoBehaviour, IInteractable
{
    public void Interact(GameObject interactor)
    {
        Debug.Log(interactor.name + " interacted with " + gameObject.name);
    }

    public string GetInteractionPrompt()
    {
        return "Interact with Cube";
    }
}
