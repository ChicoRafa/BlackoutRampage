using UnityEngine;

namespace _Data.PowerUps.Scriptables
{
    [CreateAssetMenu(fileName = "PacifyingMusicPowerUp", menuName = "PowerUp/Pacifying Music")]
    public class PacifyingMusicPowerUpSO : PowerUpSO
    {
        [SerializeField] private SoundManagerSO soundManagerSO;
        [SerializeField] private AudioCueSO powerUpCue;
        [SerializeField] private GameManager gameManager;
        public override void Activate(GameObject target)
        {
            soundManagerSO.PlayDiegeticMusic(powerUpCue, duration, "Pacify" );
            gameManager.onPacifyingMusicStart?.Invoke();
        }

        public override void Deactivate(GameObject target)
        {
            gameManager.onPacifyingMusicEnd?.Invoke();
        }
    }
}
