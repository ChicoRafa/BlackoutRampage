using System;
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
        }

        private void OnDisable()
        {
            inputReader.MoveEvent -= OnMove;
            inputReader.MoveCanceledEvent -= OnMoveCanceled;
            inputReader.InteractEvent -= OnInteract;
            inputReader.SprintEvent -= OnSprint;
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