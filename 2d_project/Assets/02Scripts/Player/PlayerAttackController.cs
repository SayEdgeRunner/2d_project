using System.Threading;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    [Header("°ø°Ý")]
    [SerializeField] private float _shootCoolTime = 2f;
    private float _shootTimer = 0;

    [Header("Àû Å½Áö")]
    [SerializeField] private float _detectionRange = 6.5f;
    [SerializeField] private float _detectionCoolTime = 0.2f;
    private float _detectionTimer = 0;

    private Transform _target;
    private ContactFilter2D _filter;
    private Collider2D[] _hits;
    private const int MaxHits = 10;

    [Header("ÃÑ¾Ë")]
    [SerializeField] private GameObject _bullet;
    private Vector2 _direction;

    private Animator _animator;

    private void Awake()
    {
        _filter = new ContactFilter2D();
        _filter.SetLayerMask(LayerMask.GetMask("Enemy"));
        _hits = new Collider2D[MaxHits];

        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateTarget();
    }


    private Transform UpdateTarget()
    {
        _detectionTimer += Time.deltaTime;

        if (_detectionTimer > _detectionCoolTime)
        {
            _detectionTimer = 0;
            _target = DetectTarget();
        }
        return _target;
    }

    private Transform DetectTarget()
    {
        int hitCount = Physics2D.OverlapCircle(transform.position, _detectionRange, _filter, _hits);

        float closestDistance = Mathf.Infinity;
        Transform closesetTarget = null;

        for (int i = 0; i < hitCount; i++)
        {
            var hit = _hits[i];
            if (hit == null) continue;
            if (hit.CompareTag("Enemy") == false) continue;

            float distance = (transform.position - hit.transform.position).sqrMagnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closesetTarget = hit.transform;
            }
            _hits[i] = null;
        }
        Debug.Log(closesetTarget);
        return closesetTarget;
    }

    private void TryShoot()
    {
        _shootTimer += Time.deltaTime;

        if (_shootTimer > _shootCoolTime)
        {
            Shoot();
            _shootTimer = 0f;
        }
    }

    private void Shoot()
    {
        _animator.SetBool("IsShooting", _shootTimer > _shootCoolTime);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }
}
