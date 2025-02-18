using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class GameCanvas : MonoBehaviour
    {
        public GameObject panel;
        public GameObject continueButtonPanel;

        private void Start()
        {
            if (panel == null) Debug.LogError("GameRoundPanel is not set in the GhostGameCanvas.");
            if (continueButtonPanel == null) Debug.LogError("ContinueButtonPanel is not set in the GhostGameCanvas.");
        }

        public void Show(string gameTitle, string text, UnityAction onClickAction)
        {
            gameObject.SetActive(true);

            panel.SetActive(true);
            var titleText = panel.transform.Find("Title").GetComponent<TextMeshProUGUI>();
            var introText = panel.transform.Find("Intro").GetComponent<TextMeshProUGUI>();

            if (titleText != null)
                titleText.text = gameTitle;
            else
                Debug.LogError("Title Text component not found.");

            if (introText != null)
                introText.text = text;
            else
                Debug.LogError("Intro Text component not found.");

            continueButtonPanel.SetActive(onClickAction != null);
            var button = continueButtonPanel.transform.Find("Button").GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(onClickAction);
        }

        public void Hide()
        {
            panel.SetActive(false);
            continueButtonPanel.SetActive(false);
        }
    }
}