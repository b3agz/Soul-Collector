using UnityEngine;

namespace SoulCollector {

    /// <summary>
    /// Drop onto any GameObject to make it rotate around its Y axis.
    /// </summary>
    public class Rotator : MonoBehaviour {

        [SerializeField] private float _rotateSpeed = 50f;

        void Update() {
            transform.Rotate(new Vector3(0f, _rotateSpeed * Time.deltaTime, 0f));
        }

    }

}