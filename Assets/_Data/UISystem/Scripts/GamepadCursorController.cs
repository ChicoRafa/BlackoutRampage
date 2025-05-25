using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamepadCursorController : MonoBehaviour
{
    [Header("Cursor")]
    [SerializeField] private float cursorSpeed = 1000f;
    [SerializeField] private RectTransform cursorRectTransform;

    private Vector2 screenBounds;

    private void Start()
    {
        if (cursorRectTransform == null)
            cursorRectTransform = GetComponent<RectTransform>();

        screenBounds = new Vector2(Screen.width, Screen.height);
        Cursor.visible = false;
    }

    private void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector2 delta = new Vector2(x, y) * (cursorSpeed * Time.unscaledDeltaTime);
        Vector3 newPos = cursorRectTransform.position + new Vector3(delta.x, delta.y, 0);

        newPos.x = Mathf.Clamp(newPos.x, 0, screenBounds.x);
        newPos.y = Mathf.Clamp(newPos.y, 0, screenBounds.y);

        cursorRectTransform.position = newPos;

        if (Input.GetButtonDown("Submit"))
        {
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
