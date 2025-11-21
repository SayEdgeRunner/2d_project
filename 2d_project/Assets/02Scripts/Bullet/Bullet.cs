using Core;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 2f;
    [SerializeField] private float _lifeTime = 5f;

    private PlayerStatController _stat;

    private Vector2 _direction;
    private float _lifeTimer = 0;


    public void Init(Vector2 direction, PlayerStatController stat)
    {
        _direction = direction;
        _stat = stat;
    }

    private void OnEnable()
    {
        _lifeTimer = 0f;
    }

    private void Update()
    {
        Move();
        HandleLifeTime();
    }

    private void Move()
    {
        transform.Translate(_direction * _speed * Time.deltaTime);
    }

    private void HandleLifeTime()
    {
        _lifeTimer += Time.deltaTime;
        if (_lifeTimer < _lifeTime) return;

        PrefabPoolManager.Return(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Hit(collision);
    }

    private void Hit(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") == false) return;

        IDamageable damageable = collision.GetComponent<IDamageable>();
        damageable?.TakeDamage(_stat.CurrentStat.AttackDamage);

        PrefabPoolManager.Return(gameObject);
    }
}