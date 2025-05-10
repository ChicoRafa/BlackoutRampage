using System.Collections;
using UnityEngine;
using _Data.Customers.Orders;
using _Data.Customers.Scriptables;
using _Data.Customers.Scripts;

namespace _Data.Customers.Controllers {
    public enum ClientState {
        WalkingToSlot,
        Waiting,
        Leaving
    }

    public class Client : MonoBehaviour {
        [Header("Client Type")]
        [SerializeField] private ClientType clientType;

        private ClientPatienceUI patienceUI;
        private float patienceTimer;
        private float maxPatience;
        private bool hasStartedPatience = false;
        private bool isFrontOfQueue = false;

        private ClientState currentState = ClientState.WalkingToSlot;
        private Coroutine moveRoutine;

        private ClientQueueManager queueManager;

        public void SetQueueManager(ClientQueueManager manager) {
            queueManager = manager;
        }

        public Order CurrentOrder { get; private set; }

        private void Awake() {
            GetComponent<Renderer>().material.color = clientType.bodyColor;
            patienceUI = GetComponentInChildren<ClientPatienceUI>(true);
            if (patienceUI == null) {
                Debug.LogError($"❌ {gameObject.name} is missing a ClientPatienceUI component!");
            }
        }

        private void Start() {
            CurrentOrder = OrderGenerator.GenerateRandomOrder();
            Debug.Log($"🟢 {gameObject.name} spawned with order of {CurrentOrder.Items.Count} items.");
        }

        private void Update() {
            if (currentState == ClientState.Waiting && hasStartedPatience) {
                patienceTimer -= Time.deltaTime;

                if (patienceUI != null && maxPatience > 0f) {
                    float normalized = Mathf.Clamp01(patienceTimer / maxPatience);
                    patienceUI.UpdatePatience(normalized);
                }

                if (patienceTimer <= 0f) {
                    Debug.Log($"😡 {gameObject.name} ran out of patience!");
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
                float patienceBonus = Mathf.Lerp(1.0f, 0.5f, queueIndex / 4f);
                float rawPatience = Random.Range(clientType.baseMinPatience, clientType.baseMaxPatience);
                maxPatience = rawPatience * patienceBonus;
                patienceTimer = maxPatience;
                hasStartedPatience = true;

                Debug.Log($"⏳ {gameObject.name} joined queue at position {queueIndex}. Patience: {maxPatience:F1}s");

                patienceUI?.UpdatePatience(1f);
                if (patienceUI != null) patienceUI.gameObject.SetActive(true);
            }

            currentState = ClientState.Waiting;

            if (isFrontOfQueue) {
                Debug.Log($"🛎️ {gameObject.name} is now at the front and ready to be served.");
            }
        }

        public void OnInteractSimulated() {
            if (!IsReadyToBeServed()) return;

            Debug.Log($"✅ {gameObject.name} has been served!");
            LeaveSatisfied();
        }

        public bool IsReadyToBeServed() {
            return currentState == ClientState.Waiting && isFrontOfQueue;
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

            if (patienceUI != null) patienceUI.gameObject.SetActive(false); // ✅ Ocultar barra

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
            Debug.Log($"❌ {gameObject.name} couldn't join queue and leaves immediately.");
            StartCoroutine(MoveToExit(exitPosition));
        }

        public bool IsLeaving() {
            return currentState == ClientState.Leaving;
        }
    }
}
