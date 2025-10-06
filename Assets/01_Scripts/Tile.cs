using UnityEngine;

namespace SoulCollector {

    public class Tile : MonoBehaviour {

        [SerializeField] private bool _isDestructable = false;
        [SerializeField] private float _dropSpeed = 1f;
        public bool DestroySelf { get; private set; } = false;

        public void DropTile() {
            if (!_isDestructable) return;
            DestroySelf = true;
        }

        void Update() {

            // If this tile is not set to destroy itself, just return, we don't want to do anything.
            if (!DestroySelf) return;

            // If it is set to destroy itself, move it downwards.
            transform.position += _dropSpeed * Time.deltaTime * Vector3.down;

            // Once it has fallen far enough, destroy it.
            if (transform.position.y < -50f) Destroy(gameObject);

        }

    }

}