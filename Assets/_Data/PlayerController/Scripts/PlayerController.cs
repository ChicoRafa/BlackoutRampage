using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace _Data.PlayerController.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        private GameManager gameManager;//Unused variable, but kept for future use

        [Header("Movement")] [Tooltip("Player's movement variables")]
        private Rigidbody playerRigidbody;

        [FormerlySerializedAs("playerSpeed")] [SerializeField] private float playerWalkSpeed = 500f;
        [SerializeField] private float playerSprintSpeed = 650f;
        [SerializeField] private float playerRotationSpeed = 1f * 10;
        [SerializeField] private float currentSpeed = 0f;
        private Vector3 moveDirection = Vector3.zero;
        private bool isSprinting = false;

        [Header("Interaction")] [Tooltip("Player's interaction variables")] [SerializeField]
        private InputActionAsset playerInputActionAsset;
        private InteractionZone interactionZone;
        
        [Header("Animation")] [Tooltip("Player's animation variables")]
        private Animator playerAnimator;

        private void Start()
        {
            gameManager = FindFirstObjectByType<GameManager>();
            playerRigidbody = GetComponent<Rigidbody>();
            playerInputActionAsset = FindFirstObjectByType<InputActionAsset>();
            interactionZone = GetComponentInChildren<InteractionZone>();
            
            playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            playerAnimator = GetComponentInChildren<Animator>();
            currentSpeed = playerWalkSpeed;
        }

        private void FixedUpdate()
        {
            PerformMovement();
            UpdateAnimation();
        }

        private void OnEnable()
        {
            playerInputActionAsset.Enable();
            playerInputActionAsset["Player/Move"].performed += OnMove;
            playerInputActionAsset["Player/Move"].canceled += OnMove;
            playerInputActionAsset["Player/Interact"].performed += OnInteract;
            playerInputActionAsset["Player/Interact"].canceled += OnInteract;
            playerInputActionAsset["Player/Sprint"].performed += OnSprint;
            playerInputActionAsset["Player/Sprint"].canceled += OnSprint;
        }

        private void OnDisable()
        {   
            if (playerInputActionAsset == null) return;
            
            playerInputActionAsset.Disable();
            playerInputActionAsset["Player/Move"].performed -= OnMove;
            playerInputActionAsset["Player/Move"].canceled -= OnMove;
            playerInputActionAsset["Player/Interact"].performed -= OnInteract;
            playerInputActionAsset["Player/Interact"].canceled -= OnInteract;
            playerInputActionAsset["Player/Sprint"].performed -= OnSprint;
            playerInputActionAsset["Player/Sprint"].canceled -= OnSprint;
        }

        internal void OnMove(InputAction.CallbackContext context)
        {
            if (!context.canceled)
            {
                Vector2 input = context.ReadValue<Vector2>();
                moveDirection = new Vector3(input.x, 0, input.y);
                moveDirection = moveDirection.normalized;
            }
            else
            {
                moveDirection = Vector3.zero;
            }
        }
        
        internal void OnInteract(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            interactionZone.TryInteract();
        }
        
        internal void OnSprint(InputAction.CallbackContext context)
        {
            isSprinting = context.performed;
            currentSpeed = isSprinting ? playerSprintSpeed : playerWalkSpeed;
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
                playerRotationSpeed * Time.deltaTime);
        }
        
        private void UpdateAnimation()
        {
            float rawSpeed = playerRigidbody.linearVelocity.magnitude;
    
            // Removes small values to avoid jittering
            if (rawSpeed < 0.01f)
                rawSpeed = 0f;

            float normalizedSpeed = rawSpeed / playerSprintSpeed;
            playerAnimator.SetFloat("PlayerSpeed", normalizedSpeed, 0.1f, Time.deltaTime);
        }
    }
}