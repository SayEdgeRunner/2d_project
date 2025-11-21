using UnityEngine;

public class PlayerStatController : MonoBehaviour
{
    [Header("플레이어 스탯")]
    public PlayerStat BaseStat;
    public PlayerStat CurrentStat;

    private void Awake()
    {
        CurrentStat = new PlayerStat();
        InitStat();
    }

    private void InitStat()
    {
        CopyStat(BaseStat, CurrentStat);
    }

    private void CopyStat(PlayerStat baseStat, PlayerStat currentStat)
    {
        var fields = typeof(PlayerStat).GetFields();
        foreach(var field in fields)
        {
            field.SetValue(currentStat, field.GetValue(baseStat));
        }
    }
}
