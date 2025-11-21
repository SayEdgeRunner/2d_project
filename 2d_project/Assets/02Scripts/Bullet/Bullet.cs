using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 3f;
    private Vector2 _direction;

    public void SetDirection(Vector2 direction)
    {
        _direction = direction;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.Translate(_direction * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") == false) return;
        
        Destroy(gameObject);
    }
}