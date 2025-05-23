using UnityEngine;

namespace _Data.PowerUps.Scriptables
{
    [CreateAssetMenu(fileName = "PacifyingMusicPowerUp", menuName = "PowerUp/Pacifying Music")]
    public class PacifyingMusicPowerUpSO : PowerUpSO
    {
        [SerializeField] private SoundManagerSO soundManagerSO;
        [SerializeField] private AudioCueSO powerUpCue;
        [SerializeField] private float patienceMultiplier = 0.5f;
        public override void Activate(GameObject target, GameManager gameManager)
        {
            soundManagerSO.PlayDiegeticMusic(powerUpCue, duration, "Pacify" );
            gameManager.ChangePatienceLevelMultiplier(patienceMultiplier);
        }

        public override void Deactivate(GameObject target, GameManager gameManager)
        {
            gameManager.RestorePatienceLevelMultiplier();
        }
    }
}
