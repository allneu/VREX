using Unity.Mathematics;
using UnityEngine;

namespace Excavator.Movement
{
    public class IKExcavator : MonoBehaviour
    {
        private Excavator excavatorObj;
        private Rigidbody rb;

        private void Awake()
        {
            InitializeExcavator();
            ConfigureRigidbody();
        }

        private void Update()
        {
            if (excavatorObj == null) return;
            excavatorObj.OrientExcavatorCylinders();
        }

        private void InitializeExcavator()
        {
            excavatorObj = new Excavator(gameObject);
        }

        private void ConfigureRigidbody()
        {
            excavatorObj.ExObject.AddComponent<Rigidbody>();
            rb = excavatorObj.ExObject.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        public void ArmIK(Vector3 stickTarget, Vector3 bucketTarget)
        {
            SetSwingAngle(stickTarget);
            SetBoomAndStickAngles(stickTarget);
            SetBucketAngle(stickTarget, bucketTarget);
        }

        private void SetSwingAngle(Vector3 stickTarget)
        {
            var directionToTarget = stickTarget - excavatorObj.SwingAxis.position;
            directionToTarget.y = 0; // Project to x-z plane

            var forwardDirection = excavatorObj.SwingAxis.forward;
            forwardDirection.y = 0; // Project to x-z plane

            directionToTarget.Normalize();
            forwardDirection.Normalize();

            var pSwingAngle = Vector3.SignedAngle(forwardDirection, directionToTarget, Vector3.up);
            excavatorObj.swingGlobalRotation = new Vector3(
                excavatorObj.swingGlobalRotation.x,
                excavatorObj.swingGlobalRotation.y + pSwingAngle,
                excavatorObj.swingGlobalRotation.z
            );
        }

        private void SetBoomAndStickAngles(Vector3 stickTarget)
        {
            var boomJointToTargetVec = stickTarget - excavatorObj.BodyBoomJoint.position;
            var targetToUpVec = Vector3.Angle(excavatorObj.SwingAxis.up, boomJointToTargetVec);
            var bodyToTarget = boomJointToTargetVec.magnitude;
            var bodyToTarget2 = Mathf.Pow(bodyToTarget, 2F);

            var pBoomAngle = Mathf.Acos(
                (excavatorObj.Geometry.boomLength2 + bodyToTarget2 - excavatorObj.Geometry.stickLength2) /
                (2F * excavatorObj.Geometry.boomLength * bodyToTarget)
            ) * Mathf.Rad2Deg;

            var pStickAngle = Mathf.Acos(
                (excavatorObj.Geometry.stickLength2 + excavatorObj.Geometry.boomLength2 - bodyToTarget2) /
                (2F * excavatorObj.Geometry.boomLength * excavatorObj.Geometry.stickLength)
            ) * Mathf.Rad2Deg;

            if (!float.IsNaN(pBoomAngle))
            {
                pBoomAngle = targetToUpVec - pBoomAngle;
                excavatorObj.boomLocalRotationX = Mathf.Clamp(pBoomAngle,
                    ExcavatorConstants.IKConstants.Angles.MinBoomAngle,
                    ExcavatorConstants.IKConstants.Angles.MaxBoomAngle);
            }

            if (!float.IsNaN(pStickAngle))
                excavatorObj.stickLocalRotationX = Mathf.Clamp(pStickAngle,
                    ExcavatorConstants.IKConstants.Angles.MinStickAngle,
                    ExcavatorConstants.IKConstants.Angles.MaxStickAngle);
        }

        private void SetBucketAngle(Vector3 stickTarget, Vector3 bucketTarget)
        {
            var bucketDirection = bucketTarget - stickTarget;
            var stickDirection = stickTarget - excavatorObj.BoomStickJoint.position;
            var bucketAngle = Vector3.SignedAngle(bucketDirection, stickDirection, excavatorObj.BucketAxis.right);
            excavatorObj.bucketLocalRotationX = -1 * Mathf.Clamp(bucketAngle,
                ExcavatorConstants.IKConstants.Angles.MinBucketAngle,
                ExcavatorConstants.IKConstants.Angles.MaxBucketAngle);
        }

        public Excavator GetExcavatorObj()
        {
            return excavatorObj;
        }
    }
}