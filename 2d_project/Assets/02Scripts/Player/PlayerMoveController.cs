using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private const string HorizontalAxis = "Horizontal";
    private const string VerticalAxis = "Vertical";
    private const string IsRunningParam = "IsRunning";

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void HandleMovement()
    {
        float h = Input.GetAxisRaw(HorizontalAxis);
        float v = Input.GetAxisRaw(VerticalAxis);

        Vector2 direction = new Vector2(h, v).normalized;

        UpdateAnimation(direction, h);
        MoveCharacter(direction);
    }

    private void UpdateAnimation(Vector2 direction, float horizontalInput)
    {
        _animator.SetBool(IsRunningParam, direction != Vector2.zero);

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
