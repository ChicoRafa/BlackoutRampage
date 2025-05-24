using UnityEngine;

namespace _Data.PowerUps.Scriptables
{
    [CreateAssetMenu(fileName = "PowerUp", menuName = "PowerUp")]
    public abstract class PowerUpSO : ScriptableObject
    {
        [Header("PowerUp Config")]
        public string powerUpName;
        public Sprite icon;
        public float duration = 5f;
        public float durationMultiplier = 1.5f;
        public GameObject pickupPrefab;
        public PerksSO perksData;

        public abstract void Activate(GameObject target);

        public abstract void Deactivate(GameObject target);
        public virtual float GetEffectiveDuration()
        {
            if (perksData != null && perksData.perkLongerPowerUps)
                return duration * durationMultiplier;
            return duration;
        }
    }
}
