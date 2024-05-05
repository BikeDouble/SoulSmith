using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class CampManager : Node3D
{
	[Export]
	private Array<CampFoundation> _foundations;

	private List<CampBuilding> _buildingInventory;
	private List<Unit> _unitInventory;

	public void Initialize()
	{
		InitializeInventories();
	}

	private void InitializeInventories()
	{
		_buildingInventory = new List<CampBuilding>();
		_unitInventory = new List<Unit>();
	}

	public void AddUnitToInventory(Unit unit)
	{
		_unitInventory.Add(unit);
	}

	public void OnRoundEnd()
	{
		foreach (CampFoundation foundation in _foundations)
		{
			foundation.TickBuilding();
		}
	}
}
