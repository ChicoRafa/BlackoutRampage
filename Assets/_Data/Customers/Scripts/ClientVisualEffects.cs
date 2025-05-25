using System.Collections;
using UnityEngine;

namespace _Data.Customers.Scripts {
    public class ClientVisualEffects : MonoBehaviour
    {
        [Header("Icon Prefabs")]
        [SerializeField] private GameObject moneyBonusIcon;
        [SerializeField] private GameObject angryIcon;
        [SerializeField] private GameObject happyIcon;
        [SerializeField] private GameObject happinessBonusIcon;
        [SerializeField] private GameObject karenPenaltyIcon;
        [SerializeField] private GameObject policeCallIcon;
        [SerializeField] private GameObject patienceReducedIcon;

        [Header("Anchors")]
        [SerializeField] private Transform leavingAnchor;
        [SerializeField] private Transform effectAnchor;

        private bool leavingEffectActive = false;

        public void ShowMoneyBonus()
        {
            SpawnEffect(moneyBonusIcon, effectAnchor);
        }

        public void ShowAngryIcon()
        {
            SpawnEffect(angryIcon, leavingAnchor, true);
        }

        public void ShowHappyIcon()
        {
            SpawnEffect(happyIcon, leavingAnchor, true);
        }

        public void ShowPoliceHappinessBonus()
        {
            SpawnEffect(happinessBonusIcon, effectAnchor);
        }

        public void ShowPoliceCallAway()
        {
            SpawnEffect(policeCallIcon, effectAnchor);
        }

        public void ShowPatienceReduced()
        {
            if (leavingEffectActive) return;
            SpawnEffect(patienceReducedIcon, effectAnchor);
        }

        private void SpawnEffect(GameObject prefab, Transform anchor, bool markAsLeaving = false)
        {
            if (prefab == null || anchor == null) return;

            StartCoroutine(SpawnEffectWithLifecycle(prefab, anchor, markAsLeaving));
        }
        
        private IEnumerator SpawnEffectWithLifecycle(GameObject prefab, Transform anchor, bool markAsLeaving)
        {
            GameObject instance = Instantiate(prefab, anchor);
            FloatingEffect3D floating = instance.GetComponent<FloatingEffect3D>();

            if (markAsLeaving) leavingEffectActive = true;

            float duration = 1.5f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                if (floating != null)
                {
                    floating.progress = elapsed / duration;
                }
                yield return null;
            }

            if (markAsLeaving) leavingEffectActive = false;
            Destroy(instance);
        }
    }
}