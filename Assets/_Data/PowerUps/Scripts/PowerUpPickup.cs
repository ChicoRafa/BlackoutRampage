using _Data.PowerUps.Scriptables;
using UnityEngine;

namespace _Data.PowerUps.Scripts
{
    public class PowerUpPickup : MonoBehaviour
    {
        [Header("PowerUps Data")]
        [SerializeField] private PowerUpSO powerUpData;
        [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 90f, 0f);

        [Header("SFX")]
        [SerializeField] private SoundManagerSO soundManagerSO;
        [SerializeField] private AudioCueSO powerUpCue;
        
        private GameManager gameManager;

        private void Update()
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            powerUpData.Activate(other.gameObject, gameManager);
            soundManagerSO.OnPlaySFX(powerUpCue, "PickPower", 1f);
            
            GetComponent<Collider>().enabled = false;
            GetComponentInChildren<MeshRenderer>().enabled = false;
            
            StartCoroutine(DeactivateAfterDelay(other.gameObject));
        }

        private System.Collections.IEnumerator DeactivateAfterDelay(GameObject target)
        {
            yield return new WaitForSeconds(powerUpData.duration);
            powerUpData.Deactivate(target, gameManager);
            Destroy(transform.parent.gameObject);
        }
        
        public void Init(PowerUpSO powerUp, GameManager gameManager)
        {
            powerUpData = powerUp;
            this.gameManager = gameManager;
        }
    }
}
