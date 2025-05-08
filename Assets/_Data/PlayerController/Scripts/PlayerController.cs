using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Data.Scripts.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        private GameManager gameManager;//Unused variable, but kept for future use

        [Header("Movement")] [Tooltip("Player's movement variables")]
        private Rigidbody playerRigidbody;

        [SerializeField] private float playerSpeed = 1f * 100;
        [SerializeField] private float playerRotationSpeed = 1f * 10;
        private Vector3 moveDirection = Vector3.zero;

        [Header("Interaction")] [Tooltip("Player's interaction variables")] [SerializeField]
        private InputActionAsset playerInputActionAsset;
        private InteractionZone interactionZone;

        void Start()
        {
            gameManager = FindFirstObjectByType<GameManager>();
            playerRigidbody = GetComponent<Rigidbody>();
            playerInputActionAsset = FindFirstObjectByType<InputActionAsset>();
            interactionZone = GetComponentInChildren<InteractionZone>();
            
            playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        void FixedUpdate()
        {
            PerformMovement();
        }

        private void OnEnable()
        {
            playerInputActionAsset.Enable();
            playerInputActionAsset["Player/Move"].performed += OnMove;
            playerInputActionAsset["Player/Move"].canceled += OnMove;
            playerInputActionAsset["Player/Interact"].performed += OnInteract;
        }

        private void OnDisable()
        {   
            if (playerInputActionAsset == null) return;
            
            playerInputActionAsset.Disable();
            playerInputActionAsset["Player/Move"].performed -= OnMove;
            playerInputActionAsset["Player/Move"].canceled -= OnMove;
            playerInputActionAsset["Player/Interact"].performed -= OnInteract;
        }

        internal void OnMove(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();
            moveDirection = new Vector3(input.x, 0, input.y);
            moveDirection = moveDirection.normalized;
        }
        
        internal void OnInteract(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            interactionZone.TryInteract();
        }

        private void PerformMovement()
        {
            playerRigidbody.linearVelocity = moveDirection * (playerSpeed * Time.deltaTime);
            
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
    }
}