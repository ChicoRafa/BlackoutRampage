using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Data.UISystem.Scripts
{
    public class CustomHoverHandler : MonoBehaviour, 
        IPointerEnterHandler, IPointerExitHandler, 
        ISelectHandler, IDeselectHandler
    {
        [SerializeField] private GameObject objectToActivateWhenHovered;
        [SerializeField] private bool debugMode = false;
        [SerializeField] private float hoverDelay = 0.05f; // Pequeño delay para evitar flickering
        
        private bool isHovered = false;
        private bool isSelected = false;
        private Coroutine hoverCoroutine;
        private Button associatedButton;

        private void Start()
        {
            // Obtener referencia al botón asociado si existe
            associatedButton = GetComponent<Button>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (debugMode) Debug.Log($"[CustomHoverHandler] OnPointerEnter: {gameObject.name}");
            
            SetHoverState(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (debugMode) Debug.Log($"[CustomHoverHandler] OnPointerExit: {gameObject.name}");
            
            SetHoverState(false);
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (debugMode) Debug.Log($"[CustomHoverHandler] OnSelect: {gameObject.name}");
            
            isSelected = true;
            SetHoverState(true);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (debugMode) Debug.Log($"[CustomHoverHandler] OnDeselect: {gameObject.name}");
            
            isSelected = false;
            SetHoverState(false);
        }

        private void SetHoverState(bool shouldHover)
        {
            // Cancelar corrutina anterior si existe
            if (hoverCoroutine != null)
            {
                StopCoroutine(hoverCoroutine);
                hoverCoroutine = null;
            }

            // Verificar si el botón está interactuable
            if (associatedButton && !associatedButton.interactable)
            {
                shouldHover = false;
            }

            // Solo cambiar si el estado es diferente
            if (isHovered != shouldHover)
            {
                isHovered = shouldHover;
                
                if (hoverDelay > 0)
                {
                    hoverCoroutine = StartCoroutine(DelayedHoverChange(shouldHover));
                }
                else
                {
                    ApplyHoverState(shouldHover);
                }
            }
        }

        private System.Collections.IEnumerator DelayedHoverChange(bool shouldHover)
        {
            yield return new WaitForSecondsRealtime(hoverDelay);
            
            // Verificar que el estado no haya cambiado durante el delay
            if (isHovered == shouldHover)
            {
                ApplyHoverState(shouldHover);
            }
        }

        private void ApplyHoverState(bool shouldHover)
        {
            if (!objectToActivateWhenHovered) return;
            objectToActivateWhenHovered.SetActive(shouldHover);
                
            if (debugMode) 
            {
                Debug.Log($"[CustomHoverHandler] Setting hover object {shouldHover} for {gameObject.name}");
            }
        }

        // Método público para forzar un estado específico
        public void ForceHoverState(bool state)
        {
            SetHoverState(state);
        }

        // Método para verificar si está en hover
        public bool IsHovered()
        {
            return isHovered;
        }

        // Método para verificar si está seleccionado
        public bool IsSelected()
        {
            return isSelected;
        }

        // Limpiar estado cuando se desactiva
        private void OnDisable()
        {
            if (hoverCoroutine != null)
            {
                StopCoroutine(hoverCoroutine);
                hoverCoroutine = null;
            }
            
            isHovered = false;
            isSelected = false;
            
            if (objectToActivateWhenHovered)
            {
                objectToActivateWhenHovered.SetActive(false);
            }
        }
    }
}