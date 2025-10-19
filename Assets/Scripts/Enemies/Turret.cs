using System.Collections;
using TMPro;
using UnityEngine;

namespace Enemies
{
    public class Turret : EnemyBase
    {
        private float cooldownTimer;
        public float cooldown;
        public GameObject bulletPrefab;
        public Transform shootPos;
        public GameObject cantBeDamagedText;

        protected override void Update()
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                StartCoroutine(BurstShot(3, 0.4f));
                cooldownTimer = cooldown;
            }
        }

        private IEnumerator BurstShot(int count, float shootDelay)
        {
            for (int i = 0; i < count; i++)
            {
                Fire();
                yield return new WaitForSeconds(shootDelay);
            }
            yield return null;
        }

        private void Fire()
        {
            var bulletObj = Instantiate(bulletPrefab, shootPos.position, Quaternion.identity);
            var bullet = bulletObj.GetComponent<Bullet>();
            
            bullet.Setup(transform.right, gameObject);
        }

        public override void Die()
        {
            
        }

        public override void Damage(int damage)
        {
            StartCoroutine(DisplayText(.7f));
        }

        private IEnumerator DisplayText(float delay)
        {
            cantBeDamagedText.SetActive(true);
            yield return new WaitForSeconds(delay);
            cantBeDamagedText.SetActive(false);
        }
    }
}
