using UnityEngine;

namespace SoulCollector {

    /// <summary>
    /// Place on any GameObject to make it bob up and down smoothly.
    /// </summary>
    public class ObjectBob : MonoBehaviour {

        /// <summary>
        /// How fast the GameObject bobs up and down.
        /// </summary>
        [SerializeField] private float _bobSpeed = 3f;

        /// <summary>
        /// The amount of bob in Unity units. The GameObject will travel this far above and below its starting position.
        /// </summary>
        [SerializeField] private float _bobAmount = 0.25f;

        /// The starting position of the GameObject on the Y axis.
        private float _startY;

        private void Start() {

            _startY = transform.localPosition.y;

        }

        void Update() {

            // Get the current height of the GameObject by feeding Time.time (seconds since game started) into the Mathf.Sin function, which gives us a smooth
            // height bob between -1 and 1. Multiplying it by speed changes how quickly we move along the sine wave, and multiplying the result by an amount
            // changes how far the bob goes. Add the result to the cached starting position and we have our bob height.
            float yPos = _startY + (Mathf.Sin(Time.time * _bobSpeed) * _bobAmount);
            transform.localPosition = new(transform.localPosition.x, yPos, transform.localPosition.z);

        }

    }

}