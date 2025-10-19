using System;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class EndMenuManager : MonoBehaviour
    {
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI highScoreText;
        public PlayerData data;
        private float timer;
        private string timerString;
        private string highScoreString;
        private TimeSpan highSpan;
        
        private void Start()
        {
            GameManager.instance.StopTimer();
            timer = GameManager.instance.GetTimer();
            TimeSpan t = TimeSpan.FromSeconds(timer);
            timerString = string.Format("{0:D2}:{1:D2}:{2:D3}", t.Minutes, t.Seconds, t.Milliseconds);
            
            if (timer < data.highScore || data.highScore <= 0f)
            {
                data.highScore = timer;
            }
            
            highSpan = TimeSpan.FromSeconds(data.highScore);
            highScoreString = string.Format("{0:D2}:{1:D2}:{2:D3}", highSpan.Minutes, highSpan.Seconds, highSpan.Milliseconds);
        }

        private void Update()
        {
            highScoreString = string.Format("{0:D2}:{1:D2}:{2:D3}", highSpan.Minutes, highSpan.Seconds, highSpan.Milliseconds);
            timerText.text = timerString;
            highScoreText.text = highScoreString;
        }

        public void Restart()
        {
            SceneManager.LoadScene(0);
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}
