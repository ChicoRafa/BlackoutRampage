using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

namespace _Data.UISystem.Scripts
{
    public class GamepadCursorController : MonoBehaviour
    {
        [Header("Cursor Settings")]
        [SerializeField] private float cursorSpeed = 1000f;
        [SerializeField] private RectTransform cursorRectTransform;
        [SerializeField] private float clickCooldown = 0.1f;
        
        private Vector2 screenBounds;
        private CustomHoverHandler currentHoverHandler = null;
        private Button currentButton = null;
        private float lastClickTime = 0f;
        private bool isInitialized = false;
        private Canvas currentCanvas;

        private void Start()
        {
            InitializeCursor();
        }

        private void InitializeCursor()
        {
            if (!cursorRectTransform)
                cursorRectTransform = GetComponent<RectTransform>();

            screenBounds = new Vector2(Screen.width, Screen.height);
            
            currentCanvas = GetComponentInParent<Canvas>();
            if (!currentCanvas)
                currentCanvas = FindFirstObjectByType<Canvas>();

            if (cursorRectTransform)
            {
                cursorRectTransform.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
            }

            isInitialized = true;
        }

        private void Update()
        {
            if (!isInitialized || !cursorRectTransform.gameObject.activeSelf)
                return;

            UpdateCursorPosition();
            HandleInput();
        }

        private void UpdateCursorPosition()
        {
            // Get Gamepad input
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");

            // No input, return
            if (Mathf.Abs(x) < 0.1f && Mathf.Abs(y) < 0.1f)
                return;

            // Calc position
            Vector2 delta = new Vector2(x, y) * (cursorSpeed * Time.unscaledDeltaTime);
            Vector3 newPos = cursorRectTransform.position + new Vector3(delta.x, delta.y, 0);

            // Apply screen margins
            float margin = 10f;
            newPos.x = Mathf.Clamp(newPos.x, margin, screenBounds.x - margin);
            newPos.y = Mathf.Clamp(newPos.y, margin, screenBounds.y - margin);

            cursorRectTransform.position = newPos;

            UpdateHoverDetection();
        }

        private void UpdateHoverDetection()
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = cursorRectTransform.position
            };

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, raycastResults);

            CustomHoverHandler foundHoverHandler = null;
            Button foundButton = null;

            foreach (var result in raycastResults)
            {
                if (!foundHoverHandler)
                    foundHoverHandler = result.gameObject.GetComponent<CustomHoverHandler>();
                
                if (!foundButton)
                {
                    foundButton = result.gameObject.GetComponent<Button>();
                    if (foundButton && !foundButton.interactable)
                        foundButton = null;
                }

                if (foundHoverHandler && foundButton)
                    break;
            }
            HandleHoverChange(foundHoverHandler);
            HandleButtonChange(foundButton);
        }

        private void HandleHoverChange(CustomHoverHandler newHoverHandler)
        {
            if (currentHoverHandler == newHoverHandler)
                return;

            if (currentHoverHandler)
            {
                currentHoverHandler.OnPointerExit(null);
            }

            currentHoverHandler = newHoverHandler;
            if (!currentHoverHandler) return;
            currentHoverHandler.OnPointerEnter(null);
        }

        private void HandleButtonChange(Button newButton)
        {
            if (currentButton == newButton)
                return;

            // Deseleccionar botón anterior
            if (currentButton)
            {
                var selectable = currentButton.GetComponent<Selectable>();
                if (selectable)
                {
                    selectable.OnDeselect(null);
                }
            }

            // Seleccionar nuevo botón
            currentButton = newButton;
            if (!currentButton) return;
            {
                var selectable = currentButton.GetComponent<Selectable>();
                if (selectable)
                {
                    selectable.OnSelect(null);
                }
            }
        }

        private void HandleInput()
        {
            if (Input.GetButtonDown("Submit"))
            {
                HandleClick();
            }
        }

        private void HandleClick()
        {
            if (Time.unscaledTime - lastClickTime < clickCooldown)
                return;

            lastClickTime = Time.unscaledTime;

            if (currentButton && currentButton.interactable)
            {
                currentButton.onClick.Invoke();
                
                if (!currentHoverHandler) return;
                currentHoverHandler.OnPointerExit(null);
                currentHoverHandler = null;
                return;
            }

            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = cursorRectTransform.position
            };

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, raycastResults);

            foreach (var result in raycastResults)
            {
                var button = result.gameObject.GetComponent<Button>();
                if (button && button.interactable)
                {
                    button.onClick.Invoke();
                    break;
                }
            }
        }

        public void SetCursorPosition(Vector2 position)
        {
            if (!cursorRectTransform) return;
            cursorRectTransform.position = position;
            UpdateHoverDetection();
        }

        public void ResetCursorToCenter()
        {
            SetCursorPosition(new Vector2(Screen.width / 2f, Screen.height / 2f));
        }

        public void SetCursorSpeed(float newSpeed)
        {
            cursorSpeed = newSpeed;
        }

        private void OnDisable()
        {
            if (currentHoverHandler)
            {
                currentHoverHandler.OnPointerExit(null);
                currentHoverHandler = null;
            }

            if (!currentButton) return;
            var selectable = currentButton.GetComponent<Selectable>();
            if (selectable)
            {
                selectable.OnDeselect(null);
            }
            currentButton = null;
        }
    }
}