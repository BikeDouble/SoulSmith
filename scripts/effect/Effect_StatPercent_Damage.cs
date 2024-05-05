using FormsMoves;
using FormsTypes;
using Godot;
using System;
using FormsStats;

public partial class Effect_StatPercent_Damage : Effect_StatPercent
{
    private DamageType _damageType;

    public Effect_StatPercent_Damage(DamageType damageType,
                  StatType stat,
                  double statPercent,
                  EffectTargetingStyle targetingStyle = EffectTargetingStyle.MoveTarget,
                  Emotion emotion = Emotion.Typeless,
                  PackedScene packedVisualization = null,
                  double visualizationDelay = 0f) 
                  :base(stat,
                        statPercent,
                        targetingStyle,
                        emotion,
                        packedVisualization,
                        visualizationDelay)
    {
        _damageType = damageType;
    }

    public override EffectResult GetEffectResult(Unit sender, Unit target, EffectResult parentEffectResult = null)
    {
        int rawDamage = CalculateMagnitude(sender);
        int damage = CalculateDamageToTarget(rawDamage, _damageType, target);
        int hPChange = damage * -1;
        return new EffectResult(sender, target, this, hPChange);
    }
}
