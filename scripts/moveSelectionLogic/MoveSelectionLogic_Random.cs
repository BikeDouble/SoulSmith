using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;
using FormsMoves;

public partial class MoveSelectionLogic_Random : MoveSelectionLogic
{
	public override void SelectMoveInput(Team thisTeam, Team enemyTeam)
	{
		base.SelectMoveInput(thisTeam, enemyTeam);
		SelectUser();
		SelectMove();
		SelectTarget();
		ReturnMoveInputToCombatManager();
	}
	
	public override void SelectUser()
	{
		Array<Unit> activeUnits = Team.GetActiveUnits();
		if (activeUnits.Count == 0)
		{
			PassTurn();
			return;
		}
		int selectedIndex = GD.RandRange(0, activeUnits.Count - 1);
		ReceiveUser(activeUnits[selectedIndex]);
	}
	
	public override void SelectMove()
	{
		int userPosition = GetUser().CombatPosition;
		Unit userUnit = Team.GetUnit(userPosition);
        List<Move> moveSet = userUnit.MoveSet;
        if (moveSet.Count == 0)
        {
            PassTurn();
			return;
        }
        int selectedIndex = GD.RandRange(0, moveSet.Count - 1);
		ReceiveMove(moveSet[selectedIndex]);
	}
	
	public override void SelectTarget()
	{
		Array<Unit> viableTargets = GetViableTargets();
        if (viableTargets.Count == 0)
        {
            PassTurn();
			return;
        }
        int selectedIndex = GD.RandRange(0, viableTargets.Count - 1);
		ReceiveTarget(viableTargets[selectedIndex]);
	}
}
