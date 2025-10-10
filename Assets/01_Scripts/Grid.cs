using System.Collections.Generic;
using John;
using UnityEngine;

namespace SoulCollector {

    public class Grid : MonoBehaviour {

        public static Grid Instance { get; private set; }
        void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            Application.targetFrameRate = 60;

        }

        public Shake CameraShake;

        [Tooltip("The width and depth of the play area.")]
        [SerializeField] private int _gridSize = 5;

        [Tooltip("The number the collectables in this grid.")]
        [SerializeField] private int _numberOfCollectables = 5;

        [Tooltip("The prefab for a solid floor tile.")]
        [SerializeField] private Tile _solidTile;

        [Tooltip("The prefab for a drop floor tile.")]
        [SerializeField] private Tile _dropTile;

        [Tooltip("The prefab for the collectables")]
        [SerializeField] private GameObject _collectablePrefab;

        [Tooltip("The score UI Element.")]
        [SerializeField] private Score _score;

        [Tooltip("The player character.")]
        [SerializeField] private Player _player;

        private int _currentScore;

        public bool HasCollectedAll =>_currentScore >= _numberOfCollectables;

        public bool SuspendControls { get; private set; }

        // For setting the camera to the middle of the grid.
        public float HalfGrid => _gridSize / 2f;

        Tile[,] _grid;

        /// <summary>
        /// Resets/Creates a new grid to play.
        /// </summary>
        public void NewGame() {
            _currentScore = 0;
            CreateGrid();
            UpdateScore(true);
            PlaceObjects();
            SuspendControls = false;
        }

        public void ClearGrid() {

            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }
            _player.gameObject.SetActive(false);
        }

        /// <summary>
        /// Creates a grid using instantiated _gridCellPrefabs to a size of _gridSize.
        /// </summary>
        private void CreateGrid() {

            // Initialise new 2D array to store the grid cells in so that we can reference them later.
            _grid = new Tile[_gridSize, _gridSize];

            // Loop through each row.
            for (int x = 0; x < _gridSize; x++) {
                // Loop through each column.
                for (int z = 0; z < _gridSize; z++) {

                    int rnd = Random.Range(0, 10);
                    if (rnd > 8) continue;
                    Tile prefab = (rnd > 6) ? _dropTile : _solidTile;

                    Vector3 position = new(x, 0f, z);
                    Tile newCell = Instantiate(prefab, position, Quaternion.identity, transform);
                    newCell.name = $"{x}, {z}";
                    _grid[x, z] = newCell;

                }
            }
        }

        /// <summary>
        /// Creates a list of all positions in the level grid and uses that list to spawn
        /// collectables at random positions on the grid. Everytime a position is used, it
        /// is removed from the list to ensure it is not used again.
        /// </summary>
        private void PlaceObjects() {

            List<Vector2Int> positions = new();
            for (int x = 0; x < _gridSize; x++) {
                for (int z = 0; z < _gridSize; z++) {
                    if (!CanTraverse(x, z)) continue;
                    positions.Add(new(x, z));
                }
            }

            int i = 0;
            while (i < _numberOfCollectables) {
                int rndIndex = Random.Range(0, positions.Count);
                Vector3 position = new(positions[rndIndex].x, 0f, positions[rndIndex].y);
                GameObject newCollectable = Instantiate(_collectablePrefab, position, Quaternion.identity, transform);
                positions.RemoveAt(rndIndex);
                i++;
            }

            int playerIndex = Random.Range(0, positions.Count);
            Vector3 playerPos = new(positions[playerIndex].x, 0f, positions[playerIndex].y);
            _player.transform.position = playerPos;
            _player.transform.forward = new Vector3(0f, 0f, -1f);
            _player.gameObject.SetActive(true);

        }

        /// <summary>
        /// Checks if a position is inside the bounds of the grid.
        /// </summary>
        /// <param name="position">Vector3 of the position being checked.</param>
        /// <returns>True if the position is out of bounds, false if not.</returns>
        public bool IsOutOfBounds(Vector3 position) {

            if (position.x < 0 || position.x > _gridSize - 1) return true;
            if (position.z < 0 || position.z > _gridSize - 1) return true;
            return false;

        }

        /// <summary>
        /// Checks if a given position is traversable.
        /// </summary>
        /// <param name="x">The horizontal tile coordinate.</param>
        /// <param name="y">The vertical tile coordinate.</param>
        /// <returns>True if the position is traversable.</returns>
        public bool CanTraverse(int x, int y) => CanTraverse(new(x, 0f, y));

        /// <summary>
        /// Checks if a given position is traversable.
        /// </summary>
        /// <param name="position">The position we are trying to traverse.</param>
        /// <returns>True if the position is traversable.</returns>
        public bool CanTraverse(Vector3 position) {

            // If the cell is out of bounds, we cannot traverse it.
            if (IsOutOfBounds(position)) return false;

            // Convert the position to ints.
            int x = Maths.RoundToInt(position.x);
            int z = Maths.RoundToInt(position.z);

            // If the cell has been destroyed, we cannot traverse it.
            if (_grid[x, z] == null || _grid[x, z].DestroySelf) return false;

            // If we get here, we can traverse it.
            return true;

        }

        /// <summary>
        /// Called when a grid cell has been stepped off of and can no longer be used.
        /// </summary>
        /// <param name="position">The cell position we are stepping off of.</param>
        public void DiscardCell(Vector3 position) {

            if (IsOutOfBounds(position)) {
                Debug.LogError($"Attempted to discard cell at {position.x}, {position.y}, but that position is out of bounds.");
            }

            // Convert the position to ints.
            int x = Maths.RoundToInt(position.x);
            int z = Maths.RoundToInt(position.z);

            // Handle the grid cell.
            _grid[x, z].DropTile();
        }

        /// <summary>
        /// Updates the score UI element to reflect the current score.
        /// </summary>
        public void UpdateScore(bool suppressBounce = false) => _score.UpdateScore(_currentScore, _numberOfCollectables, suppressBounce);

        /// <summary>
        /// Increments the current score and updates the UI element to reflect the change.
        /// </summary>
        public void IncrementScore() {
            _score.UpdateScore(++_currentScore, _numberOfCollectables);
            if (_currentScore >= _numberOfCollectables) {
                SuspendControls = true;
            }
        }

    }
}