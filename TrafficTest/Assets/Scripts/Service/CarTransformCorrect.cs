using UnityEngine;

public class CarTransformCorrect : MonoBehaviour
{
    private Transform _transform;
    private Vector3 _start;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _start = _transform.localPosition;
    }

    private void Update()
    {
        _transform.localPosition = _start + Vector3.left * (_transform.parent.forward.x + 2) * _transform.parent.forward.x * 0.25f + Vector3.right * (2 - _transform.parent.forward.z) * _transform.parent.forward.z * 0.25f;
    }
}
