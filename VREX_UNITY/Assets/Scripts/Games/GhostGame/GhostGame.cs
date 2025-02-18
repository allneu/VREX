using System.Collections;
using Controllers;
using DataCollectors;
using Excavator;
using UnityEngine;
using UnityEngine.Events;

namespace Games.GhostGame
{
    public class GhostGame : Game
    {
        public ExcavatorGhostController excavatorGhostController;
        private ExcavatorComparator excavatorComparator;

        private bool previousBoomSuccess;
        private bool previousBucketSuccess;
        private bool previousExcavatorSuccess;
        private bool previousStickSuccess;
        private bool previousSwingSuccess;

        private void Start()
        {
            ValidateComponents();
            InitializeGame();
        }

        private void Update()
        {
            if (!isGameOn) return;

            if (excavatorComparator == null)
            {
                Debug.LogError("ExcavatorComparator is not initialized.");
                return;
            }

            if (CheckGhostSuccess() && !physicsExcavator.isControlled) FinishRound();
        }

        public void StartGame(PlayerData playerData, UnityAction onGameFinished, ControllerType controller)
        {
            if (excavatorGhostController == null || physicsExcavator == null)
            {
                Debug.LogError("Game components are not properly set. Ensure all required components are assigned.");
                return;
            }

            this.controller = controller;

            excavatorGhostController.gameObject.SetActive(true);
            excavatorComparator = new ExcavatorComparator(physicsExcavator.GetExcavatorObj(),
                excavatorGhostController.ikExcavator.GetExcavatorObj());
            excavatorGhostController.SetGhostActive(true);

            base.StartGame(GameType.GhostGame,
                "Position the excavator to the ghost position as fast as you can using the presented controller.",
                StartRoundZero,
                playerData, LOGGameProgress, onGameFinished);
        }

        private void StartRoundZero()
        {
            StartRound();
            ShowUI("Trial Round", "Try to match the ghost using the " + controller + " controller.", null);
        }

        private void StartRound()
        {
            if (round < excavatorGhostController.targetObjects.Length)
                StartCoroutine(StartRoundCoroutine());
            else
                FinishGame();
        }

        private IEnumerator StartRoundCoroutine()
        {
            excavatorGhostController.SetNextTarget(round);
            base.StartRound(LOGExcavatorPositions);
            yield return null;
            excavatorComparator.StartRoundRotations();
        }

        public void FinishRound()
        {
            base.FinishRound("", LOGExcavatorPositions);
            var text = "You have successfully matched the ghost. Click to continue.";
            if (round == 1 && controller == ControllerType.Joystick)
                text = "You have completed the trial round. Click continue to start ROUND 1.";
            ShowUI("Congratulations!", text, round == 0 ? StartRoundZero : StartRound);
        }

        private new void FinishGame()
        {
            base.FinishGame();
            excavatorGhostController.SetGhostActive(false);
        }

        private void ValidateComponents()
        {
            if (excavatorGhostController == null) Debug.LogError("ExcavatorGhost is not set in the GhostGame.");

            if (physicsExcavator == null) Debug.LogError("PhysicsExcavator is not set in the GhostGame.");
        }

        private void InitializeGame()
        {
            if (excavatorGhostController != null) excavatorGhostController.SetGhostActive(false);
            previousBoomSuccess = false;
            previousStickSuccess = false;
            previousBucketSuccess = false;
            previousSwingSuccess = false;
            previousExcavatorSuccess = false;
        }

        private bool CheckGhostSuccess()
        {
            var boomSuccess = excavatorComparator.AreBoomRotationsEqual();
            if (boomSuccess != previousBoomSuccess)
            {
                GameLogger.LogConditional("BOOM Rotation " + (boomSuccess ? "MATCHED" : "FAILED"));
                GameLogger.LogConditional(LOGGameProgress());
                excavatorGhostController.SetSuccessfulGhostObj(
                    excavatorGhostController.ikExcavator.GetExcavatorObj().Boom, boomSuccess);
                previousBoomSuccess = boomSuccess;
            }

            var stickSuccess = excavatorComparator.AreStickRotationsEqual();
            if (stickSuccess != previousStickSuccess)
            {
                GameLogger.LogConditional("STICK Rotation " + (stickSuccess ? "MATCHED" : "FAILED"));
                GameLogger.LogConditional(LOGGameProgress());
                excavatorGhostController.SetSuccessfulGhostObj(
                    excavatorGhostController.ikExcavator.GetExcavatorObj().Stick, stickSuccess);
                previousStickSuccess = stickSuccess;
            }

            var bucketSuccess = excavatorComparator.AreBucketRotationsEqual();
            if (bucketSuccess != previousBucketSuccess)
            {
                GameLogger.LogConditional("BUCKET Rotation " + (bucketSuccess ? "MATCHED" : "FAILED"));
                GameLogger.LogConditional(LOGGameProgress());
                excavatorGhostController.SetSuccessfulGhostObj(
                    excavatorGhostController.ikExcavator.GetExcavatorObj().Bucket, bucketSuccess);
                excavatorGhostController.SetSuccessfulGhostObj(
                    excavatorGhostController.ikExcavator.GetExcavatorObj().LinkageTopAxis.gameObject, bucketSuccess);
                previousBucketSuccess = bucketSuccess;
            }

            var swingSuccess = excavatorComparator.AreSwingRotationsEqual();
            if (swingSuccess != previousSwingSuccess)
            {
                GameLogger.LogConditional("SWING Rotation " + (swingSuccess ? "MATCHED" : "FAILED"));
                GameLogger.LogConditional(LOGGameProgress());
                excavatorGhostController.SetSuccessfulGhostObj(
                    excavatorGhostController.ikExcavator.GetExcavatorObj().ExBase, swingSuccess);
                previousSwingSuccess = swingSuccess;
            }

            var excavatorSuccess = boomSuccess && stickSuccess && bucketSuccess && swingSuccess;
            if (excavatorSuccess != previousExcavatorSuccess)
            {
                GameLogger.LogConditional("EXCAVATOR RotationS " + (excavatorSuccess ? "MATCHED" : "FAILED"));
                GameLogger.LogConditional(LOGGameProgress());
                excavatorGhostController.SetSuccessfulGhost(excavatorSuccess);
                previousExcavatorSuccess = excavatorSuccess;
            }

            return excavatorSuccess;
        }

        private void LOGExcavatorPositions()
        {
            GameLogger.LogAll($"[Physics] Boom: {physicsExcavator.GetExcavatorObj().BoomAxis.rotation}, " +
                              $"Stick: {physicsExcavator.GetExcavatorObj().StickAxis.rotation}, " +
                              $"Bucket: {physicsExcavator.GetExcavatorObj().BucketAxis.rotation}, " +
                              $"Swing: {physicsExcavator.GetExcavatorObj().SwingAxis.rotation.y}\n" +
                              $"[Ghost] Boom: {excavatorGhostController.ikExcavator.GetExcavatorObj().BoomAxis.rotation}, " +
                              $"Stick: {excavatorGhostController.ikExcavator.GetExcavatorObj().StickAxis.rotation}, " +
                              $"Bucket: {excavatorGhostController.ikExcavator.GetExcavatorObj().BucketAxis.rotation}, " +
                              $"Swing: {excavatorGhostController.ikExcavator.GetExcavatorObj().SwingAxis.rotation}");
        }

        private string LOGGameProgress()
        {
            if (excavatorComparator == null) return "ExcavatorComparator is not initialized.";

            return "Ghost Game Info:\n" +
                   $"Boom: Diff={excavatorComparator.GetBoomRotationDifference()}, " +
                   $"Prog={excavatorComparator.GetBoomRotationProgressPercentage()}%, " +
                   $"Success={excavatorComparator.AreBoomRotationsEqual()}\n" +
                   $"Stick: Diff={excavatorComparator.GetStickRotationDifference()}, " +
                   $"Prog={excavatorComparator.GetStickRotationProgressPercentage()}%, " +
                   $"Success={excavatorComparator.AreStickRotationsEqual()}\n" +
                   $"Bucket: Diff={excavatorComparator.GetBucketRotationDifference()}, " +
                   $"Prog={excavatorComparator.GetBucketRotationProgressPercentage()}%, " +
                   $"Success={excavatorComparator.AreBucketRotationsEqual()}\n" +
                   $"Swing: Diff={excavatorComparator.GetSwingRotationDifference()}, " +
                   $"Prog={excavatorComparator.GetSwingRotationProgressPercentage()}%, " +
                   $"Success={excavatorComparator.AreSwingRotationsEqual()}";
        }
    }
}