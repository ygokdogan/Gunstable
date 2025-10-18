using System;
using Input;
using UnityEngine;
using Utilities;

namespace Player
{
    public class PlayerAnimatorController : MonoBehaviour
    {
        public Animator anim;
        private InputHandler input;
        private PlayerController player;

        public GameObject withJetpack;
        public GameObject withoutJetpack;
        
        public FlashEffect flash;

        private GameObject currentSprites;

        private void Awake()
        {
            currentSprites = withoutJetpack;
            anim = currentSprites.GetComponent<Animator>();
            player = GetComponent<PlayerController>();
            input = GetComponent<InputHandler>();
        }

        private void Update()
        {
            SetIdleWalkParameters();
        }

        public void UnlockJetpack()
        {
            if (currentSprites == withJetpack)
                return;
            
            withJetpack.SetActive(true);
            currentSprites = withJetpack;
            anim = withJetpack.GetComponent<Animator>();
            withoutJetpack.SetActive(false);
        }

        private void SetIdleWalkParameters()
        {
            if (!player.isGrounded)
            {
                anim.SetBool("walk", false);
                anim.SetBool("idle", true);
            }
            else
            {
                if (Mathf.Abs(input.MoveInput.x) > 0)
                {
                    anim.SetBool("walk", true);
                    anim.SetBool("idle", false);
                    anim.SetBool("fly", false);
                }
                else
                {
                    anim.SetBool("walk", false);
                    anim.SetBool("idle", true);
                    anim.SetBool("fly", false);
                }    
            }
            
        }
    }
}
