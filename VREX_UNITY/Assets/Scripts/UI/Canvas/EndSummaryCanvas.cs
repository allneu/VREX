using TMPro;
using UnityEngine;

namespace UI
{
    public class EndSummaryCanvas : MonoBehaviour
    {
        public GameObject panel;

        private void Start()
        {
            if (panel == null) Debug.LogError("GameRoundPanel is not set in the GhostGameCanvas.");
        }

        public void Show(PlayerData playerData)
        {
            gameObject.SetActive(true);

            panel.SetActive(true);
            var titleText = panel.transform.Find("Title").GetComponent<TextMeshProUGUI>();
            var introText = panel.transform.Find("Intro").GetComponent<TextMeshProUGUI>();

            if (titleText != null)
                titleText.text = "THE END";
            else
                Debug.LogError("Title Text component not found.");

            if (introText != null)
                introText.text = "Thank you " + playerData.username + " for taking part in the game!";
            else
                Debug.LogError("Intro Text component not found.");
        }
    }
}