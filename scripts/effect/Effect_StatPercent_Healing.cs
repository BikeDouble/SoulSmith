using FormsMoves;
using FormsStats;
using FormsTypes;
using Godot;
using System;

public partial class Effect_StatPercent_Healing : Effect_StatPercent
{
    public Effect_StatPercent_Healing(StatType stat,
                  double statPercent,
                  EffectTargetingStyle targetingStyle = EffectTargetingStyle.MoveTarget,
                  Emotion emotion = Emotion.Typeless,
                  PackedScene packedVisualization = null,
                  double visualizationDelay = 0f)
                  : base(stat,
                        statPercent,
                        targetingStyle,
                        emotion,
                        packedVisualization,
                        visualizationDelay)
    {

    }

    public override EffectResult GetEffectResult(Unit sender, Unit target, EffectResult parentEffectResult = null)
    {
        int rawHealing = CalculateMagnitude(sender);
        int healing = rawHealing;
        return new EffectResult(sender, target, this, healing);
    }
}
