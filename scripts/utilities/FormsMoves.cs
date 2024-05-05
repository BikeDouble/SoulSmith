using System.Collections.Generic;
using FormsStats;
using FormsTypes;

namespace FormsMoves
{

    public enum EffectType
    {
        Damage,
        OnHit,
        Healing,
        Modifier
    }

    public enum DamageType
    {
        Hit,
        Essence
    }

    public enum MoveTargetingStyle
    {
        Enemy,
        AllyOrSelf,
        Ally
    }

    public enum EffectTargetingStyle
    {
        MoveTarget,
        Self,
        LowestHPEnemy,
        HighestHPEnemy,
        LowestHPAllyOrSelf,
        Attacker,
        ParentTarget,
        ParentSender,
        DeterminedPreOffer
    }

    public enum EffectTrigger
    {
        None,
        OnHitting,
        OnBeingHit,
        OnMoveBegin,
        OnMoveEnd,
        OnTurnBegin,
        OnTurnEnd,
        OnRoundEnd,
        OnRoundBegin
    }
    
    public struct EffectInput
    {
        public EffectInput(Effect effect, Unit sender, Unit target = null)
        {
            Effect = effect;
            Sender = sender;
            Target = target;
        }

        public void SwapSenderAndTarget()
        {
            Unit temp = Sender;
            Sender = Target;
            Target = temp;
        }

        public Effect Effect;
        public Unit Sender;
        public Unit Target;
    }

    public readonly struct Move
    {
        public Move(string name,
                    List<Effect> effects,
                    MoveTargetingStyle targetingStyle = MoveTargetingStyle.Enemy,
                    Emotion type = FormsTypes.Emotion.Typeless)
        {
            Name = name;
            Effects = effects;
            TargetingStyle = targetingStyle;
            Type = type;
        }

        public List<Effect> Effects { get; }
        public FormsTypes.Emotion Type { get; }
        public MoveTargetingStyle TargetingStyle { get; }
        public string Name { get; }
        //public int Index = -1; 
    }

    public struct MoveInput
    {
        public Move Move;
        public Unit Sender;
        public Unit Target;
    }

    public static class MoveHelpers
    {
        public static Effect StatHit(double percent, StatType stat)
        {
            Effect effect = new Effect_StatPercent_Damage(DamageType.Hit,
                                                          stat,
                                                          percent
                                                          );
            effect.ChildEffects.Add(new Effect_Trigger_OnHit());
            effect.ChildEffects.Add(new Effect_Trigger(EffectTrigger.OnBeingHit, EffectTargetingStyle.ParentTarget));
            return effect;
        }

        //Returns an attack that deals (percent * attacker's attack stat)
        public static Effect AttackHit(double percent)
        {
            Effect effect = new Effect_StatPercent_Damage(DamageType.Hit,
                                                          StatType.Attack,
                                                          percent
                                                          );
            effect.ChildEffects.Add(new Effect_Trigger_OnHit());
            effect.ChildEffects.Add(new Effect_Trigger(EffectTrigger.OnBeingHit, EffectTargetingStyle.ParentTarget));
            return effect;
        }

    }
}

