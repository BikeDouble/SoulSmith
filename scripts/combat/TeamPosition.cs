using Godot;
using System;
using Godot.Collections;
using FormsMoves;

public partial class TeamPosition : Node2D
{
	[Export]
	private int _positionNumber;
	[Export]
	private Unit _unit;

	private bool _movedThisRound = false;
	private bool _containsUnit = true;
	private bool _playerControlled;

    public override void _Ready()
    {
		//TODO move
		AssignUnit(_unit);
    }

	public void DeleteUnit()
	{
		if (_containsUnit)
		{
            RemoveChild(_unit);
            _unit.Delete();
            _unit = null;
            _containsUnit = false;

        }
	}

    //
    // Listeners
    //

    public void ShowMoveSelectUI()
	{
		if (!_movedThisRound && _containsUnit)
		{
			_unit.ShowMoveSelectUI();
		}
	}
	
	public void HideMoveSelectUI()
	{
		if (_containsUnit)
		{
			_unit.HideMoveSelectUI();
		}
	}
	
	public void ShowTargetSelectUI()
	{
		if (_containsUnit)
		{
			_unit.ShowTargetSelectUI();
		}
	}

	public void HideTargetSelectUI()
	{
		if (_containsUnit)
		{
			_unit.HideTargetSelectUI();
		}
	}

	[Signal]
	public delegate void OfferMoveAndUserEventHandler(Wrapper<MoveInput> wrappedMoveInput);

	//Connected to Unit
	private void OnOfferMoveAndUser(Wrapper<MoveInput> wrappedMoveInput)
	{
		EmitSignal(SignalName.OfferMoveAndUser, wrappedMoveInput);
	}

    [Signal]
    public delegate void OfferTargetEventHandler(Unit target);

    //Connected to Unit
    private void OnOfferTarget(Unit target)
	{
		EmitSignal(SignalName.OfferTarget, target);
	}

	public void AssignUnit(Unit unit)
	{
		_unit = unit;
        _containsUnit = true;
		_unit.PlayerControlled = _playerControlled;
		_unit.OfferMoveAndUser += OnOfferMoveAndUser;
		_unit.OfferTarget += OnOfferTarget;
		_unit.OfferEffectInput += OfferEffect;
		_unit.ZeroHP += OnUnitZeroHP;
		_unit.CombatPosition = _positionNumber;

        _unit.OnJoinCombat();
    }

	public void RemoveUnitFromCombat()
	{
		if (!_containsUnit)
		{
			return;
		}

		_unit.RemoveFromCombat();
		RemoveChild(_unit);
		_unit = null;
		_containsUnit = false;
	}

	[Signal]
	public delegate void UnitZeroHPEventHandler(Unit unit);

	private void OnUnitZeroHP()
	{
		EmitSignal(SignalName.UnitZeroHP, _unit);
	}
		
	public void OnUnitMoveAction()
	{
		_movedThisRound = true;
	}
	
	public void OnUnitLeaveCombat()
	{
		_containsUnit = false;
	}
	
	public void OnBeginRound()
	{
		_movedThisRound = false;
	}

    [Signal]
    public delegate void OfferEffectInputEventHandler(Wrapper<EffectInput> wrappedEffectInput, GodotObject sender);

    private void OfferEffect(Wrapper<EffectInput> wrappedEffectInput, GodotObject sender)
    {
        EmitSignal(SignalName.OfferEffectInput, wrappedEffectInput, this);
    }

    public void ExecuteEffect(EffectResult effectResult)
    {
        if (_containsUnit)
		{
			if (effectResult.Trigger == EffectTrigger.OnMoveBegin)
			{
				_movedThisRound = true;
			}
			_unit.ExecuteEffect(effectResult);
		}
		else
		{
			effectResult.EffectSuccessful = false;
		}
    }

    public Unit Unit { get { return _unit; } } //Make sure this contains unit first!
	public bool ContainsUnit {  get { return _containsUnit; } }
	public bool MovedThisRound { get { return _movedThisRound; } set { _movedThisRound = value; } }
	public int PositionNumber {  get { return _positionNumber; } }
	public bool PlayerControlled { get { return _playerControlled; } set { _playerControlled = value; } }
}
