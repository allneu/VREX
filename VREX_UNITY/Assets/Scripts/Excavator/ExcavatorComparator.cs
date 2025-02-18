using Games;
using UnityEngine;

namespace Excavator
{
    public class ExcavatorComparator
    {
        private readonly Excavator currentExcavator;
        private readonly Excavator targetExcavator;

        private Quaternion startBoomRotation;
        private Quaternion startBucketRotation;
        private Quaternion startStickRotation;
        private Quaternion startSwingRotation;

        public ExcavatorComparator(Excavator current, Excavator target)
        {
            targetExcavator = target;
            currentExcavator = current;
            StartRoundRotations();
        }

        public void StartRoundRotations()
        {
            startBoomRotation = currentExcavator.BoomAxis.rotation;
            startStickRotation = currentExcavator.StickAxis.rotation;
            startBucketRotation = currentExcavator.BucketAxis.rotation;
            startSwingRotation = currentExcavator.SwingAxis.rotation;
        }

        public bool AreBoomRotationsEqual()
        {
            return IsRotationSimilar(targetExcavator.BoomAxis.rotation, currentExcavator.BoomAxis.rotation,
                GameConstants.GhostGame.BoomAngleError);
        }

        public bool AreStickRotationsEqual()
        {
            return IsRotationSimilar(targetExcavator.StickAxis.rotation, currentExcavator.StickAxis.rotation,
                GameConstants.GhostGame.StickAngleError);
        }

        public bool AreBucketRotationsEqual()
        {
            return IsRotationSimilar(targetExcavator.BucketAxis.rotation, currentExcavator.BucketAxis.rotation,
                GameConstants.GhostGame.BucketAngleError);
        }

        public bool AreSwingRotationsEqual()
        {
            return IsRotationSimilar(targetExcavator.SwingAxis.rotation, currentExcavator.SwingAxis.rotation,
                GameConstants.GhostGame.SwingAngleError);
        }

        public bool AreLocalRotationsEqual()
        {
            return AreBoomRotationsEqual() &&
                   AreStickRotationsEqual() &&
                   AreBucketRotationsEqual() &&
                   AreSwingRotationsEqual();
        }

        private bool IsRotationSimilar(Quaternion rotation1, Quaternion rotation2, float error)
        {
            return Quaternion.Angle(rotation1, rotation2) <= error;
        }

        public float GetBoomRotationDifference()
        {
            return Quaternion.Angle(targetExcavator.BoomAxis.rotation, currentExcavator.BoomAxis.rotation);
        }

        public float GetStickRotationDifference()
        {
            return Quaternion.Angle(targetExcavator.StickAxis.rotation, currentExcavator.StickAxis.rotation);
        }

        public float GetBucketRotationDifference()
        {
            return Quaternion.Angle(targetExcavator.BucketAxis.rotation, currentExcavator.BucketAxis.rotation);
        }

        public float GetSwingRotationDifference()
        {
            return Quaternion.Angle(targetExcavator.SwingAxis.rotation, currentExcavator.SwingAxis.rotation);
        }

        public float GetBoomRotationProgressPercentage()
        {
            return CalculateProgressPercentage(startBoomRotation, targetExcavator.BoomAxis.rotation,
                currentExcavator.BoomAxis.rotation);
        }

        public float GetStickRotationProgressPercentage()
        {
            return CalculateProgressPercentage(startStickRotation, targetExcavator.StickAxis.rotation,
                currentExcavator.StickAxis.rotation);
        }

        public float GetBucketRotationProgressPercentage()
        {
            return CalculateProgressPercentage(startBucketRotation, targetExcavator.BucketAxis.rotation,
                currentExcavator.BucketAxis.rotation);
        }

        public float GetSwingRotationProgressPercentage()
        {
            return CalculateProgressPercentage(startSwingRotation, targetExcavator.SwingAxis.rotation,
                currentExcavator.SwingAxis.rotation);
        }

        private float CalculateProgressPercentage(Quaternion startRotation, Quaternion targetRotation,
            Quaternion currentRotation)
        {
            var totalDifference = Quaternion.Angle(startRotation, targetRotation);
            var currentDifference = Quaternion.Angle(currentRotation, targetRotation);

            if (totalDifference == 0)
                return 100f;

            return 100 - currentDifference / totalDifference * 100f;
        }
    }
}