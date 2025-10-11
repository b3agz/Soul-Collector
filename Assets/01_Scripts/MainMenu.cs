using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoulCollector {

    public class MainMenu : MonoBehaviour {

        /// <summary>
        /// Loads the game scene using SceneManager.
        /// </summary>
        public void StartGame() => SceneManager.LoadScene("Game");

        /// <summary>
        /// Closes the game when run as an application (does not work in editor or web build).
        /// </summary>
        public void QuitGame() => Application.Quit();

    }
}