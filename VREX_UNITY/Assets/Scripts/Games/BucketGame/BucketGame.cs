using DataCollectors;
using UnityEngine;
using UnityEngine.Events;

namespace Games.SandGame
{
    public class BucketGame : Game
    {
        public BucketFill bucketFill;
        public SandController sandController;

        public int failedRound;

        private SandPositionComparator sandPositionComparator;

        private void Start()
        {
            ValidateComponents();
            InitializeGame();
        }

        private void Update()
        {
            var bucketRotation = physicsExcavator.GetExcavatorObj().BucketAxis.rotation;
            var angle = Vector3.Angle(bucketRotation * Vector3.up, Vector3.down);
            
            if (bucketFill.isBucketFilled && angle > GameConstants.BucketGame.EmptyFillBucketAngle)
            {
                if (!IsAboveSandPile())
                {
                    if (isGameOn)
                        DropSand();
                    else
                        bucketFill.EmptyBucket();
                }
                else
                {
                    bucketFill.isBucketFilled = false;
                }
            }
        }

        private bool IsAboveSandPile()
        {
            RaycastHit hit;
            if (Physics.Raycast(bucketFill.transform.position, Vector3.down, out hit))
                if (hit.collider.CompareTag("SandPile"))
                    return true;
            return false;
        }

        public void StartGame(PlayerData playerData, UnityAction onGameFinished, ControllerType controller)
        {
            if (sandController == null || bucketFill == null || physicsExcavator == null)
            {
                Debug.LogError("Game components are not properly set. Ensure all required components are assigned.");
                return;
            }

            sandController.Show();
            bucketFill.InitBucketFill();
            sandController.SetNextTarget(0);
            this.controller = controller;

            base.StartGame(GameType.BucketGame,
                "Your mission is to scoop up the stone and drop it precisely at the highlighted spot.",
                StartRoundZero, playerData, LOGGameProgress, onGameFinished);
        }

        private void StartRoundZero()
        {
            StartRound();
            ShowUI("Trial Round",
                "Try to drop the stone at the target location using the " +
                controller + " controller.", null);
        }

        private void StartRound()
        {
            if (round < sandController.targetObjects.Length)
            {
                // bucketFill.DestroySmallSandPiles();
                sandController.SetNextTarget(round);
                base.StartRound(LOGGamePositions);
            }
            else
            {
                FinishGame();
            }
        }

        private void DropSand()
        {
            var droppedSandPosition = bucketFill.EmptyBucket();
            var isSuccessful = sandPositionComparator.IsPositionEqual(droppedSandPosition);
            if (isSuccessful)
                FinishRound(droppedSandPosition);
            else
                RoundFailed(droppedSandPosition);

            GameLogger.LogConditional(
                $"SAND DROPPED AT: {droppedSandPosition}. TARGET {(isSuccessful ? "REACHED" : "MISSED")}");
            GameLogger.LogConditional(LOGGameProgress());
        }

        public void FinishRound(Vector3 position)
        {
            base.FinishRound(" with " + failedRound + " failed attempts", LOGGamePositions);

            var text = "You have reached the target. Click to continue.";
            if (round == 1 && controller == ControllerType.Joystick)
                text = "You have completed the trial round. Click continue to start ROUND 1.";
            ShowUI("Congratulations!", text, round == 0 ? StartRoundZero : StartRound);
        }

        private void RoundFailed(Vector3 position)
        {
            failedRound++;

            ShowUI("Almost there!", "You have failed to hit the target, try again.", null);
        }

        private new void FinishGame()
        {
            base.FinishGame();
            sandController.Hide();
            bucketFill.FinishBucketFill();
        }

        private void ValidateComponents()
        {
            if (physicsExcavator == null) Debug.LogError("PhysicsExcavator object is not set for the Bucket Game.");
            if (bucketFill == null) Debug.LogError("BucketFill object is not set for the Bucket Game.");
            if (sandController == null) Debug.LogError("SandController object is not set for the Bucket Game.");
        }

        private void InitializeGame()
        {
            if (sandController != null) sandController.Hide();
            if (bucketFill != null) bucketFill.InitBucketFill();
            sandPositionComparator = new SandPositionComparator(
                sandController?.sandTarget,
                bucketFill?.dropSandPoint,
                sandController?.bigSandPile,
                physicsExcavator?.GetExcavatorObj().SwingAxis
            );
        }

        private void LOGGamePositions()
        {
            GameLogger.LogAll($"dropSandPoint position: {bucketFill.dropSandPoint.transform.position}, " +
                              $"bigSandPile position: {sandController.bigSandPile.transform.position}, " +
                              $"targetLocation position: {sandPositionComparator.TargetLocation.transform.position}, " +
                              $"fullDistance: {sandPositionComparator.GetFullDistance()}, " +
                              $"fullAngle: {sandPositionComparator.GetFullAngle()}");
        }

        private string LOGGameProgress()
        {
            var bucketStatus = bucketFill.isBucketFilled ? "isFilled" :
                bucketFill.canBeFilled ? "canBeFilled" : "isEmpty";
            var info = $"Bucket: {bucketStatus}\n";

            info += $"Target: Dist={sandPositionComparator.GetDistanceToTarget()}, " +
                    $"DirectDist={sandPositionComparator.GetDirectDistanceToTarget()}, " +
                    $"PositionProg={sandPositionComparator.GetPositionProgressToTargetPercentage()}%, " +
                    $"CurrAngle={sandPositionComparator.GetCurrentAngleToTarget()}, " +
                    $"AngleProg={sandPositionComparator.GetAngleProgressToTargetPercentage()}%";

            info += $"SandPile: Dist={sandPositionComparator.GetDistanceToBigSandPile()}, " +
                    $"DirectDist={sandPositionComparator.GetDirectDistanceToBigSandPile()}, " +
                    $"PositionProg={sandPositionComparator.GetPositionProgressToSandPilePercentage()}%, " +
                    $"CurrAngle={sandPositionComparator.GetCurrentAngleToSandPile()}, " +
                    $"AngleProg={sandPositionComparator.GetAngleProgressToSandPilePercentage()}%";

            return info;
        }
    }
}