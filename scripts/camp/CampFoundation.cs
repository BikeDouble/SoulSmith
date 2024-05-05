using Godot;
using System;

public partial class CampFoundation : Node3D
{
	private bool _containsBuilding = false;
	private CampBuilding _building = null;

	public void AddBuilding(CampBuilding building)
	{
		_building = building;
		_containsBuilding = true;
	}

	public void TickBuilding()
	{
		if (_containsBuilding && _building.Working)
		{
			_building.Tick();
		}
	}

}
