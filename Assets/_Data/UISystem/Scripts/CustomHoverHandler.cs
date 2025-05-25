using UnityEngine;
using UnityEngine.EventSystems;

namespace _Data.UISystem.Scripts
{
    public class CustomHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
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
        
        public void OnSelect(BaseEventData eventData)
        {
            objectToActivateWhenHovered.SetActive(true);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            objectToActivateWhenHovered.SetActive(false);
        }
    }
}
