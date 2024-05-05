using FormsMoves;
using Godot;
using FormsTypes;
using System.Collections.Generic;
using FormsStats;


public abstract class Effect 
{
    public Effect(EffectTargetingStyle targetingStyle = EffectTargetingStyle.MoveTarget,
                  Emotion emotion = Emotion.Typeless,
                  PackedScene packedVisualization = null,
                  double visualizationDelay = 0f) 
    {
        _targetingStyle = targetingStyle;
        _emotion = emotion;
        _packedVisualization = packedVisualization;
        _visualizationDelay = visualizationDelay;
        _childEffects = new List<Effect>();
    }

    private Emotion _emotion;
    private EffectTargetingStyle _targetingStyle;

    //For child effects
    private bool _requiresPriority = false;
    private bool _swapSenderAndTarget = false;
    private List<Effect> _childEffects;

    //For visualizations 
    private PackedScene _packedVisualization;
    private double _visualizationDelay = 0f;

    public virtual EffectResult GetEffectResult(Unit sender, Unit target, EffectResult parentEffectResult = null)
    {
        return null;
    }

    //
    // Damage calculations
    //

    public int CalculateDamageToTarget(int rawDamage, DamageType damageType, Unit target)
    {
        if (rawDamage <= 0)
        {
            return 0;
        }

        int retDamage;

        switch (damageType)
        {
            case DamageType.Hit:
                retDamage = CalculateHitDamage(rawDamage, damageType, target);
                break;
            case DamageType.Essence:
                retDamage = CalculateEssenceDamage(rawDamage, damageType, target);
                break;
            default:
                retDamage = 0;
                break;
        }

        return retDamage;
    }

    private int CalculateHitDamage(int damage, DamageType damageType, Unit target)
    {
        damage = DamageRoll(damage);
        damage = StandardDefenseCalculation(damage, target.GetModStat(StatType.Defense));
        return damage;
    }

    private int CalculateEssenceDamage(int damage, DamageType damageType, Unit target)
    {
        damage = StandardDefenseCalculation(damage, target.GetModStat(StatType.Defense));
        return damage;
    }

    private int DamageRoll(int damage)
    {
        const double RANGE = 0.2f;
        double coef = GD.RandRange(1f - RANGE, 1f + RANGE);
        int result = (int)(coef * damage);
        if (result <= 0)
        {
            result = 1;
        }
        return result;
    }

    private int StandardDefenseCalculation(int damage, int defense)
    {
        double coef = (double)100 / (double)defense;
        int ret = (int)(damage * coef);
        if (ret <= 0)
        {
            ret = 1;
        }
        return ret;
    }

    public PackedScene PackedVisualization { get { return _packedVisualization; } set { _packedVisualization = value; } }
    public EffectTargetingStyle TargetingStyle { get { return _targetingStyle; } set { _targetingStyle = value; } }
    public double VisualizationDelay { get { return _visualizationDelay; } set { _visualizationDelay = value; } }
    public List<Effect> ChildEffects { get { return _childEffects; } set { _childEffects = value;} }
    public bool RequiresPriority { get { return _requiresPriority; } set { _requiresPriority = value; } }
    public bool SwapSenderAndTarget { get { return _swapSenderAndTarget; } set { _swapSenderAndTarget = value; } }
}
