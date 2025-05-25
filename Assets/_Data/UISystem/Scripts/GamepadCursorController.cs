using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Data.UISystem.Scripts
{
    public class GamepadCursorController : MonoBehaviour
    {
        [Header("Cursor")]
        [SerializeField] private float cursorSpeed = 1000f;
        [SerializeField] private RectTransform cursorRectTransform;

        private Vector2 screenBounds;
        private CustomHoverHandler currentHoverHandler = null;

        private void Start()
        {
            if (!cursorRectTransform)
                cursorRectTransform = GetComponent<RectTransform>();

            screenBounds = new Vector2(Screen.width, Screen.height);
        }

        private void FixedUpdate()
        {
            // Solo mover el cursor si está activo
            if (!cursorRectTransform.gameObject.activeSelf)
                return;

            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");

            Vector2 delta = new Vector2(x, y) * (cursorSpeed * Time.unscaledDeltaTime);
            Vector3 newPos = cursorRectTransform.position + new Vector3(delta.x, delta.y, 0);

            newPos.x = Mathf.Clamp(newPos.x, 0, screenBounds.x);
            newPos.y = Mathf.Clamp(newPos.y, 0, screenBounds.y);

            cursorRectTransform.position = newPos;

            // Raycast UI
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = newPos;

            var raycastResults = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);

            CustomHoverHandler foundHoverHandler = null;
            foreach (var result in raycastResults)
            {
                foundHoverHandler = result.gameObject.GetComponent<CustomHoverHandler>();
                if (foundHoverHandler) break;
            }

            if (!foundHoverHandler && currentHoverHandler)
            {
                currentHoverHandler.OnPointerExit(null);
                currentHoverHandler = null;
            }
            else if (foundHoverHandler && foundHoverHandler != currentHoverHandler)
            {
                if (currentHoverHandler) currentHoverHandler.OnPointerExit(null);
                foundHoverHandler.OnPointerEnter(null);
                currentHoverHandler = foundHoverHandler;
            }

            if (!Input.GetButtonDown("Submit")) return;
            {
                foreach (var result in raycastResults)
                {
                    var button = result.gameObject.GetComponent<Button>();
                    if (!button || !button.interactable) continue;

                    button.onClick.Invoke();
                    break;
                }

                if (!currentHoverHandler) return;
                currentHoverHandler.OnPointerExit(null);
                currentHoverHandler = null;
            }
        }
    }
}