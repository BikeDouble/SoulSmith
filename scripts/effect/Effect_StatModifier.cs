using Godot;
using System;
using FormsStats;
using FormsMoves;
using FormsTypes;

public partial class Effect_StatModifier : Effect
{
    private StatModifier _statModifier;

    public Effect_StatModifier(StatModifier statModifier, 
                               EffectTargetingStyle targetingStyle = EffectTargetingStyle.MoveTarget,
                               Emotion emotion = Emotion.Typeless,
                               PackedScene visualization = null,
                               double visualizationDelay = 0f) : base(targetingStyle, emotion, visualization, visualizationDelay)
    {
        _statModifier = statModifier;
    }

    public override EffectResult GetEffectResult(Unit sender, Unit target, EffectResult parentEffectResult = null)
    {
        Modifier modifier = new Modifier_Stat(_statModifier);
        EffectResult result = new EffectResult(sender, target, this, modifier);
        return result;
    }
}