using FormsTypes;
using Godot;
using System;
using System.Collections.Generic;
using FormsMoves;

public class UnitTemplate 
{
    private List<Stat> _statsList;
    private List<Move> _moveSet;
    private Emotion _emotion;
    private PackedScene _sprite;

    public List<Stat> StatsList { get { return _statsList; } }
    public List<Move> MoveSet { get {  return _moveSet; } }
    public Emotion Emotion { get { return _emotion; } }
    public PackedScene Sprite { get { return _sprite; } }
}
