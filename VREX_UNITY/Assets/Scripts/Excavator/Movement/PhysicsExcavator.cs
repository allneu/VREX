using System;
using Controllers.ExcavatorIKController;
using Games.SandGame;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

namespace Excavator.Movement
{
    public class PhysicsExcavator : MonoBehaviour
    {
        // Mass in kg
        public int excavatorMass = 86995;
        public int baseMass = 67950;
        public int boomMass = 8310;
        public int stickMass = 4930;
        public int bucketMass = 2293;

        // Swing parameters
        public float maxSwingTorque = 260000f; // 260,000 N/m
        public float maxSwingAngularVelocity = 6.2f; // Max angular velocity in RPM

        // Hydraulic system parameters
        // cylinder parameters in mm
        public float boomCylinderBore = 210f;
        public float boomCylinderStroke = 1967f;

        public float stickCylinderBore = 220f;
        public float stickCylinderStroke = 2262f;

        public float bucketCylinderBore = 220f;
        public float bucketCylinderStroke = 1586f;

        public float maxCylinderPressure = 350f; // in Bars
        public float oilFlowRate = 980f; // in liters per second

        // Controllers
        public XRJoystick joystickLeft;
        public XRJoystick joystickRight;
        public ExcavatorIKController excavatorIKController;

        public bool isControlled;

        public BucketFill bucketFill;

        // The excavator object
        private Excavator _excavatorObj;
        private bool _followIKcontroller;
        private IKExcavator _ikExcavator;
        private bool _isIKControllerActive;
        private bool _isLeftJoystickActive;
        private bool _isRightJoystickActive;

        private void Awake()
        {
            _excavatorObj = new Excavator(gameObject);
            AssignPhysics();
            _followIKcontroller = false;
            isControlled = false;
            if (excavatorIKController != null)
            {
                excavatorIKController.onControllerSelect.AddListener(MoveExcavator);
                _ikExcavator = excavatorIKController.GetIKExcavator();
            }
            else
            {
                Debug.LogError("Excavator Controller reference not set for Physics Excavator");
            }

            if (joystickLeft != null)
            {
                joystickLeft.onValueChangeX.AddListener(JoystickLeftX);
                joystickLeft.onValueChangeY.AddListener(JoystickLeftY);
            }
            else
            {
                Debug.LogError("Left joystick reference not set for Physics Excavator");
            }

            if (joystickRight != null)
            {
                joystickRight.onValueChangeX.AddListener(JoystickRightX);
                joystickRight.onValueChangeY.AddListener(JoystickRightY);
            }
            else
            {
                Debug.LogError("Right joystick reference not set for Physics Excavator");
            }

            if (bucketFill == null) Debug.LogError("BucketFill reference not set for Physics Excavator");
        }

        private void Update()
        {
            isControlled = _isLeftJoystickActive || _isRightJoystickActive || _isIKControllerActive;

            var controller = _ikExcavator.GetExcavatorObj();
            if (_followIKcontroller)
            {
                var allFalse = true;

                var minDelta = GetRotationDirection(_excavatorObj.SwingAxis.localRotation.eulerAngles.y,
                    controller.SwingAxis.localRotation.eulerAngles.y);
                if (Mathf.Abs(minDelta) > ExcavatorConstants.PhysicsConstants.JointMovementThreshold && Mathf.Abs(minDelta) < 360 - ExcavatorConstants.PhysicsConstants.JointMovementThreshold)
                {
                    MoveSwing(NormalizeIKToRange(minDelta));
                    allFalse = false;
                }
                else
                {
                    MoveSwing(0);
                }

                minDelta = GetRotationDirection(_excavatorObj.BoomAxis.localRotation,
                    controller.BoomAxis.localRotation);
                if (Mathf.Abs(minDelta) > ExcavatorConstants.PhysicsConstants.JointMovementThreshold && Mathf.Abs(minDelta) < 360 - ExcavatorConstants.PhysicsConstants.JointMovementThreshold)
                {
                    MoveBoom(NormalizeIKToRange(minDelta));
                    allFalse = false;
                }
                else
                {
                    MoveBoom(0);
                }

                minDelta = GetRotationDirection(_excavatorObj.StickAxis.localRotation,
                    controller.StickAxis.localRotation);
                if (Mathf.Abs(minDelta) > ExcavatorConstants.PhysicsConstants.JointMovementThreshold && Mathf.Abs(minDelta) < 360 - ExcavatorConstants.PhysicsConstants.JointMovementThreshold)
                {
                    MoveStick(NormalizeIKToRange(minDelta));
                    allFalse = false;
                }
                else
                {
                    MoveStick(0);
                }

                minDelta = GetRotationDirection(_excavatorObj.BucketAxis.localRotation,
                    controller.BucketAxis.localRotation);
                if (Mathf.Abs(minDelta) > ExcavatorConstants.PhysicsConstants.JointMovementThreshold && Mathf.Abs(minDelta) < 360 - ExcavatorConstants.PhysicsConstants.JointMovementThreshold)
                {
                    MoveBucket(-1 * NormalizeIKToRange(minDelta));
                    allFalse = false;
                }
                else
                {
                    MoveBucket(0);
                }

                if (allFalse && !_isIKControllerActive) _followIKcontroller = false;
            }
            else
            {
                SynchronizeIKController();
            }

            _excavatorObj.OrientExcavatorCylinders();
        }

        public Excavator GetExcavatorObj()
        {
            return _excavatorObj;
        }

        private float GetRotationDirection(float currentBearing, float targetBearing)
        {
            var alpha = targetBearing - currentBearing;
            var beta = targetBearing - currentBearing + 360;
            var gamma = targetBearing - currentBearing - 360;

            // Determine which of alpha, beta, or gamma is the smallest in absolute terms
            var minDelta = MinByAbs(alpha, MinByAbs(beta, gamma));

            return minDelta;
        }

        private float GetRotationDirection(Quaternion currentBearing, Quaternion targetBearing)
        {
            var rotationDifference = Quaternion.Inverse(currentBearing) * targetBearing;
            rotationDifference.ToAngleAxis(out var angleDifference, out var axis);

            var signedAngleDifference = Vector3.Dot(axis, Vector3.right) > 0 ? angleDifference : -angleDifference;
            
            return signedAngleDifference;
        }
        private float NormalizeIKToRange(float value)
        {
            var range = ExcavatorConstants.PhysicsConstants.IKControllerNormalizationRange;

            var normalized = value;
            if (value > 180)
                normalized -= 360;
            else if (value < -180)
                normalized += 360;
            
            // Scale and shift to the range of -1 to 1
            normalized = Mathf.Clamp(normalized, -range, range);
            return Mathf.Abs(normalized) / range * Mathf.Sign(value);
        }

        private static float MinByAbs(float a, float b)
        {
            if (Math.Abs(a) < Math.Abs(b))
                return a;
            return b;
        }

        private void AssignPhysics()
        {
            // Add colliders
            _excavatorObj.ExObject.AddComponent<BoxCollider>();
            _excavatorObj.SwingAxis.AddComponent<MeshCollider>();
            _excavatorObj.BoomAxis.AddComponent<MeshCollider>();
            _excavatorObj.StickAxis.AddComponent<MeshCollider>();
            _excavatorObj.BucketAxis.AddComponent<MeshCollider>();

            var col = _excavatorObj.SwingAxis.GetComponent<MeshCollider>();
            col.convex = true;
            col = _excavatorObj.BoomAxis.GetComponent<MeshCollider>();
            col.convex = true;
            col = _excavatorObj.StickAxis.GetComponent<MeshCollider>();
            col.convex = true;
            col = _excavatorObj.BucketAxis.GetComponent<MeshCollider>();
            col.convex = true;

            // Add Rigidbody components and set them to kinematic
            AddKinematicRigidbody(_excavatorObj.ExObject.transform, excavatorMass);
            AddKinematicRigidbody(_excavatorObj.SwingAxis, baseMass);
            AddKinematicRigidbody(_excavatorObj.BoomAxis, boomMass);
            AddKinematicRigidbody(_excavatorObj.StickAxis, stickMass);
            AddKinematicRigidbody(_excavatorObj.BucketAxis, bucketMass);

            // Initialize the positions
            MoveBoom(0);
            MoveBucket(0);
            MoveStick(0);
            MoveSwing(0);
        }

        private void AddKinematicRigidbody(Transform obj, float mass)
        {
            var rb = obj.AddComponent<Rigidbody>();
            rb.mass = mass;
            rb.isKinematic = true;
        }

        public void SetLeftJoystickActive(bool active)
        {
            _isLeftJoystickActive = active;
        }

        public void SetRightJoystickActive(bool active)
        {
            _isRightJoystickActive = active;
        }

        public void SetIKControllerActive(bool active)
        {
            _isIKControllerActive = active;
        }

        private void JoystickLeftX(float val)
        {
            StopMovement();
            MoveSwing(val);
        }

        private void JoystickLeftY(float val)
        {
            StopMovement();
            MoveStick(-1 * val);
        }

        private void JoystickRightX(float val)
        {
            StopMovement();
            MoveBucket(-1 * val);
        }

        private void JoystickRightY(float val)
        {
            StopMovement();
            MoveBoom(val);
        }

        private void MoveExcavator()
        {
            _followIKcontroller = true;
            _isIKControllerActive = true;
        }

        private void StopMovement()
        {
            if (_followIKcontroller)
            {
                _followIKcontroller = false;
                MoveSwing(0);
                MoveBoom(0);
                MoveBucket(0);
                MoveStick(0);
            }
        }

        private void SynchronizeIKController()
        {
            float interpolationFactor = Time.deltaTime * 5.0f;

            _ikExcavator.GetExcavatorObj().SwingAxis.localRotation = Quaternion.Slerp(
                _ikExcavator.GetExcavatorObj().SwingAxis.localRotation,
                _excavatorObj.SwingAxis.localRotation,
                interpolationFactor
            );

            _ikExcavator.GetExcavatorObj().BoomAxis.localRotation = Quaternion.Slerp(
                _ikExcavator.GetExcavatorObj().BoomAxis.localRotation,
                _excavatorObj.BoomAxis.localRotation,
                interpolationFactor
            );

            _ikExcavator.GetExcavatorObj().StickAxis.localRotation = Quaternion.Slerp(
                _ikExcavator.GetExcavatorObj().StickAxis.localRotation,
                _excavatorObj.StickAxis.localRotation,
                interpolationFactor
            );

            _ikExcavator.GetExcavatorObj().BucketAxis.localRotation = Quaternion.Slerp(
                _ikExcavator.GetExcavatorObj().BucketAxis.localRotation,
                _excavatorObj.BucketAxis.localRotation,
                interpolationFactor
            );
        }

        private void MoveSwing(float val)
        {
            MoveSwingJointKinematic(
                _excavatorObj.SwingAxis.transform,
                val
            );
        }

        private void MoveBoom(float val)
        {
            MoveArmJoint(
                _excavatorObj.BoomAxis.transform,
                val,
                boomCylinderBore,
                boomCylinderStroke,
                () => _excavatorObj.Geometry.leverBoomLength,
                () => _excavatorObj.Geometry.LeverBoomAngle(),
                x => IsRotationStepAllowed(x,
                    _excavatorObj.BoomAxis.localRotation.x * Mathf.Sign(_excavatorObj.BoomAxis.localRotation.w),
                    ExcavatorConstants.PhysicsConstants.Angles.MinBoomAngle,
                    ExcavatorConstants.PhysicsConstants.Angles.MaxBoomAngle),
                boomMass + stickMass + bucketMass
            );
        }

        private void MoveStick(float val)
        {
            MoveArmJoint(
                _excavatorObj.StickAxis.transform,
                val,
                stickCylinderBore,
                stickCylinderStroke,
                () => _excavatorObj.Geometry.leverStickLength,
                () => _excavatorObj.Geometry.LeverStickAngle(),
                x => IsRotationStepAllowed(x,
                    _excavatorObj.StickAxis.localRotation.x * Mathf.Sign(_excavatorObj.StickAxis.localRotation.w),
                    ExcavatorConstants.PhysicsConstants.Angles.MinStickAngle,
                    ExcavatorConstants.PhysicsConstants.Angles.MaxStickAngle),
                stickMass + bucketMass
            );
        }

        private void MoveBucket(float val)
        {
            MoveArmJoint(
                _excavatorObj.BucketAxis.transform,
                val,
                bucketCylinderBore,
                bucketCylinderStroke,
                () => _excavatorObj.Geometry.leverBucketLength,
                () => _excavatorObj.Geometry.LeverBucketAngle(),
                x => IsRotationStepAllowed(x,
                    _excavatorObj.BucketAxis.localRotation.x * Mathf.Sign(_excavatorObj.BucketAxis.localRotation.w),
                    ExcavatorConstants.PhysicsConstants.Angles.MinBucketAngle,
                    ExcavatorConstants.PhysicsConstants.Angles.MaxBucketAngle),
                bucketMass
            );
        }

        private void MoveSwingJointKinematic(Transform jointTransform, float val)
        {
            var loadMass = 0f;
            if (bucketFill.isBucketFilled) loadMass += ExcavatorConstants.PhysicsConstants.SandLoadMass;
            
            if (Mathf.Abs(val) > ExcavatorConstants.PhysicsConstants.JointMovementThreshold)
            {
                // TODO - calculate the distance to the center of the loaded mass
                var distance = 1f;
                
                // Calculate moment of inertia (I = mass * distance^2)
                float momentOfInertia = loadMass * Mathf.Pow(distance, 2);
                
                float targetAngularVelocity = maxSwingAngularVelocity * val;
                
                // Calculate the torque required to lift the load
                var requiredTorque = momentOfInertia * targetAngularVelocity;

                // Calculate the load factor as a ratio of required torque to maximum motor torque
                // no load means load factor is 0, full load means load factor is 1
                var loadFactor = Mathf.Clamp01(requiredTorque / maxSwingTorque);

                // Adjust angular velocity based on the load factor (inverted so heavier load = slower)
                var adjustedAngularVelocity = maxSwingAngularVelocity * (1 - loadFactor);

                // Calculate the rotation step
                var rotationStep = adjustedAngularVelocity * Time.deltaTime * Mathf.Sign(val);
                jointTransform.Rotate(Vector3.up, rotationStep);
            }
        }

        private void MoveArmJoint(Transform jointTransform, float val, float cylinderBore,
            float cylinderStroke, Func<float> getLeverLength, Func<float> getAngle, Func<float, bool> isRotationInRange,
            float loadMass)
        {
            if (Mathf.Abs(val) > ExcavatorConstants.PhysicsConstants.JointMovementThreshold)
            {
                var rotationStep = CalculateRotationStep(val, cylinderBore, cylinderStroke,
                    getLeverLength, getAngle, loadMass);

                if (isRotationInRange(val)) jointTransform.Rotate(Vector3.right, rotationStep);
            }
        }

        private float CalculateRotationStep(float val, float cylinderBore,
            float cylinderStroke, Func<float> getLeverLength, Func<float> getAngle, float loadMass)
        {
            if (bucketFill.isBucketFilled) loadMass += ExcavatorConstants.PhysicsConstants.SandLoadMass;

            // Convert cylinder bore from millimeters to meters for radius calculation
            var boreRadius = cylinderBore * 0.001f;

            // Convert cylinder stroke from millimeters to meters for volume calculation
            var strokeMeters = cylinderStroke * 0.001f;

            // Convert maximum cylinder pressure from bar to pascals
            var pressurePascals = maxCylinderPressure * 100000f;

            // Calculate the piston area in square meters
            var area = Mathf.PI * Mathf.Pow(boreRadius / 2, 2);

            // Calculate the force exerted by the hydraulic cylinder in Newtons
            var force = pressurePascals * area;

            // Calculate the volume of the hydraulic cylinder in cubic meters, then convert to liters
            var volume = area * strokeMeters * 1000f;

            // Calculate the time it takes to fully pressurize the cylinder in seconds, taking flow rate into account
            var time = volume / oilFlowRate * 60f;

            // Calculate the linear velocity of the hydraulic cylinder in meters per second
            var cylinderVelocity = strokeMeters / time;

            // Convert linear cylinder velocity to angular velocity in degrees per second
            var angularVelocity = cylinderVelocity / getLeverLength() * 180 / Mathf.PI;

            // The maximum torque the system can provide
            var motorTorque = force * getLeverLength() * Mathf.Abs(Mathf.Sin(getAngle()));

            // Change the torque value by the joystick input
            motorTorque *= Mathf.Abs(val);

            // Calculate the torque required to lift the load
            var requiredTorque = loadMass * 9.81f * getLeverLength() * Mathf.Abs(Mathf.Sin(getAngle()));

            // Calculate the load factor as a ratio of required torque to maximum motor torque
            // no load means load factor is 0, full load means load factor is 1
            var loadFactor = Mathf.Clamp01(requiredTorque / motorTorque);

            // Adjust angular velocity based on the load factor (inverted so heavier load = slower)
            var adjustedAngularVelocity = angularVelocity * (1 - loadFactor);

            // Calculate the rotation step
            var rotationStep = adjustedAngularVelocity * Time.deltaTime * val;
            return rotationStep;
        }

        private bool IsRotationStepAllowed(float direction, float angle, float min, float max)
        {
            if (direction > 0) return angle < max;
            return angle > min;
        }
        public void ResetInitialPosition()
        {
            _excavatorObj.ResetInitialPosition();
            _followIKcontroller = false;
            
            _ikExcavator.GetExcavatorObj().SwingAxis.localRotation = _excavatorObj.SwingAxis.localRotation;
            _ikExcavator.GetExcavatorObj().BoomAxis.localRotation = _excavatorObj.BoomAxis.localRotation;
            _ikExcavator.GetExcavatorObj().StickAxis.localRotation = _excavatorObj.StickAxis.localRotation;
            _ikExcavator.GetExcavatorObj().BucketAxis.localRotation = _excavatorObj.BucketAxis.localRotation;
        }
    }
}