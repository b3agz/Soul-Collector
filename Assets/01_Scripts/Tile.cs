using UnityEngine;

namespace SoulCollector {

    public class Tile : MonoBehaviour {

        [SerializeField] private float _dropSpeed = 1f;
        public bool DestroySelf = false;

        void Update() {

            if (!DestroySelf) return;

            transform.position += Vector3.down * Time.deltaTime * _dropSpeed;

        }

    }

}