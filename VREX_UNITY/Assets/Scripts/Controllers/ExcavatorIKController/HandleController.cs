using UnityEngine;
using UnityEngine.Animations;

namespace Controllers.ExcavatorIKController
{
    public class HandleController : MonoBehaviour
    {
        public Transform stickControllerTarget;
        public Transform bucketControllerTarget;
        public GameObject excDirection;

        [HideInInspector] public bool followController;
        private ParentConstraint bucketParentConstraint;

        private float initialTargetDistance;
        private ParentConstraint stickParentConstraint;

        private void Awake()
        {
            followController = false;
            InitializeControllerTarget(stickControllerTarget, out stickParentConstraint, "Stick");
            InitializeControllerTarget(bucketControllerTarget, out bucketParentConstraint, "Bucket");

            if (bucketControllerTarget == null || stickControllerTarget == null)
            {
                Debug.LogError("Controller targets not set in Handle Controller");
                return;
            }

            if (excDirection == null)
            {
                Debug.LogError("Excavator direction not set in Handle Controller");
                return;
            }

            initialTargetDistance = Vector3.Distance(stickControllerTarget.position, bucketControllerTarget.position);
        }

        private void Update()
        {
            if (stickControllerTarget == null || bucketControllerTarget == null)
            {
                Debug.LogError("Controller targets not set in Handle Controller");
                return;
            }

            if (followController)
            {
                SetParentConstraintsActive(false);
                FollowController();
            }
            else
            {
                SetParentConstraintsActive(true);
                AlignWithTargets();
            }
        }

        private void InitializeControllerTarget(Transform target, out ParentConstraint parentConstraint,
            string targetName)
        {
            if (target == null)
            {
                Debug.LogError($"{targetName} controller target not set in Handle Controller");
                parentConstraint = null;
                return;
            }

            parentConstraint = target.GetComponent<ParentConstraint>();
            if (parentConstraint == null)
                Debug.LogError($"ParentConstraint not found on {targetName} Controller Target");
        }

        private void SetParentConstraintsActive(bool isActive)
        {
            if (stickParentConstraint != null)
                stickParentConstraint.constraintActive = isActive;

            if (bucketParentConstraint != null)
                bucketParentConstraint.constraintActive = isActive;
        }

        private void FollowController()
        {
            var handlePosition = transform.position;
            bucketControllerTarget.position = handlePosition;

            var angle = Vector3.SignedAngle(transform.up, Vector3.up, bucketControllerTarget.right);
            bucketControllerTarget.rotation = excDirection.transform.rotation;
            bucketControllerTarget.localRotation = Quaternion.Euler(-angle, 0, 0);

            stickControllerTarget.position = handlePosition + bucketControllerTarget.forward * initialTargetDistance;
        }

        private void AlignWithTargets()
        {
            transform.position = bucketControllerTarget.position;
        }
    }
}