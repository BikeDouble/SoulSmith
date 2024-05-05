using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;
using FormsMoves;

//This class contains all the moves defined in MoveLibrary classes
public partial class MoveLibrary : Node
{
	public List<Move> Moves = new List<Move>(); //This list contains all moves 
	
	//Dummy function to be overridden, shouldn't be called
	public virtual List<Move> CreateMoves()
	{
		GD.Print("Bad CreateMoves Call");
		return null;
	}
	
	// Called when the node enters the scene tree for the first time.
	public void Initialize()
	{
		Moves = CreateAllMoves();
	}

	public List<Move> CreateAllMoves()
	{
		List<Move> moves = new List<Move>();

		Array<Node> children = GetChildren();
		
		// Collect all moves from all children, and concatenate them into the master array

		foreach (Node node in children)
		{
			moves.AddRange(((MoveLibrary)node).CreateMoves());
		}
		
		return moves;
	}
}
