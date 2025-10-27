using UnityEngine;
using John;

namespace SoulCollector {

    public class Player : MonoBehaviour {

        [Tooltip("How quickly the character lerps from one position to another.")]
        [SerializeField] private float _lerpSpeed = 1f;

        [Tooltip("Curve that determines the speed of the character's lean when moving.")]
        [SerializeField] private AnimationCurve _leanCurve;

        [Tooltip("How far the character leans (on the X rotation axis) when at the peak of the curve.")]
        [SerializeField] private float _leanAmount = 5f;

        [Tooltip("How quickly the character rotates to face the direction they are moving.")]
        [SerializeField] private float _rotateSpeed = 12f;

        [Tooltip("Particles prefab that is spawned every time the player collects a collectable.")]
        [SerializeField] private GameObject _particlesPrefab;

        private float _lerpFactor = 1f;
        private Vector3 _lerpStartPos;
        private Vector3 _lerpEndPos;
        private Transform _meshesObject;

        private bool _up, _down, _left, _right;
        private bool _directionPressed;
        private bool _isLeaning;
        private float _deadZone = 0.2f;
        private Vector3 _lookDirection;

        void Awake() {

            _meshesObject = transform.Find("Meshes");

        }

        void Start() {

            SnapToGrid();

        }

        void Update() {

            // If the player controls aren't suspended, get the player's inputs. Otherwise, make sure the direction
            // bools are set to false.
            if (!Grid.Instance.SuspendControls) {
                GetInput();
            }
            else {
                _up = _down = _left = _right = false;
            }
            
            TurnToLook();

            // If the lerp factor is less than one, that means we are currently moving, so move the character.
            if (_lerpFactor < 1f) {
                _lerpFactor += _lerpSpeed * Time.deltaTime;
                Vector3 position = Maths.Lerp(_lerpStartPos, _lerpEndPos, _lerpFactor);
                position.y = transform.position.y;
                transform.position = position;
                Lean();
                return;
            }



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
            //transform.forward = direction;
            

            // Make sure the player is not moving out of the bounds of the grid.
            Vector3 newPosition = transform.position + direction;
            if (!Grid.Instance.CanTraverse(newPosition)) return;

            // Set variables for traversal between grid cells.
            _lerpStartPos = transform.position;
            _lerpEndPos = newPosition;
            _lerpFactor = 0;
            Grid.Instance.TraverseCell(_lerpStartPos);

        }

        /// <summary>
        /// Gets the player's input and sets the relevant variables accordingly.
        /// </summary>
        private void GetInput() {

            // Get player's input using GetAxis so that we can grab WASD, arrow, and gamepad controls.
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // Create a direction vector with our input axis.
            _lookDirection = new Vector3(horizontal, 0f, vertical);

            // Unity's horizontal and vertical axis produce an analogue -1 to 1 value, we need an on or off,
            // so convert them to bools factoring in a deadzone so the controls are not too sensitive.
            _up = vertical > _deadZone;
            _down = vertical < -_deadZone;
            _right = horizontal > _deadZone;
            _left = horizontal < -_deadZone;

            // Get whether any direction is currently veing pressed.
            _directionPressed = _up || _down || _right || _left;

        }

        /// <summary>
        /// Adjusts the rotation of the character's meshes based on movement to give the impression
        /// or leaning.
        /// </summary>
        private void Lean() {

            // If we don't have a meshes object, return rather than throwing an error.
            if (_meshesObject == null) return;

            // If we are currently leaning but no direction is being pressed, set leaning to
            // false. Else if direction is pressed and our lean factor is over half way, set it
            // to true.
            if (!_directionPressed) _isLeaning = false;
            else if (_directionPressed && _lerpFactor > 0.49f) _isLeaning = true;

            float modifiedLerpFactor = _lerpFactor;

            // If we are currently leaning, force our lerp factor to 0.5f.
            if (_isLeaning) {
                modifiedLerpFactor = 0.5f;
            }

            // Calculate how much to lean.
            float lean = _leanAmount * _leanCurve.Evaluate(modifiedLerpFactor);

            // Set the rotation of the player character. We are only adjusting the local rotation
            // of the mesh, which is a child object of player object. As the player object always faces
            // "forward", we only need to rotate the local X axis.
            _meshesObject.localRotation = Quaternion.Euler(lean, 0f, 0f);

        }

        /// <summary>
        /// Smoothly rotates the character to face the current look direction.
        /// </summary>
        private void TurnToLook() {

            if (_lookDirection.sqrMagnitude > 0.001f) {
                Quaternion target = Quaternion.LookRotation(_lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * _rotateSpeed);
            }

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

            if (other.CompareTag("Collectable")) {
                GameObject particles = Instantiate(_particlesPrefab, other.transform.position, Quaternion.identity);
                Grid.Instance.IncrementScore();
                Grid.Instance.CameraShake.ShakeIt();
                Destroy(other.gameObject);
            }

        }

    }
}