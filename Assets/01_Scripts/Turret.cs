using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace SoulCollector {

    public class Turret : MonoBehaviour {

        [SerializeField] private int _stepsPerShot = 3;
        [SerializeField] private int _shotsPerRound = 3;
        [SerializeField] private Projectile _cannonBall;
        [SerializeField] private float _rotateSpeed = 17f;
        [SerializeField] private Score _powerBar;
        private Vector3 _lookDirection;
        private Tile _target;
        private int _currentSteps = 0;

        private List<Tile> _targets = new();

        void Start() {
            _powerBar.UpdateScore(0, _stepsPerShot, true);
        }

        private void Update() {
            TurnToLook();
        }

        /// <summary>
        /// Fires the turret (unless the cannon ball is already in motion).
        /// </summary>
        /// <param name="target">The location we are firing at.</param>
        public void SetTargets(Tile[] tiles) {

            // Create a list with the top <_shotsPerRound> Tiles from the tiles array, accounting for
            // if there are less tiles in the array than <_shotsPerRound>
            _targets = new();
            for (int i = 0; i < Mathf.Min(_shotsPerRound, tiles.Length); i++) {
                _targets.Add(tiles[i]);
            }

            // Prevent player from moving until we are done firing our balls.
            Grid.Instance.SuspendControls = true;

            Fire();

        }

        private void Fire() {

            // Can't fire at a target if we don't have one. But also make sure player can move again.
            if (_targets.Count <= 0) {
                Grid.Instance.SuspendControls = false;
                return;
            }

            // Get the top (weakest) tile and then remove it from the list.
            Tile tile = _targets[0];
            _targets.RemoveAt(0);

            _cannonBall.Fire(transform.position, tile, Fire);

        }

        public void AddStep() {
            _currentSteps++;
            if (_currentSteps >= _stepsPerShot) {
                _currentSteps = 0;
                Grid.Instance.FireTurret();
            }
            _powerBar.UpdateScore(_currentSteps, _stepsPerShot);
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

    }

}
