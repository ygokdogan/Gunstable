using System;
using UnityEngine;

namespace Utilities
{
    public class TutorialManager : MonoBehaviour
    {
        public GameObject tutorialPanel;
        public GameObject[] menuElements;

        private void Awake()
        {
            tutorialPanel.SetActive(false);
        }

        public void OpenMenu()
        {
            tutorialPanel.SetActive(true);
            foreach (GameObject menu in menuElements)
            {
                menu.SetActive(false);
            }
        }

        public void CloseMenu()
        {
            tutorialPanel.SetActive(false);
            foreach (GameObject menu in menuElements)
            {
                menu.SetActive(true);
            }
        }
    }
}
