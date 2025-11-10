using UnityEngine;
using John;

namespace SoulCollector {

    public class CameraLooker : MonoBehaviour {

        [SerializeField] private float _followSpeed = 0.3f;
        [SerializeField] private float _sensitivity = 4f;
        [SerializeField] private float _zPosLock = 4f;
        [SerializeField] private float _maxHorizontal = 3f;
        [SerializeField] private float _minHorizontal = 1f;
        [SerializeField] private float _idleTime = 2f;
        [SerializeField] private Transform _player;

        private float _mouseX;
        private float _xValue;
        private float _idleTimer;

        void LateUpdate() {

            if (_player == null || !_player.gameObject.activeSelf) return;

            _mouseX = Input.GetAxis("Mouse X");

            // If the mouse is moving, make sure timer is reset and add the mouse to xValue.
            if (!Mathf.Approximately(0f, _mouseX)) {
                _idleTimer = 0f;
                _xValue = Maths.Clamp(_xValue - _mouseX * _sensitivity, _minHorizontal, _maxHorizontal);
                _xValue += transform.position.x;    // Account for camera position in world.
            }
            else if (_idleTimer < _idleTime) {
                _xValue = transform.position.x;     // Else if timer has expired, make sure xValue is set to default.
            }
            else {
                _idleTimer += Time.deltaTime;       // Else increment timer.
            }

            // Get the player position and set the x position to xValue.
            Vector3 targetPos = _player.position;
            targetPos.x = _xValue;

            // Lock our Z position so we don't tilt forwards and backwards.
            targetPos.z = _zPosLock;

            // Get the current and desired rotation and the Slerp between the two.
            Quaternion startRot = transform.rotation;
            transform.LookAt(targetPos);
            Quaternion endRot = transform.rotation;
            transform.rotation = Quaternion.Slerp(startRot, endRot, Time.deltaTime * _followSpeed);

        }
    }
}

