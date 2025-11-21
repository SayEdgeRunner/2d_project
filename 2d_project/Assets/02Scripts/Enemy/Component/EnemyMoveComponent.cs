using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
    public class EnemyMoveComponent : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 3.0f;

        private Rigidbody2D _rigidbody;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Move(Vector2 direction)
        {
            if (direction.sqrMagnitude <= 0f) return;
            Vector2 targetPosition = _rigidbody.position + direction * (_moveSpeed * Time.fixedDeltaTime);
            _rigidbody.MovePosition(targetPosition);

            UpdateFlip(direction.x);
        }

        public void Stop()
        {
            _rigidbody.linearVelocity = Vector2.zero;
        }

        private void UpdateFlip(float directionX)
        {
            if (directionX > 0)
            {
                _spriteRenderer.flipX = false;
            }
            else if (directionX < 0)
            {
                _spriteRenderer.flipX = true;
            }
        }
    }
}