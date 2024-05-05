using Godot;
using System;
using FormsMoves;

public partial class EffectVisualizationListener
{
    private EffectVisualization _visualization;
    private bool _readyForExecute = true;

    public EffectVisualizationListener(EffectInput input)
    {
        PackedScene packedVisualization = input.Effect.PackedVisualization;
        if (packedVisualization == null)
        {
            return;
        }
        BeginVisualization(input.Sender,
                           input.Target,
                           packedVisualization,
                           input.Effect.VisualizationDelay);
    }

    public void BeginVisualization(Unit user,
                                   Unit target,
                                   PackedScene packedVisualization,
                                   double delay = 0f)
    {
        _visualization = (EffectVisualization)packedVisualization.Instantiate();
        if (_visualization == null)
        {
            return;
        }
        _readyForExecute = false;
        _visualization.ReadyEffect += OnVisualizationExecuteEffect;
        _visualization.BeginVisualization(user, target, delay);
    }

    private void OnVisualizationExecuteEffect()
    {
        _readyForExecute = true;
    }

    public EffectVisualization Visualization { get { return _visualization; } }
    public bool ReadyForExecute {  get { return _readyForExecute; } }
}
