using UnityEngine;
using UnityEngine.Rendering;

namespace Excavator.Movement
{
    public class MiniatureExcavatorStatusSimulator : MonoBehaviour
    {
        public PhysicsExcavator physicsExcavator;

        [SerializeField] private Color excavatorColor = Color.white;

        private Excavator _statusExcavator;

        private readonly float _transparency = 0.3f;
        
        private void Awake()
        {
            _statusExcavator = new Excavator(gameObject);
            ChangeMaterialsColor(gameObject, excavatorColor);
        }

        private void Start()
        {
            if (physicsExcavator == null)
            {
                Debug.LogError("PhysicsExcavator reference not set for MiniatureExcavatorStatusSimulator.");
                return;
            }
        }

        private void Update()
        {
            if (physicsExcavator == null) return;
            SimulateExcavator();
        }

        private void SimulateExcavator()
        {
            float interpolationFactor = Time.deltaTime * 5.0f;

            _statusExcavator.SwingAxis.localRotation = Quaternion.Slerp(
                _statusExcavator.SwingAxis.localRotation,
                physicsExcavator.GetExcavatorObj().SwingAxis.localRotation,
                interpolationFactor
            );

            _statusExcavator.BoomAxis.localRotation = Quaternion.Slerp(
                _statusExcavator.BoomAxis.localRotation,
                physicsExcavator.GetExcavatorObj().BoomAxis.localRotation,
                interpolationFactor
            );

            _statusExcavator.StickAxis.localRotation = Quaternion.Slerp(
                _statusExcavator.StickAxis.localRotation,
                physicsExcavator.GetExcavatorObj().StickAxis.localRotation,
                interpolationFactor
            );

            _statusExcavator.BucketAxis.localRotation = Quaternion.Slerp(
                _statusExcavator.BucketAxis.localRotation,
                physicsExcavator.GetExcavatorObj().BucketAxis.localRotation,
                interpolationFactor
            );

            _statusExcavator.OrientExcavatorCylinders();
        }

        private void SetMaterialTransparency(Material material)
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

        private void ChangeMaterialsColor(GameObject obj, Color newColor)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
            foreach (var material in r.materials)
            {
                SetMaterialTransparency(material);
                newColor.a = _transparency;
                material.color = newColor;
                material.SetColor(Shader.PropertyToID("_BaseColor"), newColor);
            }
        }
    }
}