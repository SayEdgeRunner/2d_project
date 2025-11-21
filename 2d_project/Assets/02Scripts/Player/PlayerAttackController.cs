using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    [Header("공격")]
    [SerializeField] private float _shootCoolTime = 2f;
    private float _shootTimer = 0;

    [Header("총알")]
    [SerializeField] private Bullet _bullet;
    [SerializeField] private Transform _firePosition;

    [Header("카메라")]
    [SerializeField] private Camera _camera;


    public void HandleAttack()
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

        Bullet bullet = Instantiate(_bullet, _firePosition.position, Quaternion.identity);
        bullet.SetDirection(direction);
    }
}
