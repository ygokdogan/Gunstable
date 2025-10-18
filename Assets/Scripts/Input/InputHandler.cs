using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class InputHandler : MonoBehaviour
    {
        private PlayerControls controls;
        
        public float inputHoldTime = 0.2f;
        
        public Vector2 MoveInput { get; private set; }
        public Vector2 MousePosition { get; private set; }
        
        public bool JumpInput { get; private set; }
        public bool ShootInput { get; private set; }
        public bool InteractInput { get; private set; }
        public bool TestInput { get; private set; }
        public bool JumpInputReleased { get; private set; }
        private float jumpInputStartTime;

        private void Awake()
        {
            controls = new PlayerControls();
            
            controls.Gameplay.Movement.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
            controls.Gameplay.Movement.canceled += ctx => MoveInput = Vector2.zero;
            
            controls.Gameplay.MousePosition.performed += ctx => MousePosition = ctx.ReadValue<Vector2>();

            controls.Gameplay.Jump.performed += ctx =>
            {
                JumpInput = true;
                jumpInputStartTime = Time.time;
                JumpInputReleased = false;
            };
            controls.Gameplay.Jump.canceled += ctx => JumpInputReleased = true;
            
            controls.Gameplay.Test.performed += ctx => TestInput = true;
            controls.Gameplay.Test.canceled += ctx => TestInput = false;

            controls.Gameplay.Shoot.performed += ctx => ShootInput = true;
            controls.Gameplay.Shoot.canceled += ctx => ShootInput = false;
            
            controls.Gameplay.Interact.performed += ctx => InteractInput = true;
            controls.Gameplay.Interact.canceled += ctx => InteractInput = false;
        }

        private void Update()
        {
            JumpBuffer();
        }

        private void JumpBuffer()
        {
            if (JumpInputReleased && Time.time >= inputHoldTime + jumpInputStartTime)
            {
                JumpInput = false;
            }
        }

        public void UseJumpInput() => JumpInput = false;
        public void UseTestInput() => TestInput = false;
        public void UseShootInput() => ShootInput = false;
        public void UseInteractInput() => InteractInput = false;

        private void OnEnable()
        {
            controls.Enable();
        }

        private void OnDisable()
        {
            controls.Disable();
        }
    }
}
