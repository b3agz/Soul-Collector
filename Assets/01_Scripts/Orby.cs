using UnityEngine;
using System.Collections.Generic;

namespace SoulCollector {

    public class Orby : MonoBehaviour {

        [SerializeField] private float _drift = 0.125f;
        [SerializeField] private float _speed = 0.2f;

        private List<Orb> _orbs = new();

        public void Start() {

            foreach (Transform child in transform) {
                _orbs.Add(new Orb(child, _drift, _speed));
            }
        }

        public void Update() {

            foreach (Orb orb in _orbs) {
                orb.UpdatePosition(Time.deltaTime);
            }
        }
    }

    public class Orb {

        public Vector3 CurrentPos => Transform.localPosition;
        public Vector3 TargetPos { get; private set; }
        public Transform Transform { get; private set; }

        private float _drift;
        private float _speed;

        public Orb(Transform transform, float drift = 0.125f, float speed = 0.2f) {
            Transform = transform;
            _drift = drift;
            _speed = speed;
            NewDestination();
        }

        public void NewDestination() {
            Vector3 target = new Vector3();
            target.x = Random.Range(-_drift, _drift);
            target.y = Random.Range(-_drift, _drift);
            target.z = Random.Range(-_drift, _drift);
            TargetPos = target;
        }

        public void UpdatePosition(float delta) {

            Transform.localPosition = Vector3.MoveTowards(CurrentPos, TargetPos, delta * _speed);
            if (Vector3.Distance(CurrentPos, TargetPos) < 0.1f) NewDestination();

        }

    }

}