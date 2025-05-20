using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace _Data.PlayerController.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        private GameManager gameManager; //Unused variable, but kept for future use

        [Header("Movement")] [Tooltip("Player's movement variables")]
        private Rigidbody playerRigidbody;
        [SerializeField] private PlayerMovementConfig movementConfig;
        [SerializeField] private float currentSpeed = 0f;
        private Vector3 moveDirection = Vector3.zero;
        private bool isSprinting = false;
        private float speedMultiplier = 1f;
        private Vector3 lastCollisionNormal = Vector3.up;


        [Header("Interaction")] [Tooltip("Player's interaction variables")] [SerializeField]
        private InputReader inputReader;
        [HideInInspector] public InteractionZone interactionZone;

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
        }

        private void FixedUpdate()
        {
            PerformMovement();
            UpdateAnimation();
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
            isSprinting = isPressed;
            currentSpeed = isSprinting ? movementConfig.sprintSpeed : movementConfig.walkSpeed;
        }

        private void OnDrop()
        {
            playerInventory.DropItem();
        }

        private void OnSelectItem(int slotIndex)
        {
            playerInventory.UpdateSelectedSlot(slotIndex);
        }
        
        private void OnCollisionStay(Collision collision)
        {
            lastCollisionNormal = collision.contacts[0].normal;
        }

        private void OnPause()
        {
            onPause.Invoke();
        }

        private void PerformMovement()
        {
            Vector3 desiredMove = moveDirection * (currentSpeed * Time.deltaTime * speedMultiplier);

            if (moveDirection != Vector3.zero)
            {
                desiredMove = Vector3.ProjectOnPlane(desiredMove, lastCollisionNormal);
            }

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
    }
}