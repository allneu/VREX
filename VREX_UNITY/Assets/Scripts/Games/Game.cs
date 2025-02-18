using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using Controllers.ExcavatorIKController;
using DataCollectors;
using Excavator.Movement;
using UI;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;

namespace Games
{
    public class Game : MonoBehaviour
    {
        public GameCanvas gameCanvas;
        public JoystickControllers joystickControllers;
        public ExcavatorIKController miniatureController;
        public PhysicsExcavator physicsExcavator;
        [SerializeField] public List<Transform> anchors;
        public XROrigin XROrigin;

        [HideInInspector] public int round;
        [HideInInspector] public ControllerType controller;
        [HideInInspector] public bool isGameOn;
        private GameType gameType;
        private Func<string> LOGGameProgress;
        private Coroutine loggingCoroutine;
        private UnityAction onGameFinished;

        private PlayerData playerData;
        private DateTime roundStartTime;

        private TeleportToAnchor teleportToAnchor;

        private void Awake()
        {
            if (gameCanvas == null)
                Debug.LogError(
                    "GameCanvas is not set in the Game. Please assign a GameCanvas object in the inspector.");
            if (joystickControllers == null)
                Debug.LogError(
                    "JoystickController is not set in the Game. Please assign a JoystickController object in the inspector.");
            if (miniatureController == null)
                Debug.LogError(
                    "MiniatureController is not set in the Game. Please assign a MiniatureController object in the inspector.");
            if (physicsExcavator == null)
                Debug.LogError(
                    "PhysicsExcavator is not set in the Game. Please assign a PhysicsExcavator object in the inspector.");
            if (anchors == null || anchors.Count == 0)
                Debug.LogError("Anchors are not set in the Game. Please assign a list of anchors in the inspector.");
            if (XROrigin == null)
                Debug.LogError("XROrigin is not set in the Game. Please assign an XROrigin object in the inspector.");

            teleportToAnchor = new TeleportToAnchor(XROrigin);

            foreach (var anchor in anchors) anchor.gameObject.SetActive(false);
        }

        protected void StartGame(GameType gameType, string text, UnityAction onClickAction, PlayerData playerData,
            Func<string> LOGGameProgress, UnityAction onGameFinished)
        {
            if (gameCanvas == null || joystickControllers == null || miniatureController == null)
            {
                Debug.LogError("Game components are not properly set. Ensure all required components are assigned.");
                return;
            }

            round = 0;
            if (playerData != null) this.playerData = playerData;
            this.onGameFinished = onGameFinished;
            this.gameType = gameType;
            this.LOGGameProgress = LOGGameProgress;

            joystickControllers.gameObject.SetActive(false);
            miniatureController.Hide();
            physicsExcavator.ResetInitialPosition();

            gameCanvas.Show(gameType == GameType.BucketGame ? "BUCKET GAME" : "GHOST GAME", text,
                () => { onClickAction(); });

            GameLogger.LogAll((gameType == GameType.BucketGame ? "BUCKET GAME" : "GHOST GAME") + " STARTED.");
        }

        protected void ShowUI(string title, string text, UnityAction onClickAction)
        {
            if (gameCanvas == null)
            {
                Debug.LogError(
                    "GameCanvas is not set in the Game. Please assign a GameCanvas object in the inspector.");
                return;
            }

            gameCanvas.Show(title, text, onClickAction);
        }

        protected void ManageControllers()
        {
            if (joystickControllers == null || miniatureController == null)
            {
                Debug.LogError(
                    "Controller components are not properly set. Ensure all required components are assigned.");
                return;
            }

            switch (controller)
            {
                case ControllerType.Joystick:
                    joystickControllers.gameObject.SetActive(true);
                    joystickControllers.ActivateXRGrab(true);
                    miniatureController.Hide();
                    break;
                case ControllerType.Miniature:
                    joystickControllers.gameObject.SetActive(false);
                    miniatureController.Show();
                    miniatureController.activateXRGrab(true);
                    break;
                default:
                    Debug.LogError("Invalid controller type.");
                    break;
            }
        }

        protected void StartRound(Action logStartPositions)
        {
            if (gameCanvas == null)
            {
                Debug.LogError(
                    "GameCanvas is not set in the Game. Please assign a GameCanvas object in the inspector.");
                return;
            }

            if (round > 0) gameCanvas.Hide();

            teleportToAnchor.Teleport(anchors, round);
            physicsExcavator.GetExcavatorObj().ResetInitialPosition();
            ManageControllers();
            isGameOn = true;

            roundStartTime = DateTime.Now;

            GameLogger.LogAll((gameType == GameType.BucketGame ? "BUCKET" : "GHOST") + " ROUND " + round + " " +
                              controller.ToString().ToUpper() + " STARTED");
            logStartPositions();
            loggingCoroutine = StartCoroutine(LogInfoEverySecond());
        }

        protected void FinishRound(string bucketFailures, Action logPositions)
        {
            isGameOn = false;

            joystickControllers.ActivateXRGrab(false);
            miniatureController.activateXRGrab(false);

            var roundEndTime = DateTime.Now;
            var roundDuration = (roundEndTime - roundStartTime).TotalSeconds;

            var logText = (gameType == GameType.BucketGame ? "BUCKET" : "GHOST") + " ROUND " + round + " " +
                          controller.ToString().ToUpper() + " FINISHED IN " + roundDuration + " SEC" + bucketFailures +
                          ".";
            GameLogger.LogAll(logText);

            round++;

            if (loggingCoroutine != null)
            {
                StopCoroutine(loggingCoroutine);
                loggingCoroutine = null;
            }

            logPositions();
        }

        protected void FinishGame()
        {
            joystickControllers.gameObject.SetActive(false);
            miniatureController.Hide();

            GameLogger.LogAll((gameType == GameType.BucketGame ? "BUCKET GAME" : "GHOST GAME") + " FINISHED");
            ShowUI("Congratulations!", "You have completed the game.", onGameFinished);
        }

        private IEnumerator LogInfoEverySecond()
        {
            while (true)
            {
                GameLogger.LogTimestamp(LOGGameProgress());
                yield return new WaitForSeconds(1);
            }
        }
    }
}