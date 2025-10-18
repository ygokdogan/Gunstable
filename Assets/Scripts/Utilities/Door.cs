using System;
using Interfaces;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utilities
{
    public class Door : MonoBehaviour, IInteractable
    {
        private PlayerController player;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        public void Interact(PlayerController player)
        {
            player.isChangingScene = true;
            var nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            SceneManager.LoadScene(nextSceneIndex);   
        }
    }
}
