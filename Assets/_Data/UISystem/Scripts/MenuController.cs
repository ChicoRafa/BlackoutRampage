using UnityEngine;

namespace _Data.UISystem.Scripts
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private GameObject inGameElements;
        [SerializeField] private GameObject gameCursor;
        [SerializeField] private GameObject helpMenu;
        [SerializeField] private bool isMainMenu = false;
        private InputUtils.InputScheme currentInputScheme;

        private void FixedUpdate()
        {
            currentInputScheme = InputUtils.GetCurrentScheme();

            bool isGamepad = currentInputScheme is InputUtils.InputScheme.Xbox or InputUtils.InputScheme.PlayStation;
            bool shouldShowCursor = isMainMenu ? isGamepad : isGamepad && inGameElements && !inGameElements.activeSelf;

            if (gameCursor.activeSelf != shouldShowCursor)
                gameCursor.SetActive(shouldShowCursor);
            Cursor.visible = !shouldShowCursor;
            
            UpdateHelpMenuIcons();
        }

        private void UpdateHelpMenuIcons()
        {
            if (!helpMenu) return;

            foreach (Transform item in helpMenu.transform)
            {
                foreach (Transform child in item)
                {
                    string name = child.name;

                    if (!name.StartsWith("KBM_") && !name.StartsWith("Xbox_") && !name.StartsWith("PS_")) continue;
                    bool isKBM = name.StartsWith("KBM_");
                    bool isXbox = name.StartsWith("Xbox_");
                    bool isPS = name.StartsWith("PS_");

                    bool shouldBeActive =
                        (isKBM && currentInputScheme == InputUtils.InputScheme.KeyboardMouse) ||
                        (isXbox && currentInputScheme == InputUtils.InputScheme.Xbox) ||
                        (isPS && currentInputScheme == InputUtils.InputScheme.PlayStation);

                    child.gameObject.SetActive(shouldBeActive);
                }
            }
        }
    }
}
