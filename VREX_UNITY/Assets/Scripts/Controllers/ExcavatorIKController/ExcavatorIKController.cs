using Excavator.Movement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace Controllers.ExcavatorIKController
{
    public class ExcavatorIKController : MonoBehaviour
    {
        public HandleController handleController;
        public IKExcavator ikExcavator;
        public GameObject padestal;
        public MiniatureExcavatorStatusSimulator excavatorStatusSimulator;
        public PhysicsExcavator physicsExcavator;

        [SerializeField] [Tooltip("Events to trigger when the stick target position changes")]
        private readonly ValueChangeEvent _mOnControllerSelect = new();

        private Transform bucketTarget;

        private bool controllerActive;
        private Transform stickTarget;

        /// <summary>
        ///     Events to trigger when the stick target position changes
        /// </summary>
        public ValueChangeEvent onControllerSelect => _mOnControllerSelect;

        private void Awake()
        {
            if (handleController == null)
                Debug.LogError("HandleController object is not set for the Excavator Controller.");
            if (ikExcavator == null) Debug.LogError("IKExcavator object is not set for the Excavator Controller.");
            if (padestal == null) Debug.LogWarning("Padestal object is not set for the Excavator Controller.");
            if (excavatorStatusSimulator == null)
                Debug.LogWarning("ExcavatorStatusSimulator object is not set for the Excavator Controller.");
        }

        private void Start()
        {
            controllerActive = false;
            if (handleController != null)
            {
                stickTarget = handleController.stickControllerTarget.transform;
                bucketTarget = handleController.bucketControllerTarget.transform;
            }
            else
            {
                Debug.LogError("HandleController object is not set for the Excavator Controller.");
            }

            if (ikExcavator == null) Debug.LogError("IKExcavator object is not set for the Excavator Controller.");
        }

        private void Update()
        {
            if (stickTarget != null && bucketTarget != null)
            {
                if (controllerActive) ikExcavator.ArmIK(stickTarget.position, bucketTarget.position);
            }
            else
            {
                Debug.LogError(
                    "Could not calculate the IK position. Please assign the HandleController with StickTarget and BucketTarget correctly.");
            }

            gameObject.transform.rotation = physicsExcavator.transform.rotation;
        }

        public IKExcavator GetIKExcavator()
        {
            if (ikExcavator == null) Debug.LogError("IK excavator in the excavator controller not initialized yet.");

            return ikExcavator;
        }

        public void activateXRGrab(bool activate)
        {
            var grabInteractable = handleController.GetComponent<XRGrabInteractable>();
            grabInteractable.enabled = activate;
        }

        public void InvokeOnStickControllerSelect()
        {
            controllerActive = true;
            handleController.followController = true;
            onControllerSelect.Invoke();
        }

        public void InvokeOnStickControllerDeselect()
        {
            controllerActive = false;
            handleController.followController = false;
        }

        public void Show()
        {
            ikExcavator.GetExcavatorObj().Show();
            handleController.gameObject.SetActive(true);
            if (padestal != null) padestal.SetActive(true);
            if (excavatorStatusSimulator != null) excavatorStatusSimulator.gameObject.SetActive(true);
        }

        public void Hide()
        {
            handleController.gameObject.SetActive(false);
            ikExcavator.GetExcavatorObj().Hide();
            if (padestal != null) padestal.SetActive(false);
            if (excavatorStatusSimulator != null) excavatorStatusSimulator.gameObject.SetActive(false);
        }

        public class ValueChangeEvent : UnityEvent
        {
        }
    }
}