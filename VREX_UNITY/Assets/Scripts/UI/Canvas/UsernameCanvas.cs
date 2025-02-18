using Microsoft.MixedReality.Toolkit.Experimental.UI;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UsernameCanvas : MonoBehaviour
    {
        public TMP_InputField usernameInputField;
        public NonNativeKeyboard nonNativeKeyboard;

        private void Start()
        {
            if (usernameInputField == null) Debug.LogError("UsernameInputField is not set in UsernameCanvas");

            nonNativeKeyboard.InputField = usernameInputField;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            NonNativeKeyboard.Instance.PresentKeyboard();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public string GetUsername()
        {
            var username = usernameInputField.text;
            Debug.Log("Username: " + username);
            return username;
        }
    }
}