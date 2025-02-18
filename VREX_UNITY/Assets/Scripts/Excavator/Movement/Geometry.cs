using UnityEngine;

namespace Excavator.Movement
{
    public class Geometry
    {
        private readonly Transform bodyBoomJoint;
        private readonly Transform boomCylinderBodyJoint;

        private readonly Transform boomCylinderBoomJoint;
        private readonly Transform boomStickJoint;
        private readonly Transform bucketCylinderBucketJoint;

        private readonly Transform bucketCylinderStickJoint;
        private readonly Transform bucketLinkageJoint;

        private readonly Transform linkageTopLeftJoint;
        private readonly Transform linkageTopRightJoint;

        private readonly Transform stickBucketJoint;
        private readonly Transform stickCylinderBoomJoint;

        private readonly Transform stickCylinderStickJoint;

        public Geometry(Transform bodyBoomJoint, Transform boomStickJoint,
            Transform stickBucketJoint, Transform bucketLinkageJoint, Transform boomCylinderBoomJoint,
            Transform boomCylinderBodyJoint,
            Transform stickCylinderBoomJoint, Transform stickCylinderStickJoint, Transform bucketCylinderStickJoint,
            Transform bucketCylinderBucketJoint, Transform linkageTopLeftJoint, Transform linkageTopRightJoint)
        {
            this.bodyBoomJoint = bodyBoomJoint;
            this.boomStickJoint = boomStickJoint;

            this.stickBucketJoint = stickBucketJoint;
            this.bucketLinkageJoint = bucketLinkageJoint;

            this.boomCylinderBoomJoint = boomCylinderBoomJoint;
            this.boomCylinderBodyJoint = boomCylinderBodyJoint;

            this.stickCylinderBoomJoint = stickCylinderBoomJoint;
            this.stickCylinderStickJoint = stickCylinderStickJoint;

            this.bucketCylinderStickJoint = bucketCylinderStickJoint;
            this.bucketCylinderBucketJoint = bucketCylinderBucketJoint;

            this.linkageTopLeftJoint = linkageTopLeftJoint;
            this.linkageTopRightJoint = linkageTopRightJoint;

            initialDistanceLeftLinkageToBucketBC =
                Vector3.Distance(linkageTopLeftJoint.position, bucketLinkageJoint.position);
        }

        public float boomLength => (boomStickJoint.position - bodyBoomJoint.position).magnitude;
        public float boomLength2 => Mathf.Pow(boomLength, 2);

        public float stickLength => (stickBucketJoint.position - boomStickJoint.position).magnitude;
        public float stickLength2 => Mathf.Pow(stickLength, 2);

        // The distance between the boom to body joint and the boom cylinder joint
        public float leverBoomLength => (bodyBoomJoint.position - boomCylinderBoomJoint.position).magnitude;

        // The distance between the stick to boom joint and the boom stick joint
        public float leverStickLength => (boomStickJoint.position - stickCylinderStickJoint.position).magnitude;

        // The distance between the bucket-linkage joint and the stick-bucket joint
        public float leverBucketLength => (stickBucketJoint.position - bucketCylinderBucketJoint.position).magnitude;

        public float distanceBucketToStickCd =>
            Vector3.Distance(bucketLinkageJoint.position, stickBucketJoint.position);

        public float distanceStickToRightLinkageDa =>
            Vector3.Distance(stickBucketJoint.position, linkageTopRightJoint.position);

        public float distanceRightToLeftLinkageAb =>
            Vector3.Distance(linkageTopRightJoint.position, linkageTopLeftJoint.position);

        public float initialDistanceLeftLinkageToBucketBC { get; }

        public float distanceRightLinkageToBucketAc =>
            Vector3.Distance(linkageTopRightJoint.position, bucketLinkageJoint.position);

        public float LeverBoomAngle()
        {
            var leverVector = (boomCylinderBoomJoint.position - bodyBoomJoint.position).normalized;
            var cylinderVector = (boomCylinderBoomJoint.position - boomCylinderBodyJoint.position).normalized;

            // Calculate the angle under which the hydraulic cylinder applies force to the boom
            var angle = Vector3.Angle(leverVector, cylinderVector) - 180f;

            return angle;
        }

        public float LeverStickAngle()
        {
            var leverVector = (stickCylinderStickJoint.position - boomStickJoint.position).normalized;
            var cylinderVector = (stickCylinderStickJoint.position - stickCylinderBoomJoint.position).normalized;

            // Calculate the angle under which the hydraulic cylinder applies force to the boom
            var angle = Vector3.Angle(leverVector, cylinderVector);

            return angle * 1.5f;
        }

        public float LeverBucketAngle()
        {
            var leverVector = (bucketCylinderBucketJoint.localPosition - stickBucketJoint.localPosition).normalized;
            var cylinderVector = (bucketCylinderBucketJoint.position - bucketCylinderStickJoint.position).normalized;

            // Calculate the angle under which the hydraulic cylinder applies force to the bucket
            var angle = Vector3.Angle(leverVector, cylinderVector);

            return angle;
        }
    }
}