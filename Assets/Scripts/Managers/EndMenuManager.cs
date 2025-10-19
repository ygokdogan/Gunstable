using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class EndMenuManager : MonoBehaviour
    {
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
