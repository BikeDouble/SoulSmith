using Godot;
using System;
using System.Collections.Generic;
using FormsMoves;
using FormsStats;
using FormsTypes;

public partial class Unit : Node2D
{
	[ExportGroup("Children")]
	[Export]
	private UnitUI _uI;
	[Export] 
	private UnitStats _stats;
	[Export]
	private UnitSprite _sprite;

	private List<Move> _moveSet = new List<Move>();
	private bool _inCombat = false;
	private bool _playerControlled = false;
	private int _combatPosition;
	private Emotion _emotion;
	
	public Unit()
	{

	}

	public Unit(UnitTemplate template)
	{
		_uI = new UnitUI();
		_stats = new UnitStats(template.StatsList);
		//TODO randomized moveset?
		_moveSet = template.MoveSet;
		_emotion = template.Emotion;
		_sprite = (UnitSprite)template.Sprite.Instantiate();
	}

    public Unit(UnitTemplate template, List<Stat> statsList, List<Move> moveSet)
    {
        _uI = new UnitUI();
        _stats = new UnitStats(statsList);
        //TODO randomized moveset?
        _moveSet = moveSet;
        _emotion = template.Emotion;
        _sprite = (UnitSprite)template.Sprite.Instantiate();
    }

    public override void _Ready()
	{
		Initialize();
	}

	private void Initialize()
	{
		InitializeStats();
		//TODO REMOVE
		_moveSet.Add(Hit());
		_moveSet.Add(DoubleHit());
		_moveSet.Add(AttackUp());
	}

	private void InitializeStats()
	{
		_stats.UpdateUI += UpdateUI;
		_stats.ZeroHP += EmitZeroHPSignal;
		_stats.OfferEffectInput += OfferEffect;
	}

	[Signal]
	public delegate void OfferEffectInputEventHandler(Wrapper<EffectInput> wrappedEffectInput, GodotObject sender);

	private void OfferEffect(Wrapper<EffectInput> wrappedEffectInput, GodotObject sender)
	{
		EmitSignal(SignalName.OfferEffectInput, wrappedEffectInput, this);
	}

	public void Delete()
	{
		QueueFree();
	}

	//
	// Combat related functions
	//

	//Everything the unit needs to do to prepare for combat
	public void OnJoinCombat()
	{
		_inCombat = true;
		_stats.CombatPosition = _combatPosition;
		InitializeUI();
	}

	public void RemoveFromCombat()
	{
		_inCombat = false;
		_combatPosition = -1;
	}

	public int GetModStat(StatType stat)
	{
		return _stats.GetModStat(stat);
	}

	public int GetBaseStat(StatType stat)
	{
		return _stats.GetBaseStat(stat);
	}

	[Signal]
	public delegate void ZeroHPEventHandler();

	private void EmitZeroHPSignal()
	{
		EmitSignal(SignalName.ZeroHP);
	}

	//
	// UI related functions
	//
	public void UpdateUI()
	{
		_uI.Update(_stats);
	}

	public void InitializeUI()
	{
		_uI.Initialize(_moveSet, _playerControlled);
		_uI.MoveButtonPressed += OnMoveButtonPressed;
		_uI.TargetButtonPressed += OnTargetButtonPressed;
	}

	public void ShowMoveSelectUI()
	{
		if (_moveSet.Count > 0)
		{
			_uI.ShowMoveSelect();
		}
	}

	public void ShowTargetSelectUI()
	{
		_uI.ShowTargetSelect();
	}

	[Signal]
	public delegate void OfferMoveAndUserEventHandler(Wrapper<MoveInput> wrappedMoveInput);

	//Listens to UI MoveButtonPressed
	private void OnMoveButtonPressed(Wrapper<Move> wrappedMove)
	{
		MoveInput moveInput = new MoveInput();
		moveInput.Move = wrappedMove;
		moveInput.Sender = this;
		EmitSignal(SignalName.OfferMoveAndUser, new Wrapper<MoveInput>(moveInput));
	}

	[Signal]
	public delegate void OfferTargetEventHandler(Unit target);

	//Listens to UI TargetButtonPressed
	private void OnTargetButtonPressed()
	{
		EmitSignal(SignalName.OfferTarget, this);
	}

	public void HideMoveSelectUI()
	{
		_uI.HideMoveSelect();
	}

	public void HideTargetSelectUI()
	{
		_uI.HideTargetSelect();
	}

	public bool InCombat { get { return _inCombat; } }
	public UnitStats Stats { get { return _stats; } }
	public List<Move> MoveSet { get { return _moveSet; } }
	public int CombatPosition { get { return _combatPosition; } set { _combatPosition = value; } }
	public bool PlayerControlled { get { return _playerControlled; } set { _playerControlled = value; } }
	public UnitUI UI { get { return _uI; } }
	public UnitSprite Sprite { get { return _sprite; } }

	//TODO REMOVE!!

	//TODO Move this to a proper resource library
	private PackedScene _eV = GD.Load<PackedScene>("res://scenes/effectVisualizations/effect_visualization_missile_typeless_pellet.tscn");

	//Temporary code
	private Move Hit()
	{
		List<Effect> effects = new List<Effect>();
		Effect effect = MoveHelpers.AttackHit(1f);
		effect.PackedVisualization = _eV;
		effects.Add(effect);

		Move move = new Move("Hit", effects);

		return move;
	}

	private Move DoubleHit()
	{

		List<Effect> effects = new List<Effect>();
		Effect effect = MoveHelpers.AttackHit(0.4f);
		effect.PackedVisualization = _eV;
		effect.TargetingStyle = EffectTargetingStyle.MoveTarget;
		effects.Add(effect);

		effect = MoveHelpers.AttackHit(0.4f);
		effect.PackedVisualization = _eV;
		effect.TargetingStyle = EffectTargetingStyle.MoveTarget;
		effect.VisualizationDelay = 0.3f;
		effects.Add(effect);

		Move move = new Move("Double Hit", effects);
		return move;
	}

	public void ExecuteEffect(EffectResult effectResult)
	{
		_stats.ExecuteEffect(effectResult);
	}

	private Move AttackUp()
	{
		List<Effect> effects = new List<Effect>();
		StatModifier statModifier = new StatModifier(StatType.Attack, 0, 4, 1);
		Effect effect = new Effect_StatModifier(statModifier, EffectTargetingStyle.MoveTarget);
		effects.Add(effect);

		Move move = new Move("Attack Up", effects, MoveTargetingStyle.AllyOrSelf);

		return move;
	}
}
