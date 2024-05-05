using Godot;
using System;
using Godot.Collections;
using FormsMoves;

public partial class MoveSelectionLogic : Node
{	
	private MoveInput _moveInput;
	private bool _turnPassed;
	private Team _thisTeam;
	private Team _enemyTeam;

	[Signal]
	public delegate void OfferCompleteMoveInputEventHandler(Wrapper<MoveInput> wrappedMoveInput);

    [Signal]
    public delegate void ShowMoveSelectUIEventHandler();

    [Signal]
    public delegate void ShowTargetSelectUIEventHandler(Wrapper<MoveTargetingStyle> wrappedMoveTargetingStyle,
        int userPosition);

    public void ReturnMoveInputToCombatManager()
	{
		if (_turnPassed)
		{
			return;
		}

		if ((_moveInput.Sender != null) && (_moveInput.Target != null))
		{
			EmitSignal(SignalName.OfferCompleteMoveInput, new Wrapper<MoveInput>(_moveInput));
		}
	}

    [Signal]
    public delegate void OfferPassTurnEventHandler();
    public void PassTurn()
	{
		if (_turnPassed) 
		{ 
			return; 
		}
		_turnPassed = true;
		EmitSignal(SignalName.OfferPassTurn);
	}
	
	public override void _Ready()
	{
		
	}
	
	public virtual void OnTeamJoinCombat()
	{
		
	}
	
	//
	// Selecters
	//
	
	public virtual void SelectMoveInput(Team thisTeam, Team enemyTeam)
	{
		_thisTeam = thisTeam;
		_enemyTeam = enemyTeam;
		_turnPassed = false;
	}
	
	public virtual void SelectMove()
	{
		GD.Print("Bad SelectMove call in MoveSelectionLogic");
	}
	
	public virtual void SelectUser()
	{
		GD.Print("Bad SelectUser call in MoveSelectionLogic");
	}
	
	public virtual void SelectTarget()
	{
		GD.Print("Bad SelectUser call in MoveSelectionLogic");
	}
	
	//
	// Receivers
	//
	
	public virtual void ReceiveMove(Move move)
	{
		SetMove(move);
	}
	
	public virtual void ReceiveUser(Unit user)
	{
		SetUser(user);
	}
	
	public virtual void ReceiveTarget(Unit target)
	{
		SetTarget(target);
	}
	
	//
	// Setters
	//
	
	public void SetMove(Move move)
	{
		_moveInput.Move = move;
	}
	
	public void SetUser(Unit user)
	{
		_moveInput.Sender = user;
	}
	
	public void SetTarget(Unit target)
	{
		_moveInput.Target = target;
	}
	
	//
	// Getters
	//
	
	public Move GetMove()
	{
		return _moveInput.Move;
	}
	
	public Unit GetUser()
	{
		return _moveInput.Sender;
	}
	
	public Unit GetTarget()
	{
		return _moveInput.Target;
	}
	
	/*public Array<TeamPosition> GetPositions()
	{
		Team thisTeam = Team;
		return thisTeam.GetPositions();
	}
	
	public Array<TeamPosition> GetEnemyPositions()
	{
		Team thisTeam = Team;
		CombatManager combatManager = ((CombatManager)thisTeam.GetParent());
		Team enemyTeam = combatManager.GetEnemyTeam(thisTeam);
		Array<TeamPosition> enemyPositions = enemyTeam.GetPositions();
		
		return enemyPositions;
	}*/
	
	// Assumes move and user are already set
	public Array<Unit> GetViableTargets()
	{
		MoveTargetingStyle targetingStyle = GetMove().TargetingStyle;
		
		Array<Unit> viableTargets;
		
		switch (targetingStyle)
		{
			case MoveTargetingStyle.AllyOrSelf:
				viableTargets = Team.GetUnits();
				return viableTargets;
			case MoveTargetingStyle.Ally:
				viableTargets = Team.GetUnits();
				int positionNumber = GetUser().CombatPosition;
				viableTargets.RemoveAt(positionNumber);
				return viableTargets;
			case MoveTargetingStyle.Enemy:
				viableTargets = EnemyTeam.GetUnits();
				return viableTargets;
			default:
				viableTargets = new Array<Unit>();
				return viableTargets;
		}
	}

	//public Team Team { get { return (Team)GetParent(); } }
	public Unit User { get { return _moveInput.Sender; } }
	public Team Team { get { return _thisTeam; } }
	public Team EnemyTeam { get { return _enemyTeam; } }
}
