using UnityEngine;
using UnityEngine.XR.Content.Interaction;

namespace Controllers
{
    public class JoystickControllers : MonoBehaviour
    {
        public XRJoystick joystickLeftController;
        public XRJoystick joystickRightController;

        private void Start()
        {
            CheckJoystick(joystickLeftController, "JoystickLeftController");
            CheckJoystick(joystickRightController, "JoystickRightController");
        }

        private void CheckJoystick(XRJoystick joystick, string name)
        {
            if (joystick == null) Debug.LogError($"{name} object is not set for the Joystick Controller.");
        }

        public void ActivateXRGrab(bool activate)
        {
            ToggleJoystick(joystickLeftController, "Joystick Left Controller", activate);
            ToggleJoystick(joystickRightController, "Joystick Right Controller", activate);
        }

        private void ToggleJoystick(XRJoystick joystick, string name, bool activate)
        {
            var joystickComp = joystick?.GetComponent<XRJoystick>();
            if (joystickComp == null) Debug.LogError($"XRJoystick component is not set for the {name}.");
            else joystickComp.enabled = activate;
        }
    }
}