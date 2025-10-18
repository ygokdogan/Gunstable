using System;
using System.Collections.Generic;
using Input;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player
{
    public class PlayerCombatManager : MonoBehaviour
    {
        private InputHandler input;
        private PlayerController player;

        public Transform gunTransform;
        public Animator gunAnim;
        public Transform shootPos;
        public GameObject[] bullets;
        
        private float lastShootTime = -2f;
        
        private void Awake()
        {
            player = GetComponent<PlayerController>();
            input = GetComponent<InputHandler>();
            gunTransform = gunAnim.transform;
        }

        private void Update()
        {
            if (input.ShootInput && CanShoot())
            {
                Shoot();
                gunAnim.SetTrigger("fire");
            }
            
            CheckIfShouldFlip();
            Vector2 direction = Camera.main.ScreenToWorldPoint(input.MousePosition) - gunTransform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            gunTransform.rotation = Quaternion.Euler(0, 0, angle);
            var gunSprite = gunTransform.GetComponent<SpriteRenderer>();
            if (gunSprite)
            {
                gunSprite.flipY = angle > 90 || angle < -90;
            }
        }

        private void CheckIfShouldFlip()
        {
            if (DetermineShootDirection().x > 0 && player.FacingDirection != 1)
            {
                player.Flip();
            }

            if (DetermineShootDirection().x < 0 && player.FacingDirection != -1)
            {
                player.Flip();
            }
        }

        private void Shoot()
        {
            input.UseShootInput();
            lastShootTime = Time.time;
            
            var bulletPrefab = RandomizeBullet();
            var bulletObj =Instantiate(bulletPrefab, shootPos.position, Quaternion.identity);
            var bullet = bulletObj.GetComponent<Bullet>();

            var shootDir = DetermineShootDirection();
            
            switch (bullet.type)
            {
                case Bullet.BulletType.Default:
                    bullet.Setup(shootDir, gameObject);
                    break;

                case Bullet.BulletType.Pump:
                    ShootPumpShot(bullet, shootDir);
                    break;

                case Bullet.BulletType.Recoil:
                    ShootRecoilShot(bullet, shootDir);
                    break;

                case Bullet.BulletType.Skip:
                    Debug.Log("Skipped");
                    break;
            }
        }

        private void ShootPumpShot(Bullet baseBullet, Vector2 direction)
        {
            int pelletCount = 5;
            float totalSpreadAngle = 20f;
            
            float angleStep = totalSpreadAngle / (pelletCount - 1);
            float startAngle = -totalSpreadAngle / 2;
            

            for (int i = 0; i < pelletCount; i++)
            {
                float angle = startAngle + (angleStep * i);
                Quaternion rotation = Quaternion.Euler(0,0, angle);
                Vector2 spreadDir = rotation * direction;

                var pelletObj = Instantiate(baseBullet.gameObject, shootPos.position, Quaternion.identity);
                var pellet = pelletObj.GetComponent<Bullet>();
                pellet.type = Bullet.BulletType.Pump;
                pellet.Setup(spreadDir, gameObject);
            }
            
            Destroy(baseBullet.gameObject);
        }

        private void ShootRecoilShot(Bullet baseBullet, Vector2 direction)
        {
            player.ApplyRecoil(-direction, 30);
            baseBullet.Setup(direction, gameObject);
        }

        private bool CanShoot()
        {
            return Time.time >= lastShootTime + player.data.shootCooldown;
        }

        private Vector2 DetermineShootDirection()
        {
            var mousePos = Camera.main.ScreenToWorldPoint(input.MousePosition);
            var shootDir = mousePos - shootPos.position;
            return shootDir.normalized;
        }

        private GameObject RandomizeBullet()
        {
            var rand = Random.Range(0, 100);
            switch (rand)
            {
                case < 15:
                    return bullets[3]; // Skip
                case < 35:
                    return bullets[2]; // Recoil
                case < 55:
                    return bullets[1]; // Pump
                case < 100:
                    return bullets[0]; // Default
                default:
                {
                    return bullets[0];
                    break;
                }
            }
        }
    }
    
}
