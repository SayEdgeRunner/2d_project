using UnityEngine;

[System.Serializable]
public class PlayerStat
{
    public float Health = 100f;     
    public float MaxHealth = 100f;
    public float MoveSpeed = 5f;   
    public float AttackDamage = 20f;       
    public float AttackSpeed = 0.5f;
    public float AttackCoolTime = 1f;

    public float SandevistanDuration = 3f;
    public float SandevistanCooldown = 15f;

    public float SandevistanExtraDuration = 1f;
    public float SandevistanCooldownReduceRate = 0.2f;
    public float SandevistanDamageBonusRate = 0.3f;
    public int SandevistanBulletRicochet = 2;
    public float SandevistanEndBuffDuration = 3f;
    public float SandevistanEndMoveSpeedBonus = 0.3f;

    public float BlinkDistance = 5f;
    public int BlinkCount = 2;
    public float BlinkInvincibleTime = 1f;
    public float BlinkShockArea = 3f;
    public float BlinkShockDamageRate = 1.5f;
    public float BlinkFieldDuration = 2f;
    public float BlinkFieldRange = 5f;

    public float PlasmaFieldRange = 5f;
    public float PlasmaFieldDamageRate = 0.8f;
    public float LaserDamageBonusRate = 0.5f;
    public int LaserPierceCount = 5;
    public float MissileFireInterval = 2f;
    public int MissileCount = 3;
    public float MissileDamageRate = 2.0f;

    public float HackRange = 8f;
    public float HackCooldown = 8f;
    public int HackTargetCount = 1;
    public float HackDuration = 2f;

    public float HackDotDamageRate = 1.0f;
    public float HackDotDuration = 5f;
    public int HackSpreadCount = 3;
    public float HackSpreadRange = 5f;
    public int HackExplodeCondition = 20;
    public float HackExplodeDamageRate = 1.5f;

    public float HackSlowRate = 0.3f;
    public float HackSlowDuration = 2f;
    public int HackSlowSpreadCount = 6;
    public float HackDamageIncreaseRate = 0.3f;
    public int HackFieldCondition = 50;
    public float HackFieldRange = 8f;
    public float HackFieldDuration = 5f;
    public float HackFieldDamageRate = 1.2f;

    public int HackControlCount = 3;
    public float HackControlDuration = 5f;
    public int HackControlSpreadCount = 2;
    public float HackControlExplodeRange = 5f;
    public float HackControlExplodeDamageRate = 2.0f;

    public float BerserkDuration = 8f;
    public float BerserkCooldown = 12f;
    public float BerserkDamageRate = 0.5f;
    public float BerserkAttackSpeed = 0.3f;
    public float BerserkRange = 3f;

    public float PunchInterval = 3f;
    public float PunchDistance = 10f;
    public float PunchDamageRate = 2.5f;
    public float PunchFieldDuration = 1f;
    public float PunchFieldDamageRate = 0.8f;
    public float PunchBuffDuration = 5f;
    public float PunchBuffDamageRate = 0.3f;
    public float PunchShockRange = 8f;

    public float FrenzyAttackSpeedBonus = 0.2f;
    public Vector2 FrenzyAttackRange = new Vector2(7f, 5f);
    public int FrenzyJumpCount = 3;
    public float FrenzyJumpArea = 6f;
    public float FrenzyJumpDamageRate = 2.2f;

    public float PsychoProjectileInterval = 2f;
    public int PsychoProjectileCount = 2;
    public float PsychoProjectileDamageRate = 1.5f;
    public int PsychoProjectilePierce = 3;
    public float PsychoExplodeRange = 4f;
    public float PsychoExplodeDamageRate = 1.8f;
    public float PsychoProjectileSizeMultiplier = 2f;
    public float PsychoFinalDamageBonusRate = 1.0f;


    public PlayerStat Clone()
    {
        return this.MemberwiseClone() as PlayerStat;
    }
}
