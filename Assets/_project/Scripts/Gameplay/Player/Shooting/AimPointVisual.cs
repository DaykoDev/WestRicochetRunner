using UnityEngine;

namespace WestRicochet.Gameplay.Player.Shooting
{
    public class AimPointVisual : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [Header("Visual Settings")]
        [SerializeField, Min(0.01f)] private float _worldSize = 0.6f;
        [SerializeField, Min(0f)] private float _cameraOffset = 0.05f;

        private void Awake()
        {
            if (_spriteRenderer == null)
            {
                Debug.LogError("AimPointVisual: SpriteRenderer is not assigned.", this);

                enabled = false;
            }

            transform.localScale = Vector3.one * _worldSize;
            SetVisible(false);
        }

        public void UpdateVisual(Vector3 aimPosition, Transform cameraTransform)
        {
            transform.position = aimPosition - cameraTransform.forward * _cameraOffset;
            transform.rotation = cameraTransform.rotation;
        }

        public void SetVisible(bool isVisible)
        {
            _spriteRenderer.enabled = isVisible;
            enabled = isVisible;
        }
    }
}