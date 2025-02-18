using Controllers.ExcavatorIKController;
using UnityEngine;

namespace UI
{
    public class TutorialCanvas : MonoBehaviour
    {
        public GameObject joystickPanel;
        public GameObject miniaturePanel;

        public GameObject joystickController;
        public ExcavatorIKController miniatureController;

        public GameObject joystickTutorialButtonPanel;
        public GameObject miniatureTutorialButtonPanel;

        private void Start()
        {
            if (joystickPanel == null) Debug.LogError("JoystickPanel is not set in TutorialCanvas");

            if (miniaturePanel == null) Debug.LogError("MiniaturePanel is not set in TutorialCanvas");

            if (joystickController == null) Debug.LogError("JoystickController is not set in TutorialCanvas");

            if (miniatureController == null) Debug.LogError("MiniatureController is not set in TutorialCanvas");

            if (joystickTutorialButtonPanel == null)
                Debug.LogError("JoystickTutorialButtonPanel is not set in TutorialCanvas");

            if (miniatureTutorialButtonPanel == null)
                Debug.LogError("MiniatureTutorialButtonPanel is not set in TutorialCanvas");
        }

        public void Show()
        {
            gameObject.SetActive(true);
            StartJoystickTutorial();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void StartJoystickTutorial()
        {
            miniaturePanel.SetActive(false);
            miniatureController.Hide();
            miniatureTutorialButtonPanel.SetActive(false);

            joystickPanel.SetActive(true);
            joystickController.SetActive(true);
            joystickTutorialButtonPanel.SetActive(true);
        }

        public void HideJoystickTutorialInfo()
        {
            joystickPanel.SetActive(false);
        }

        public void StartMiniatureTutorial()
        {
            joystickController.SetActive(false);
            joystickPanel.SetActive(false);
            joystickTutorialButtonPanel.SetActive(false);

            miniaturePanel.SetActive(true);
            miniatureController.Show();
            miniatureTutorialButtonPanel.SetActive(true);
        }

        public void HideMiniatureTutorialInfo()
        {
            miniaturePanel.SetActive(false);
        }
    }
}