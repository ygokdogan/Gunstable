using System;
using Interfaces;
using Player;
using UnityEngine;
using Utilities;

namespace Enemies
{
    public abstract class EnemyBase : MonoBehaviour, IDamageable
    {
        [Header("Stats")] 
        public int maxHealth;
        public int currentHealth;
        public float moveSpeed;
        
        [Header("Layers")]
        public LayerMask playerLayer;
        public LayerMask groundLayer;
        protected Rigidbody2D rb;

        public float agroRange; 
        protected PlayerController player;
        protected Animator anim;

        protected bool isGrounded;
        protected bool hitWall;
        protected bool isOnLedge;
        protected bool isOnLedgeBack;
        protected bool hitWallBack;
        
        [Header("Collision Checks")]
        public Transform groundCheck;
        public Transform wallCheck;
        public Transform ledgeCheck;
        public Transform ledgeCheckBack;
        public Transform wallCheckBack;
        
        public float groundCheckRadius;
        public float wallCheckDistance;

        public ParticleSystem bloodParticles;

        private FlashEffect flash;
        
        public int FacingDirection { get; private set; }

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponentInChildren<Animator>();
            flash = GetComponentInChildren<FlashEffect>();
        }

        protected virtual void Start()
        {
            currentHealth = maxHealth;
            FacingDirection = 1;
        }

        protected virtual void Update()
        {
            if (!player)
            {
                FindPlayerInRange();
            }
        }

        protected virtual void FixedUpdate()
        {
            CollisionChecks();
        }

        protected virtual void FindPlayerInRange()
        {
            var playerFound = Physics2D.OverlapCircleAll(transform.position, agroRange, playerLayer);
            if (playerFound.Length <= 0) return;
            
            player = playerFound[0].GetComponent<PlayerController>();
        }

        private void CollisionChecks()
        {
            isGrounded = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, groundLayer).Length > 0;
            isOnLedge = !Physics2D.Raycast(ledgeCheck.position, Vector2.down, wallCheckDistance, groundLayer);
            isOnLedgeBack = !Physics2D.Raycast(ledgeCheckBack.position, Vector2.down, wallCheckDistance, groundLayer);
            hitWall = Physics2D.Raycast(wallCheck.position, Vector2.right * FacingDirection, wallCheckDistance, groundLayer);
            hitWallBack = Physics2D.Raycast(wallCheckBack.position, Vector2.right * -FacingDirection, wallCheckDistance, groundLayer);
        }
        
        protected void CheckIfShouldFlip(int x)
        {
            if (x > 0 && FacingDirection != 1 || x < 0 && FacingDirection != -1) Flip();
        }

        protected void Flip()
        {
            transform.Rotate(0, 180, 0);
            FacingDirection *= -1;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, agroRange);
        }

        public virtual void Damage(int damage)
        {
            currentHealth -= damage;
            CameraShake.Shake(5f, 0.1f);
            flash?.TriggerFlash();
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public virtual void Die()
        {
            var ps = Instantiate(bloodParticles, transform.position, Quaternion.identity);
            ps.Play();
            Destroy(ps.gameObject, 4f);
        }
    }
}
