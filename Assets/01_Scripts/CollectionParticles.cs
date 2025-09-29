using UnityEngine;

namespace SoulCollector {

    public class CollectionParticles : MonoBehaviour {

        [SerializeField] private ParticleSystem _particles;

        void Update() {

            if (_particles.isPlaying) return;
            Destroy(gameObject);

        }

    }

}