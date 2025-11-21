using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerAttackController _attack;
    private PlayerMoveController _move;

    public void Awake()
    {
        _attack = GetComponent<PlayerAttackController>();
        _move = GetComponent<PlayerMoveController>();
    }

    public void Update()
    {
        _attack.HandleAttack();
        _move.HandleMovement();
    }
}
