using UnityEngine;

public class ComputerInteractable : InteractableBase
{
    [SerializeField] private PerksSO perksData;
    private Truck truck;

    private void Start()
    {
        truck = FindFirstObjectByType<Truck>();
        if (truck == null)
        {
            Debug.LogError("🚚 Truck not found in the scene!");
        }
    }

    public override void Interact(GameObject interactor)
    {
        truck?.TruckArrives();
    }

    public override bool CanInteract(GameObject interactor)
    {
        return perksData != null && perksData.perkCallTruck;
    }

    public override string GetInteractionPrompt()
    {
        return "Call Truck";
    }
}