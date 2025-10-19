using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        
        public AudioManager audioManager;
        
        private float speedrunTimer = 0f;
        private bool gameStarted = false;
        private bool isPaused;
        private void Awake()
        {
            if(instance != null)
                Destroy(gameObject);
        
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            speedrunTimer = 0f;
            audioManager = GetComponent<AudioManager>();
            audioManager.Play("MainMenuMusic");
        }

        private void Update()
        {
            if (gameStarted)
            {
                speedrunTimer += Time.deltaTime;
            }
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
            audioManager.Stop("MainMenuMusic");
            audioManager.Play("InGameMusic");
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
