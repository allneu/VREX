using System;
using UnityEngine;

namespace Games.SandGame
{
    public class SandPositionComparator
    {
        public readonly GameObject BigSandPile;
        public readonly GameObject DropSandPoint;
        public readonly Transform SwingAxis;
        public readonly GameObject TargetLocation;

        public SandPositionComparator(GameObject targetLocation, GameObject dropSandPoint, GameObject bigSandPile,
            Transform swingAxis)
        {
            TargetLocation = targetLocation;
            DropSandPoint = dropSandPoint;
            BigSandPile = bigSandPile;
            SwingAxis = swingAxis;
        }

        public bool IsPositionEqual(Vector3 sandPilePosition)
        {
            var targetPosition = TargetLocation.transform.position;

            var xDifference = Mathf.Abs(sandPilePosition.x - targetPosition.x);
            var zDifference = Mathf.Abs(sandPilePosition.z - targetPosition.z);

            return xDifference < GameConstants.BucketGame.SandToTargetError &&
                   zDifference < GameConstants.BucketGame.SandToTargetError;
        }

        public Tuple<float, float> GetDistanceToTarget()
        {
            var targetPosition = TargetLocation.transform.position;
            var dropPosition = DropSandPoint.transform.position;

            var xDifference = Mathf.Abs(dropPosition.x - targetPosition.x);
            var zDifference = Mathf.Abs(dropPosition.z - targetPosition.z);

            return Tuple.Create(xDifference, zDifference);
        }

        public float GetDirectDistanceToTarget()
        {
            var targetPosition = TargetLocation.transform.position;
            var dropPosition = DropSandPoint.transform.position;

            var xDifference = dropPosition.x - targetPosition.x;
            var zDifference = dropPosition.z - targetPosition.z;

            return Mathf.Sqrt(xDifference * xDifference + zDifference * zDifference);
        }

        public Tuple<float, float> GetDistanceToBigSandPile()
        {
            var bigSandPilePosition = BigSandPile.transform.position;
            var dropPosition = DropSandPoint.transform.position;

            var xDifference = Mathf.Abs(dropPosition.x - bigSandPilePosition.x);
            var zDifference = Mathf.Abs(dropPosition.z - bigSandPilePosition.z);

            return Tuple.Create(xDifference, zDifference);
        }

        public float GetDirectDistanceToBigSandPile()
        {
            var bigSandPilePosition = BigSandPile.transform.position;
            var dropPosition = DropSandPoint.transform.position;

            var xDifference = dropPosition.x - bigSandPilePosition.x;
            var zDifference = dropPosition.z - bigSandPilePosition.z;

            return Mathf.Sqrt(xDifference * xDifference + zDifference * zDifference);
        }

        public float GetPositionProgressToTargetPercentage()
        {
            var startPosition = BigSandPile.transform.position;
            var endPosition = TargetLocation.transform.position;
            var dropPosition = DropSandPoint.transform.position;

            var totalDistance = Mathf.Sqrt(Mathf.Pow(endPosition.x - startPosition.x, 2) +
                                           Mathf.Pow(endPosition.z - startPosition.z, 2));
            var remainingDistance = Mathf.Sqrt(Mathf.Pow(endPosition.x - dropPosition.x, 2) +
                                               Mathf.Pow(endPosition.z - dropPosition.z, 2));

            var progress = (totalDistance - remainingDistance) / totalDistance * 100;
            return progress;
        }

        public float GetPositionProgressToSandPilePercentage()
        {
            var startPosition = TargetLocation.transform.position;
            var endPosition = BigSandPile.transform.position;
            var dropPosition = DropSandPoint.transform.position;

            var totalDistance = Mathf.Sqrt(Mathf.Pow(endPosition.x - startPosition.x, 2) +
                                           Mathf.Pow(endPosition.z - startPosition.z, 2));
            var remainingDistance = Mathf.Sqrt(Mathf.Pow(endPosition.x - dropPosition.x, 2) +
                                               Mathf.Pow(endPosition.z - dropPosition.z, 2));

            var progress = (totalDistance - remainingDistance) / totalDistance * 100;
            return progress;
        }

        public float GetFullAngle()
        {
            var startPosition = BigSandPile.transform.position;
            var swingPosition = SwingAxis.position;
            var endPosition = TargetLocation.transform.position;

            var vectorA = swingPosition - startPosition;
            var vectorB = endPosition - swingPosition;

            return Vector3.Angle(vectorA, vectorB);
        }

        public float GetFullDistance()
        {
            var startPosition = BigSandPile.transform.position;
            var endPosition = TargetLocation.transform.position;

            var xDifference = endPosition.x - startPosition.x;
            var zDifference = endPosition.z - startPosition.z;

            return Mathf.Sqrt(xDifference * xDifference + zDifference * zDifference);
        }


        public float GetCurrentAngleToTarget()
        {
            var dropPosition = DropSandPoint.transform.position;
            var swingPosition = SwingAxis.position;
            var endPosition = TargetLocation.transform.position;

            var vectorA = swingPosition - dropPosition;
            var vectorB = endPosition - swingPosition;

            return 180f - Vector3.Angle(vectorA, vectorB);
        }

        public float GetCurrentAngleToSandPile()
        {
            var dropPosition = DropSandPoint.transform.position;
            var swingPosition = SwingAxis.position;
            var endPosition = BigSandPile.transform.position;

            var vectorA = swingPosition - dropPosition;
            var vectorB = endPosition - swingPosition;

            return 180f - Vector3.Angle(vectorA, vectorB);
        }

        public float GetAngleProgressToTargetPercentage()
        {
            var fullAngle = GetFullAngle();
            var currentAngle = GetCurrentAngleToTarget();

            return (1 - currentAngle / fullAngle) * 100;
        }

        public float GetAngleProgressToSandPilePercentage()
        {
            var fullAngle = GetFullAngle();
            var currentAngle = GetCurrentAngleToSandPile();

            return (1 - currentAngle / fullAngle) * 100;
        }
    }
}