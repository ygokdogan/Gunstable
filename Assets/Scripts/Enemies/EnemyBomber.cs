using System.Collections;
using Interfaces;
using Player;
using UnityEngine;

namespace Enemies
{
    public class EnemyBomber : EnemyBase
    {
        [Header("Bomber Settings")]
        public GameObject explosionEffect;
        public float explodeRange = 1.5f;
        public float explodeRadius = 3f;
        public float jumpExplodeDelay = 0.2f;
        public float explodeDelay = 0.2f;
        
        private EnemyBomberState state;
        private bool isExploding;
        private bool moveAfterChase;

        protected override void Awake()
        {
            base.Awake();
            state = EnemyBomberState.Idle;
            isExploding = false;
            moveAfterChase = false;
            
            anim.SetBool("idle", true);
            anim.SetBool("walk", false);
        }

        protected override void Update()
        {
            base.Update();

            switch (state)
            {
                case EnemyBomberState.Idle:
                    HandleIdle();
                    break;
                case EnemyBomberState.Chase:
                    HandleChase();
                    break;
                case EnemyBomberState.Explode:
                    // handled via coroutine
                    break;
            }
        }

        #region State Handlers

        private void HandleIdle()
        {
            anim.SetBool("idle", true);
            anim.SetBool("walk", false);

            if (player)
            {
                if (moveAfterChase)
                {
                    Flip();
                    rb.linearVelocityX = FacingDirection * moveSpeed;
                    moveAfterChase = false;
                }

                if (!moveAfterChase)
                {
                    float dist = Vector2.Distance(player.transform.position, transform.position);
                
                    if (dist <= agroRange  && !(isOnLedge || hitWall))
                    {
                        SwitchState(EnemyBomberState.Chase);
                    }
                }
            }
        }

        private void HandleChase()
        {
            if (isExploding || !player) return;
            if (isOnLedge || hitWall)
            {
                rb.linearVelocity = Vector2.zero;
                moveAfterChase = true;
                SwitchState(EnemyBomberState.Idle);
                return;
            }
            
            var targetPos = player.transform.position;
            var direction = targetPos.x < transform.position.x ? Vector2.left : Vector2.right;

            CheckIfShouldFlip((int)Mathf.Sign(direction.x));
            
            float dist = Vector2.Distance(targetPos, transform.position);

            // --- Normal Explosion ---
            if (dist < explodeRange)
            {
                StartCoroutine(Explode(explodeDelay));
                isExploding = true;
                anim.SetTrigger("explode");
                SwitchState(EnemyBomberState.Explode);
                return;
            }

            // --- Chase movement ---
            rb.linearVelocity = direction * moveSpeed;
            anim.SetBool("walk", true);
            anim.SetBool("idle", false);
        }

        #endregion

        #region Coroutines
        private IEnumerator Explode(float delay)
        {
            isExploding = true;
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            
            yield return new WaitForSeconds(delay);
            TriggerExplosion(explodeRadius);
        }

        private void TriggerExplosion(float radius, float offsetY = 0f)
        {
            var explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);

            var cols = Physics2D.OverlapCircleAll(transform.position, radius, playerLayer);
            foreach (var col in cols)
            {
                if (col.TryGetComponent(out IDamageable damageable))
                {
                    damageable.Damage(3);
                    if (player)
                    {
                        var dir = player.transform.position - transform.position;
                        player.ApplyRecoil(dir, 15f);
                    }
                }
            }

            Destroy(gameObject, .3f);
            //StartCoroutine(DestroyAfterDelay(explosion, 3));
        }

        private IEnumerator DestroyAfterDelay(GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(obj);
            Destroy(gameObject);
        }

        #endregion

        #region Utility

        private void SwitchState(EnemyBomberState newState)
        {
            state = newState;
        }

        private void CheckIfShouldFlip(int direction)
        {
            if (direction == 0) return;

            if (direction < 0 && FacingDirection != -1)
            {
                Flip();
            }
            else if (direction > 0 && FacingDirection != 1)
            {
                Flip();
            }
        }

        public override void Die()
        {
            base.Die();
            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explodeRadius);
        }

        #endregion
    }

    public enum EnemyBomberState
    {
        Idle,
        Chase,
        JumpExplode,
        Explode
    }
}
