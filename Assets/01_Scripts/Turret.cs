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
        [SerializeField] private Transform _cannonMouth;
        [SerializeField] private GameObject _cannonTurnSign;
        [SerializeField] private GameObject _cantSeeText;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private LineRenderer _laserSight;
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

            // If we encounter targets we can't hit, we need to remove them from the targets list.
            // We can'd do that during the loop as it will cause us to skip over one after the list
            // is reshuffled, so we add any unhittable target tiles to a list of ints and remove
            // them after.
            List<Tile> tilesToRemove = new();

            // Make sure the player knows it's the cannon's turn so they're not confused as to why
            // they can't move.
            _cannonTurnSign.SetActive(true);
            for (int i = 0; i < _shotsPerRound; i++) {

                _laserSight.gameObject.SetActive(false);
                _lookDirection = (_targets[i].transform.position - transform.position).normalized;

                // Hacky bullshit to give cannon chance to rotate to face new target.
                yield return new WaitForSeconds(0.25f);

                _laserSight.gameObject.SetActive(true);

                Ray ray = new(transform.position, _lookDirection);
                float distance = Vector3.Distance(_targets[i].transform.position, transform.position);

                Vector3[] points = new Vector3[2];
                points[0] = _cannonMouth.position;

                // Run a raycast to see if there are any obstacles between the turret and the target tile.
                if (Physics.Raycast(ray, out RaycastHit hitInfo, distance, _layerMask)) {

                    // If we hit an obstacle, the lasersight should end where the hit was.
                    points[1] = hitInfo.point;
                    tilesToRemove.Add(_targets[i]);
                    _cantSeeText.SetActive(true);

                }
                else {

                    // If we don't hit an obstacle, end the lasersight at the target and activate the marker.
                    points[1] = _targets[i].transform.position;
                    _targets[i].ShowMarker();

                }

                _laserSight.SetPositions(points);
                yield return new WaitForSeconds(1f);
                _cantSeeText.SetActive(false);
            }

            // Remove unhittable targets from our targets list.
            foreach(Tile tile in tilesToRemove) {
                _targets.Remove(tile);
            }

            _laserSight.gameObject.SetActive(false);

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

            StartCoroutine(DelayedFire());

        }

        private IEnumerator DelayedFire() {

            // Get the top (weakest) tile and then remove it from the list.
            Tile tile = _targets[0];
            _targets.RemoveAt(0);

            _lookDirection = (tile.transform.position - transform.position).normalized;

            yield return new WaitForSeconds(0.25f);

            _cannonBall.Fire(_cannonMouth.position, tile, Fire);

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
