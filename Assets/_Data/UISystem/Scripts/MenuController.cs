using UnityEngine;

namespace _Data.UISystem.Scripts
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private GameObject inGameElements;
        [SerializeField] private GameObject gameCursor;
        [SerializeField] private GameObject helpMenu;
        [SerializeField] private bool isMainMenu = false;
        [SerializeField] private bool forceShowCursor = false; // for debugging purposes
        
        private InputUtils.InputScheme currentInputScheme;
        private InputUtils.InputScheme previousInputScheme;
        private GamepadCursorController cursorController;
        private bool wasGamepadActive = false;

        private void Start()
        {
            if (gameCursor) 
                cursorController = gameCursor.GetComponent<GamepadCursorController>();
            
            currentInputScheme = InputUtils.GetCurrentScheme();
            previousInputScheme = currentInputScheme;
        }

        private void Update()
        {
            currentInputScheme = InputUtils.GetCurrentScheme();
            if (currentInputScheme != previousInputScheme)
            {
                OnInputSchemeChanged();
                previousInputScheme = currentInputScheme;
            }

            UpdateCursorState();
            UpdateHelpMenuIcons();
        }

        private void OnInputSchemeChanged()
        {
            if (!cursorController || !IsGamepadActive()) return;
            cursorController.ResetCursorToCenter();
        }

        private void UpdateCursorState()
        {
            bool isGamepadActive = IsGamepadActive();
            bool shouldShowGamepadCursor = ShouldShowGamepadCursor(isGamepadActive);

            if (gameCursor && gameCursor.activeSelf != shouldShowGamepadCursor)
            {
                gameCursor.SetActive(shouldShowGamepadCursor);
                if (shouldShowGamepadCursor && cursorController)
                {
                    cursorController.ResetCursorToCenter();
                }
            }

            bool shouldShowSystemCursor = ShouldShowSystemCursor(isGamepadActive, shouldShowGamepadCursor);
            
            if (Cursor.visible != shouldShowSystemCursor)
            {
                Cursor.visible = shouldShowSystemCursor;
                Cursor.lockState = shouldShowSystemCursor ? CursorLockMode.None : CursorLockMode.Locked;
            }
            
            wasGamepadActive = isGamepadActive;
        }

        private bool IsGamepadActive()
        {
            return currentInputScheme is InputUtils.InputScheme.Xbox or InputUtils.InputScheme.PlayStation;
        }

        private bool ShouldShowGamepadCursor(bool isGamepadActive)
        {
            if (forceShowCursor) return true;
            if (!isGamepadActive) return false;
            if (isMainMenu) return true;
            if (!inGameElements) return true;
            
            return !inGameElements.activeSelf;
        }

        private bool ShouldShowSystemCursor(bool isGamepadActive, bool showingGamepadCursor)
        {
            if (showingGamepadCursor) return false;
            if (isGamepadActive) return false;
            if (!inGameElements) return true;
            
            return !inGameElements.activeSelf;

        }

        private void UpdateHelpMenuIcons()
        {
            if (!helpMenu) return;

            foreach (Transform item in helpMenu.transform)
            {
                foreach (Transform child in item)
                {
                    string name = child.name;

                    if (!name.StartsWith("KBM_") && !name.StartsWith("Xbox_") && !name.StartsWith("PS_")) 
                        continue;

                    bool isKBM = name.StartsWith("KBM_");
                    bool isXbox = name.StartsWith("Xbox_");
                    bool isPS = name.StartsWith("PS_");

                    bool shouldBeActive =
                        (isKBM && currentInputScheme == InputUtils.InputScheme.KeyboardMouse) ||
                        (isXbox && currentInputScheme == InputUtils.InputScheme.Xbox) ||
                        (isPS && currentInputScheme == InputUtils.InputScheme.PlayStation);

                    if (child.gameObject.activeSelf != shouldBeActive)
                    {
                        child.gameObject.SetActive(shouldBeActive);
                    }
                }
            }
        }

        // Debug tool
        // private void OnGUI()
        // {
        //     if (!Application.isEditor) return;
        //
        //     GUILayout.BeginArea(new Rect(10, 10, 300, 150));
        //     GUILayout.Label($"Input Scheme: {currentInputScheme}");
        //     GUILayout.Label($"Is Gamepad Active: {IsGamepadActive()}");
        //     GUILayout.Label($"Should Show Gamepad Cursor: {ShouldShowGamepadCursor(IsGamepadActive())}");
        //     GUILayout.Label($"System Cursor Visible: {Cursor.visible}");
        //     GUILayout.Label($"Gamepad Cursor Active: {gameCursor && gameCursor.activeSelf}");
        //     
        //     if (GUILayout.Button("Reset Cursor"))
        //     {
        //         if (cursorController)
        //             cursorController.ResetCursorToCenter();
        //     }
        //     
        //     GUILayout.EndArea();
        // }
    }
}