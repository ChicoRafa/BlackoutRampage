using UnityEngine;

namespace _Data.PowerUps.Scripts
{
    public class TimedDestroyer : MonoBehaviour
    {
        public void DestroyAfter(float seconds)
        {
            Invoke(nameof(DestroySelf), seconds);
        }

        private void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}
