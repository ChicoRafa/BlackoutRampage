using _Data.Customers.FSM;
using UnityEngine;
using _Data.Customers.Orders;
using _Data.Customers.Scriptables;
using Unity.VisualScripting;

namespace _Data.Customers.Scripts {

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
        private ClientFSM fsm;
        private bool hasReceivedValidItem = false;
        private bool hasReceivedInvalidItem = false;
        private bool isMovingImpatiently = false;

        private ClientQueueManager queueManager;
        private GameManager gameManager;
        public Order CurrentOrder { get; private set; }
        public ClientMovement GetMovement() => movement;
        public ClientFSM GetFSM() => fsm;
        public ClientQueueManager GetQueueManager() => queueManager;
        
        public ClientPatienceController GetPatienceController() => patienceController;

        public ClientStateSO InitialState => clientType.initialState;
        public ClientStateSO LeaveSatisfiedState => clientType.leaveSatisfiedState;
        public ClientStateSO LeaveAngryState => clientType.leaveAngryState;
        public ClientStateSO WaitingState => clientType.waitingState;
        public ClientStateSO WalkingToQueueSlotState => clientType.walkingToQueueSlotState;
        public ClientStateSO BeingServedState => clientType.beingServedState;
        
 
        public void Init(ClientType clientType, ClientQueueManager queueManager, GameManager gameManager)
        {
            if (!clientType || !queueManager) return;

            this.clientType = clientType;
            this.queueManager = queueManager;
            this.gameManager = gameManager;
  
            if (clientType.modelPrefab) {
                modelInstance = Instantiate(clientType.modelPrefab, transform);
                modelInstance.transform.localPosition = new Vector3(0f, -0.7f, 0f);
                modelInstance.transform.localRotation = Quaternion.identity;

                animator = modelInstance.GetComponentInChildren<Animator>();
                if (animator && clientType.animatorController) {
                    animator.runtimeAnimatorController = clientType.animatorController;
                }
            }

            // Components
            patienceUI = GetComponentInChildren<ClientPatienceUI>(true);
            if (!patienceUI) {
                Debug.LogError($"❌ {gameObject.name} is missing a ClientPatienceUI component!");
            }

            movement = gameObject.AddComponent<ClientMovement>();
            movement.Init(animator, modelInstance);

            patienceController = gameObject.AddComponent<ClientPatienceController>();
            patienceController.Init(patienceUI, gameManager, GetQueueManager().GetClientIndex(this)); 
            
            fsm = gameObject.AddComponent<ClientFSM>();
            fsm.Init(this, InitialState);
            
            CurrentOrder = OrderGenerator.GenerateRandomOrder();
            patienceUI?.SetOrder(CurrentOrder);
        }
        
        public void StartPatience(System.Action onDepleted) {
            patienceController.StartPatience(onDepleted);
        }
        
        public void StartLeaving() {
            int score = Mathf.FloorToInt(
                (float)(CurrentOrder.GetOriginalCount() - CurrentOrder.GetRemainingCount()) 
                / CurrentOrder.GetOriginalCount() * 100f
            );
            gameManager.UpdateHappiness(score);

            queueManager.DequeueClient(this);
            patienceController.Deactivate();
        }

        public void ShowAngryEffect() {
            Debug.Log($"😡 {name} is angry!");
            // TODO: Add VFX or audio if needed
        }

        public void ShowHappyEffect() {
            Debug.Log($"😊 {name} is happy!");
            // TODO: Add VFX or audio if needed
        }

        public void LookForward() {
            if (modelInstance) {
                modelInstance.transform.forward = Vector3.forward;
            }
        }
        
        public override void Interact(GameObject interactor) {
            if (fsm.GetCurrentState() != BeingServedState) return;

            var inventory = interactor.GetComponent<PlayerInventoryScript>();
            if (inventory == null) return;

            int selectedSlot = inventory.GetSelectedSlot();
            var heldItem = inventory.itemsInInventory[selectedSlot];
            if (heldItem == null) return;

            var productScript = heldItem.GetComponent<ProductScript>();
            if (productScript == null) return;

            var itemType = productScript.GetProduct();

            if (CurrentOrder.ContainsProduct(itemType)) {
                CurrentOrder.RemoveProduct(itemType);
                gameManager.UpdateMoney(itemType.sellingPrice);
                soundManager.PlaySFX(audioCue, "Coins", 1f);

                Destroy(heldItem);
                inventory.ClearSlot(selectedSlot);
                patienceUI?.SetOrder(CurrentOrder);

                hasReceivedValidItem = true;
            } else {
                Destroy(heldItem);
                inventory.ClearSlot(selectedSlot);
                hasReceivedInvalidItem = true;
            }
        }
        
        public bool HasReceivedValidItem() => hasReceivedValidItem;
        public bool HasReceivedInvalidItem() => hasReceivedInvalidItem;

        public void ClearInteractionFlags() {
            hasReceivedValidItem = false;
            hasReceivedInvalidItem = false;
        }

        public override string GetInteractionPrompt() => "Give item";
        public override bool CanInteract(GameObject interactor) =>
            fsm.GetCurrentState() == BeingServedState;
        
        public bool IsLeaving() =>
            fsm.GetCurrentState() == LeaveAngryState || fsm.GetCurrentState() == LeaveSatisfiedState;
        
        public void MoveImpatiently()
        {
            if (isMovingImpatiently) return;
            isMovingImpatiently = true;

            Vector3 basePos = queueManager.GetAssignedSlotFor(this);
            movement.MoveImpatiently(basePos, () => {
                isMovingImpatiently = false;
            });
        }
        
        public void PoliceCalledAway() {
            Debug.Log($"🚓 {name} got a call and is leaving the queue!");
            fsm.TransitionTo(LeaveAngryState);
        }
        
        public void AddPoliceHappinessBonus(int amount) {
            Debug.Log($"👮‍♂️ {name} awarded a police bonus of {amount} happiness!");
            gameManager.UpdateHappiness(amount);
        }
        
        public void AddKarenHappinessPenalty(int amount) {
            gameManager.UpdateHappiness(-amount);
        }
        
        public void ReducePatience(float fraction) {
            patienceController.ReducePatienceByAbsoluteFraction(fraction);
        }
    }
}
