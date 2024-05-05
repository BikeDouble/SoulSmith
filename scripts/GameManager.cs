using Godot;
using System;

public partial class GameManager : Node2D
{
	[Export]
	private CombatManager _combatManager;
	[Export]
	private CampManager _campManager;
	[Export]
	private MoveLibrary _moveLibrary;
	
	
	private int _roundCount = 1;

	public override void _Ready()
	{
		Initialize();
	}

	private void Initialize()
	{
		InitializeLibraries();
		InitializeCombat();
		InitializeCamp();
	}

	private void InitializeLibraries()
	{
		_moveLibrary.Initialize();
	}

	private void InitializeCamp()
	{
		_campManager.Initialize();
	}

	private void InitializeCombat()
	{
		_combatManager.Initialize();
		_combatManager.OfferUnitToInventory += OnOfferUnitToInventory;
		_combatManager.OnRoundEnd += OnRoundEnd;
		_combatManager.BeginRound();
	}

	//Listens to combat manager
	private void OnOfferUnitToInventory(Unit unit, CombatManager sender)
	{
		_campManager.AddUnitToInventory(unit);
	}

	//Listens to combat manager
	private void OnRoundEnd(CombatManager sender)
	{
		_combatManager.BeginRound();
		_campManager.OnRoundEnd();
		_roundCount++;
		//_combatManager.UpdateRoundCount(_roundCount);
	}
}
