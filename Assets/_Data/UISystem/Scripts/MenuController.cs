using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Data.UISystem.Scripts
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private GameObject inGameElements;
        [SerializeField] private GameObject gameCursor;
        private InputUtils.InputScheme currentInputScheme;

        void Update()
        {
            currentInputScheme = InputUtils.GetCurrentScheme();

            bool isGamepad = currentInputScheme is InputUtils.InputScheme.Xbox or InputUtils.InputScheme.PlayStation;
            bool shouldShowCursor = isGamepad && !inGameElements.activeSelf;

            if (gameCursor.activeSelf != shouldShowCursor)
                gameCursor.SetActive(shouldShowCursor);
        }
    }
}