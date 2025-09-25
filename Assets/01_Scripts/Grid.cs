using UnityEngine;

public class Grid : MonoBehaviour
{

    [Tooltip("The width and depth of the play area.")]
    [SerializeField] private int _gridSize = 5;

    [Tooltip("The prefab for a single grid cell.")]
    [SerializeField] private GameObject _gridCellPrefab;

    // For setting the camera to the middle of the grid.
    public float HalfGrid => _gridSize / 2f;

    void Start()
    {
        CreateGrid();
    }

    /// <summary>
    /// Creates a grid using instantiated _gridCellPrefabs to a size of _gridSize.
    /// </summary>
    private void CreateGrid()
    {
        // Loop through each row.
        for (int x = 0; x < _gridSize; x++)
        {
            // Loop through each column.
            for (int z = 0; z < _gridSize; z++)
            {
                Vector3 position = new(x, 0f, z);
                GameObject newCell = Instantiate(_gridCellPrefab, position, Quaternion.identity, transform);
                newCell.name = $"{x}, {z}";
            }
        }
    }

    /// <summary>
    /// Checks if a position is inside the bounds of the grid.
    /// </summary>
    /// <param name="position">Vector3 of the position being checked.</param>
    /// <returns>True if the position is out of bounds, false if not.</returns>
    public bool IsOutOfBounds(Vector3 position)
    {
        if (position.x < 0 || position.x > _gridSize - 1) return true;
        if (position.z < 0 || position.z > _gridSize - 1) return true;
        return false;
    }

}
