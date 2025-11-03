using UnityEngine;

namespace SoulCollector {

    public class Turret : MonoBehaviour {

        [SerializeField] private Projectile _cannonBall;
        [SerializeField] private float _rotateSpeed = 17f;
        private Vector3 _lookDirection;

        private void Start() {
            Fire(new Vector3(4f, 0f, 5f));
        }

        /// <summary>
        /// Fires the turret (unless the cannon ball is already in motion).
        /// </summary>
        /// <param name="target">The location we are firing at.</param>
        private void Fire(Vector3 target) {

            if (_cannonBall.Fired) return;

            _lookDirection = (target - transform.position).normalized;
            _cannonBall.Fire(transform.position, target);

        }

        private void Update() {
            TurnToLook();
        }

        /// <summary>
        /// Smoothly rotates the character to face the current look direction.
        /// </summary>
        private void TurnToLook()
        {

            if (_lookDirection.sqrMagnitude > 0.001f)
            {
                Quaternion target = Quaternion.LookRotation(_lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * _rotateSpeed);
            }

        }

    }

}
