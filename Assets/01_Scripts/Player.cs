using UnityEngine;
using John;

namespace SoulCollector {

    public class Player : MonoBehaviour {

        [Tooltip("The amount of time after one movement before the player can move again.")]
        [SerializeField] private float _coolDownAmount = 0.2f;

        [SerializeField] private float _lerpSpeed = 1f;

        [SerializeField] private AnimationCurve _leanCurve;
        [SerializeField] private float _leanAmount = 5f;

        private float _lerpFactor = 1f;
        private Vector3 _lerpStartPos;
        private Vector3 _lerpEndPos;
        private Transform _meshesObject;

        private bool _up, _down, _left, _right;
        private float _deadZone = 0.2f;

        private Grid _grid;

        void Awake() {

            _grid = FindFirstObjectByType<Grid>();
            if (_grid == null) Debug.LogError("No Grid component was found in the scene.");
            _grid.SetPlayer(this);
            _meshesObject = transform.Find("Meshes");

        }

        void Start() {

            SnapToGrid();

        }

        void Update() {

            if (_lerpFactor < 1f) {
                _lerpFactor += _lerpSpeed * Time.deltaTime;
                Vector3 position = Maths.Lerp(_lerpStartPos, _lerpEndPos, _lerpFactor);
                position.y = transform.position.y;
                transform.position = position;
                Lean();
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

            // If none of the directions were pressed, we don't need to do anything else.
            if (!_up && !_down && !_right && !_left) {
                return;
            }

            // Create a direction from the player input. We only want to allow the player to move one direction at a
            // time, so we use if else blocks to ensure only one direction is used.
            Vector3 direction = new();
            if (_up) direction.z++;
            else if (_down) direction.z--;
            else if (_right) direction.x++;
            else if (_left) direction.x--;

            // Set the player's forward direction to the direction we are moving.
            transform.forward = direction;

            // Make sure the player is not moving out of the bounds of the grid.
            Vector3 newPosition = transform.position + direction;
            if (!_grid.CanTraverse(newPosition)) return;

            // Set variables for traversal between grid cells.
            _lerpStartPos = transform.position;
            _lerpEndPos = newPosition;
            _lerpFactor = 0;
            _grid.DiscardCell(_lerpStartPos);

        }

        private void Lean() {
            if (_meshesObject == null) return;
            float lean = _leanAmount * _leanCurve.Evaluate(_lerpFactor);
            _meshesObject.localRotation = Quaternion.Euler(lean, 0f, 0f);

        }

        /// <summary>
        /// Snaps the transform to whole numbers on each axis, enforcing a defacto grid with cells of 1x1 units.
        /// </summary>
        private void SnapToGrid() {

            Vector3 position = transform.position;
            position.x = Maths.RoundToInt(position.x);
            position.y = Maths.RoundToInt(position.y);
            position.z = Maths.RoundToInt(position.z);
            transform.position = position;

        }

        void OnTriggerEnter(Collider other) {

            Destroy(other.gameObject);

        }

    }
}