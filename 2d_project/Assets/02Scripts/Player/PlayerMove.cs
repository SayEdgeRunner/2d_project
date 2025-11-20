using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private float _directionX = 0f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector2 direction = new Vector2(h, v).normalized;

        _animator.SetBool("IsRunning", direction != Vector2.zero);

        if (direction != Vector2.zero)
        {
            _directionX = direction.x;
        }

        if (_directionX < 0)
        {
            _spriteRenderer.flipX = true;
        }
        else
        {
            _spriteRenderer.flipX = false;
        }

        Vector2 position = transform.position;
        Vector2 newPosition = position + (direction * _speed) * Time.deltaTime;
        
        transform.position = newPosition;
    }
}
