using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoulCollector {
    public class UI : MonoBehaviour {

        [SerializeField] private GameObject _pauseMenu;
        [SerializeField] private GameObject _gameOverMenu;
        [SerializeField] private GameObject _gameWonMenu;
        [SerializeField] private GameObject _scoreBoard;
        [SerializeField] private GameObject _mainMenuAssets;

        public UIState State {
            get {
                if (_pauseMenu != null && _pauseMenu.activeSelf) return UIState.Pause;
                if (_gameOverMenu != null && _gameOverMenu.activeSelf) return UIState.GameOver;
                if (_gameWonMenu != null && _gameWonMenu.activeSelf) return UIState.GameWon;
                return UIState.None;
            }
        }

        void Start() {
            Time.timeScale = 1f;
        }

        void Update() {
            if (State == UIState.Pause) {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    UnPauseGame();
                }
            } else if (State == UIState.None) {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    PauseGame();
                } else {

                    // If the fail and win states both happen in the same frame, sorry bro, you lose.
                    if (Grid.Instance.GameOver) {
                        SetMenu(_gameOverMenu, true);
                        Time.timeScale = 0f;
                    } else if (Grid.Instance.HasCollectedAll) {
                        SetMenu(_gameWonMenu, true);
                        Time.timeScale = 0f;
                    }
                }
            }
        }

        /// <summary>
        /// Ensures timeScale is set back to 1 and loads the main menu scene.
        /// </summary>
        public void MainMenu() {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }

        /// <summary>
        /// Starts a new game, disabling all the menus and calling the relevant functions to reset the level.
        /// </summary>
        public void NewGame() {
            SceneManager.LoadScene("Game");
        }

        /// <summary>
        /// Closes the game when run as an application (does not work in editor or web build).
        /// </summary>
        public void ExitGame() {
            Application.Quit();
        }

        /// <summary>
        /// Sets timeScale to zero to stop player movement and other activity, opens the pause menu.
        /// </summary>
        public void PauseGame() {
            Time.timeScale = 0f;
            _pauseMenu.SetActive(true);
        }

        /// <summary>
        /// Sets timeScale back to 1 and closes the pause menu.
        /// </summary>
        public void UnPauseGame() {
            Time.timeScale = 1f;
            _pauseMenu.SetActive(false);
        }

        /// <summary>
        /// Sets given Menu GameObject's active state to state, checking that the menu exists first.
        /// </summary>
        /// <param name="menu">GameObject of the menu.</param>
        /// <param name="state">Bool representing the desired active state.</param>
        private void SetMenu(GameObject menu, bool state) {
            if (menu != null) menu.SetActive(state);
        }

    }

    public enum UIState {

        Pause,
        GameOver,
        GameWon,
        None

    }
}