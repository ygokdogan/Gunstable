using UnityEngine;

namespace Utilities
{
    public class FlashEffect : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public Material flashMaterial;
        private Material originalMaterial;
        public float flashDuration = 0.1f;

        private float flashTimer;
        private bool isFlashing;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            originalMaterial = spriteRenderer.material;
        }

        private void Update()
        {
            if (isFlashing)
            {
                flashTimer -= Time.deltaTime;
                if (flashTimer <= 0)
                {
                    EndFlash();
                }
            }
        }

        public void TriggerFlash()
        {
            spriteRenderer.material = flashMaterial;
            flashTimer = flashDuration;
            isFlashing = true;
        }

        private void EndFlash()
        {
            spriteRenderer.material = originalMaterial;
            isFlashing = false;
        }
    }
}
