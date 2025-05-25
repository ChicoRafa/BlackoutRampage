using System;
using UnityEngine;

public class InteractionZone : MonoBehaviour
{
    [Header("Interact objects")]
    private IInteractable interactableObject;
    private GameObject interactor;

    [Header("Prompt")]
    [SerializeField] private GameObject interactionPromptPrefab;
    [SerializeField] private PromptIconSet iconSet;

    [Header("Shelving")]
    [HideInInspector] public Shelving shelving;
    
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
        if (interactableObject != null) return;
        OnInteractableDetected(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (interactableObject != null) return;
        OnInteractableDetected(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Shelving>() && shelving)
            shelving = null;

        if (!other.TryGetComponent(out IInteractable interactable)) return;
        if (interactable != interactableObject) return;

        interactable.HideInteractPrompt();
        interactableObject = null;
    }

    private void OnInteractableDetected(Collider other)
    {
        if (other.GetComponent<Shelving>())
            shelving = other.GetComponent<Shelving>();

        if (!other.TryGetComponent(out IInteractable interactable)) return;

        if (!interactable.CanInteract(interactor)) return;

        interactableObject = interactable;
        
        interactableObject.ShowInteractPrompt(interactor);
    }
}
