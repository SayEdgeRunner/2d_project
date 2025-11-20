using System.Collections;
using System.Threading;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    [Header("°ø°Ý")]
    [SerializeField] private float _shootCoolTime = 2f;
    private float _shootTimer = 0;

    [Header("ÃÑ¾Ë")]
    [SerializeField] private GameObject _bullet;
    [SerializeField] private Transform _firePosition;

    private Camera _camera;


    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        TryShoot();
    }

    private void TryShoot()
    {
        _shootTimer += Time.deltaTime;

        if (_shootTimer < _shootCoolTime) return;

        Shoot();
        _shootTimer = 0f;
    }

    private void Shoot()
    {
        Vector3 cursorPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -_camera.transform.position.z);
        Vector3 mouseWorldPosition = _camera.ScreenToWorldPoint(cursorPosition);
        Vector2 direction = (mouseWorldPosition - _firePosition.position).normalized;

        var bulletObject = Instantiate(_bullet, _firePosition.position, Quaternion.identity);
        Bullet bullet = bulletObject.GetComponent<Bullet>();
        bullet.SetDirection(direction);
    }
}
