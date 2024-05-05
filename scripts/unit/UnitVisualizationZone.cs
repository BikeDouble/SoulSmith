using Godot;
using System;

public partial class UnitVisualizationZone : ReferenceRect
{
    public Vector2 GetRandomPointGlobal(Vector2 parentScale)
    {
        Vector2 point = new Vector2(GD.Randf(), GD.Randf()) * Size * Scale * parentScale;
        point = GlobalPosition + point;

        return point;
    }
}
