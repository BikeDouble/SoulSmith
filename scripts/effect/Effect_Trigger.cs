using FormsMoves;
using FormsTypes;
using Godot;
using System;

public partial class Effect_Trigger : Effect
{
    private EffectTrigger _trigger;

    public Effect_Trigger(EffectTrigger trigger,
                          EffectTargetingStyle targetingStyle = EffectTargetingStyle.MoveTarget,
                          Emotion emotion = Emotion.Typeless,
                          PackedScene packedVisualization = null,
                          double visualizationDelay = 0f) : base(targetingStyle, emotion, packedVisualization, visualizationDelay)
    {
        _trigger = trigger;
    }

    public override EffectResult GetEffectResult(Unit sender, Unit target, EffectResult parentEffectResult = null)
    {
        return new EffectResult(sender, target, this, _trigger);
    }
}
