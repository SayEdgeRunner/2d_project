using UnityEngine;

public abstract class SkillBase
{
    public ESkillCategory Category { get; private set; }
    public int Level { get; private set; } = 1;
    public const int MaxLevel = 5;

    protected SkillBase(ESkillCategory category)
    {
        Category = category;
    }

    public virtual void LevelUp()
    {
        if (Level >= MaxLevel) return;
        Level++;
    }

    public abstract void Apply(PlayerStatController stat); // 스탯 증가
    public abstract void Execute(PlayerController player, PlayerStatController stat); // 공격 행동
}
