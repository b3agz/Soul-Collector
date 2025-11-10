using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace SoulCollector {

    public class MainMenu : MonoBehaviour {

        [SerializeField] private TMP_Dropdown _sortDropown;

        /// <summary>
        /// Loads the game scene using SceneManager.
        /// </summary>
        public void StartGame() => SceneManager.LoadScene("Game");

        /// <summary>
        /// Closes the game when run as an application (does not work in editor or web build).
        /// </summary>
        public void QuitGame() => Application.Quit();

        public void SortMethodOnChange() {
            Grid.SetSortMethod(_sortDropown.value);
        }

    }
}