using Godot;
using System;

public partial class UnitUIMoveButtonLabel : RichTextLabel
{
	public void SetText(string text)
	{
		Text = $"[center]{text}[/center]";
	}
}
