using Godot;
using System;

public partial class CombatUI : Control
{
	[Export]
	private RichTextLabel _roundCounter;

	public void Update(int roundNumber)
	{
		UpdateRoundCounter(roundNumber);
	}

	private void UpdateRoundCounter(int roundNumber)
	{
		_roundCounter.Text = $"Round {roundNumber}";
	}
}
