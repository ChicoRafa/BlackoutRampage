using UnityEngine;
using _Data.Customers.Orders;
using _Data.Customers.Scriptables;

namespace _Data.Customers.Scripts {
    public enum ClientState {
        WalkingToSlot,
        Waiting,
        Leaving
    }

    public class Client : InteractableBase {
        [Header("Client Type")]
        [SerializeField] private ClientType clientType;

        [Header("Sound")]
        [SerializeField] private SoundManagerSO soundManager;
        [SerializeField] private AudioCueSO audioCue;

        private GameObject modelInstance;
        private Animator animator;
        private ClientPatienceUI patienceUI;

        private ClientMovement movement;
        private ClientPatienceController patienceController;

        private bool hasStartedPatience = false;
        private bool isBeingServed = false;
        private ClientState currentState = ClientState.WalkingToSlot;

        private ClientQueueManager queueManager;
        private GameManager gameManager;
         public Order CurrentOrder { get; private set; }
        
        public void Init(ClientType clientType, ClientQueueManager queueManager, GameManager gameManager)
        {
            if (!clientType || !queueManager) return;
            
            this.queueManager = queueManager;
            this.gameManager = gameManager;
  
            // Attach character model
            if (clientType.modelPrefab) {
                modelInstance = Instantiate(clientType.modelPrefab, transform);
                modelInstance.transform.localPosition = new Vector3(0f, -0.7f, 0f);
                modelInstance.transform.localRotation = Quaternion.identity;

                animator = modelInstance.GetComponentInChildren<Animator>();
                if (animator && clientType.animatorController) {
                    animator.runtimeAnimatorController = clientType.animatorController;
                }
            }

            patienceUI = GetComponentInChildren<ClientPatienceUI>(true);
            if (!patienceUI) {
                Debug.LogError($"❌ {gameObject.name} is missing a ClientPatienceUI component!");
            }

            movement = gameObject.AddComponent<ClientMovement>();
            movement.Init(animator, modelInstance);

            patienceController = gameObject.AddComponent<ClientPatienceController>();
            
            CurrentOrder = OrderGenerator.GenerateRandomOrder();
            Debug.Log($"🟢 {gameObject.name} spawned with order of {CurrentOrder.GetOriginalCount()} items.");
            patienceUI?.SetOrder(CurrentOrder);
        }
        
        public void MoveToQueuePosition(Vector3 targetPosition, int queueIndex, bool isServiceSlot) {
            isBeingServed = isServiceSlot;
            currentState = ClientState.WalkingToSlot;

            movement.MoveTo(targetPosition, () => OnReachedQueuePosition(queueIndex, isServiceSlot));
        }

        private void OnReachedQueuePosition(int queueIndex, bool isServiceSlot) {
            if (isServiceSlot && modelInstance) {
                modelInstance.transform.forward = Vector3.forward;
            }

            if (!hasStartedPatience) {
                patienceController.StartPatience(patienceUI, queueIndex, LeaveAngry);
                hasStartedPatience = true;
            }

            currentState = ClientState.Waiting;

            if (isBeingServed) {
                Debug.Log($"🛎️ {gameObject.name} is now ready to be served.");
            }
        }

        public bool IsReadyToBeServed() {
            return currentState == ClientState.Waiting && isBeingServed;
        }

        private void LeaveSatisfied() {
            Debug.Log($"🚶 {gameObject.name} is leaving satisfied.");
            StartLeaving();
        }

        private void LeaveAngry() {
            Debug.Log($"💢 {gameObject.name} is leaving angry.");
            StartLeaving();
        }

        private void StartLeaving() {

            int score = Mathf.FloorToInt(
                (float)(CurrentOrder.GetOriginalCount() - CurrentOrder.GetRemainingCount()) 
                / CurrentOrder.GetOriginalCount() * 100
            );
            gameManager.UpdateHappiness(score);

            currentState = ClientState.Leaving;
            queueManager.DequeueClient(this);
            patienceController.DeactivateUI();

            movement.MoveTo(queueManager.GetExitPoint().position, () => {
                Debug.Log($"💥 {gameObject.name} exited the store.");
                Destroy(gameObject);
            });
        }

        public void LeaveBecauseQueueIsFull(Vector3 exitPosition) {
            Debug.Log($"❌ {gameObject.name} couldn't join queue and leaves immediately.");
            movement.MoveTo(exitPosition, () => Destroy(gameObject));
        }

        public bool IsLeaving() {
            return currentState == ClientState.Leaving;
        }

        public override void Interact(GameObject interactor) {
            if (!IsReadyToBeServed()) return;

            var inventory = interactor.GetComponent<PlayerInventoryScript>();
            if (inventory == null) return;

            int selectedSlot = inventory.GetSelectedSlot();
            var heldItem = inventory.itemsInInventory[selectedSlot];
            if (heldItem == null) return;

            var productScript = heldItem.GetComponent<ProductScript>();
            if (productScript == null) {
                Debug.LogWarning("🛑 Held item doesn't have a ProductScript");
                return;
            }

            var itemType = productScript.GetProduct();

            if (CurrentOrder.ContainsProduct(itemType)) {
                
                CurrentOrder.RemoveProduct(itemType);
                gameManager.UpdateMoney(itemType.sellingPrice);
                soundManager.PlaySFX(audioCue, "Coins", 1f);
                
                Debug.Log($"✅ {name} received: {itemType.name}");

                Destroy(heldItem);
                inventory.ClearSlot(selectedSlot);
                patienceUI?.SetOrder(CurrentOrder);
                
                if (CurrentOrder.GetRemainingCount() == 0) {
                    LeaveSatisfied();
                }
            } else {
                Debug.Log($"❌ {name} didn't ask for {itemType.name}");
                Destroy(heldItem);
                inventory.ClearSlot(selectedSlot);
                LeaveAngry();
            }
        }

        public override string GetInteractionPrompt() {
            return "Give item";
        }

        public override bool CanInteract(GameObject interactor) {
            return IsReadyToBeServed();
        }
    }
}
