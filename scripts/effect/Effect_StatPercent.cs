using FormsMoves;
using FormsStats;
using FormsTypes;
using Godot;
using System;

public partial class Effect_StatPercent : Effect
{
    private StatType _stat; //Which stat effects effect
    private double _statPercent; //How much of the stat effects the effect

    public Effect_StatPercent(StatType stat,
                  double statPercent,
                  EffectTargetingStyle targetingStyle = EffectTargetingStyle.MoveTarget,
                  Emotion emotion = Emotion.Typeless,
                  PackedScene packedVisualization = null,
                  double visualizationDelay = 0f)
                  :base(targetingStyle, emotion, packedVisualization, visualizationDelay)
    {
        _stat = stat;
        _statPercent = statPercent;
    }

    public int CalculateMagnitude(Unit statEffector)
    {
        int magnitude = statEffector.Stats.GetModStat(_stat);
        magnitude = (int)(magnitude * _statPercent);
        return magnitude;
    }
}
