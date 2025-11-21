using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    [Header("총알")]
    [SerializeField] private Bullet _bullet;
    [SerializeField] private Transform _firePosition;

    [Header("카메라")]
    [SerializeField] private Camera _camera;

    private PlayerStatController _stat;

    private float _coolTimer = 0;

    public void Init(PlayerStatController stat)
    {
        _stat = stat;
    }

    public void HandleAttack()
    {
        _coolTimer += Time.deltaTime;

        float coolTime = _stat.CurrentStat.AttackCoolTime;

        if (_coolTimer < coolTime) return;

        Shoot();
        _coolTimer = 0f;
    }

    private void Shoot()
    {
        Vector3 cursorPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -_camera.transform.position.z);
        Vector3 worldPosition = _camera.ScreenToWorldPoint(cursorPosition);
        Vector2 direction = (worldPosition - _firePosition.position).normalized;

        Bullet bullet = Instantiate(_bullet, _firePosition.position, Quaternion.identity);
        bullet.SetDirection(direction);
    }
}
