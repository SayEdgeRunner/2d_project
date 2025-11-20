using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private float _smoothTime = 0.1f;
    private Vector3 _velocity = Vector3.zero;
    private Vector3 _offset;
    private Transform _player;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _offset = transform.position - _player.position;
    }

    private void LateUpdate()
    {
        if (_player == null) return;

        Vector3 targetPosition = _player.position + _offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, _smoothTime);
    }
}