using UnityEngine;
using TMPro;

namespace SoulCollector {

    public class Tile : MonoBehaviour {

        private int _maxHealth = 10;
        public int Health { get; private set; }
        public bool DestroySelf { get; private set; } = false;

        [SerializeField] private float _colLerpSpeed = 3f;
        [SerializeField] private float _dropSpeed = 1f;
        [SerializeField] private Color[] _colours;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private TextMeshPro _healthIndicator;

        private Color _currentColour => _colours[_colourIndex];
        private int _colourIndex = 0;

        private void Start() {
            Health = Random.Range(0, _maxHealth);
            UpdateColourIndex();
            _meshRenderer.material.color = _currentColour;
            _healthIndicator.text = Health.ToString();
        }

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

        public void DropTile() {
            DestroySelf = true;
            Grid.Instance.RemoveTile(new((int)transform.position.x, (int)transform.position.z));
        }

        private void UpdateColourIndex() =>_colourIndex = Mathf.RoundToInt(((float)Health / _maxHealth) * (_colours.Length - 1));

        void Update() {

            // If this tile is not set to destroy itself, just return, we don't want to do anything.
            if (!DestroySelf) {
                if (_colourIndex < 0 || _colourIndex >= _colours.Length) Debug.Log(_colourIndex);
                _meshRenderer.material.color = Color.Lerp(_meshRenderer.material.color, _currentColour, Time.deltaTime * _colLerpSpeed);
                return;
            }

            // If it is set to destroy itself, move it downwards.
            transform.position += _dropSpeed * Time.deltaTime * Vector3.down;

            // Once it has fallen far enough, destroy it.
            if (transform.position.y < -50f) Destroy(gameObject);

        }
    }
}

