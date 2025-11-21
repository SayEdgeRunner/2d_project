using UnityEngine;

[RequireComponent(typeof(PlayerAttackController))]
[RequireComponent(typeof(PlayerMoveController))]
[RequireComponent(typeof(PlayerStatController))]
public class PlayerController : MonoBehaviour
{
    private PlayerAttackController _attack;
    private PlayerMoveController _move;
    private PlayerStatController _stat;

    private void Awake()
    {
        _attack = GetComponent<PlayerAttackController>();
        _move = GetComponent<PlayerMoveController>();
        _stat = GetComponent<PlayerStatController>();

        _attack.Init(_stat);
        _move.Init(_stat);
    }

    private void Update()
    {
        _attack.HandleAttack();
        _move.HandleMovement();
    }
}
