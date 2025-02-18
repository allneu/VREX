using System.Collections.Generic;
using DataCollectors;
using UnityEngine;
using UnityEngine.Rendering;

namespace Games.SandGame
{
    public class BucketFill : MonoBehaviour
    {
        public GameObject bucketSand;

        public GameObject smallSandPile;
        public GameObject dropSandPoint;

        private readonly List<GameObject> _createdSandPiles = new();
        private GameObject _bucketSandGhost;
        private bool _canBeFilled;

        private bool _isBucketFilled;

        public bool isBucketFilled
        {
            get => _isBucketFilled;
            set
            {
                _isBucketFilled = value;
                if (value) canBeFilled = false;
                bucketSand.SetActive(_isBucketFilled);
                if (_isBucketFilled) GameLogger.LogConditional("BUCKET IS FILLED");
            }
        }

        public bool canBeFilled
        {
            set
            {
                _canBeFilled = value;
                if (_bucketSandGhost != null) _bucketSandGhost.SetActive(_canBeFilled);
            }
            get => _canBeFilled;
        }

        private void Awake()
        {
            if (bucketSand == null) Debug.LogError("BucketSand object is not set for the BucketFill.");
            if (smallSandPile == null) Debug.LogError("SmallSandPile object is not set for the BucketFill.");
            if (dropSandPoint == null) Debug.LogError("DropSandPoint object is not set for the BucketFill.");

            bucketSand.SetActive(false);
            smallSandPile.SetActive(false);
            isBucketFilled = false;
            canBeFilled = false;

            _bucketSandGhost = Instantiate(bucketSand, bucketSand.transform.position, bucketSand.transform.rotation,
                bucketSand.transform.parent);
            if (_bucketSandGhost == null)
            {
                Debug.LogError("Failed to duplicate a ghost object for the bucket sand.");
            }
            else
            {
                _bucketSandGhost.name = bucketSand.name + "_Ghost";

                // Change material to purple for all children
                ChangeMaterialColor(_bucketSandGhost);

                _bucketSandGhost.SetActive(false);
            }
        }

        private void Update()
        {
            var angle = Vector3.Angle(transform.rotation * Vector3.up, Vector3.down);
            if (canBeFilled && angle <= GameConstants.BucketGame.EmptyFillBucketAngle) isBucketFilled = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            var bucketRotation = transform.rotation;
            var angle = Vector3.Angle(bucketRotation * Vector3.up, Vector3.down);
            if (!isBucketFilled && !canBeFilled
                                && angle > GameConstants.BucketGame.EmptyFillBucketAngle
                                && (other.CompareTag("SandPile"))) // other.CompareTag("Ground")
            {
                canBeFilled = true;
                GameLogger.LogConditional("BUCKET CAN BE FILLED");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (canBeFilled && other.CompareTag("SandPile"))
            {
                canBeFilled = false;
                GameLogger.LogConditional("BUCKET CANNOT BE FILLED");
            }
        }

        private void ChangeMaterialColor(GameObject obj)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                var materials = renderer.materials;
                for (var i = 0; i < materials.Length; i++)
                {
                    materials[i] = new Material(materials[i]);
                    materials[i].color = new Color(0f, 1f, 0f, 0.5f);
                    SetMaterialTransparency(materials[i], 0.5f);
                }

                renderer.materials = materials;
            }
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

            var color = material.color;
            color.a = alpha;
            material.color = color;
            material.SetColor(Shader.PropertyToID("_BaseColor"), color);
        }

        public void InitBucketFill()
        {
            bucketSand.SetActive(false);
            if (_bucketSandGhost != null) _bucketSandGhost.SetActive(false);
            DestroySmallSandPiles();
            isBucketFilled = false;
            canBeFilled = false;
        }

        public void FinishBucketFill()
        {
            bucketSand.SetActive(false);
            if (_bucketSandGhost != null) _bucketSandGhost.SetActive(false);
            DestroySmallSandPiles();
            isBucketFilled = false;
            canBeFilled = false;
        }

        public Vector3 EmptyBucket()
        {
            isBucketFilled = false;
            canBeFilled = false;
            return SetSmallSandPile();
        }

        private Vector3 SetSmallSandPile()
        {
            var dropSandPosition = dropSandPoint.transform.position;
            var newSandPile = Instantiate(smallSandPile, dropSandPosition, smallSandPile.transform.rotation,
                smallSandPile.transform.parent);
            if (newSandPile == null)
            {
                Debug.LogError("Failed to instantiate a new small sand pile.");
                return Vector3.zero;
            }

            newSandPile.SetActive(true);
            _createdSandPiles.Add(newSandPile);
            return dropSandPosition;
        }

        public void DestroySmallSandPiles()
        {
            foreach (var sandPile in _createdSandPiles) Destroy(sandPile);
            _createdSandPiles.Clear();
        }
    }
}