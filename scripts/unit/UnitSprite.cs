using Godot;
using System;

public partial class UnitSprite : AnimatedSprite2D
{
    [Export]
    private UnitVisualizationZone _visualizationZone;

    public UnitVisualizationZone VisualizationZone { get { return _visualizationZone; } }

    public Vector2 GetRandomVisualizationPointGlobal()
    {
        return _visualizationZone.GetRandomPointGlobal(GlobalScale);
    }

    public Vector2 GetVisualizationSizeGlobal()
    {
        return _visualizationZone.Size * GlobalScale;
    }
}
