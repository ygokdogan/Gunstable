using System;
using Managers;
using TMPro;
using UnityEngine;

namespace Utilities
{
    public class TimerUI : MonoBehaviour
    {
        public TextMeshProUGUI timerText;
        private TimeSpan span;
        private float timer;
        
        
        private void Update()
        {
            timer = GameManager.instance.GetTimer();
            span = TimeSpan.FromSeconds(timer);
            timerText.text = $"{span.Minutes:D2}.{span.Seconds:D2}";
        }
    }
}
