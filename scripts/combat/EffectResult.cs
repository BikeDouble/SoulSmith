using FormsMoves;
using Godot;
using System;
using System.Collections.Generic;

public partial class EffectResult : GodotObject
{
    public EffectResult(Unit sender, Unit target, Effect effect)
    {
        Sender = sender;
        Target = target;
        Effect = effect;
        HPChange = 0;
        Modifier = null;
        Trigger = EffectTrigger.None;
        ChildEffects = null;
    }

    public EffectResult(Unit sender, Unit target, Effect effect, EffectTrigger trigger, List<Effect> childEffects = null)
    {
        Sender = sender;
        Target = target;
        Effect = effect;
        HPChange = 0;
        Modifier = null;
        Trigger = trigger;
        ChildEffects = childEffects;
    }

    public EffectResult(Unit sender, Unit target, Effect effect, int hPChange, bool gainDecay = true, List<Effect> childEffects = null)
    {
        Sender = sender;
        Target = target;
        Effect = effect;
        HPChange = hPChange;
        Modifier = null;
        Trigger = EffectTrigger.None;
        ChildEffects = childEffects;
        GainDecay = gainDecay;
    }

    public EffectResult(Unit sender, Unit target, Effect effect, Modifier modifier, List<Effect> childEffects = null)
    {
        Sender = sender;
        Target = target;
        Effect = effect;
        HPChange = 0;
        Modifier = modifier;
        Trigger = EffectTrigger.None;
        ChildEffects = childEffects;
    }

    public Unit Sender { get; }
    public Unit Target { get; }
    public Effect Effect { get; }
    public int HPChange { get; }
    public bool GainDecay { get; }
    public Modifier Modifier { get; }
    public EffectTrigger Trigger { get; }
    public List<Effect> ChildEffects { get; }
    public int EffectiveHPChange = 0;
    public bool EffectSuccessful = true;
}
