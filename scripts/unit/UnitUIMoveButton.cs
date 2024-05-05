using Godot;
using System;
using FormsMoves;

public partial class UnitUIMoveButton : TextureButton
{
	[Export]
	private UnitUIMoveButtonLabel _label;
	[Export]
	private int _position;

	private bool _containsMove = false;
	private Move _move;

	public void InitializeButtonWithMove(Move move)
	{
		if (move.Name == "NullMove")
		{
			InitializeButtonAsEmptySlot();
			return;
		}
		
		_move = move;
		_containsMove = true;
		SetLabelText(move.Name);
		
	}

	public void InitializeButtonAsEmptySlot()
	{
		SetLabelText("Empty");
	}

	private void SetLabelText(string text)
	{
		_label.SetText(text);
	}

	[Signal]
	public delegate void MoveButtonPressedEventHandler(Wrapper<Move> wrappedMove);
	
	//Connected to pressed signal
	private void OnPressed()
	{
		if (_containsMove)
		{
			EmitSignal(SignalName.MoveButtonPressed, new Wrapper<Move>(_move));
		}
	}
}

