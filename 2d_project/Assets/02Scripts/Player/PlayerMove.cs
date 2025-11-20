using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector2 direction = new Vector2(h, v).normalized;

        UpdateAnimation(direction, h);
        MoveCharacter(direction);
    }

    private void UpdateAnimation(Vector2 direction, float horizontalInput)
    {
        _animator.SetBool("IsRunning", direction != Vector2.zero);

        if (horizontalInput < 0)
        {
            _spriteRenderer.flipX = true;
        }
        else if (horizontalInput > 0)
        {
            _spriteRenderer.flipX = false;
        }
    }

    private void MoveCharacter(Vector2 direction)
    {
        transform.Translate(direction * _speed * Time.deltaTime);
    }
}
