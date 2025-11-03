using UnityEngine;
using John;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace SoulCollector {

    public class Score : MonoBehaviour {

        [SerializeField] private Slider _slider;
        [SerializeField] private TextMeshProUGUI _scoreNumber;
        [SerializeField] private float _slideSpeed = 8f;
        [SerializeField] private AnimationCurve _bounceCurve;
        [SerializeField] private float _bounceSpeed = 5f;
        [SerializeField] private float _bounceSize = 0.2f;
        private float _target;

        public void Update() {
            _slider.value = Mathf.MoveTowards(_slider.value, _target, Time.deltaTime * _slideSpeed);
        }

        /// <summary>
        /// Updates the slider score UI element, treating the total collectables as the max amount.
        /// </summary>
        /// <param name="score">Current score.</param>
        /// <param name="maxScore">Total collectables.</param>
        public void UpdateScore(int score, int maxScore, bool suppressBounce = false) {
            _slider.maxValue = maxScore;
            _target = score;
            if (_scoreNumber != null)
                _scoreNumber.text = score.ToString();

            if (suppressBounce) return;

            if (_bounceRoutine != null) {
                StopCoroutine(_bounceRoutine);
            }
            _bounceRoutine = StartCoroutine(Bounce());
        }

        private Coroutine _bounceRoutine;
        private IEnumerator Bounce() {

            Vector3 bounceVector = Vector3.one * _bounceSize;
            float factor = 0f;
            while (factor < 1f) {
                transform.localScale = Vector3.one + (bounceVector * _bounceCurve.Evaluate(factor));
                if (_scoreNumber != null)
                    _scoreNumber.transform.localScale = (Vector3.one * 1.3f) + (bounceVector * _bounceCurve.Evaluate(factor));
                factor += Time.deltaTime * _bounceSpeed;
                yield return null;
            }
            transform.localScale = Vector3.one;
            if (_scoreNumber != null)
                _scoreNumber.transform.localScale = Vector3.one;
        }

    }

}