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
        public MusicPlayer musicPlayer;
        
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
            musicPlayer = GetComponent<MusicPlayer>();
            audioManager = GetComponent<AudioManager>();
            var menuMusic = audioManager.Get("MainMenuMusic");
            musicPlayer.PlayMusic(menuMusic.clip);
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
            var inGameMusic = audioManager.Get("InGameMusic");
            musicPlayer.PlayMusic(inGameMusic.clip);
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}
