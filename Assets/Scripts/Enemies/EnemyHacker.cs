using UnityEngine;

namespace Enemies
{
    public class EnemyHacker : EnemyBase
    {
        private float patrolIdleTime;
        private float patrolMoveTime;

        private float idleStartTime;
        private float moveStartTime;

        public float fearSpeed = 10f;
        public float fearRange = 5f;

        private float flipDuration = 0.6f;
        private float flipStartTime;

        private int initialFearDir;
        
        private bool isMoving = false;
        private bool decidedInitialDir = false;

        private EnemyHackerState state;

        protected override void Start()
        {
            base.Start();
            state = EnemyHackerState.Idle;
        }

        protected override void Update()
        {
            base.Update();

            if (!player) return;
            
            HandleHacking();

            switch (state)
            {
                case EnemyHackerState.Patrol:
                    HandlePatrolling();
                    anim.SetBool("walk", true);
                    anim.SetBool("idle", false);
                    anim.SetBool("fear", false);
                    break;
                case EnemyHackerState.Idle:
                    HandleIdle();
                    anim.SetBool("walk", false);
                    anim.SetBool("idle", true);
                    anim.SetBool("fear", false);
                    break;
                case EnemyHackerState.Fear:
                    HandleFear();
                    anim.SetBool("walk", false);
                    anim.SetBool("idle", false);
                    anim.SetBool("fear", true);
                    break;
                default:
                    break;
            }
        }

        private void HandleIdle()
        {
            rb.linearVelocity = Vector2.zero;
            
            var dist = Vector2.Distance(player.transform.position, transform.position);
            if (dist < fearRange)
            {
                SwitchState(EnemyHackerState.Fear);
            }
        }

        private void HandleHacking()
        {
            var dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist < agroRange)
            {
                player.SetIsHacked(true);
            }
            else
            {
                player.SetIsHacked(false);
            }
        }

        private void HandleFear()
        { 
            if (!decidedInitialDir)
            {
                initialFearDir = player.transform.position.x > transform.position.x ? -1 : 1;
                decidedInitialDir = true;
                    
                if (FacingDirection != initialFearDir)
                {
                    Flip();
                }
                flipStartTime = Time.time;
            }

            rb.linearVelocityX = FacingDirection * fearSpeed;
            var dist = Vector2.Distance(player.transform.position, transform.position);

            if (Time.time >= flipStartTime + flipDuration || isOnLedge || hitWall)
            {
                Flip();
            }

            if (dist <= 1f)
            {
                var playerDir = player.transform.position.x > transform.position.x ? 1 : -1;
                if (FacingDirection == playerDir)
                {
                    Flip();
                }
            }
            
            if (dist > fearRange + 3f)
            {
                decidedInitialDir = false;
                SwitchState(EnemyHackerState.Idle);
            }
            
        }

        private void HandlePatrolling()
        {
            if (isMoving)
            {
                rb.linearVelocityX = FacingDirection * moveSpeed;
                if (Time.time >= moveStartTime + patrolMoveTime || isOnLedge || hitWall)
                {
                    isMoving = false;
                    idleStartTime = Time.time;
                }
            }
            else if (!isMoving)
            {
                rb.linearVelocity = Vector2.zero;   
                if (Time.time >= idleStartTime + patrolIdleTime)
                {
                    Flip();
                    isMoving = true;
                    moveStartTime = Time.time;
                }
            }
            
            var dist = Vector2.Distance(player.transform.position, transform.position);
            if (dist < fearRange)
            {
                SwitchState(EnemyHackerState.Fear);
            }
        }

        private void SwitchState(EnemyHackerState newState)
        {
            state = newState;
        }

        public override void Die()
        {
            base.Die();
            player.SetIsHacked(false);
            Destroy(gameObject);
        }
    }

    public enum EnemyHackerState
    {
        Patrol,
        Fear,
        Idle
    }
}
