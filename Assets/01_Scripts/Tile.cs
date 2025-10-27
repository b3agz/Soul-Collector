using UnityEngine;

namespace SoulCollector {

    public class Tile : MonoBehaviour {

        public int Health { get; private set; }
        public bool DestroySelf { get; private set; } = false;

        [SerializeField] private float _colLerpSpeed = 3f;
        [SerializeField] private float _dropSpeed = 1f;
        [SerializeField] private Color[] _colours;
        [SerializeField] private MeshRenderer _meshRenderer;

        private Color _currentColour => _colours[Health - 1];

        private void Start() {
            Health = _colours.Length;
            _meshRenderer.material.color = _currentColour;
        }

        public void TakeDamage() {

            if (DestroySelf) return;

            Health = Mathf.Max(0, Health - 1);
            if (Health <= 0) {
                DropTile();
                return;
            }
        }

        public void DropTile() {
            DestroySelf = true;
        }

        void Update() {

            // If this tile is not set to destroy itself, just return, we don't want to do anything.
            if (!DestroySelf) {
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

