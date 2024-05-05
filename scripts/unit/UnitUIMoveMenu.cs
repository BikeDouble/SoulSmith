using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using FormsMoves;

public partial class UnitUIMoveMenu : Control
{
	[Export]
	private Array<UnitUIMoveButton> _moveButtons;
	
	private int _buttonCount;
	
	public override void _Ready()
	{
		_buttonCount = _moveButtons.Count;
	}
	
	public void InitializeMoveMenu(List<Move> moves, int moveCount)
	{
		
		InitializeMoveButtons(moves, moveCount);
	}
	
	private void InitializeMoveButtons(List<Move> moves, int moveCount)
	{
		for (int i = 0; i < _buttonCount; i++)
		{
			if (i < moveCount)
			{
				_moveButtons[i].InitializeButtonWithMove(moves[i]);
			}
			else
			{
				_moveButtons[i].InitializeButtonAsEmptySlot();
			}
			_moveButtons[i].MoveButtonPressed += OnMoveButtonPressed;
		}
	}

	[Signal]
	public delegate void MoveButtonPressedEventHandler(Wrapper<Move> wrappedMove);

	//Listens to move buttons
	public void OnMoveButtonPressed(Wrapper<Move> wrappedMove)
	{
		EmitSignal(SignalName.MoveButtonPressed, wrappedMove);
	}
}
