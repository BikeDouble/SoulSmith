

using FormsStats;
using Godot;
using FormsMoves;

public partial class Modifier : Node
{
    private Unit _host;
    private Unit _applier;
    private bool _permanent = true;
    private int _roundsDuration;

    public Modifier(int duration = -1)
    {
        if (duration >= 0)
        {
            _permanent = false;
            _roundsDuration = duration;
        }
    }

    public virtual void ApplyTo(Unit host, Unit applier = null)
    {
        _applier = applier;
        _host = host;
    }

    public virtual void OnApplied()
    {

    }

    [Signal]
    public delegate void RemoveModifierEventHandler(Modifier sender);

    public virtual void Remove()
    {
        EmitSignal(SignalName.RemoveModifier, this);
        OnRemoved();
        QueueFree();
    }

    public virtual void OnRemoved()
    {
        
    }

    [Signal]
    public delegate void OfferEffectInputEventHandler(Wrapper<EffectInput> wrappedEffectInput, GodotObject sender);
    
    public void OfferEffect(EffectInput effectInput)
    {
        EmitSignal(SignalName.OfferEffectInput, new Wrapper<EffectInput>(effectInput), this);
    }

    //
    //Triggers
    //

    public virtual void OnHitting(Unit target)
    {

    }

    public virtual void OnBeingHit(Unit hitter)
    {

    }

    public virtual void OnMoveBegin(Unit target)
    {

    }

    public virtual void OnMoveEnd(Unit target)
    {

    }

    public virtual void OnTurnBegin(Unit self)
    {

    }

    public virtual void OnTurnEnd(Unit mover)
    {

    }

    public virtual void OnRoundBegin(Unit self)
    {

    }

    public virtual void OnRoundEnd(Unit self)
    {
        if (!_permanent) 
        {
            _roundsDuration--;
            if (_roundsDuration <= 0)
            {
                Remove();
            }
        }
    }

    public virtual void SetRoundsDuration(int duration)
    {
        _permanent = false;
        _roundsDuration = duration;
    }

    public virtual StatModifier GetStatModifier(StatType stat)
    {
        return new StatModifier();
    }


}
