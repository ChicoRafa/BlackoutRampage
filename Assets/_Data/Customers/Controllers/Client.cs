using System.Collections;
using UnityEngine;
using _Data.Customers.Orders;

namespace _Data.Customers.Controllers {
    public class Client : MonoBehaviour {
        [Header("Client patience settings")]
        public float baseMinPatience = 5f;
        public float baseMaxPatience = 10f;

        private float patienceTimer;
        private bool hasStartedPatience = false;
        private bool isFrontOfQueue = false;

        private ClientState currentState = ClientState.WalkingToSlot;
        private Coroutine moveRoutine;

        private ClientQueueManager queueManager;

        public void SetQueueManager(ClientQueueManager manager) {
            queueManager = manager;
        }

        public Order CurrentOrder { get; private set; }

        private void Start() {
            CurrentOrder = OrderGenerator.GenerateRandomOrder();
            Debug.Log($"🛒 {gameObject.name} new order:");
        }

        private void Update() {
            if (currentState == ClientState.Waiting && hasStartedPatience) {
                patienceTimer -= Time.deltaTime;

                if (patienceTimer <= 0f) {
                    Debug.Log($"💢 {gameObject.name} ran out of patience!");
                    LeaveAngry();
                }
            }
        }

        public void MoveToQueuePosition(Vector3 targetPosition, bool isFront, int queueIndex) {
            isFrontOfQueue = isFront;

            if (moveRoutine != null) {
                StopCoroutine(moveRoutine);
            }

            moveRoutine = StartCoroutine(MoveToPositionRoutine(targetPosition, queueIndex));
        }

        private IEnumerator MoveToPositionRoutine(Vector3 targetPosition, int queueIndex) {
            currentState = ClientState.WalkingToSlot;

            while (Vector3.Distance(transform.position, targetPosition) > 0.1f) {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, 3f * Time.deltaTime);
                yield return null;
            }

            moveRoutine = null;

            if (!hasStartedPatience) {
                float patienceBonus = Mathf.Lerp(1.0f, 0.5f, queueIndex / 4f); // 0 = 100%, 4 = 50%
                float rawPatience = Random.Range(baseMinPatience, baseMaxPatience);
                patienceTimer = rawPatience * patienceBonus;
                hasStartedPatience = true;
                Debug.Log($"🟢 {gameObject.name} now waiting. Patience: {patienceTimer:F1}s");
            }

            currentState = ClientState.Waiting;
        }

        public void OnInteractSimulated() {
            if (!IsReadyToBeServed()) return;

            Debug.Log($"✅ {gameObject.name} has been served.");
            LeaveSatisfied();
        }

        public bool IsReadyToBeServed() {
            return currentState == ClientState.Waiting && isFrontOfQueue;
        }

        private void LeaveSatisfied() {
            StartLeaving();
        }

        private void LeaveAngry() {
            StartLeaving();
        }

        private void StartLeaving() {
            currentState = ClientState.Leaving;
            queueManager.DequeueClient(this);
            StartCoroutine(MoveToExit(queueManager.GetExitPoint().position));
        }

        private IEnumerator MoveToExit(Vector3 exitPos) {
            while (Vector3.Distance(transform.position, exitPos) > 0.1f) {
                transform.position = Vector3.MoveTowards(transform.position, exitPos, 3f * Time.deltaTime);
                yield return null;
            }

            Debug.Log($"💥 {gameObject.name} exited the store.");
            Destroy(gameObject);
        }

        public void LeaveBecauseQueueIsFull(Vector3 exitPosition) {
            Debug.Log($"❌ {gameObject.name} queue full, leaving immediately.");
            StartCoroutine(MoveToExit(exitPosition));
        }

        public bool IsLeaving() {
            return currentState == ClientState.Leaving;
        }
    }
}
