using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamepadCursorController : MonoBehaviour
{
    [SerializeField] private float cursorSpeed = 1000f; // velocidad del cursor en pantalla
    [SerializeField] private RectTransform cursorRectTransform;

    private Vector2 screenBounds;

    void Start()
    {
        if (cursorRectTransform == null)
            cursorRectTransform = GetComponent<RectTransform>();

        // Definimos límites según la resolución actual
        screenBounds = new Vector2(Screen.width, Screen.height);
        Cursor.visible = false; // ocultamos cursor del sistema
    }

    void Update()
    {
        // Lectura del dpad o joystick izquierdo (Eje Horizontal y Vertical del mando)
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        // Mover el cursor según input, frame-rate independiente
        Vector2 delta = new Vector2(x, y) * (cursorSpeed * Time.unscaledDeltaTime);

        Vector3 newPos = cursorRectTransform.position + new Vector3(delta.x, delta.y, 0);

        // Clamp para que no salga de pantalla
        newPos.x = Mathf.Clamp(newPos.x, 0, screenBounds.x);
        newPos.y = Mathf.Clamp(newPos.y, 0, screenBounds.y);

        cursorRectTransform.position = newPos;

        // Detectar botón de “click” (X en Xbox, A en Switch/PS)
        if (Input.GetButtonDown("Submit"))
        {
            // Raycast UI para detectar botón debajo del cursor
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = newPos;

            var raycastResults = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);

            foreach (var result in raycastResults)
            {
                var button = result.gameObject.GetComponent<Button>();
                if (!button || !button.interactable) continue;
                button.onClick.Invoke();
                break;
            }
        }
    }
}
