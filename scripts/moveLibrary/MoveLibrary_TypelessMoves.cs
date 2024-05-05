using Godot;
using System;
using System.Collections.Generic;
using FormsMoves;

public partial class MoveLibrary_TypelessMoves : MoveLibrary
{

    //TODO Move this to a proper resource library
    private PackedScene _eV = GD.Load<PackedScene>("res://scenes/effectVisualizations/effect_visualization_missile_typeless_pellet.tscn");

    public override List<Move> CreateMoves()
	{
		List<Move> moves = new List<Move>();
		moves.AddRange(BasicMoves());
		
		return moves;
	}
	
	private List<Move> BasicMoves()
	{
		List<Move> moves = new List<Move>();
		moves.Add(Hit());
		moves.Add(DoubleHit());
		
		return moves;
	}

    private Move Hit()
    {
        List<Effect> effects = new List<Effect>();
        Effect effect = MoveHelpers.AttackHit(1f);
        effect.PackedVisualization = _eV;
        effects.Add(effect);

        Move move = new Move("Hit", effects);

        return move;
    }

    private Move DoubleHit()
    {

        List<Effect> effects = new List<Effect>();
        Effect effect = MoveHelpers.AttackHit(0.4f);
        effect.PackedVisualization = _eV;
        effect.TargetingStyle = EffectTargetingStyle.MoveTarget;
        effects.Add(effect);

        effect = MoveHelpers.AttackHit(0.4f);
        effect.PackedVisualization = _eV;
        effect.TargetingStyle = EffectTargetingStyle.MoveTarget;
        effect.VisualizationDelay = 0.3f;
        effects.Add(effect);

        Move move = new Move("Double Hit", effects);
        return move;
    }
}
