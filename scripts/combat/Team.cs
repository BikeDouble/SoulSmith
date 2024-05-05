using Godot;
using System;
using FormsMoves;
using Godot.Collections;
using System.Collections.Generic;

public partial class Team : Node2D
{
	[Export]
	private MoveSelectionLogic _mSL;
	[Export]
	private bool _playerControlled;
	[Export]
	private Array<TeamPosition> _positions;
	
	public override void _Ready()
	{
		InitializePositions();
	}

	private void InitializePositions()
	{
		foreach (TeamPosition position in _positions)
		{
			position.PlayerControlled = _playerControlled;
			position.OfferMoveAndUser += OnOfferMoveAndUser;
			position.OfferTarget += OnOfferTarget;
			position.OfferEffectInput += OfferEffect;
			position.UnitZeroHP += OnUnitZeroHP;
		}
	}

    [Signal]
    public delegate void OfferEffectInputEventHandler(Wrapper<EffectInput> wrappedEffectInput, GodotObject sender);

    private void OfferEffect(Wrapper<EffectInput> wrappedEffectInput, GodotObject sender)
    {
        EmitSignal(SignalName.OfferEffectInput, wrappedEffectInput, this);
    }

    public void DeleteUnit(Unit unit)
	{
		TeamPosition position = GetPositionWithUnit(unit);

		if (position == null)
		{
			return;
		}

		position.DeleteUnit();
	}

	//
	// Listeners
	//

	[Signal]
	public delegate void UnitZeroHPEventHandler(Unit unit, Team team);

	private void OnUnitZeroHP(Unit unit)
	{
		EmitSignal(SignalName.UnitZeroHP, unit, this);
	}

	public void OnBeginRound()
	{
		foreach (TeamPosition position in _positions)
		{
			position.OnBeginRound();
		}
	}

	[Signal]
	public delegate void OfferCompleteMoveInputEventHandler(Wrapper<MoveInput> wrappedMoveInput);

	//Listens to msl OfferMoveInput
	private void OnOfferCompleteMoveInput(Wrapper<MoveInput> wrappedMoveInput)
	{
		EmitSignal(SignalName.OfferCompleteMoveInput, wrappedMoveInput);
	}

	[Signal]
	public delegate void OfferPassTurnEventHandler();
	
	//Listens to msl OfferPassTurn
	private void OnOfferPassTurn()
	{
		EmitSignal(SignalName.OfferPassTurn);
	}

	[Signal]
	public delegate void ShowMoveSelectUIEventHandler();

	//Listens to msl ShowMoveSelectUI
	private void OnShowMoveSelectUI()
	{
		EmitSignal(SignalName.ShowMoveSelectUI);
	}

	[Signal]
    public delegate void ShowTargetSelectUIEventHandler(Wrapper<MoveTargetingStyle> wrappedMoveTargetingStyle,
														int userPosition);

    //Listens to msl
    private void OnShowTargetSelectUI(Wrapper<MoveTargetingStyle> wrappedMoveTargetingStyle, int userPosition)
	{
		EmitSignal(SignalName.ShowTargetSelectUI, wrappedMoveTargetingStyle, userPosition);
	}

	public void ShowMoveSelectUIOrder()
	{
		foreach (TeamPosition position in _positions)
		{
			position.ShowMoveSelectUI();
		}
	}

	public void ShowTargetSelectUIOrder(Array<int> positionNumbers)
	{
		foreach (int positionNumber in positionNumbers)
		{
			_positions[positionNumber].ShowTargetSelectUI();
		}
	}

	[Signal]
	public delegate void OfferMoveAndUserEventHandler(Wrapper<MoveInput> wrappedMoveInput);

	//Connected to team positions
	private void OnOfferMoveAndUser(Wrapper<MoveInput> wrappedMoveInput)
	{
		EmitSignal(SignalName.OfferMoveAndUser, wrappedMoveInput);
	}

    [Signal]
    public delegate void OfferTargetEventHandler(Unit target);

    //Connected to team positions
    private void OnOfferTarget(Unit target)
    {
        EmitSignal(SignalName.OfferTarget, target);
    }


    public void GiveMoveAndUserToMSL(MoveInput moveInput)
	{
		_mSL.ReceiveUser(moveInput.Sender);
		_mSL.ReceiveMove(moveInput.Move);
	}

	public void GiveTargetToMSL(Unit target)
	{
		_mSL.ReceiveTarget(target);
	}

	public void HideMoveSelectUI()
	{
		foreach (TeamPosition position in _positions)
		{
			position.HideMoveSelectUI();
		}
	}

	public void HideTargetSelectUI()
	{
		foreach (TeamPosition position in _positions)
		{
			position.HideTargetSelectUI();
		}
	}

	//
	// Getters
	//
	
	// Returns true if this team has at least one active unit
	public bool HasActiveUnit()
	{
		foreach (TeamPosition position in _positions)
		{
			if (!position.MovedThisRound && position.ContainsUnit)
			{
				return true;
			}
		}	
		
		return false;
	}

	public bool ContainsUnit(Unit unit)
	{
		foreach (TeamPosition position in _positions)
		{
			if ((position.ContainsUnit) && (position.Unit == unit))
			{
				return true;
			}
		}

		return false;
	}

    public TeamPosition GetPositionWithUnit(Unit unit)
    {
        foreach (TeamPosition position in _positions)
        {
            if ((position.ContainsUnit) && (position.Unit == unit))
            {
                return position;
            }
        }

        return null;
    }

    public Array<TeamPosition> GetPositions()
	{
		return _positions.Duplicate();
	}

	public Unit GetUnit(int positionNumber)
	{
		TeamPosition position = _positions[positionNumber];
		if (position.ContainsUnit)
		{
            return position.Unit;
        }

		return null;
	}
	
	//Returns all units that can still move this turn
	public Array<Unit> GetActiveUnits()
	{
		Array<Unit> activeUnits = new Array<Unit>();
		foreach (TeamPosition position in _positions)
		{
			if (!position.MovedThisRound && position.ContainsUnit)
			{
				activeUnits.Add(position.Unit);
			}
		}	
		return activeUnits;
	}

    //Returns all units that can still move this turn as UnitStats
    public Array<UnitStats> GetActiveUnitStats()
    {
        Array<UnitStats> activeUnits = new Array<UnitStats>();
        foreach (TeamPosition position in _positions)
        {
            if (!position.MovedThisRound && position.ContainsUnit)
            {
                activeUnits.Add(position.Unit.Stats);
            }
        }

        return activeUnits;
    }

    //Returns all units currently in this team
    public Array<Unit> GetUnits()
	{
		Array<Unit> activeUnits = new Array<Unit>();
		foreach (TeamPosition position in _positions)
		{
			if (position.ContainsUnit)
			{
				activeUnits.Add(position.Unit);
			}
		}
		return activeUnits;
	}

    public Array<UnitStats> GetUnitStats()
    {
        Array<UnitStats> activeUnits = new Array<UnitStats>();
        foreach (TeamPosition position in _positions)
        {
            if (position.ContainsUnit)
            {
                activeUnits.Add(position.Unit.Stats);
            }
        }
        return activeUnits;
    }

	public void RemoveUnitFromCombat(Unit unit)
	{
		TeamPosition position = GetPositionWithUnit(unit);

		if (position != null)
		{
            position.RemoveUnitFromCombat();
        }
	}
	
	//
	// Move Selection
	//
	
	public void InitializeMSL(String scriptPath)
	{
		Script script;
		Node mSLNode = (Node)_mSL;
		ulong objId = mSLNode.GetInstanceId();
		script = (Script)GD.Load(scriptPath);
		mSLNode.SetScript(script);
		_mSL = (MoveSelectionLogic)InstanceFromId(objId);
		_mSL.OfferCompleteMoveInput += OnOfferCompleteMoveInput;
		_mSL.OfferPassTurn += OnOfferPassTurn;
		_mSL.ShowMoveSelectUI += OnShowMoveSelectUI;
		_mSL.ShowTargetSelectUI += OnShowTargetSelectUI;

		_mSL.OnTeamJoinCombat();
	}
	
	public void SelectMoveInput(Team thisTeam, Team enemyTeam) 
	{
		_mSL.SelectMoveInput(thisTeam, enemyTeam);
	}

    public void ExecuteEffect(EffectResult effectResult)
    {
		TeamPosition targetPosition = GetPositionWithUnit(effectResult.Target);
		if (targetPosition != null)
		{
			targetPosition.ExecuteEffect(effectResult);
		}
		else
		{
			effectResult.EffectSuccessful = false;
		}
    }

    public bool PlayerControlled { get { return _playerControlled; } }
}
