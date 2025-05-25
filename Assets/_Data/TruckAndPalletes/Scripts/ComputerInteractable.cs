using UnityEngine;

public class ComputerInteractable : InteractableBase
{
    [Header("Perks Data")]
    [SerializeField] private PerksSO perksData;
    
    private GameManager gameManager;
    private Truck truck;

    private void Start()
    {
        truck = FindFirstObjectByType<Truck>();
        gameManager = FindFirstObjectByType<GameManager>();
    }

    public override void Interact(GameObject interactor)
    {
        if (truck == null || gameManager == null) return;
        truck.TruckArrives();
        gameManager?.UpdateMoney(-truck.GetTruckCost());
    }

    public override bool CanInteract(GameObject interactor)
    {
        int currentMoney = gameManager != null ? gameManager.GetCurrentMoney() : 0;
        return perksData != null && perksData.perkCallTruck && currentMoney >= truck.GetTruckCost();
    }

    public override string GetInteractionPrompt()
    {
        return "Call Truck - 1000 coins";
    }
}
