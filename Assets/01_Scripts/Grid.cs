using System.Collections.Generic;
using John;
using UnityEngine;

namespace SoulCollector {

    public class Grid : MonoBehaviour {

        [Tooltip("The width and depth of the play area.")]
        [SerializeField] private int _gridSize = 5;

        [Tooltip("The number the collectables in this grid.")]
        [SerializeField] private int _numberOfCollectables = 5;

        [Tooltip("The prefab for a single grid cell.")]
        [SerializeField] private Tile _gridCellPrefab;

        [Tooltip("The prefab for the collectables")]
        [SerializeField] private GameObject _collectablePrefab;

        private Player _player;

        // For setting the camera to the middle of the grid.
        public float HalfGrid => _gridSize / 2f;

        Tile[,] _grid;

        void Start() {

            CreateGrid();

        }

        public void SetPlayer(Player player) => _player = player;

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

                    Vector3 position = new(x, 0f, z);
                    Tile newCell = Instantiate(_gridCellPrefab, position, Quaternion.identity, transform);
                    newCell.name = $"{x}, {z}";
                    _grid[x, z] = newCell;

                }
            }
            PlaceObjects();
        }

        private void PlaceObjects() {

            List<Vector2Int> positions = new();
            for (int x = 0; x < _gridSize; x++) {

                for (int z = 0; z < _gridSize; z++) {
                    positions.Add(new(x, z));
                }
            }

            for (int i = 0; i < _numberOfCollectables; i++) {

                int rndIndex = Random.Range(0, positions.Count);
                Vector3 position = new(positions[rndIndex].x, 0f, positions[rndIndex].y);
                GameObject newCollectable = Instantiate(_collectablePrefab, position, Quaternion.identity, transform);
                positions.RemoveAt(rndIndex);

            }

            int playerIndex = Random.Range(0, positions.Count);
            Vector3 playerPos = new(positions[playerIndex].x, 0f, positions[playerIndex].y);
            _player.transform.position = playerPos;

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
            _grid[x, z].DestroySelf = true;
        }
    }

}