using Godot;
using System;
using System.Collections.Generic;

public partial class CampBuilding 
{
    private List<Unit> _units;
    private int _inputSlots;
    private PackedScene _packedMesh;
    private int _remainingDuration = 0;
    private bool _working = false;

    public CampBuilding(int inputSlots, PackedScene packedMesh = null)
    {
        _inputSlots = inputSlots;
        _units = new List<Unit>();
        _packedMesh = packedMesh;
    }

    public virtual void Tick()
    {
        _remainingDuration--;
        if (_remainingDuration <= 0) 
        {
            Complete();
        }
    }

    public virtual void Begin()
    {
        _remainingDuration = CalculateWorkTime();
        _working = true;
    }

    public virtual void Complete()
    {
        _working = false;
    }

    public void Cancel()
    {
        _working = false;
    }

    public virtual int CalculateWorkTime()
    {
        return 1;
    }

    public bool Working { get { return _working; } }
}
