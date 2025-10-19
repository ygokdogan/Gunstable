using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class EndMenuManager : MonoBehaviour
    {
        public TextMeshProUGUI timerText;
        private float timer;
        private string timerString;
        
        private void Start()
        {
            GameManager.instance.StopTimer();
            timer = GameManager.instance.GetTimer();
            TimeSpan t = TimeSpan.FromSeconds(timer);
            timerString = string.Format("{0:D2}:{1:D2}:{2:D3}", t.Minutes, t.Seconds, t.Milliseconds);
        }

        private void Update()
        {
            timerText.text = timerString;
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
