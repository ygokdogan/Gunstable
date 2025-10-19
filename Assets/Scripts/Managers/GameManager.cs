using System;
using Input;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Utilities;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        
        public AudioManager audioManager;
        public PlayerData playerData;
        public InputHandler input;
        
        private float speedrunTimer = 0f;
        public bool gameStarted = false;
        private bool isPaused;
        private void Awake()
        {
            if(instance != null)
                Destroy(instance.gameObject);
        
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            audioManager = GetComponent<AudioManager>();
            if (input == null)
            {
                input = GetComponent<InputHandler>();
            }
        }

        private void Start()
        {
            speedrunTimer = 0f;
            audioManager.Play("MainMenuMusic");
        }

        private void Update()
        {
            if (gameStarted)
            {
                speedrunTimer += Time.deltaTime;
            }
            
            ReturnToMenu();
        }

        public void PauseGame(GameObject menuPanel)
        {
            isPaused = !isPaused;
            menuPanel.SetActive(isPaused);
            
            Time.timeScale = isPaused ? 0f : 1f;
        }

        public void PlayGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            gameStarted = true;
            playerData.acquiredJetpack = false;
            audioManager.Stop("MainMenuMusic");
            audioManager.Play("InGameMusic");
        }

        private void ReturnToMenu()
        {
            if (gameStarted)
            {
                if (input.PauseInput)
                {
                    input.UsePauseInput();
                    SceneManager.LoadScene(0);
                }
            }
        }

        public void Exit()
        {
            Application.Quit();
        }

        public float GetTimer()
        {
            return speedrunTimer;
        }

        public void StopTimer()
        {
            gameStarted = false;
        }
    }
}
