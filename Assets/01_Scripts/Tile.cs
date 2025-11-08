using UnityEngine;
using TMPro;

namespace SoulCollector {

    public class Tile : MonoBehaviour {

        public int Health { get; private set; }
        public bool DestroySelf { get; private set; } = false;

        [SerializeField] private float _colLerpSpeed = 3f;
        [SerializeField] private float _dropSpeed = 1f;
        [SerializeField] private Color[] _colours;
        [SerializeField] private Color _invulnerableColour;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private TextMeshPro _healthIndicator;

        private Color _currentColour => _colours[_colourIndex];
        private int _colourIndex = 0;
        private Vector2Int _coord;

        /// <summary>
        /// Initialises this tile with the required health.
        /// </summary>
        /// <param name="health">The amount of health this tile starts with.</param>
        public void Init(int health, Vector2Int coord) {
            Health = health;
            _coord = coord;
            UpdateColourIndex();
            _meshRenderer.material.color = _currentColour;
            _healthIndicator.text = Health.ToString();
        }

        /// <summary>
        /// Sets this tile to invulnerable. For simplicities-sake, we do this by just giving
        /// it a really high health value.
        /// </summary>
        public void MakeInvulnerable() {
            _meshRenderer.material.color = _invulnerableColour;
            _healthIndicator.text = "";
            Health = int.MaxValue;
        }

        /// <summary>
        /// Removes <paramref name="amount"/> from the tile's health. If health is zero, drops tile.
        /// </summary>
        /// <param name="amount">The amount of damage to deal.</param>
        public void TakeDamage(int amount) {

            // Don't take damage if we are already destroyed and just haven't despawned yet.
            if (DestroySelf) return;

            Health = Mathf.Max(0, Health - 1);
            if (Health <= 0) {
                DropTile();
                return;
            }

            UpdateColourIndex();

            _healthIndicator.text = Health.ToString();
        }

        /// <summary>
        /// Sets this tile to drop away, becoming no longer traversible.
        /// </summary>
        public void DropTile() {
            DestroySelf = true;
            Grid.Instance.RemoveTile(_coord);
        }

        /// <summary>
        /// Updates the current colour index by converting the current health into a percentage of
        /// max health, then multiplying the length of the colour array by that percentage.
        /// </summary>
        private void UpdateColourIndex() => _colourIndex = Mathf.RoundToInt(((float)Health / Grid.Instance.MaxHealth) * (_colours.Length - 1));

        void Update() {

            // If this tile is not set to destroy itself, just return, we don't want to do anything.
            if (!DestroySelf) {

                Color color = Health > Grid.Instance.MaxHealth ? _invulnerableColour : _currentColour;

                _meshRenderer.material.color = Color.Lerp(_meshRenderer.material.color, color, Time.deltaTime * _colLerpSpeed);
                return;

            }

            // If it is set to destroy itself, move it downwards.
            transform.position += _dropSpeed * Time.deltaTime * Vector3.down;

            // Once it has fallen far enough, destroy it.
            if (transform.position.y < -50f) Destroy(gameObject);

        }
    }
}

