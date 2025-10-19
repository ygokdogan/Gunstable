using System;
using Input;
using Interfaces;
using Player;
using UnityEngine;

namespace Utilities
{
    public class PlayerInteractableTrigger : MonoBehaviour
    {
        private IInteractable interactable;
        private InputHandler input;
        private PlayerController player;
        public GameObject interactPrompt;

        private void Awake()
        {
            player = GetComponent<PlayerController>();
            input = GetComponent<InputHandler>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<IInteractable>() != null)
            {
                interactable = other.GetComponent<IInteractable>();
                interactPrompt.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.GetComponent<IInteractable>() != null)
            {
                interactable = null;
                interactPrompt.SetActive(false);
            }
        }

        private void Update()
        {
            if (input.InteractInput && interactable != null)
            {
                input.UseInteractInput();
                interactable.Interact(player);
            }
        }

        public void FlipInteractPrompt()
        {
            interactPrompt.transform.Rotate(0f, 180f,0f);
        }
    }
}
