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
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.GetComponent<IInteractable>() != null)
            {
                interactable = null;
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
    }
}
