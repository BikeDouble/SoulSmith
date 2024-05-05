using Godot;

public partial class EffectVisualization : AnimatedSprite2D
{
    private double _lifespan = 3f; //Time in seconds before visualization automatically completes
    private bool _enabled = false;
    private double _delay = 0f;

    [Signal]
    public delegate void ReadyEffectEventHandler();

    public EffectVisualization()
    {
        Pause();
    }

    public override void _Process(double delta)
    {
        if (_enabled)
        {
            OnProcess(delta);
        }
        else
        {
            _delay -= delta;
            if (_delay <= 0)
            {
                EnableVisualization();
            }
        }
    }

    public override void _Ready()
    {

    }

    //Tells the combat manager to apply the effect now, so that it is synced with the visualization
    public virtual void EmitReadyEffect()
    {
        EmitSignal(SignalName.ReadyEffect);
    }

    public virtual void BeginVisualization(Unit user, Unit target, double delay = 0f)
    {
        _delay = delay;
        Hide();
        Pause();
    }

    public virtual void EnableVisualization()
    {
        _enabled = true;
        Show();
        Play();
    }

    public virtual void EndVisualization()
    {
        Hide();
        QueueFree();
    }

    public virtual void OnProcess(double delta)
    {
        _lifespan -= delta;
        if (_lifespan <= 0)
        {
            EmitReadyEffect();
            EndVisualization();
        }
    }

    public void SetLifespan(double lifespan) 
    {
        _lifespan = lifespan;
    }
}
