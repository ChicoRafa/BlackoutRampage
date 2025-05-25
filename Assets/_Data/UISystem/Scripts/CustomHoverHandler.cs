using UnityEngine;
using UnityEngine.EventSystems;

public class CustomHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject objectToActivateWhenHovered;

    public void OnPointerEnter(PointerEventData eventData)
    {
        objectToActivateWhenHovered.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        objectToActivateWhenHovered.SetActive(false);
    }
}