using System.Collections;
using Controllers.ExcavatorIKController;
using Excavator.Movement;
using UnityEngine;
using UnityEngine.Rendering;

namespace Controllers
{
    public class ExcavatorGhostController : MonoBehaviour
    {
        public HandleController handleController;
        public IKExcavator ikExcavator;
        public GameObject[] targetObjects;
        public int targetIndex;
        public bool isActive = true;
        [Range(0f, 1f)] public float transparency = 0.7f;
        [ColorUsage(false, true)] public Color ghostColor = Color.gray;
        [ColorUsage(false, true)] public Color successColor = new(0.1718413f, 0.6509434f, 0.09518512f, 1f);

        private int lastTargetIndex;
        private Vector3 lastTargetPosition;
        private Quaternion lastTargetRotation;
        private GameObject targetObject;

        private void Start()
        {
            ValidateComponents();
            InitializeGhost();
        }

        private void Update()
        {
            if (!isActive || targetObjects.Length == 0) return;

            if (lastTargetIndex != targetIndex || HasTargetMoved()) SetNextTarget();
        }

        private void ValidateComponents()
        {
            if (ikExcavator == null) Debug.LogError("IKExcavator object is not set for the Excavator Controller.");
            if (handleController == null)
                Debug.LogError("HandleController object is not set for the Excavator Controller.");
            if (targetObjects.Length == 0) Debug.LogError("No target objects set for the Excavator Controller.");
        }

        private void InitializeGhost()
        {
            SetSuccessfulGhost(false);
            SetGhostExcavatorTransparency(transparency);
            SetNextTarget(0);
            SetGhostActive(isActive);
        }

        private bool HasTargetMoved()
        {
            return lastTargetPosition != targetObject.transform.position ||
                   lastTargetRotation != targetObject.transform.rotation;
        }

        public void SetGhostActive(bool active)
        {
            isActive = active;
            ikExcavator.gameObject.SetActive(active);
            SetRendererEnabled(ikExcavator.gameObject);
            foreach (var target in targetObjects) SetRendererEnabled(target);
        }

        public void SetSuccessfulGhost(bool isSuccessful)
        {
            SetSuccessfulGhostObj(ikExcavator.gameObject, isSuccessful);
        }

        public void SetSuccessfulGhostObj(GameObject obj, bool isSuccessful)
        {
            ChangeYellowMaterialsColor(obj, isSuccessful ? successColor : ghostColor);
        }

        public void SetNextTarget(int index)
        {
            targetIndex = index;
            SetNextTarget();
        }

        private void SetNextTarget()
        {
            if (ikExcavator == null || handleController == null || targetObjects.Length == 0) return;

            lastTargetIndex = targetIndex;
            targetObject = targetObjects[lastTargetIndex % targetObjects.Length];

            if (targetObject == null)
            {
                Debug.LogError("Target object in Excavator Ghost Controller is null.");
                return;
            }

            handleController.transform.position = targetObject.transform.position;
            handleController.transform.rotation = targetObject.transform.rotation;
            handleController.followController = true;

            lastTargetPosition = targetObject.transform.position;
            lastTargetRotation = targetObject.transform.rotation;

            StartCoroutine(UpdateArmIK());
            SetSuccessfulGhost(false);
        }

        private IEnumerator UpdateArmIK()
        {
#if UNITY_EDITOR
            // Code to execute in Unity Editor
            if (ikExcavator != null && handleController != null)
                ikExcavator.ArmIK(handleController.stickControllerTarget.position,
                    handleController.bucketControllerTarget.position);
            else
                Debug.LogError("IKExcavator or HandleController is not set in Excavator Ghost Controller.");
            yield return null;
#else
    yield return new WaitForEndOfFrame();
    if (ikExcavator != null && handleController != null)
    {
        ikExcavator.ArmIK(handleController.stickControllerTarget.position,
            handleController.bucketControllerTarget.position);
    }
    else
    {
        Debug.LogError("IKExcavator or HandleController is not set in Excavator Ghost Controller.");
    }
#endif
        }

        private void SetRendererEnabled(GameObject obj)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers) r.enabled = isActive;
        }

        public void SetGhostExcavatorTransparency(float alpha)
        {
            var renderers = ikExcavator.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
            foreach (var material in r.materials)
                SetMaterialTransparency(material, alpha);
        }

        private void SetMaterialTransparency(Material material, float alpha)
        {
            material.SetFloat(Shader.PropertyToID("_Surface"), 1);
            material.SetFloat(Shader.PropertyToID("_Blend"), 0);
            material.SetInt(Shader.PropertyToID("_SrcBlend"), (int)BlendMode.SrcAlpha);
            material.SetInt(Shader.PropertyToID("_DstBlend"), (int)BlendMode.OneMinusSrcAlpha);
            material.SetInt(Shader.PropertyToID("_ZWrite"), 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;
        }

        private void ChangeYellowMaterialsColor(GameObject obj, Color newColor)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
            foreach (var material in r.materials)
                if (material.name.ToLower().Contains("yellow"))
                {
                    SetMaterialTransparency(material, transparency);
                    newColor.a = transparency;
                    material.color = newColor;
                    material.SetColor(Shader.PropertyToID("_BaseColor"), newColor);
                }
        }
    }
}