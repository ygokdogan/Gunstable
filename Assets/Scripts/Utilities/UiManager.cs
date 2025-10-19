using System;
using UnityEngine;

namespace Utilities
{
    public class UiManager : MonoBehaviour
    {
        public GameObject jetpackDisplay;
        public GameObject jetpackDisabled;

        private static bool jetpackActive = false;
        private static bool isHacked = false;

        private void Update()
        {
            jetpackDisplay.SetActive(jetpackActive);
            jetpackDisabled.SetActive(isHacked);
        }

        public static void UnlockJetpack()
        {
            jetpackActive = true;
        }

        public static void SetIsHacked(bool hacked)
        {
            isHacked = hacked;
        }
    }
}
