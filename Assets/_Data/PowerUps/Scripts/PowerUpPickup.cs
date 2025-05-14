using _Data.PowerUps.Scriptables;
using UnityEngine;

namespace _Data.PowerUps.Scripts
{
    public class PowerUpPickup : MonoBehaviour
    {
        [SerializeField] private PowerUpSO powerUpData;
        [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 90f, 0f); // grados por segundo

        private void Update()
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            powerUpData.Activate(other.gameObject);
            StartCoroutine(DeactivateAfterDelay(other.gameObject));
            Destroy(transform.parent.gameObject);
        }

        private System.Collections.IEnumerator DeactivateAfterDelay(GameObject target)
        {
            yield return new WaitForSeconds(powerUpData.duration);
            powerUpData.Deactivate(target);
        }
        
        public void SetPowerUp(PowerUpSO powerUp) => powerUpData = powerUp;
    }
}
