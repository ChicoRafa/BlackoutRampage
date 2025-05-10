using UnityEngine;
using UnityEngine.UI;

namespace _Data.Customers.Scripts {
    public class ClientPatienceUI : MonoBehaviour {
        [SerializeField] private Image fillBar;

        private Camera mainCamera;

        private void Awake() {
            mainCamera = Camera.main;
        }

        private void LateUpdate() {
            // Make the UI face the camera
            if (mainCamera != null)
                transform.forward = mainCamera.transform.forward;
        }

        /// <summary>
        /// Updates the fill amount and color based on normalized patience (0–1).
        /// </summary>
        /// <param name="normalizedValue">Value between 0 and 1.</param>
        public void UpdatePatience(float normalizedValue) {
            normalizedValue = Mathf.Clamp01(normalizedValue);
            fillBar.fillAmount = normalizedValue;

            // Change color based on remaining patience
            if (normalizedValue > 0.6f) {
                fillBar.color = new Color32(0, 255, 0, 255); // Green
            } else if (normalizedValue > 0.3f) {
                fillBar.color = new Color32(255, 216, 0, 255); // Yellow
            } else {
                fillBar.color = new Color32(255, 76, 76, 255); // Red
            }
        }
    }
}