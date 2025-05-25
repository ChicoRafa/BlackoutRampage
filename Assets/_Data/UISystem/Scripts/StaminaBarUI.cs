using UnityEngine;
using UnityEngine.UI;

public class StaminaBarUI : MonoBehaviour
{
    [Header("Stamina")]
    [SerializeField] private Image staminaBar;

    internal void UpdateStamina(float staminaPercentage)
    {
        staminaPercentage = Mathf.Clamp01(staminaPercentage);
        staminaBar.rectTransform.localScale = new Vector3(staminaPercentage, 1f, 1f);
    }
}
