using System.Collections;
using UnityEngine;
using _Data.Customers.Orders;

namespace _Data.Customers.Controllers {
    public class Client : MonoBehaviour {
        [Header("Client patience settings")]
        public float maxPatience = 10f;

        private float patienceTimer;
        private ClientState currentState = ClientState.WalkingToSlot;
        private Coroutine moveRoutine;

        private ClientQueueManager queueManager;

        public void SetQueueManager(ClientQueueManager manager) {
            queueManager = manager;
        }

        public Order CurrentOrder { get; private set; }

        private void Start() {
            // Don't start in Waiting — wait until they reach queue slot
            CurrentOrder = OrderGenerator.GenerateRandomOrder();

            Debug.Log($"🛒 New client order:");
            foreach (var item in CurrentOrder.Items) {
                Debug.Log($"- {item.Product.productName} x{item.Quantity}");
            }
        }

        private void Update() {
            if (currentState == ClientState.Waiting) {
                patienceTimer -= Time.deltaTime;

                if (patienceTimer <= 0f) {
                    Debug.Log("⌛ Client lost patience.");
                    LeaveAngry();
                }
            }
        }

        public void MoveToQueuePosition(Vector3 targetPosition, bool isFirstInLine) {
            if (moveRoutine != null) {
                StopCoroutine(moveRoutine);
            }

            moveRoutine = StartCoroutine(MoveToPositionRoutine(targetPosition, isFirstInLine));
        }

        private IEnumerator MoveToPositionRoutine(Vector3 targetPosition, bool becomesActive) {
            currentState = ClientState.WalkingToSlot;

            while (Vector3.Distance(transform.position, targetPosition) > 0.1f) {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, 3f * Time.deltaTime);
                yield return null;
            }

            moveRoutine = null;

            if (becomesActive) {
                currentState = ClientState.Waiting;
                patienceTimer = maxPatience;
                Debug.Log("🟢 Client is now waiting at the front of the queue.");
            }
        }

        public void OnInteractSimulated() {
            if (currentState != ClientState.Waiting) return;

            Debug.Log("🤝 Client has been attended (mock).");
            LeaveSatisfied();
        }

        private void LeaveSatisfied() {
            Debug.Log("😄 Client is satisfied.");
            queueManager.DequeueClient(this);
            StartCoroutine(LeaveRoutine(queueManager.GetExitPoint().position));
        }

        private void LeaveAngry() {
            Debug.Log("😡 Client is angry.");
            queueManager.DequeueClient(this);
            StartCoroutine(LeaveRoutine(queueManager.GetExitPoint().position));
        }

        private IEnumerator LeaveRoutine(Vector3 exitPos) {
            currentState = ClientState.Leaving;

            while (Vector3.Distance(transform.position, exitPos) > 0.1f) {
                transform.position = Vector3.MoveTowards(transform.position, exitPos, 3f * Time.deltaTime);
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
