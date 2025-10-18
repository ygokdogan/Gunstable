using UnityEngine;

namespace Enemies
{
    public class EnemyGunner : EnemyBase
    {
        public GameObject bulletPrefab;
        public float shootCooldown = 1f;
        public float minDistanceToFlee = 5f;
        public float stopFleeDistance = 7f;
        public float fleeSpeed = 12f;
        public float moveInBetweenDuration = 0.5f;

        public Animator gunAnim;
        public Transform shootPos;
        private Transform gunTransform;
        
        private float cooldownTimer;
        private float moveInBetweenStartTime;
        private float distanceToPlayer;

        private bool decidedMoveInBetweenDir;
        private int moveInBetweenDir;
        
        private EnemyGunnerState state;

        protected override void Awake()
        {
            base.Awake();
            state = EnemyGunnerState.Idle;
            gunTransform = gunAnim.transform;
            cooldownTimer = shootCooldown;
        }

        protected override void Update()
        {
            base.Update();
            cooldownTimer -= Time.deltaTime;
            
            if (player)
            {
                var shootDir = player.transform.position - transform.position;
                CheckIfShouldFlip(Mathf.RoundToInt(shootDir.x));
            
                float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
            
                gunTransform.rotation = Quaternion.Euler(0, 0, angle);
                var gunSprite = gunTransform.GetComponent<SpriteRenderer>();
                if (gunSprite)
                {
                    gunSprite.flipY = angle > 90 || angle < -90;
                }
                distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
                
                switch (state)
                {
                    case EnemyGunnerState.Idle:
                        HandleIdle();
                        anim.SetBool("idle", true);
                        anim.SetBool("walk", false);
                        break;
                    case EnemyGunnerState.Shoot:
                        HandleShooting();
                        gunAnim.SetTrigger("fire");
                        break;
                    case EnemyGunnerState.Flee:
                        HandleFleeing();
                        anim.SetBool("walk", true);
                        anim.SetBool("idle", false);
                        break;
                    case EnemyGunnerState.MoveInBetween:
                        HandleMoveInBetween();
                        anim.SetBool("walk", true);
                        anim.SetBool("idle", false);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                HandleIdle();
            }

        }

        private void HandleShooting()
        {
            rb.linearVelocity = Vector2.zero;
            var shootDir = player.transform.position - transform.position;
            
            if (CanShoot())
            {
                var bulletObj = Instantiate(bulletPrefab, shootPos.position, Quaternion.identity);
                var bullet = bulletObj.GetComponent<Bullet>();
                
                bullet.Setup(shootDir, gameObject);
                cooldownTimer = shootCooldown;
                SwitchState(EnemyGunnerState.MoveInBetween);
            }
        }

        private void HandleFleeing()
        {
            if (isOnLedge || isOnLedgeBack || hitWallBack || hitWall)
            {
                Flip();
                SwitchState(EnemyGunnerState.Idle);
            }
            
            var fleeDir = (player.transform.position - transform.position).x > 0 ? -1 : 1;
            rb.linearVelocityX = fleeDir * fleeSpeed;
            CheckIfShouldFlip(fleeDir);

            if (stopFleeDistance < distanceToPlayer)
            {
                if (CanShoot())
                {
                    SwitchState(EnemyGunnerState.Shoot);
                }
                else
                {
                    SwitchState(EnemyGunnerState.Idle);
                }
            }
        }

        private void HandleMoveInBetween()
        {
            if (!decidedMoveInBetweenDir)
            {
                var rand = Random.Range(0, 2);
                moveInBetweenDir = rand % 2 == 0 ? -1 : 1;
                decidedMoveInBetweenDir = true;
                moveInBetweenStartTime = Time.time;
            }
            
            CheckIfShouldFlip(moveInBetweenDir);

            rb.linearVelocityX = moveInBetweenDir * moveSpeed;

            if (isOnLedge || isOnLedgeBack || hitWallBack || hitWall)
            {
                Flip();
                decidedMoveInBetweenDir = false;
                SwitchState(EnemyGunnerState.Idle);
            }
            else if (distanceToPlayer <= minDistanceToFlee)
            {
                decidedMoveInBetweenDir = false;
                SwitchState(EnemyGunnerState.Flee);
            }

            if (Time.time >= moveInBetweenStartTime + moveInBetweenDuration)
            {
                decidedMoveInBetweenDir = false;
                SwitchState(EnemyGunnerState.Idle);
            }
        }

        private void HandleIdle()
        {
            rb.linearVelocity = Vector2.zero;
            
            if (player)
            {
                if (CanShoot())
                {
                    SwitchState(EnemyGunnerState.Shoot);
                }

                if (distanceToPlayer <= minDistanceToFlee && !(isOnLedge || isOnLedgeBack || hitWallBack || hitWall))
                {
                    SwitchState(EnemyGunnerState.Flee);
                }
            }
        }

        private void SwitchState(EnemyGunnerState newState)
        {
            state = newState;
        }

        private bool CanShoot()
        {
            return cooldownTimer <= 0;
        }

        public override void Die()
        {
            base.Die();
            Destroy(gameObject);
        }
    }

    public enum EnemyGunnerState
    {
        Idle,
        Shoot,
        Flee,
        MoveInBetween
    }
}
