using Godot;
using System;
using Godot.Collections;
using FormsMoves;
using System.Collections.Generic;

public partial class CombatManager : Node2D
{
	//This team always goes first in the round
	private const int TEAMGOESFIRSTINDEX = 1;

	[Export]
	private Array<Team> _teams;
	[Export]
	private EffectQueue _effectQueue;
	[Export]
	private CombatUI _combatUI;

	private Queue<Unit> _unitDeathQueue;
	private int _activeTeamIndex; //Which team will be acting next
	private int _turnCount = 0;
	private int _consecutivePassedTurns = 0;
	private int _roundCount = 0;
	private bool _awaitingMoveInput;

	public void Initialize()
	{
		InitializeTeams();
		InitializeQueues();
	}

	public override void _Process(double delta)
	{
		if (!_awaitingMoveInput && _effectQueue.IsEmpty())
		{
			ClearDeathQueue();
			BeginTurn();
		}
	}

	private void ClearDeathQueue()
	{
		foreach (Unit unit in _unitDeathQueue) 
		{
			Team team = GetTeamWithUnit(unit);
			if (team != null)
			{
				if (team.PlayerControlled)
				{
					team.RemoveUnitFromCombat(unit);
					InsertUnitToInventory(unit);
				}
				else
				{
					team.DeleteUnit(unit);
				}
			}
		}
	}

	[Signal]
	public delegate void OfferUnitToInventoryEventHandler(Unit unit, CombatManager sender);

	private void InsertUnitToInventory(Unit unit)
	{
		EmitSignal(SignalName.OfferUnitToInventory, unit, this);
	}

	//
	// Getters
	//

	private Team GetEnemyTeam(Team callingTeam)
	{
		foreach (Team team in _teams)
		{
			if (team != callingTeam)
			{
				return team;
			}
		}
		return null;
	}

	private Team GetNextActiveTeam()
	{
		return _teams[GetNextActiveTeamIndex()];
	}

	private int GetNextActiveTeamIndex()
	{
		int nextIndex = _activeTeamIndex + 1;

		if (nextIndex >= _teams.Count)
		{
			nextIndex = 0;
		}

		return nextIndex;
	}

	private Team GetTeamWithUnit(Unit unit)
	{
		foreach (Team team in _teams)
		{
			if (team.ContainsUnit(unit))
			{
				return team;
			}
		}

		return null;
	}

	private Array<Unit> GetAllActiveUnits()
	{
		Array<Unit> units = new Array<Unit>();
		foreach (Team team in _teams)
		{
			units.AddRange(team.GetActiveUnits());
		}
		return units;
	}

	//
	// Combat Processing
	//

	private void InitializeTeams()
	{
		foreach (Team team in _teams)
		{
			team.OfferCompleteMoveInput += OnOfferCompleteMoveInput;
			team.OfferPassTurn += OnOfferPassTurn;
			team.OfferMoveAndUser += OnOfferMoveAndUser;
			team.OfferTarget += OnOfferTarget;
			team.OfferEffectInput += OnOfferEffectInput;
			team.ShowMoveSelectUI += OnShowMoveSelectUI;
			team.ShowTargetSelectUI += OnShowTargetSelectUI;
			team.UnitZeroHP += OnUnitZeroHP;

			String scriptPath;
			if (team.PlayerControlled)
			{
				scriptPath = "res://scripts/moveSelectionLogic/MoveSelectionLogic_PlayerInput.cs";
			}
			else
			{
				scriptPath = "res://scripts/moveSelectionLogic/MoveSelectionLogic_Random.cs";
			}
			team.InitializeMSL(scriptPath);
		}
	}

	private void InitializeQueues()
	{
		_effectQueue.Initialize();
		_effectQueue.OfferEffectResult += OnOfferEffectResult;

		_unitDeathQueue = new Queue<Unit>();
	}
	
	//
	// Round Processing
	//
	
	public void BeginRound() //AKA round end, there's not really a difference
	{
		_effectQueue.OnRoundBegin(GetAllActiveUnits());
		_turnCount = 0;
		_activeTeamIndex = TEAMGOESFIRSTINDEX;
		_roundCount++;
		
		PrepareTeamsForNewRound();
		_combatUI.Update(_roundCount);
	}

	private void PrepareTeamsForNewRound()
	{
		foreach (Team team in _teams)
		{
			team.OnBeginRound();
		}
	}

	//
	// Turn Processing
	//

	public void BeginTurn()
	{
		CycleActiveTeam();
		_turnCount++;
		_awaitingMoveInput = true;
		ActiveTeam.SelectMoveInput(ActiveTeam, GetEnemyTeam(ActiveTeam));
	}
	
	private void CycleActiveTeam()
	{
		_activeTeamIndex = GetNextActiveTeamIndex();
	}

	//Listens to both teams
	public void OnOfferCompleteMoveInput(Wrapper<MoveInput> wrappedMoveInput)
	{
		_awaitingMoveInput = false;
		_effectQueue.EnqueueMove(wrappedMoveInput);
		
		//BeginTurn();
	}

	//Listens to both teams
	public void OnOfferPassTurn()
	{
		_awaitingMoveInput = false;
		_consecutivePassedTurns++;

		if (IsTurnOver())
		{
			_consecutivePassedTurns = 0;
			EndRound();
		}
	}

	[Signal]
	public delegate void OnRoundEndEventHandler(CombatManager sender);

	private void EndRound()
	{
		ClearDeathQueue();

		_effectQueue.OnRoundEnd(GetAllActiveUnits());
		EmitSignal(SignalName.OnRoundEnd, this);
	}

	//Listens to both teams
	private void OnOfferMoveAndUser(Wrapper<MoveInput> wrappedMoveInput)
	{
		foreach (Team team in _teams)
		{
			team.HideMoveSelectUI();
		}
		ActiveTeam.GiveMoveAndUserToMSL(wrappedMoveInput);
	}

	//Listens to both teams
	private void OnOfferTarget(Unit target)
	{
		foreach (Team team in _teams)
		{
			team.HideTargetSelectUI();
		}
		ActiveTeam.GiveTargetToMSL(target);
	}

	//Listens to both teams
	private void OnShowMoveSelectUI()
	{
		ActiveTeam.ShowMoveSelectUIOrder();
	}

	//Listens to both teams
	private void OnUnitZeroHP(Unit unit, Team team)
	{
		_unitDeathQueue.Enqueue(unit);
	}

	private void OnShowTargetSelectUI(Wrapper<MoveTargetingStyle> wrappedMoveTargetingStyle, int userPosition)
	{
		MoveTargetingStyle targetingStyle = wrappedMoveTargetingStyle;
		Array<int> positions;
		Team showingTeam;
		
		switch (targetingStyle)
		{
			case MoveTargetingStyle.AllyOrSelf:
				positions = new Array<int>{0, 1, 2};
				showingTeam = ActiveTeam;
				break;
			case MoveTargetingStyle.Ally:
				positions = new Array<int>{0, 1, 2};
				positions.RemoveAt(userPosition);
				showingTeam = ActiveTeam;
				break;
			case MoveTargetingStyle.Enemy:
				positions = new Array<int>{0, 1, 2};
				showingTeam = GetEnemyTeam(ActiveTeam);
				break;
			default:
				positions = new Array<int>();
				showingTeam = null;
				break;
		}

		showingTeam.ShowTargetSelectUIOrder(positions);
	}

	//Listens to both teams
	private void OnOfferEffectInput(Wrapper<EffectInput> wrappedEffectInput, GodotObject sender)
	{
		EffectInput effectInput = wrappedEffectInput;
		_effectQueue.EnqueueEffect(effectInput);
	}

	//Listens to EffectQueue
	private void OnOfferEffectResult(EffectResult result)
	{
		ExecuteEffect(result);
	}

	private void ExecuteEffect(EffectResult result)
	{
		Unit target = result.Target;
		Team targetTeam = GetTeamWithUnit(target);
		if (targetTeam != null)
		{
			targetTeam.ExecuteEffect(result);
		}
		else
		{
			result.EffectSuccessful = false;
		}
	}

	private bool IsTurnOver()
	{
		return (_consecutivePassedTurns >= _teams.Count);
	}

	public Team ActiveTeam { get { return _teams[_activeTeamIndex]; } }	
}

