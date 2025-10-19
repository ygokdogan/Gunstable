using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    public class HealthDisplay : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private List<Image> headIcons = new List<Image>(); 

        [Header("Sprites")]
        [SerializeField] private Sprite fullHead;

        private int maxHealth;

        public void Initialize(int maxHealth)
        {
            this.maxHealth = maxHealth;
            UpdateDisplay(maxHealth);
        }

        public void UpdateDisplay(int currentHealth)
        {
            for (int i = 0; i < headIcons.Count; i++)
            {
                if (i < currentHealth)
                {
                    headIcons[i].color = Color.white;
                }
                else
                {
                    headIcons[i].color = new Color(1, 1, 1, 0.1f);
                }
            }
        }
    }
}
