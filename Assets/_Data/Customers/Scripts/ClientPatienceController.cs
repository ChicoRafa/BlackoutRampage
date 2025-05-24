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
        private GameManager gameManager;
        private float patienceSpeedMultiplier = 1f;

        private void Update() {
            if (!isActive || currentPatience <= 0f) return;

            
            currentPatience -= Time.deltaTime * patienceSpeedMultiplier;

            float normalized = Mathf.Clamp01(currentPatience / maxPatience);
            ui?.UpdatePatience(normalized);

            if (currentPatience <= 0f) {
                isActive = false;
                onPatienceDepleted?.Invoke();
            }
        }

        public void SetGameManager(GameManager gameManager)
        {
            this.gameManager = gameManager;
            if (this.gameManager != null)
            {
                gameManager.onPatienceLevelMultiplierChanged.AddListener(OnPatienceLevelMultiplierChanged);
            }
        }

        public void StartPatience(ClientPatienceUI ui, int queueIndex, Action onDepletedCallback)
        {
            if (isActive) return;
      
            this.ui = ui;
            onPatienceDepleted = onDepletedCallback;

            patienceSpeedMultiplier = gameManager.GetPatienceLevelMultiplier();
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

        public void Deactivate() {
            if (gameManager != null)
            {

                gameManager.onPatienceLevelMultiplierChanged.AddListener(OnPatienceLevelMultiplierChanged);
            }
            ui?.gameObject.SetActive(false);
        }
        
        private void OnPatienceLevelMultiplierChanged()
        {
            patienceSpeedMultiplier = gameManager.GetPatienceLevelMultiplier();
        }
        
        public bool IsActive() => isActive;
    }
}
