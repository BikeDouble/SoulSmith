using Godot;
using System;
using System.Collections.Generic;
using FormsMoves;

public partial class UnitUI : Control
{
	[Export]
	private UnitUIMoveMenu _moveMenu;
	[Export]
	private UnitUIHealthBar _healthBar;
	[Export]
	private TextureButton _targetButton;
	[Export]
	private UnitStats _stats;
	
	public void Initialize(List<Move> moveSet, bool playerControlled = false)
	{
		InitializeMoveMenu(moveSet);
		UpdateHealthBar(_stats);
	}
	
	public void Update(UnitStats stats)
	{
		UpdateHealthBar(stats);
	}

	//
	// Move related functions
	//
	
	private void InitializeMoveMenu(List<Move> moveSet)
	{
		_moveMenu.InitializeMoveMenu(moveSet, moveSet.Count);
		_moveMenu.MoveButtonPressed += OnMoveButtonPressed;
	}
	
	// 
	// Move selection related functions
	//
	
	public void ShowMoveSelect()
	{
		_moveMenu.Show();
	}
	
	public void HideMoveSelect()
	{
		_moveMenu.Hide();
	}
	
	public void ShowTargetSelect()
	{
		_targetButton.Show();
	}
	
	public void HideTargetSelect()
	{
		_targetButton.Hide();
	}

	[Signal]
	public delegate void MoveButtonPressedEventHandler(Wrapper<Move> wrappedMove);

	//Listens to moveMenu
	public void OnMoveButtonPressed(Wrapper<Move> wrappedMove)
	{
		EmitSignal(SignalName.MoveButtonPressed, wrappedMove);
	}

	[Signal]
	public delegate void TargetButtonPressedEventHandler();
	
	// Called by TargetButton.pressed
	private void OnTargetButtonPressed()
	{
		EmitSignal(SignalName.TargetButtonPressed);
	}
	
	//
	// Healthbar related functions
	//
	
	private void UpdateHealthBar(UnitStats stats)
	{
		_healthBar.Update(stats);
	}
	
	//TODO Remove
	public Unit Unit { get { return (Unit)GetParent(); } }
}



