using System;
using Player;
using UnityEngine;

namespace Utilities
{
    public class ConfinerManager : MonoBehaviour
    {
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<PlayerController>();
                if (player.isChangingScene) return;

                player.Die();
            }
        }
    }
}
