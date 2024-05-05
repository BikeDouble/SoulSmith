using Godot;
using System;
using Godot.Collections;
using FormsMoves;

public partial class MoveSelectionLogic_PlayerInput : MoveSelectionLogic
{
	
	//
	// Selecters
	//
	
	public override void SelectMoveInput(Team thisTeam, Team enemyTeam)
	{
		base.SelectMoveInput(thisTeam, enemyTeam);
		if (thisTeam.HasActiveUnit())
		{
			SelectMove();
		}
		else
		{
			PassTurn();
		}
	}
	
	public override void SelectMove()
	{
		EmitSignal(SignalName.ShowMoveSelectUI);
	}
	
	// Assumes move and user are selected already
	public override void SelectTarget()
	{
		MoveTargetingStyle targetingStyle = GetMove().TargetingStyle;
		int userPosition = User.CombatPosition;
		EmitSignal(SignalName.ShowTargetSelectUI, 
				   new Wrapper<MoveTargetingStyle>(targetingStyle), 
				   userPosition);
	}
	
	//
	// Receivers
	//
	
	public override void ReceiveMove(Move move)
	{
		SetMove(move);
		SelectTarget();
	}
	
	public override void ReceiveTarget(Unit target)
	{
		SetTarget(target);
		ReturnMoveInputToCombatManager();
	}
}
