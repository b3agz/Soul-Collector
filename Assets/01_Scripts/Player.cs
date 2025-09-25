using UnityEngine;
using John;

public class Player : MonoBehaviour
{

    [Tooltip("The amount of time after one movement before the player can move again.")]
    [SerializeField] private float _coolDownAmount = 0.2f;

    private float _moveCoolDown;
    private bool _up, _down, _left, _right;
    private float _deadZone = 0.2f;

    private Grid _grid;

    void Awake()
    {
        _grid = FindFirstObjectByType<Grid>();
        if (_grid == null) Debug.LogError("No Grid component was found in the scene.");
    } 

    void Start()
    {
        SnapToGrid();
    }

    void Update()
    {

        // If the move cool down is greater zero, decrease by the amount of time that has elapsed since the last update,
        // and then return so that the player can't move until the cooldown is back at zero.
        if (_moveCoolDown > 0f)
        {
            _moveCoolDown -= Time.deltaTime;
            return;
        }

        // Get player's input using GetAxis so that we can grab WASD, arrow, and gamepad controls.
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Unity's horizontal and vertical axis produce an analogue -1 to 1 value, we need an on or off,
        // so convert them to bools factoring in a deadzone so the controls are not too sensitive.
        _up = vertical > _deadZone;
        _down = vertical < -_deadZone;
        _right = horizontal > _deadZone;
        _left = horizontal < -_deadZone;

        // Create a direction from the player input. We only want to allow the player to move one direction at a
        // time, so we use if else blocks to ensure only one direction is used.
        Vector3 direction = new();
        if (_up) direction.z++;
        else if (_down) direction.z--;
        else if (_right) direction.x++;
        else if (_left) direction.x--;

        // Make sure the player is not moving out of the bounds of the grid.
        Vector3 newPosition = transform.position + direction;
        if (_grid.IsOutOfBounds(newPosition)) return;

        // Move the player by adding the direction to the current position.
        transform.position = newPosition;
        
        // If any direction was pressed, set our move cool down to the cooldown amount.
        if (_up || _down || _left || _right)
        {
            _moveCoolDown = _coolDownAmount;
        }

    }

    /// <summary>
    /// Snaps the transform to whole numbers on each axis, enforcing a defacto grid with cells of 1x1 units.
    /// </summary>
    private void SnapToGrid()
    {
        Vector3 position = transform.position;
        position.x = Maths.RoundToInt(position.x);
        position.y = Maths.RoundToInt(position.y);
        position.z = Maths.RoundToInt(position.z);
        transform.position = position;
    }

}
