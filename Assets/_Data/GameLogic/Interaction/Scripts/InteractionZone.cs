using UnityEngine;

public class InteractionZone : MonoBehaviour
{
    private IInteractable interactableObject;
    private GameObject interactor;
    
    [SerializeField] private GameObject interactionPromptPrefab;
    [SerializeField] private PromptIconSet iconSet;
    private GameObject currentPromptInstance;
    private InteractionPromptUI currentPromptUI;

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
        if (other.gameObject.CompareTag("Computer"))
            isLookingAtComputer = true;

        if (other.GetComponent<Shelving>())
            shelving = other.GetComponent<Shelving>();

        if (!other.TryGetComponent(out IInteractable interactable)) return;

        if (!interactable.CanInteract(interactor)) return;

        DestroyLastInteractableObjectUI();

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
        if (other.gameObject.CompareTag("Computer"))
            isLookingAtComputer = false;

        if (other.GetComponent<Shelving>() && shelving)
            shelving = null;

        if (!other.TryGetComponent(out IInteractable interactable) || interactable != interactableObject) return;

        DestroyLastInteractableObjectUI();
    }

    private void DestroyLastInteractableObjectUI()
    {
        interactableObject = null;

        if (currentPromptInstance == null) return;
        Destroy(currentPromptInstance);
        currentPromptInstance = null;
    }
}