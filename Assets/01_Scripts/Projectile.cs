using UnityEngine;
using John;
using System;

namespace SoulCollector {

    public class Projectile : MonoBehaviour {

        [SerializeField] private int _damage = 3;
        [SerializeField] private float _speed = 1f;
        [SerializeField] private float _height = 3f;
        [SerializeField] private AnimationCurve _heightCurve;

        private Tile _target;
        private float _factor = 0f;
        [SerializeField] private bool _fired = false;
        public bool Fired => _fired;
        private Vector3 _startPos;

        private Action _onComplete;

        void Update() {

            // If we're not being fired, don't do anything.
            if (!_fired) return;

            // Using our 0-1 factor, lerp the distance between our start and end, and evaluate our
            // height curve for the height of the ball at each stage.
            Vector3 position = Maths.Lerp(_startPos, _target.transform.position, _factor);
            position.y = _heightCurve.Evaluate(_factor) * _height;

            // Increment our factor.
            _factor += Time.deltaTime * _speed;

            transform.position = position;

            // If the factor is at or over 1, we have reached our destination. Set fired to false
            // and deactivate the gameobject.
            if (_factor >= 1f) {
                Grid.Instance.CheckForPlayerDeath(transform.position);
                _fired = false;
                _target.HideMarker();
                _target.TakeDamage(_damage);
                gameObject.SetActive(false);
                _onComplete?.Invoke();

            }
        }

        /// <summary>
        /// Resets all the variables for putting the ball in motion.
        /// </summary>
        /// <param name="startPos">Where the ball is starting from.</param>
        /// <param name="target">Where the ball should end up.</param>
        public void Fire(Vector3 startPos, Tile tile, Action onComplete) {
            _fired = true;
            _startPos = startPos;
            _target = tile;
            gameObject.SetActive(true);
            _factor = 0f;
            _onComplete = onComplete;
        }
    }

}