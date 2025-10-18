using System;
using System.Collections;
using Input;
using Interfaces;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player
{
    public class PlayerController : MonoBehaviour, IDamageable
    {
        public PlayerData data;
        private PlayerAnimatorController animController;
        private Rigidbody2D rb;
        private InputHandler input;

        [Header("Collisions")] public LayerMask groundLayer;
        public Transform groundCheck;
        public float groundCheckRadius;
        [Space] public bool canJump = true;
        public bool isJumping = false;
        public bool isGrounded = true;
        public bool canMove = true;
        private bool isRolling = false;
        private bool jetpackFailed = false;
        private bool isHacked = false;
        private bool hasSlowedTime = false;
        
        private float recoilStartTime;
        private float jumpStartTime;
        private float lastRollTime;

        private Vector2 damageSourcePos;

        private int currentHealth;
        [Range(1, 6)] private int jetpackDice;
        public int FacingDirection { get; private set; }

        [Header("Dice")]
        public SpriteRenderer dice;
        public Sprite[] diceSprites;
        private Coroutine diceCoroutine;

        private void Awake()
        {
            input = GetComponent<InputHandler>();
            rb = GetComponent<Rigidbody2D>();
            animController = GetComponent<PlayerAnimatorController>();

            jumpStartTime = -2f;
            FacingDirection = 1;
        }

        private void Start()
        {
            currentHealth = data.maxHealth;
            diceCoroutine = null;
        }

        private void Update()
        {
            HandleAllMovement();

            if (!canMove && (rb.linearVelocity.magnitude < 1f || Time.time >= recoilStartTime + 0.4f))
            {
                canMove = true;
                rb.linearDamping = 0f;
            }
            
            if (data.acquiredJetpack)
            {
                animController.UnlockJetpack();
            }

            if (isGrounded && dice.gameObject.activeSelf)
            {
                dice.gameObject.SetActive(false);
            }
        }

        private void FixedUpdate()
        {
            CheckCollisions();
        }

        #region Movement

        private void HandleAllMovement()
        {
            if (!canMove) return;

            HandleHorizontalMovement();
            HandleJumping();
            HandleJetpack();
        }

        private void HandleHorizontalMovement()
        {
            rb.linearVelocityX = input.MoveInput.x * data.moveSpeed;
        }

        private void HandleJumping()
        {
            if (data.acquiredJetpack && !isHacked) return;

            if (input.JumpInput && canJump)
            {
                input.UseJumpInput();
                isJumping = true;
                jumpStartTime = Time.time;
            }

            if (isJumping)
            {
                rb.linearVelocityY = data.jumpForce;

                if (Time.time >= jumpStartTime + data.jumpDuration)
                {
                    isJumping = false;
                }
            }

            canJump = isGrounded;
        }

        private void HandleJetpack()
        {
            if (!data.acquiredJetpack) return;

            if (isGrounded && jetpackFailed)
            {
                jetpackFailed = false;
            }

            if (jetpackFailed || isHacked) return;

            if (input.JumpInput)
            {
                rb.linearVelocityY = data.jetpackForce;
                
                animController.anim.SetBool("fly", true);
                animController.anim.SetBool("idle", false);
                
                dice.gameObject.SetActive(true);

                if (!isRolling && Time.time >= lastRollTime + 0.05f)
                {
                    isRolling = true;
                    diceCoroutine = StartCoroutine(RollDice(1f));
                }
            }

            if (input.JumpInputReleased && rb.linearVelocityY > 0.1f)
            {
                rb.linearVelocityY *= 0.5f;
                
                animController.anim.SetBool("fly", false);
                animController.anim.SetBool("idle", true);

                if (isRolling)
                {
                    isRolling = false;
                    lastRollTime = Time.time;
                    StopCoroutine(diceCoroutine);
                    diceCoroutine = null;
                }
            }
        }

        public void ApplyRecoil(Vector2 direction, float force)
        {
            canMove = false;
            rb.linearDamping = 1f;
            rb.linearVelocity = direction.normalized * force;
            recoilStartTime = Time.time;
        }

        #endregion

        public void Flip()
        {
            transform.Rotate(0, 180, 0);
            FacingDirection *= -1;
        }

        #region Collision

        private void CheckCollisions()
        {
            isGrounded = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, groundLayer).Length > 0;
        }

        #endregion

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        public void Damage(int damage)
        {
            currentHealth -= damage;
            
            var knockbackDir = -(damageSourcePos - (Vector2)transform.position);
            knockbackDir.Normalize();

            ApplyRecoil(knockbackDir, 20f);
            if (!hasSlowedTime)
            {
                StartCoroutine(SlowTimeOnDamage(0.2f));
            }
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public void SetDamageSource(Vector2 source)
        {
            damageSourcePos = source;
        }

        public void SetIsHacked(bool hacked)
        {
            isHacked = hacked;
        }
        

        public void Die()
        {
            Destroy(gameObject);
        }

        public void UnlockJetpack()
        {
            animController.UnlockJetpack();
            data.acquiredJetpack = true;
        }

        private IEnumerator RollDice(float delay)
        {
            while (isRolling)
            {
                jetpackDice = Random.Range(1, 7);
                dice.sprite = diceSprites[jetpackDice - 1];
                
                if (jetpackDice == 1)
                {
                    isRolling = false;
                    lastRollTime = Time.time;
                    rb.linearVelocityY = 0f;
                    jetpackFailed = true;
                }
                
                yield return new WaitForSeconds(delay);
            } 
        }

        private IEnumerator SlowTimeOnDamage(float duration)
        {
            hasSlowedTime = true;
    
            float originalTimeScale = Time.timeScale;
            float originalFixedDeltaTime = Time.fixedDeltaTime;

            Time.timeScale = 0.15f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale; // maintain physics consistency

            yield return new WaitForSecondsRealtime(duration);

            Time.timeScale = originalTimeScale;
            Time.fixedDeltaTime = originalFixedDeltaTime;
    
            hasSlowedTime = false;
        }
    }
}
