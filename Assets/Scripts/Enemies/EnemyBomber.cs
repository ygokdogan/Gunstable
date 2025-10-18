using System;
using System.Collections;
using Interfaces;
using Player;
using UnityEngine;

namespace Enemies
{
    public class EnemyBomber : EnemyBase
    {
        public GameObject explosionEffect;
        private bool isExploding = false;
        
        
        protected override void Awake()
        {
            base.Awake();
            
            isExploding = false;
            anim.SetBool("idle", true);
            anim.SetBool("walk", false);
        }

        protected override void Update()
        {
            base.Update();
            ChasePlayer();
        }

        protected override void FindPlayerInRange()
        {
            base.FindPlayerInRange();
        }

        private void ChasePlayer()
        {
            if (isExploding || !player) return;
            if (isOnLedge || hitWall) return;
            
            var targetPos = player.transform.position;
            var direction = targetPos.x  < transform.position.x  ? Vector2.left : Vector2.right;

            if (direction == Vector2.left && FacingDirection != -1)
            {
                Flip();
            }
            else if (direction == Vector2.right && FacingDirection != 1)
            {
                Flip();
            }
            
            var yDist = targetPos.y - transform.position.y;
            var xDist = targetPos.x - transform.position.x;
            if (yDist > 4f && Mathf.Abs(xDist) < 1.5f)
            {
                var hit = Physics2D.Raycast(transform.position, Vector3.up, 5f, groundLayer);

                if (hit.collider) return;
                
                StartCoroutine(JumpAndExplode(0.2f));
                isExploding = true;
                anim.SetTrigger("explode");
                return;
            }

            if (isOnLedge || hitWall)
            {
                return;
            }

            rb.linearVelocity = direction * moveSpeed;
            anim.SetBool("walk", true);
            anim.SetBool("idle", false);
            
            var dist = Vector2.Distance(targetPos, transform.position);
            if (dist < 1.5f)
            {
                StartCoroutine(Explode(0.2f));
                isExploding = true;
                anim.SetTrigger("explode");
            }
        }

        private IEnumerator JumpAndExplode(float jumpDuration)
        {
            isExploding = true;

            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;

            Vector3 startPos = transform.position;
            Vector3 endPos = startPos + Vector3.up * 5f;
            float t = 0f;

            while (t < jumpDuration)
            {
                transform.position = Vector3.Lerp(startPos, endPos, t / jumpDuration);
                t += Time.deltaTime;
                yield return null;
            }

            var cols = Physics2D.OverlapCircleAll(transform.position, 2.5f, playerLayer);
            if (cols.Length > 0)
            {
                var explosion = Instantiate(explosionEffect, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity);
                foreach (var col in cols)
                {
                    if (col.TryGetComponent(out IDamageable damageable))
                    {
                        damageable.Damage(1);
                    }
                }

                yield return new WaitForSeconds(0.4f);
                Destroy(explosion);
            }

            Destroy(gameObject);
        }


        private IEnumerator Explode(float delay)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            
            yield return new WaitForSeconds(delay);
            var cols = Physics2D.OverlapCircleAll(transform.position, 3, playerLayer);
            if(cols.Length <= 0) yield return null;
            
            var explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            
            foreach (var col in cols)
            { 
                if (col.TryGetComponent(out IDamageable damageable))
                { 
                    damageable.Damage(1);
                    Debug.Log("Damaged");
                }
            }
            
            yield return new WaitForSeconds(0.4f);
            Destroy(explosion.gameObject);
            Destroy(gameObject);
        }

        public override void Die()
        {
            base.Die();
            Destroy(gameObject);
        }
    }
}
