using UnityEngine;
using UnityEngine.Events;

namespace _Data.PlayerController.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        private GameManager gameManager;

        [Header("Movement")] [Tooltip("Player's movement variables")]
        private Rigidbody playerRigidbody;
        [SerializeField] private PlayerMovementConfig movementConfig;
        [SerializeField] private float currentSpeed = 0f;
        private Vector3 moveDirection = Vector3.zero;
        private float speedMultiplier = 1f;
        private Vector3 lastCollisionNormal = Vector3.up;


        [Header("Interaction")] [Tooltip("Player's interaction variables")]
        [SerializeField] private InputReader inputReader;
        [HideInInspector] public InteractionZone interactionZone;
        
        [Header("Stamina Management")] [Tooltip("Player's stamina variables")]
        [SerializeField] private StaminaBarUI staminaBarUI;
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float staminaDrainRate = 20f;
        [SerializeField] private float staminaRegenRate = 10f;
        [SerializeField] private float regenDelay = 5f;
        private bool isSprinting = false;
        private float currentStamina;
        private bool wantsToSprint;
        private float timeSinceLastSprint;

        [Header("Sound Manager")][Tooltip("sound manager")]
        [SerializeField] private SoundManagerSO soundManagerSO;
        [SerializeField] private AudioCueSO playerSoundCueSO;

        [Header("Animation")] [Tooltip("Player's animation variables")]
        private Animator playerAnimator;

        [Header("Inventory")]
        private PlayerInventoryScript playerInventory;
        public Transform objectsTPSpot;

        [Header("Storehouse")]
        [HideInInspector] public GameObject storage;

        [Header("Events")]
        public UnityEvent onPause;
        

        private void Awake()
        {
            playerInventory = GetComponent<PlayerInventoryScript>();
        }

        private void Start()
        {
            gameManager = FindFirstObjectByType<GameManager>();
            playerRigidbody = GetComponent<Rigidbody>();
            interactionZone = GetComponentInChildren<InteractionZone>();

            playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            playerAnimator = GetComponentInChildren<Animator>();
            currentSpeed = movementConfig.walkSpeed;
            currentStamina = maxStamina;
            staminaBarUI.UpdateStamina(currentStamina / maxStamina);
        }

        private void FixedUpdate()
        {
            PerformMovement();
            UpdateAnimation();
            UpdateSprintState();
            HandleStamina(Time.deltaTime);
        }

        private void OnEnable()
        {
            inputReader.MoveEvent += OnMove;
            inputReader.MoveCanceledEvent += OnMoveCanceled;
            inputReader.InteractEvent += OnInteract;
            inputReader.SprintEvent += OnSprint;
            inputReader.DropEvent += OnDrop;
            inputReader.SelectItemEvent += OnSelectItem;
            inputReader.PauseEvent += OnPause;
        }

        private void OnDisable()
        {
            inputReader.MoveEvent -= OnMove;
            inputReader.MoveCanceledEvent -= OnMoveCanceled;
            inputReader.InteractEvent -= OnInteract;
            inputReader.SprintEvent -= OnSprint;
            inputReader.DropEvent -= OnDrop;
            inputReader.SelectItemEvent -= OnSelectItem;
            inputReader.PauseEvent -= OnPause;
        }

        private void OnMove(Vector2 input)
        {
            moveDirection = new Vector3(input.x, 0, input.y).normalized;
        }

        private void OnMoveCanceled()
        {
            moveDirection = Vector3.zero;
        }

        private void OnInteract()
        {
            interactionZone.TryInteract();
        }

        private void OnSprint(bool isPressed)
        {
            wantsToSprint = isPressed;
        }

        private void UpdateSprintState()
        {
            if (wantsToSprint && currentStamina > 0f)
            {
                isSprinting = true;
                timeSinceLastSprint = 0f;
            }
            else
            {
                isSprinting = false;
                timeSinceLastSprint += Time.deltaTime;
            }

            currentSpeed = isSprinting ? movementConfig.sprintSpeed : movementConfig.walkSpeed;
        }
        
        private void OnDrop()
        {
            if (!playerInventory.canDrop) return;
            
            playerInventory.DropItem();
            soundManagerSO.PlaySFX(playerSoundCueSO, "PickUp", 1f);
        }

        private void OnSelectItem(int slotIndex)
        {
            playerInventory.UpdateSelectedSlot(slotIndex);
        }
        
        private void OnCollisionStay(Collision collision)
        {
            lastCollisionNormal = collision.contacts[0].normal;
        }
        
        private void OnCollisionExit(Collision collision)
        {
            lastCollisionNormal = Vector3.up;
        }

        private void OnPause()
        {
            onPause.Invoke();
        }

        private void PerformMovement()
        {
            Vector3 desiredMove = moveDirection * (currentSpeed * Time.deltaTime * speedMultiplier);

            if (Vector3.Dot(lastCollisionNormal, Vector3.up) < 0.7f && Vector3.Dot(moveDirection, lastCollisionNormal) > 0.1f)
                lastCollisionNormal = Vector3.up;

            if (moveDirection != Vector3.zero && Vector3.Dot(lastCollisionNormal, Vector3.up) < 0.7f)
                desiredMove = Vector3.ProjectOnPlane(desiredMove, lastCollisionNormal);

            playerRigidbody.linearVelocity = desiredMove;

            if (moveDirection.Equals(Vector3.zero)) return;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            playerRigidbody.rotation = Quaternion.Slerp(
                playerRigidbody.rotation,
                targetRotation,
                movementConfig.rotationSpeed * Time.deltaTime);
        }

        private void UpdateAnimation()
        {
            float rawSpeed = playerRigidbody.linearVelocity.magnitude;

            // Removes small values to avoid jittering
            if (rawSpeed < 0.01f)
                rawSpeed = 0f;

            float normalizedSpeed = rawSpeed / movementConfig.sprintSpeed;
            playerAnimator.SetFloat("PlayerSpeed", normalizedSpeed, 0.1f, Time.deltaTime);
        }

        public void ModifySpeed(float speedMultiplier)
        {
            this.speedMultiplier = speedMultiplier;
        }
        
        private void HandleStamina(float deltaTime)
        {
            switch (isSprinting)
            {
                case true when currentStamina > 0:
                {
                    currentStamina -= staminaDrainRate * deltaTime;
                    currentStamina = Mathf.Max(currentStamina, 0f);

                    if (currentStamina <= 0f)
                    {
                        isSprinting = false;
                        currentSpeed = movementConfig.walkSpeed;
                        timeSinceLastSprint = 0f;
                    }

                    break;
                }
                default:
                {
                    if (timeSinceLastSprint >= regenDelay && currentStamina < maxStamina)
                    {
                        currentStamina += staminaRegenRate * deltaTime;
                        currentStamina = Mathf.Min(currentStamina, maxStamina);
                    }

                    break;
                }
            }
            staminaBarUI.UpdateStamina(currentStamina / maxStamina);
        }
    }
}
