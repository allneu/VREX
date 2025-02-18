using Excavator.Movement;
using UnityEngine;

namespace Excavator
{
    public class Excavator
    {
        // Joints
        public readonly Transform BodyBoomJoint;
        public readonly GameObject Boom;
        public readonly Transform BoomAxis;
        public readonly Quaternion BoomAxisInitialQ;

        // Cylinder components
        public readonly Transform BoomCylinderBarrel;
        public readonly Transform BoomCylinderBarrelTarget;
        public readonly Transform BoomCylinderBodyJoint;
        public readonly Transform BoomCylinderBoomJoint;
        public readonly Transform BoomCylinderPiston;
        public readonly Transform BoomCylinderPistonTarget;
        public readonly Transform BoomStickJoint;
        public readonly GameObject Bucket;
        public readonly Transform BucketAxis;
        public readonly Quaternion BucketAxisInitialQ;

        public readonly Transform BucketCylinderBarrel;
        public readonly Transform BucketCylinderBarrelTarget;
        public readonly Transform BucketCylinderBucketJoint;
        public readonly Transform BucketCylinderPiston;
        public readonly Transform BucketCylinderPistonTarget;
        public readonly Transform BucketCylinderStickJoint;
        public readonly Transform BucketLinkageJoint;
        public readonly GameObject ExBase;

        public readonly Transform ExcavatorForwardAxis;

        // Excavator components
        public readonly GameObject ExObject;

        // Geometry
        public readonly Geometry Geometry;
        public readonly Transform LinkageSideAxis;
        public readonly Quaternion LinkageSideAxisInitialQ;
        public readonly Transform LinkageTopAxis;
        public readonly Quaternion LinkageTopAxisInitialQ;

        // Linkage joints
        public readonly Transform LinkageTopLeftJoint;
        public readonly Transform LinkageTopRightJoint;
        public readonly GameObject Stick;
        public readonly Transform StickAxis;
        public readonly Quaternion StickAxisInitialQ;
        public readonly Transform StickBucketJoint;

        public readonly Transform StickCylinderBarrel;
        public readonly Transform StickCylinderBarrelTarget;
        public readonly Transform StickCylinderBoomJoint;
        public readonly Transform StickCylinderPiston;
        public readonly Transform StickCylinderPistonTarget;
        public readonly Transform StickCylinderStickJoint;
        public readonly Transform SwingAxis;

        // Initial rotations
        public readonly Quaternion SwingAxisInitialQ;

        // Transform components
        public readonly Transform Transform;

        public Excavator(GameObject excavator)
        {
            var swingAxisPath = "Armature/bone_MainBase";
            var boomAxisPath = "Armature/bone_MainBase/bone_Boom";
            var stickAxisPath = "Armature/bone_MainBase/bone_Boom/bone_Stick";
            var bucketAxisPath = "Armature/bone_MainBase/bone_Boom/bone_Stick/bone_Bucket";
            var linkageTopAxisPath = "Armature/bone_MainBase/bone_Boom/bone_Stick/bone_Bucket_Link_TOP";
            var linkageSideAxisPath = linkageTopAxisPath + "/bone_Bucket_Link_SIDE";

            ExObject = excavator;
            Transform = excavator.transform;

            SwingAxis = Transform.Find(swingAxisPath);
            BoomAxis = Transform.Find(boomAxisPath);
            StickAxis = Transform.Find(stickAxisPath);
            BucketAxis = Transform.Find(bucketAxisPath);
            ExcavatorForwardAxis = Transform.Find("Armature");
            LinkageTopAxis = Transform.Find(linkageTopAxisPath);
            LinkageSideAxis = Transform.Find(linkageSideAxisPath);

            SwingAxisInitialQ = SwingAxis.localRotation;
            BoomAxisInitialQ = BoomAxis.localRotation;
            StickAxisInitialQ = StickAxis.localRotation;
            BucketAxisInitialQ = BucketAxis.localRotation;
            LinkageTopAxisInitialQ = LinkageTopAxis.localRotation;
            LinkageSideAxisInitialQ = LinkageSideAxis.localRotation;

            BoomStickJoint = Transform.Find(boomAxisPath + "/Boom/Caterpillar_390F_LME:_ARM_PIN_390F_LME");
            StickBucketJoint = Transform.Find(bucketAxisPath + "/Bucket/Bucket_Pin_RIGHT");
            BodyBoomJoint = Transform.Find(boomAxisPath + "/Boom/Boom_Pin");

            BoomCylinderBoomJoint = Transform.Find(boomAxisPath + "/Boom/Cylinder_Boom_Piston_Pin");
            BoomCylinderBodyJoint = Transform.Find(swingAxisPath + "/Base/Boom_Pin_Front_Pin");

            StickCylinderStickJoint = Transform.Find(stickAxisPath + "/Stick/Cylinder_Stick_Piston_Pin");
            StickCylinderBoomJoint = Transform.Find(boomAxisPath + "/Boom/Cylinder_Stick_Barrel_Pin");

            BucketCylinderStickJoint = Transform.Find(stickAxisPath + "/Stick/Cylinder_Bucket_Barrel_Pin");

            LinkageTopLeftJoint = Transform.Find(linkageTopAxisPath + "/Bucket_Link_TOP/Bucket_Link_TOP_Pin_LEFT");
            LinkageTopRightJoint = Transform.Find(linkageTopAxisPath + "/Bucket_Link_TOP/Bucket_Link_TOP_Pin_RIGHT");
            
            BucketLinkageJoint = Transform.Find(bucketAxisPath + "/Bucket/Bucket_Pin_LEFT");
            BucketCylinderBucketJoint =
                Transform.Find(linkageTopAxisPath + "/Bucket_Link_TOP/Bucket_Link_TOP_Pin_LEFT");

            Geometry = new Geometry(BodyBoomJoint, BoomStickJoint, StickBucketJoint, BucketLinkageJoint,
                BoomCylinderBoomJoint, BoomCylinderBodyJoint, StickCylinderBoomJoint, StickCylinderStickJoint,
                BucketCylinderStickJoint, BucketCylinderBucketJoint, LinkageTopLeftJoint, LinkageTopRightJoint);

            Bucket = Transform.Find(bucketAxisPath + "/Bucket").gameObject;
            Stick = Transform.Find(stickAxisPath + "/Stick").gameObject;
            Boom = Transform.Find(boomAxisPath + "/Boom").gameObject;
            ExBase = Transform.Find(swingAxisPath + "/Base").gameObject;

            BoomCylinderBarrel = Transform.Find(swingAxisPath + "/bone_Cylinder_Boom_Barrel");
            BoomCylinderBarrelTarget = Transform.Find(boomAxisPath + "/Boom/Cylinder_Boom_Piston_Pin");

            BoomCylinderPiston = Transform.Find(boomAxisPath + "/bone_Cylinder_Boom_Piston");
            BoomCylinderPistonTarget = Transform.Find(swingAxisPath + "/Base/Boom_Pin_Front_Pin");

            StickCylinderBarrel = Transform.Find(boomAxisPath + "/bone_Cylinder_Stick_Barrel");
            StickCylinderBarrelTarget = Transform.Find(stickAxisPath + "/Stick/Cylinder_Stick_Piston_Pin");

            StickCylinderPiston = Transform.Find(stickAxisPath + "/bone_Cylinder_Stick_Piston");
            StickCylinderPistonTarget = Transform.Find(boomAxisPath + "/Boom/Cylinder_Stick_Barrel_Pin");

            BucketCylinderBarrel = Transform.Find(stickAxisPath + "/bone_Cylinder_Bucket_Barrel");
            BucketCylinderBarrelTarget =
                Transform.Find(linkageTopAxisPath + "/Bucket_Link_TOP/Bucket_Link_TOP_Pin_LEFT");

            BucketCylinderPiston = Transform.Find(linkageTopAxisPath + "/bone_Cylinder_Bucket_Piston");
            BucketCylinderPistonTarget = Transform.Find(stickAxisPath + "/Stick/Cylinder_Bucket_Barrel_Pin");
        }
        
        public Vector3 swingGlobalRotation
        {
            set => SwingAxis.rotation = Quaternion.Euler(value);
            get
            {
                var eulerAngles = SwingAxis.eulerAngles;
                eulerAngles.x = NormalizeAngle(eulerAngles.x);
                eulerAngles.y = NormalizeAngle(eulerAngles.y);
                eulerAngles.z = NormalizeAngle(eulerAngles.z);
                return eulerAngles;
            }
        }
        
        public float stickLocalRotationX
        {
            set
            {
                value = NormalizeAngle(value);
                StickAxis.localRotation = Quaternion.Euler(value,
                    StickAxisInitialQ.eulerAngles.y,
                    StickAxisInitialQ.eulerAngles.z);
            }
        }

        public float boomLocalRotationX
        {
            set
            {
                value = NormalizeAngle(value);
                BoomAxis.localRotation = Quaternion.Euler(value,
                    BoomAxisInitialQ.eulerAngles.y,
                    BoomAxisInitialQ.eulerAngles.z);
            }
        }

        public float bucketLocalRotationX
        {
            set
            {
                value = NormalizeAngle(value);
                BucketAxis.localRotation = Quaternion.Euler(value,
                    BucketAxisInitialQ.eulerAngles.y,
                    BucketAxisInitialQ.eulerAngles.z);
            }
            get
            {
                var currentEulerAngles = BucketAxis.localRotation.eulerAngles;
                return NormalizeAngle(currentEulerAngles.x);
            }
        }


        // Normalize angles to be within -180 to 180
        private float NormalizeAngle(float angle)
        {
            angle = (angle + 180) % 360;
            if (angle < 0)
                angle += 360;
            return angle - 180;
        }

        public void OrientExcavatorCylinders()
        {
            OrientBoomCylinder();
            OrientStickCylinder();
            OrientBucketCylinder();
            OrientLinkage();
        }

        public void OrientBoomCylinder()
        {
            OrientHydraulicCylinder(BoomCylinderBarrel, BoomCylinderBarrelTarget,
                BoomCylinderPiston, BoomCylinderPistonTarget, Boom.transform.up);
        }

        public void OrientStickCylinder()
        {
            OrientHydraulicCylinder(StickCylinderBarrel, StickCylinderBarrelTarget,
                StickCylinderPiston, StickCylinderPistonTarget, Stick.transform.up);
        }

        public void OrientBucketCylinder()
        {
            OrientHydraulicCylinder(BucketCylinderBarrel, BucketCylinderBarrelTarget,
                BucketCylinderPiston, BucketCylinderPistonTarget, Vector3.up);
        }

        public void OrientLinkage()
        {
            var a = Geometry.distanceRightToLeftLinkageAb;

            // The b side has to be the initial distance to calculate correct angles
            var b = Geometry.initialDistanceLeftLinkageToBucketBC;

            var c = Geometry.distanceBucketToStickCd;
            var d = Geometry.distanceStickToRightLinkageDa;

            // Always has to be the current distance to calculate correct angles
            var diagonalAc = Geometry.distanceRightLinkageToBucketAc;

            // Calculate angle A using the law of cosines with the diagonal AC
            var angleA1 =
                Mathf.Acos((Mathf.Pow(diagonalAc, 2) + Mathf.Pow(a, 2) - Mathf.Pow(b, 2)) / (2 * diagonalAc * a))
                * Mathf.Rad2Deg;
            var angleA2 =
                Mathf.Acos((Mathf.Pow(diagonalAc, 2) + Mathf.Pow(d, 2) - Mathf.Pow(c, 2)) / (2 * diagonalAc * d))
                * Mathf.Rad2Deg;

            // Check if the quadrilateral is convex or concave
            var angleA = BucketAxis.localRotation.eulerAngles.x > 180
                ? angleA1 + angleA2
                : angleA1 - angleA2;

            // Calculate angle B using the law of cosines
            var cosB = (Mathf.Pow(a, 2) + Mathf.Pow(b, 2) - Mathf.Pow(diagonalAc, 2)) / (2 * a * b);
            var angleB = Mathf.Acos(cosB) * Mathf.Rad2Deg;

            if (!float.IsNaN(angleA))
            {
                LinkageTopAxis.localRotation = Quaternion.Euler(new Vector3(-angleA,
                    LinkageTopAxisInitialQ.eulerAngles.y,
                    LinkageTopAxisInitialQ.eulerAngles.z));
            }
            else
            {
                // Debug.LogWarning("Angle A in Linkage is NaN");
            }
            if (float.IsNaN(angleB))
            {
                LinkageSideAxis.localRotation = Quaternion.Euler(new Vector3(angleB,
                    LinkageSideAxisInitialQ.eulerAngles.y,
                    LinkageSideAxisInitialQ.eulerAngles.z));
            }
            else
            {
                // Debug.LogWarning("Angle B in Linkage is NaN");
            }
        }


        private void OrientHydraulicCylinder(Transform barrel, Transform barrelTarget,
            Transform piston, Transform pistonTarget, Vector3 up)
        {
            barrel.LookAt(barrelTarget, up);
            piston.LookAt(pistonTarget, up);

            barrel.Rotate(new Vector3(90F, 0F, 0F));
            piston.Rotate(new Vector3(90F, 0F, 0F));
        }

        public void ResetInitialPosition()
        {
            SwingAxis.localRotation = SwingAxisInitialQ;
            BoomAxis.localRotation = BoomAxisInitialQ;
            StickAxis.localRotation = StickAxisInitialQ;
            BucketAxis.localRotation = BucketAxisInitialQ;
        }

        public void Show()
        {
            SetRenderersEnabled(true);
        }

        public void Hide()
        {
            SetRenderersEnabled(false);
        }

        private void SetRenderersEnabled(bool enabled)
        {
            var renderers = ExObject.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers) renderer.enabled = enabled;
        }
    }
}