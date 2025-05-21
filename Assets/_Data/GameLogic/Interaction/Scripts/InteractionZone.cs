using System;
using UnityEngine;

public class InteractionZone : MonoBehaviour
{
    private IInteractable interactableObject;
    private GameObject interactor;
    
    [SerializeField] private GameObject interactionPromptPrefab;
    [SerializeField] private PromptIconSet iconSet;

    [Header("Shelving")]
    [HideInInspector] public Shelving shelving;

    [Header("Computer and Truck perk")]
    [SerializeField] private PerksSO perksData;
    [HideInInspector] public bool isLookingAtComputer = false;
    private Truck truck;

    private void Awake()
    {
        interactor = transform.parent.gameObject;
    }

    private void Start()
    {
        truck = FindFirstObjectByType<Truck>();
    }

    internal void TryInteract()
    {
        if (perksData.perkCallTruck && isLookingAtComputer)
        {
            truck.TruckArrives();
            return;
        }

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
        if (other.gameObject.CompareTag("Computer"))
            isLookingAtComputer = false;

        if (other.GetComponent<Shelving>() && shelving)
            shelving = null;

        if (!other.TryGetComponent(out IInteractable interactable)) return;
        if (interactable != interactableObject) return;

        interactable.HideInteractPrompt();
        interactableObject = null;
    }

    private void OnInteractableDetected(Collider other)
    {
        if (other.gameObject.CompareTag("Computer"))
            isLookingAtComputer = true;

        if (other.GetComponent<Shelving>())
            shelving = other.GetComponent<Shelving>();

        if (!other.TryGetComponent(out IInteractable interactable)) return;

        if (!interactable.CanInteract(interactor)) return;

        interactableObject = interactable;
        
        interactableObject.ShowInteractPrompt(interactor);
    }
}