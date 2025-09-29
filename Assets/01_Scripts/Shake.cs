using UnityEngine;

/// <summary>
/// Shakes a GameObject. For simplicity's sake, the shaking object must
/// be a child of this object at local Vector3.zero.
/// </summary>
public class Shake : MonoBehaviour {

    public bool _shake = false;
    [SerializeField] private Transform _child;
    [SerializeField] private float _shakeAmount;
    [SerializeField] private float _shakeLength;

    private Vector3 _startPosition;
    private float _timer;

    void Start() {

        _startPosition = _child.position;

    }

    void Update() {

        // If we're not shaking, just return.
        if (!_shake) return;

        // Get a random point in the shake radius and set the local position
        // of the child to that point.
        Vector3 shakeOffset = Random.insideUnitSphere * _shakeAmount;
        _child.localPosition = shakeOffset;

        // Increment the timer and check if we are finished.
        _timer += Time.deltaTime;
        if (_timer > _shakeLength) _shake = false;

    }

    /// <summary>
    /// Resets the shake values. This ensures that if this function is
    /// called while the object is already shaking, it just resets the
    /// length of the shake.
    /// </summary>
    public void ShakeIt() {
        _timer = 0f;
        _shake = true;
    }


}
