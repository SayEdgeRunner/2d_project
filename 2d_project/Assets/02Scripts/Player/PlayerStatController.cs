using UnityEngine;

public class PlayerStatController : MonoBehaviour
{
    [Header("플레이어 스탯")]
    [SerializeField] private PlayerStat _baseStat;
    private PlayerStat _currentStat;
    public PlayerStat CurrentStat => _currentStat;

    private void Awake()
    {
        _currentStat = new PlayerStat();
        InitStat();
    }

    private void InitStat()
    {
        _currentStat = _baseStat.Clone();
    }
}
