using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace SoulCollector {

    public class Turret : MonoBehaviour {

        [SerializeField] private int _stepsPerShot = 3;
        [SerializeField] private int _shotsPerRound = 3;
        [SerializeField] private Projectile _cannonBall;
        [SerializeField] private float _rotateSpeed = 17f;
        [SerializeField] private Score _powerBar;
        [SerializeField] private GameObject _cannonTurnSign;
        [SerializeField] private LayerMask _layerMask;
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

            StartCoroutine(ShowTargetMarkers());

        }

        /// <summary>
        /// Shows the target markers on the tiles we're about to fire on with a slight delay between
        /// each one.
        /// </summary>
        private IEnumerator ShowTargetMarkers() {

            // Make sure the player knows it's the cannon's turn so they're not confused as to why
            // they can't move.
            _cannonTurnSign.SetActive(true);
            for (int i = 0; i < _shotsPerRound; i++) {
                _targets[i].ShowMarker();
                yield return new WaitForSeconds(0.5f);
            }

            Fire();

        }

        private void Fire() {

            // Can't fire at a target if we don't have one. But also make sure player can move again.
            if (_targets.Count <= 0) {
                Grid.Instance.SuspendControls = false;

                // If we're in the fire function but we're out of targets, that means we were firing
                // but we're done now. Turn off the sign.
                _cannonTurnSign.SetActive(false);
                return;
            }

            // Get the top (weakest) tile and then remove it from the list.
            Tile tile = _targets[0];
            _targets.RemoveAt(0);

            // Make sure cannon can see the target.
            Vector3 direction = (tile.transform.position - transform.position).normalized;
            Ray ray = new(transform.position, direction);
            float distance = Vector3.Distance(tile.transform.position, transform.position);

            // Sanity check.
            Debug.DrawLine(transform.position, tile.transform.position, Color.red, 1f);

            // Run a raycast to see if there are any obstacles between the turret and the target tile.
            bool canSee = !Physics.Raycast(ray, distance, _layerMask);

            // Give the cannonball the target tile and tell it to fire. It has a callback to this function, so
            // we get back here once the cannon has finished it's journey. If we can't see the target
            // (canSee is false), we just reset the target and come back here.
            _cannonBall.Fire(transform.position, tile, canSee, Fire);

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
