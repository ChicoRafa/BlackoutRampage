using System;
using UnityEngine;

namespace _Data.Customers.Scripts
{
    public class ClientPatienceController : MonoBehaviour
    {
        [Header("Patiente values")]
        [SerializeField] private float minBasePatience = 250f;
        [SerializeField] private float maxBasePatience = 300f;
        [SerializeField] private float extraPatiencePercentPerQueueIndex = 0.20f;

        private float currentPatience;
        private float maxPatience;
        private bool isActive;
        private bool hasStarted;

        private ClientPatienceUI ui;
        private Action onPatienceDepleted;
        private GameManager gameManager;
        private float patienceSpeedMultiplier = 1f;

        public void Init(ClientPatienceUI ui, GameManager gameManager, int queueIndex,
            float minClientPatience, float maxClientPatience)
        {
            this.minBasePatience = minClientPatience;
            this.maxBasePatience = maxClientPatience;
            this.gameManager = gameManager;
            if (this.gameManager != null)
                gameManager.onPatienceLevelMultiplierChanged.AddListener(OnPatienceLevelMultiplierChanged);
            
            this.ui = ui;

            float rawPatience = UnityEngine.Random.Range(minBasePatience, maxBasePatience);
            float patienceBonus = extraPatiencePercentPerQueueIndex * queueIndex;
            maxPatience = rawPatience * (1f + patienceBonus);

            currentPatience = maxPatience;

            float normalized = Mathf.Clamp01(currentPatience / maxPatience);
            ui?.UpdatePatience(normalized);
            ui?.gameObject.SetActive(true);
        }
        private void Update()
        {
            if (!isActive || !hasStarted || currentPatience <= 0f) return;
            
            currentPatience -= Time.deltaTime * patienceSpeedMultiplier;

            float normalized = Mathf.Clamp01(currentPatience / maxPatience);
            ui?.UpdatePatience(normalized);

            if (currentPatience <= 0f)
                EndPatience();
        }

        public void StartPatience(Action onDepletedCallback)
        {
            if (isActive) return;
      
            onPatienceDepleted = onDepletedCallback;
            patienceSpeedMultiplier = gameManager.GetPatienceLevelMultiplier();
            isActive = true;
            hasStarted = true;
        }

        public void Deactivate()
        {
            if (gameManager != null)
                gameManager.onPatienceLevelMultiplierChanged.AddListener(OnPatienceLevelMultiplierChanged);

            isActive = false;
            hasStarted = false;
            ui?.gameObject.SetActive(false);
        }
        
        private void OnPatienceLevelMultiplierChanged()
        {
            patienceSpeedMultiplier = gameManager.GetPatienceLevelMultiplier();
        }
        
        public bool IsActive() => isActive;
        
        public void ReducePatienceByAbsoluteFraction(float fractionOfMax)
        {
            float amountToReduce = maxPatience * fractionOfMax;
            currentPatience = Mathf.Max(0f, currentPatience - amountToReduce);

            float normalized = Mathf.Clamp01(currentPatience / maxPatience);
            ui?.UpdatePatience(normalized);

            if (currentPatience <= 0f)
                EndPatience();
        }

        public void IncreasePatienceByAbsoluteFraction(float fractionOfMax)
        {
            if (!isActive) return;
            float amountToAdd = maxPatience * fractionOfMax;
            currentPatience = Mathf.Min(maxPatience, currentPatience + amountToAdd);

            float normalized = Mathf.Clamp01(currentPatience / maxPatience);
            ui?.UpdatePatience(normalized);
        }
        
        private void EndPatience()
        {
            isActive = false;
            hasStarted = false;
            onPatienceDepleted?.Invoke();
        }
    }
}
