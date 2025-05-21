using System;
using UnityEngine;

namespace _Data.Customers.Scripts {
    public class ClientPatienceController : MonoBehaviour {
        [SerializeField] private float minBasePatience = 5f;
        [SerializeField] private float maxBasePatience = 15f;

        private float currentPatience;
        private float maxPatience;
        private bool isActive;

        private ClientPatienceUI ui;
        private Action onPatienceDepleted;

        private void Update() {
            if (!isActive || currentPatience <= 0f) return;

            currentPatience -= Time.deltaTime;

            float normalized = Mathf.Clamp01(currentPatience / maxPatience);
            ui?.UpdatePatience(normalized);

            if (currentPatience <= 0f) {
                isActive = false;
                onPatienceDepleted?.Invoke();
            }
        }

        public void StartPatience(ClientPatienceUI ui, int queueIndex, Action onDepletedCallback) {
            this.ui = ui;
            onPatienceDepleted = onDepletedCallback;

            float rawPatience = UnityEngine.Random.Range(minBasePatience, maxBasePatience);
            float patienceBonus = Mathf.Lerp(1.0f, 0.5f, queueIndex / 4f);
            maxPatience = rawPatience * patienceBonus;
            currentPatience = maxPatience;
            isActive = true;

            ui?.UpdatePatience(1f);
            if (ui != null) ui.gameObject.SetActive(true);

            Debug.Log($"⏳ Starting patience: {maxPatience:F1}s (Q{queueIndex})");
        }

        public void Reduce(float amount) {
            if (!isActive || currentPatience <= 0f) return;

            currentPatience = Mathf.Max(0, currentPatience - amount);
            float normalized = Mathf.Clamp01(currentPatience / maxPatience);
            ui?.UpdatePatience(normalized);

            if (currentPatience <= 0f) {
                isActive = false;
                onPatienceDepleted?.Invoke();
            }
        }

        public void DeactivateUI() {
            ui?.gameObject.SetActive(false);
        }
    }
}
