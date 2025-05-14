using UnityEngine;

namespace _Data.PowerUps.Scriptables
{
    [CreateAssetMenu(fileName = "PacifyingMusicPowerUp", menuName = "PowerUp/Pacifying Music")]
    public class PacifyingMusicPowerUpSO : PowerUpSO
    {
        [SerializeField] private SoundManagerSO soundManagerSO;
        [SerializeField] private AudioCueSO powerUpCue;
        public override void Activate(GameObject target)
        {
            soundManagerSO.PlayDiegeticMusic(powerUpCue, duration, "Pacify" );
            //handle patience bar
        }

        public override void Deactivate(GameObject target)
        {
           //not needed until we integrate with the patience bar
        }
    }
}
