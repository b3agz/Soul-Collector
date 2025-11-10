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

        [Tooltip("The number the obstacles in this grid.")]
        [SerializeField] private int _numberOfObstacles = 3;

        [Tooltip("The meximum health a grid tile can have.")]
        [field: SerializeField] public int MaxHealth { get; private set; } = 10;

        [Tooltip("The prefab for a solid floor tile.")]
        [SerializeField] private Tile _floorTile;

        [Tooltip("The prefab for a solid floor tile.")]
        [SerializeField] private GameObject _gridObstaclePrefab;

        [Tooltip("The prefab for the collectables")]
        [SerializeField] private GameObject _collectablePrefab;

        [Tooltip("The score UI Element.")]
        [SerializeField] private Score _score;

        [Tooltip("The player character.")]
        [SerializeField] private Player _player;

        [Tooltip("The cannon controller.")]
        [SerializeField] private Turret _turret;

        // 0 = Bubble Sort, 1 = Insertion Sort.
        public static int SortMethod { get; private set; } = 1;

        private int _currentScore;

        public bool HasCollectedAll => _currentScore >= _numberOfCollectables;

        public bool GameOver = false;

        public bool SuspendControls { get; set; }

        // For setting the camera to the middle of the grid.
        public float HalfGrid => _gridSize / 2f;

        Dictionary<Vector2Int, Tile> _grid = new();

        void Start() {
            NewGame();
        }

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

        /// <summary>
        /// Destroys all of the spawned items (grid tiles and collectables) and disables the player object.
        /// </summary>
        public void ClearGrid() {

            // All instantiated level objects are spawned as children of this object, so to remove any that are
            // still in the scene, we just need to loop through the remaining children of this GameObject.
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }
            _player.gameObject.SetActive(false);
        }

        /// <summary>
        /// Creates a grid using instantiated _gridCellPrefabs to a size of _gridSize.
        /// </summary>
        private void CreateGrid() {

            // Loop through each row.
            for (int x = 0; x < _gridSize; x++) {
                // Loop through each column.
                for (int z = 0; z < _gridSize; z++) {

                    Vector3 position = new(x, 0f, z);

                    Tile newCell = Instantiate(_floorTile, position, Quaternion.identity, transform);
                    newCell.name = $"{x}, {z}";

                    Vector2Int coord = Maths.Vector3ToVector2Int(position);
                    newCell.Init(Random.Range(2, MaxHealth), coord);
                    _grid.Add(coord, newCell);

                }
            }
        }

        /// <summary>
        /// Creates a list of all positions in the level grid and uses that list to spawn
        /// collectables at random positions on the grid. Everytime a position is used, it
        /// is removed from the list to ensure it is not used again.
        /// </summary>
        private void PlaceObjects() {

            // Loop through all of the grid positions and, if they are traversible, add them to a list.
            List<Vector2Int> positions = new();
            for (int x = 0; x < _gridSize; x++) {
                for (int z = 0; z < _gridSize; z++) {
                    if (!CanTraverse(x, z)) continue;
                    positions.Add(new(x, z));
                }
            }

            // Loop until either we have placed the desired number of collectables or we have run out of valid positions.
            // For each iteration, get a random position from the list. Since the list only contains valid cells, we can put
            // a collectable at any of the locations. Once used, we remove it from the list so that it isn't used again.
            int i = 0;
            while (i < _numberOfCollectables || positions.Count < 1) {

                int rndIndex = Random.Range(0, positions.Count);
                Vector2Int coord = positions[rndIndex];

                Vector3 position = new(coord.x, 0f, coord.y);
                GameObject newCollectable = Instantiate(_collectablePrefab, position, Quaternion.identity, transform);
                _grid[coord].MakeInvulnerable();
                positions.RemoveAt(rndIndex);
                i++;

            }

            i = 0;
            while (i < _numberOfObstacles || positions.Count < 1) {

                int rndIndex = Random.Range(0, positions.Count);
                Vector2Int coord = positions[rndIndex];

                Vector3 position = new(coord.x, 0f, coord.y);
                GameObject newCollectable = Instantiate(_gridObstaclePrefab, position, Quaternion.identity, transform);
                Destroy(_grid[coord].gameObject);
                _grid.Remove(coord);
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

            // If the cell has been destroyed, we cannot traverse it.
            Vector2Int coord = Maths.Vector3ToVector2Int(position);
            if (!_grid.ContainsKey(coord) || _grid[coord].DestroySelf) return false;

            // If we get here, we can traverse it.
            return true;

        }

        /// <summary>
        /// Sorts the tile dictionary into an array of Tiles in ascending order of health.
        /// </summary>
        public void FireTurret() {

            // Sort tiles in ascending order of health (weakest first).

            if (SortMethod == 0) {
                Tile[] tiles = Algorithms.BubbleSort(_grid);
                _turret.SetTargets(tiles);
            }
            else {
                List<Tile> tiles = Algorithms.InsertionSort(_grid);
                _turret.SetTargets(tiles.ToArray());
            }

        }

        /// <summary>
        /// Adds a step to the cannon counter and updates the health of the tile that was just
        /// vacated by the player.
        /// </summary>
        /// <param name="coord"></param>
        public void PlayerMove(Vector2Int coord) {
            _turret.AddStep();

            // If we have a tile at this location (this check should never be needed) see if the tile
            // is invulnerable (health is greater than MaxHealth). If not, damage it.
            if (_grid.TryGetValue(coord, out Tile tile)) {
                if (tile.Health > MaxHealth || tile.Health == 1) return;
                tile.TakeDamage(1);
            }
            else {
                Debug.LogError("Attempted to call PlayerMove on a tile that doesn't exist.");
            }

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

        public void RemoveTile(Vector2Int coord) {
            _grid.Remove(coord);
        }

        public void CheckForPlayerDeath(Vector3 position) {

            Vector2Int deathCoord = Maths.Vector3ToVector2Int(position);
            Vector2Int playerCoord = Maths.Vector3ToVector2Int(_player.transform.position);

            if (deathCoord == playerCoord) {
                GameOver = true;
            }

        }

        /// <summary>
        /// Sets the sorting method to use.
        /// 0 = Bubble Sort
        /// 1 = Insertion Sort
        /// </summary>
        /// <param name="value">The value of the sorting method.</param>
        public static void SetSortMethod(int value) {

            SortMethod = value;
            switch (value) {
                case 0:
                    Debug.Log("Sort Method set to Bubble Sort");
                    break;
                case 1:
                    Debug.Log("Sort Method set to Insertion Sort");
                    break;
                default:
                    Debug.LogError("Attempted to set sort method to a method we don't have!");
                    break;
            }

        }

    }
}