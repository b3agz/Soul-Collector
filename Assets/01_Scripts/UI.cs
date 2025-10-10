using UnityEngine;

namespace SoulCollector {
    public class UI : MonoBehaviour {

        [SerializeField] private GameObject _mainMenu;
        [SerializeField] private GameObject _pauseMenu;
        [SerializeField] private GameObject _gameOverMenu;
        [SerializeField] private GameObject _gameWonMenu;
        [SerializeField] private GameObject _scoreBoard;
        [SerializeField] private GameObject _mainMenuAssets;

        public UIState State {
            get {
                if (_mainMenu != null && _mainMenu.activeSelf) return UIState.Main;
                if (_pauseMenu != null && _pauseMenu.activeSelf) return UIState.Pause;
                if (_gameOverMenu != null && _gameOverMenu.activeSelf) return UIState.GameOver;
                if (_gameWonMenu != null && _gameWonMenu.activeSelf) return UIState.GameWon;
                return UIState.None;
            }
        }

        void Start() {
            MainMenu();
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
                    if (Grid.Instance.HasCollectedAll) {
                        SetMenu(_gameWonMenu, true);
                    }
                }
            }
        }

        public void MainMenu() {
            Grid.Instance.ClearGrid();
            SetMenu(_scoreBoard, false);
            SetMenu(_mainMenu, true);
            SetMenu(_mainMenuAssets, true);
            SetMenu(_gameWonMenu, false);
            UnPauseGame();
        }

        public void NewGame() {
            SetMenu(_mainMenu, false);
            SetMenu(_gameWonMenu, false);
            SetMenu(_scoreBoard, true);
            SetMenu(_mainMenuAssets, false);
            UnPauseGame();
            Grid.Instance.ClearGrid();
            Grid.Instance.NewGame();
        }

        public void ExitGame() {
            Application.Quit();
        }

        public void PauseGame() {
            Time.timeScale = 0f;
            _pauseMenu.SetActive(true);
        }

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

        Main,
        Pause,
        GameOver,
        GameWon,
        None

    }
}