using DataCollectors;
using Games;
using Games.GhostGame;
using Games.SandGame;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public UsernameCanvas usernameCanvas;
        public TutorialCanvas tutorialCanvas;

        public GameCanvas gameCanvas;
        public GhostGame ghostGame;
        public BucketGame bucketGame;

        public EndSummaryCanvas endSummaryCanvas;

        public GameObject controllers;

        private PlayerData _playerData;
        public TeleportToAnchor teleportToAnchor;

        private void Start()
        {
            if (usernameCanvas == null) Debug.LogError("UsernameCanvas is not set in the UIManager.");
            if (tutorialCanvas == null) Debug.LogError("TutorialCanvas is not set in the UIManager.");
            if (gameCanvas == null) Debug.LogError("GameCanvas is not set in the UIManager.");
            if (ghostGame == null) Debug.LogError("GhostGame is not set in the UIManager.");
            if (bucketGame == null) Debug.LogError("BucketGame is not set in the UIManager.");
            if (endSummaryCanvas == null) Debug.LogError("EndSummaryCanvas is not set in the UIManager.");
            if (controllers == null) Debug.LogError("Controllers is not set in the UIManager.");

            HideAllCanvas();
            ShowUsernameCanvas();
        }

        public void ShowUsernameCanvas()
        {
            HideAllCanvas();
            usernameCanvas.Show();
            controllers.SetActive(false);
        }

        public void ShowTutorialCanvas()
        {
            PlayerData.instance.Initialize(usernameCanvas.GetUsername());
            GameLogger.instance.Initialize();
            HideAllCanvas();
            tutorialCanvas.Show();
            controllers.SetActive(true);
        }

        public void StartGhostGameJoysticks()
        {
            HideAllCanvas();
            ghostGame.StartGame(_playerData, StartBucketGameJoysticks, ControllerType.Joystick);
        }

        public void StartBucketGameJoysticks()
        {
            HideAllCanvas();
            bucketGame.StartGame(_playerData, StartGhostGameMiniature, ControllerType.Joystick);
        }

        public void StartGhostGameMiniature()
        {
            HideAllCanvas();
            ghostGame.StartGame(_playerData, StartBucketGameMiniature, ControllerType.Miniature);
        }

        public void StartBucketGameMiniature()
        {
            HideAllCanvas();
            bucketGame.StartGame(_playerData, ShowEndSummaryCanvas, ControllerType.Miniature);
        }

        public void ShowEndSummaryCanvas()
        {
            HideAllCanvas();
            endSummaryCanvas.Show(_playerData);
            GameLogger.Close();
        }

        public void CloseFiles()
        {
            HideAllCanvas();
            GameLogger.Close();
        }

        public void HideAllCanvas()
        {
            usernameCanvas.Hide();
            tutorialCanvas.Hide();
            gameCanvas.gameObject.SetActive(false);
            endSummaryCanvas.gameObject.SetActive(false);
        }
    }
}