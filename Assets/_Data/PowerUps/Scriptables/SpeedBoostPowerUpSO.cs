using UnityEngine;

namespace _Data.PowerUps.Scriptables
{
    [CreateAssetMenu(fileName = "SpeedBoostPowerUp", menuName = "PowerUp/Speed Boost")]
    public class SpeedBoostPowerUpSO : PowerUpSO
    {
        [Header("Speed Multiplier")]
        [SerializeField] private float speedMultiplier = 1.5f;

        public override void Activate(GameObject target, GameManager gameManager)
        {
            GetEffectiveDuration();
            if (target.TryGetComponent(out PlayerController.Scripts.PlayerController player))
                player.ModifySpeed(speedMultiplier);
        }

        public override void Deactivate(GameObject target, GameManager gameManager)
        {
            if (target.TryGetComponent(out PlayerController.Scripts.PlayerController player))
                player.ModifySpeed(1f);
        }
    }
}
