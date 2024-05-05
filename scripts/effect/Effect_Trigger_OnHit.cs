using FormsMoves;
using FormsTypes;
using Godot;
using System;

public partial class Effect_Trigger_OnHit : Effect_Trigger
{

    public Effect_Trigger_OnHit(
                          EffectTargetingStyle targetingStyle = EffectTargetingStyle.MoveTarget,
                          Emotion emotion = Emotion.Typeless,
                          PackedScene packedVisualization = null,
                          double visualizationDelay = 0f) : base(EffectTrigger.OnHitting, EffectTargetingStyle.ParentTarget, emotion, packedVisualization, visualizationDelay)
    {
        RequiresPriority = true;
        SwapSenderAndTarget = true;
    }

    public override EffectResult GetEffectResult(Unit sender, Unit target, EffectResult parentEffectResult = null)
    {
        return base.GetEffectResult(sender, target);
    }
}
