using UnityEngine;

public class ComputerInteractable : InteractableBase
{
    [SerializeField] private PerksSO perksData;
    
    private GameManager gameManager;
    private Truck truck;

    private void Start()
    {
        truck = FindFirstObjectByType<Truck>();
        if (truck == null)
        {
            Debug.LogError("🚚 Truck not found in the scene!");
        }
        
        gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!");
        }
    }

    public override void Interact(GameObject interactor)
    {
        if (truck == null || gameManager == null) return;
        truck.TruckArrives();
        gameManager?.UpdateMoney(-truck.GetTruckCost());
    }

    public override bool CanInteract(GameObject interactor)
    {
        int currentMoney = gameManager != null ? gameManager.getCurrentMoney() : 0;
        return perksData != null && perksData.perkCallTruck && currentMoney >= truck.GetTruckCost();
    }

    public override string GetInteractionPrompt()
    {
        return "Call Truck - 1000 coins";
    }
}