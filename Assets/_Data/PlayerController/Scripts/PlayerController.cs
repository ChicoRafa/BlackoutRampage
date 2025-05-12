using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

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

        [Header("Interaction")] [Tooltip("Player's interaction variables")] [SerializeField]
        private InputReader inputReader;
        private InteractionZone interactionZone;

        [Header("Animation")] [Tooltip("Player's animation variables")]
        private Animator playerAnimator;

        [Header("Inventory")]
        private PlayerInventoryScript playerInventory;
        public Transform objectsTPSpot;

        private void Awake()
        {
            playerInventory = GetComponent<PlayerInventoryScript>();
        }

        private void Start()
        {
            gameManager = FindFirstObjectByType<GameManager>();
            playerRigidbody = GetComponent<Rigidbody>();
            playerInputActionAsset = FindFirstObjectByType<InputActionAsset>();
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
            playerInputActionAsset["Player/Drop"].performed += OnDrop;
            playerInputActionAsset["Player/SelectItem"].performed += OnSelectItem;
        }

        private void OnDisable()
        {
            inputReader.MoveEvent -= OnMove;
            inputReader.MoveCanceledEvent -= OnMoveCanceled;
            inputReader.InteractEvent -= OnInteract;
            inputReader.SprintEvent -= OnSprint;
            playerInputActionAsset["Player/Drop"].performed -= OnDrop;
            playerInputActionAsset["Player/SelectItem"].performed -= OnSelectItem;
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

        internal void OnDrop(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            playerInventory.DropItem();
        }

        internal void OnSelectItem(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            #region Change Inventory Slot Color
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
                playerInventory.UpdateSelectedSlot(0);
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
                playerInventory.UpdateSelectedSlot(1);
            else if (Keyboard.current.digit3Key.wasPressedThisFrame)
                playerInventory.UpdateSelectedSlot(2);
            else if (Keyboard.current.digit4Key.wasPressedThisFrame)
                playerInventory.UpdateSelectedSlot(3);
            else if (Keyboard.current.digit5Key.wasPressedThisFrame)
                playerInventory.UpdateSelectedSlot(4);
            else if (Keyboard.current.digit6Key.wasPressedThisFrame)
                playerInventory.UpdateSelectedSlot(5);
            else if (Keyboard.current.digit7Key.wasPressedThisFrame)
                playerInventory.UpdateSelectedSlot(6);
            else if (Keyboard.current.digit8Key.wasPressedThisFrame)
                playerInventory.UpdateSelectedSlot(7);
            else if (Keyboard.current.digit9Key.wasPressedThisFrame)
                playerInventory.UpdateSelectedSlot(8);
            #endregion
        }

        private void PerformMovement()
        {
            playerRigidbody.linearVelocity = moveDirection * (currentSpeed * Time.deltaTime);

            if (moveDirection.Equals(Vector3.zero))
            {
                return;
            }

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
    }
}