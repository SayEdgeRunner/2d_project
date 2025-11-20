using Unity.Mathematics;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private float _smooth = 8f;
    private GameObject _player;
    private Vector3 _offset;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _offset = transform.position - _player.transform.position;
    }

    private void LateUpdate()
    {
        if (_player == null) return;

        Vector3 targetPosition = _player.transform.position + _offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, _smooth * Time.deltaTime);
    }
}