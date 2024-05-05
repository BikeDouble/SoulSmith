using Godot;

public partial class EffectVisualization_Missile : EffectVisualization
{
    private double _totalFlightDuration = 0.5f;
    private double _remainingFlightDuration; //How long should this missile take to get from user to target?
    private Vector2 _target;
    private Vector2 _start;
    public override void OnProcess(double delta)
    {
        base.OnProcess(delta);
        _remainingFlightDuration -= delta;
        if (_remainingFlightDuration <= 0) 
        {
            EmitReadyEffect();
            EndVisualization();
        }
        MoveMissile(delta);
    }

    public override void BeginVisualization(Unit user, Unit target, double delay)
    {
        base.BeginVisualization(user, target, delay);
        _remainingFlightDuration = _totalFlightDuration;
        _target = target.Sprite.GetRandomVisualizationPointGlobal();
        _start = user.Sprite.GetRandomVisualizationPointGlobal();
        Position = _start;
    }

    public Vector2 CalculateOffsetToTarget()
    {
        Vector2 offset = _target - GlobalPosition; 

        return offset; 
    }

    public virtual void MoveMissile(double delta)
    {
        double interpolant = Mathf.Clamp(_remainingFlightDuration / _totalFlightDuration, 0, 1);
        GlobalPosition = (float)interpolant * _start + (float)(1 - interpolant) * _target;
        //MoveMissileTowardsTarget(delta);
    }

    private void MoveMissileTowardsTarget(double delta)
    {
        GlobalTranslate(CalculateOffsetToTarget() * (float)(delta / _remainingFlightDuration));
    }
}
