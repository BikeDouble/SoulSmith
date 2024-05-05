using Godot;
using System;
using FormsMoves;
using FormsStats;
using System.Collections.Generic;

public partial class UnitStats : Node
{
	private List<Stat> _statsList = new List<Stat>();

	private int _combatPosition;
	private List<Modifier> _modifiers = new List<Modifier>();

    //
    // The following getters return the stats with all necessary modifiers applied
    // 

    public override void _Ready()
    {
		_statsList.Add(new Stat(100, StatType.Defense));
        _statsList.Add(new Stat(100, StatType.Attack));

		_statsList.Add(new Stat(0, StatType.Decay));
		_statsList.Add(new Stat(33, StatType.DecayRate));

		_statsList.Add(new Stat(1000, StatType.MaxHealth));
		_statsList.Add(new Stat(1000, StatType.CurHealth));

        _statsList.Add(new Stat(100, StatType.Defense));
    }

	public UnitStats()
	{

	}

	public UnitStats(List<Stat> statsList)
	{
		_statsList = statsList;
	}

	public int GetBaseStat(StatType statType)
	{
		foreach (Stat stat in _statsList)
		{
			if (stat.Type == statType)
			{
				return stat.BaseValue;
			}
		}

		return 0;
	}

	public void SetBaseStat(StatType statType, int newValue)
	{
        for (int i = 0; i < _statsList.Count; i++)
        {
            if (_statsList[i].Type == statType)
            {
				_statsList[i].BaseValue = newValue;
            }
        }
    }

    public int GetModStat(StatType statType)
	{
		int retStat = GetBaseStat(statType);
        retStat = ApplyAllRelevantStatModifiers(retStat, statType);
		return retStat;
	}

	private int ApplyAllRelevantStatModifiers(int value, StatType stat) 
	{
        List<StatModifier> statModifiers = GetRelevantStatModifiers(stat);
        StatModifier combinedModifier = CombineStatModifiers(statModifiers);
        int retValue = ApplyStatModifier(value, combinedModifier);

		return retValue;
    }

    private List<StatModifier> GetRelevantStatModifiers(StatType stat)
    {
        List<StatModifier> modifiers = new List<StatModifier>();
        foreach (Modifier modifier in _modifiers)
        {
            StatModifier statModifier = modifier.GetStatModifier(stat);
            if (statModifier.Stats.Contains(stat))
            {
                modifiers.Add(statModifier);
            }
        }

		return modifiers;
    }

    private StatModifier CombineStatModifiers(List<StatModifier> statModifiers)
    {
        if (statModifiers.Count == 0)
        {
            return new StatModifier();
        }

        StatType stat = StatType.None;
        int flatMod = 0;
        double additiveMod = 0f;
        double multiplicativeMod = 1f;

        foreach (StatModifier statModifier in statModifiers)
        {
            flatMod += statModifier.FlatMod;
            additiveMod += statModifier.AdditiveMod;
            multiplicativeMod *= statModifier.MultiplicativeMod;

        }

        return new StatModifier(stat, flatMod, additiveMod, multiplicativeMod);
    }

	private int ApplyStatModifier(int baseStat, StatModifier modifier)
	{
		int retStat = baseStat + modifier.FlatMod;
		retStat = (int)(retStat * (modifier.AdditiveMod + 1));
		retStat = (int)(retStat * modifier.MultiplicativeMod);

		return retStat;
	}

    //
    // Modifier related functions
    //

	private void AddModifier(Modifier modifier)
	{
        _modifiers.Add(modifier);
        AddChild(modifier);
		modifier.RemoveModifier += RemoveModifier;
		modifier.OfferEffectInput += OfferEffect;
        modifier.OnApplied();
    }

	private void RemoveModifier(Modifier modifier)
	{
		_modifiers.Remove(modifier);
		RemoveChild(modifier);
	}

	[Signal]
	public delegate void OfferEffectInputEventHandler(Wrapper<EffectInput> wrappedEffectInput, GodotObject sender);

	private void OfferEffect(Wrapper<EffectInput> wrappedEffectInput, GodotObject sender)
	{
		EmitSignal(SignalName.OfferEffectInput, wrappedEffectInput, this);
	}

	//
	// Damage related functions
	//

	private int LoseHP(int damage, bool gainDecay = true)
	{
		int newHealth = GetBaseStat(StatType.CurHealth) - damage;

        SetBaseStat(StatType.CurHealth, newHealth);

		int effectiveDamage = damage;

        if (newHealth <= 0)
        {
			effectiveDamage += newHealth;
        }

        if (gainDecay)
		{
			int newDecay = GetBaseStat(StatType.Decay) + ((effectiveDamage * GetModStat(StatType.DecayRate))/100);
			SetBaseStat(StatType.Decay, newDecay);
		}

		int decayedMaxHealth = GetBaseStat(StatType.MaxHealth) - GetBaseStat(StatType.Decay);


        if (newHealth > decayedMaxHealth)
		{
			newHealth = decayedMaxHealth;
			SetBaseStat(StatType.CurHealth, newHealth);
		}

		if (newHealth <= 0)
		{
			EmitDeathSignal();
        }
		
		EmitUpdateUISignal();

		return effectiveDamage;
	}

	//
	// Signal emitters
	//

	[Signal]
	public delegate void UpdateUIEventHandler();

	private void EmitUpdateUISignal()
	{
		EmitSignal(SignalName.UpdateUI);
	}

	[Signal]
	public delegate void ZeroHPEventHandler();

	private void EmitDeathSignal()
	{
		EmitSignal(SignalName.ZeroHP);
	}

	//
	// Healing related functions
	//

	public int GainHP(int healing)
	{
		int newHealth = GetBaseStat(StatType.CurHealth + healing);
		SetBaseStat(StatType.CurHealth, newHealth);

        int effectiveHealing = healing;

        int decayedMaxHealth = GetBaseStat(StatType.MaxHealth) - GetBaseStat(StatType.Decay);

		if (newHealth > decayedMaxHealth)
		{
			int lostHealing = newHealth - decayedMaxHealth;

			effectiveHealing -= lostHealing;

			newHealth = decayedMaxHealth;
            SetBaseStat(StatType.CurHealth, newHealth);
        }

		EmitUpdateUISignal();

		return effectiveHealing;
    }

	public void OnDeath()
	{
        SetBaseStat(StatType.CurHealth, 0);
        SetBaseStat(StatType.Decay, GetBaseStat(StatType.MaxHealth));
		EmitUpdateUISignal();
    }

	//
	// Triggers
	//

	public void OnTrigger(EffectTrigger trigger, Unit sender)
	{
		switch (trigger)
		{
			case EffectTrigger.OnHitting:
				OnHitting(sender);
				break;
			case EffectTrigger.OnBeingHit:
				OnBeingHit(sender);
				break;
			case EffectTrigger.OnMoveBegin:
				OnMoveBegin(sender); 
				break;
			case EffectTrigger.OnMoveEnd: 
				OnMoveEnd(sender);
				break;
			case EffectTrigger.OnTurnBegin:
				OnTurnBegin(sender);
				break;
			case EffectTrigger.OnTurnEnd:
				OnTurnEnd(sender);
				break;
			case EffectTrigger.OnRoundBegin:
				OnRoundBegin(sender);
				break;
			case EffectTrigger.OnRoundEnd:
				OnRoundEnd(sender);
				break;
			default:
				break;
		}
	}

	private void OnHitting(Unit target)
	{
		foreach (Modifier modifier in _modifiers)
		{
			modifier.OnHitting(target);
		}
	}

	private void OnBeingHit(Unit hitter)
	{
		foreach (Modifier modifier in _modifiers)
		{
			modifier.OnBeingHit(hitter);
		}
	}

	private void OnMoveBegin(Unit target)
	{
		foreach (Modifier modifier in _modifiers)
		{
			modifier.OnMoveBegin(target);
		}
	}

	private void OnMoveEnd(Unit target)
	{
		foreach (Modifier modifier in _modifiers)
		{
			modifier.OnMoveEnd(target);
		}
	}

	private void OnTurnBegin(Unit self)
	{
		foreach (Modifier modifier in _modifiers)
		{
			modifier.OnTurnBegin(self);
		}
	}

	private void OnTurnEnd(Unit mover)
	{
		foreach (Modifier modifier in _modifiers)
		{
			modifier.OnTurnEnd(mover);
		}
	}

    private void OnRoundBegin(Unit self)
    {
        foreach (Modifier modifier in _modifiers)
        {
            modifier.OnRoundBegin(self);
        }
    }

    private void OnRoundEnd(Unit self)
	{
		foreach (Modifier modifier in _modifiers)
		{
			modifier.OnRoundEnd(self);
		}
	}

    public void ExecuteEffect(EffectResult effectResult)
    {
		if (effectResult == null)
		{
			effectResult.EffectSuccessful = false;
			return;
		}

		if (effectResult.HPChange != 0)
		{
			ExecuteHPChangeEffect(effectResult);
			return;
		}

		if (effectResult.Modifier != null)
		{
			ExecuteModifierEffect(effectResult);
			return;
		}

		if (effectResult.Trigger != EffectTrigger.None)
		{
			ExecuteTriggerEffect(effectResult);
			return;
		}

		effectResult.EffectSuccessful = false;
    }

	public void ExecuteHPChangeEffect(EffectResult effectResult)
	{
		int hPChange = effectResult.HPChange;
		if (hPChange > 0)
		{
			effectResult.EffectiveHPChange = GainHP(hPChange);
		}
		else
		{
			effectResult.EffectiveHPChange = LoseHP(hPChange * -1, effectResult.GainDecay);
		}
	}

	public void ExecuteModifierEffect(EffectResult effectResult)
	{
		Modifier modifier = effectResult.Modifier;
		modifier.ApplyTo(effectResult.Target, effectResult.Sender);
		AddModifier(modifier);
	}

	public void ExecuteTriggerEffect(EffectResult effectResult)
	{
		OnTrigger(effectResult.Trigger, effectResult.Sender);
	}

    public int CombatPosition { get { return _combatPosition; } set { _combatPosition = value; } }
}
