using UnityEngine;

public class BlinkSkill : SkillBase
{
    public BlinkSkill() : base(ESkillCategory.Sandevistan) { }

    public override void Apply(PlayerStatController stat)
    {
        // 수치 누적
    }

    public override void Execute(PlayerController player, PlayerStatController stat)
    {
        // 무엇을 할지 행동 정의 <- what
        // 어떻게 하는지는 PlayerController가 알고 있어야 함 <- how

        /*
        // 1단계: 이동
        player.DoBlink(s.BlinkDistance);

        // 2단계: 무적
        if (Level >= 2)
            player.SetInvincible(s.BlinkInvincibleTime);
        */
    }
}
