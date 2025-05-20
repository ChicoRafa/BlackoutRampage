using UnityEngine;
using _Data.Customers.Orders;
using _Data.Customers.Scriptables;

namespace _Data.Customers.Scripts {
    public enum ClientState {
        WalkingToSlot,
        Waiting,
        Leaving
    }

    public class Client : MonoBehaviour, IInteractable {
        [Header("Client Type")]
        [SerializeField] private ClientType clientType;

        private GameObject modelInstance;
        private Animator animator;
        private ClientPatienceUI patienceUI;

        private ClientMovement movement;
        private ClientPatienceController patienceController;

        private bool hasStartedPatience = false;
        private bool isBeingServed = false;
        private ClientState currentState = ClientState.WalkingToSlot;

        private ClientQueueManager queueManager;
        public Order CurrentOrder { get; private set; }

        private void Awake() {
            // Spawn and attach character model
            if (clientType.modelPrefab != null) {
                modelInstance = Instantiate(clientType.modelPrefab, transform);
                modelInstance.transform.localPosition = new Vector3(0f, -0.7f, 0f);
                modelInstance.transform.localRotation = Quaternion.identity;

                animator = modelInstance.GetComponentInChildren<Animator>();
                if (animator != null && clientType.animatorController != null) {
                    animator.runtimeAnimatorController = clientType.animatorController;
                }
            }

            patienceUI = GetComponentInChildren<ClientPatienceUI>(true);
            if (patienceUI == null) {
                Debug.LogError($"❌ {gameObject.name} is missing a ClientPatienceUI component!");
            }

            movement = gameObject.AddComponent<ClientMovement>();
            movement.Init(animator, modelInstance);

            patienceController = gameObject.AddComponent<ClientPatienceController>();
        }

        private void Start() {
            CurrentOrder = OrderGenerator.GenerateRandomOrder();
            Debug.Log($"🟢 {gameObject.name} spawned with order of {CurrentOrder.Items.Count} items.");
            patienceUI?.SetOrder(CurrentOrder);
        }

        public void SetQueueManager(ClientQueueManager manager) {
            queueManager = manager;
        }

        public void MoveToQueuePosition(Vector3 targetPosition, int queueIndex, bool isServiceSlot) {
            isBeingServed = isServiceSlot;
            currentState = ClientState.WalkingToSlot;

            movement.MoveTo(targetPosition, () => OnReachedQueuePosition(queueIndex, isServiceSlot));
        }

        private void OnReachedQueuePosition(int queueIndex, bool isServiceSlot) {
            if (isServiceSlot && modelInstance != null) {
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

        public void Interact(GameObject interactor) {
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

            if (CurrentOrder.Items.ContainsKey(itemType)) {
                CurrentOrder.Items[itemType]--;

                if (CurrentOrder.Items[itemType] <= 0)
                    CurrentOrder.Items.Remove(itemType);

                Debug.Log($"✅ {name} received: {itemType.name}");

                Destroy(heldItem);
                inventory.ClearSlot(selectedSlot);
                patienceUI?.SetOrder(CurrentOrder);

                if (CurrentOrder.Items.Count == 0) {
                    LeaveSatisfied();
                }
            } else {
                Debug.Log($"❌ {name} didn't ask for {itemType.name}");
                Destroy(heldItem);
                inventory.ClearSlot(selectedSlot);
                LeaveAngry();
            }
        }

        public string GetInteractionPrompt() {
            return "Give item";
        }

        public bool CanInteract(GameObject interactor) {
            return IsReadyToBeServed();
        }
    }
}
