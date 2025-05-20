using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using _Data.Customers.Orders;
using _Data.Products;

namespace _Data.Customers.Scripts {
    public class ClientPatienceUI : MonoBehaviour {
        [Header("Patience UI")]
        [SerializeField] private Image fillBar;

        [Header("Order UI")]
        [SerializeField] private Transform orderItemsContainer;
        [SerializeField] private GameObject orderItemUIPrefab;

        private Camera mainCamera;

        private void Awake() {
            mainCamera = Camera.main;
        }

        private void LateUpdate() {
            if (mainCamera != null)
                transform.forward = mainCamera.transform.forward;
        }

        public void UpdatePatience(float normalizedValue) {
            normalizedValue = Mathf.Clamp01(normalizedValue);
            fillBar.fillAmount = normalizedValue;

            if (normalizedValue > 0.6f) {
                fillBar.color = new Color32(0, 255, 0, 255);
            } else if (normalizedValue > 0.3f) {
                fillBar.color = new Color32(255, 216, 0, 255);
            } else {
                fillBar.color = new Color32(255, 76, 76, 255);
            }
        }
        
        public void SetOrder(Order order) {
            foreach (Transform child in orderItemsContainer) {
                Destroy(child.gameObject);
            }

            foreach (KeyValuePair<Product, int> kvp in order.Items) {
                Product product = kvp.Key;
                int quantity = kvp.Value;

                for (int i = 0; i < quantity; i++) {
                    GameObject iconGO = Instantiate(orderItemUIPrefab, orderItemsContainer);
                    Image icon = iconGO.GetComponent<Image>();

                    if (icon != null && product.sprite != null) {
                        icon.sprite = product.sprite;
                    }
                }
            }
        }
    }
}