using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerAttackController _attack;
    private PlayerMoveController _move;
    private PlayerStatController _stat;

    public void Awake()
    {
        _attack = GetComponent<PlayerAttackController>();
        _move = GetComponent<PlayerMoveController>();
        _stat = GetComponent<PlayerStatController>();

        _attack.Init(_stat);
        _move.Init(_stat);
    }

    public void Update()
    {
        _attack.HandleAttack();
        _move.HandleMovement();
    }
}
