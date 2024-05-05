using System.Collections.Generic;
using System;
using Godot;
using FormsMoves;
using Godot.Collections;

public partial class EffectQueue : Node
{
    [Export]
    private CombatManager _combatManager;

    private Queue<QueuedEffect> _queue;
    private Queue<QueuedEffect> _priorityQueue;
    private DropOutStack<EffectResult> _effectHistory; //Effect history is pushed after effect is processed
    private DropOutStack<MoveInput> _moveHistory; //Move history is pushed after move is queued
    private bool _processingEnabled = true;
    private Unit _lastMoveTarget;
    private Effect _moveBeginEffect = new Effect_Trigger(EffectTrigger.OnMoveBegin, EffectTargetingStyle.Self);
    private Effect _moveEndEffect = new Effect_Trigger(EffectTrigger.OnMoveEnd, EffectTargetingStyle.Self);
    private Effect _roundBeginEffect = new Effect_Trigger(EffectTrigger.OnRoundBegin, EffectTargetingStyle.Self);
    private Effect _roundEndEffect = new Effect_Trigger(EffectTrigger.OnRoundEnd, EffectTargetingStyle.Self);

    public readonly struct QueuedEffect
    {
        public QueuedEffect(EffectInput input)
        {
            EffectInput = input;
            VisualizationListener = new EffectVisualizationListener(input);
            ParentEffectResult = null;
        }

        public QueuedEffect(EffectInput input, EffectResult parentEffectResult)
        {
            EffectInput = input;
            VisualizationListener = new EffectVisualizationListener(input);
            ParentEffectResult = parentEffectResult;
        }

        public EffectInput EffectInput { get; }
        public EffectVisualizationListener VisualizationListener { get; }
        public EffectResult ParentEffectResult { get; }
    }

    public void Initialize()
    {
        InitializeQueue();
        InitializeHistory();
    }

    public override void _Process(double delta)
    {
        CheckAndProcess();
    }

    public void OnRoundBegin(Array<Unit> activeUnits)
    {
        EnqueueEffectForMultipleUnits(_roundBeginEffect, activeUnits);
    }

    public void OnRoundEnd(Array<Unit> activeUnits)
    {
        EnqueueEffectForMultipleUnits(_roundEndEffect, activeUnits);
    }

    private void InitializeQueue() 
    {
        _queue = new Queue<QueuedEffect>();
        _priorityQueue = new Queue<QueuedEffect>();
    }

    private void InitializeHistory()
    {
        _effectHistory = new DropOutStack<EffectResult>(50);
        _moveHistory = new DropOutStack<MoveInput>(24);
    }

    private void CheckAndProcess()
    {
        if (_processingEnabled)
        {
            if (NextEffectReady(_priorityQueue))
            {
                DequeueAndProcess(_priorityQueue);
            }
            else if (NextEffectReady(_queue))
            {
                DequeueAndProcess(_queue);
            }
        }
    }

    private void DequeueAndProcess(Queue<QueuedEffect> queue)
    {
        EffectInput effectInput = queue.Dequeue().EffectInput;

        if ((effectInput.Effect
             == null) || (effectInput.Sender == null) || (effectInput.Target == null))
        {
            GD.Print("Effect input missing one or more parameters");
            return;
        }

        if (!effectInput.Sender.InCombat || !effectInput.Target.InCombat)
        {
            GD.Print("Effect user or target no longer in combat");
            return;
        }

        ProcessEffect(effectInput);
    }

    public void EnqueueMove(MoveInput moveInput)
    {
        List<Effect> effects = moveInput.Move.Effects;
        Unit user = moveInput.Sender;
        _lastMoveTarget = moveInput.Target;

        EnqueueEffect(new EffectInput(_moveBeginEffect, user, _lastMoveTarget));
        foreach (Effect effect in effects)
        {
            EnqueueEffect(new EffectInput(effect, user, _lastMoveTarget));
        }
        EnqueueEffect(new EffectInput(_moveEndEffect, user, _lastMoveTarget));

        _moveHistory.Push(moveInput);
    }

    public void EnqueueEffectForMultipleUnits(Effect effect, Array<Unit> units)
    {
        foreach (Unit unit in units)
        {
            EnqueueEffect(new EffectInput(effect, unit));
        }
    }

    public void EnqueueEffect(EffectInput effectInput, EffectResult parentEffectResult = null)
    {
        if ((effectInput.Effect
             == null) || (effectInput.Sender == null))
        {
            GD.Print("Effect input missing effect or user");
            return;
        }

        effectInput.Target = DetermineTarget(effectInput, parentEffectResult);

        if (effectInput.Target == null)
        {
            GD.Print("Could not determine effect target");
            return;
        }

        if (effectInput.Effect.SwapSenderAndTarget)
        {
            effectInput.SwapSenderAndTarget();
        }

        QueuedEffect queuedEffect = new QueuedEffect(effectInput, parentEffectResult);
        EffectVisualization visualization = queuedEffect.VisualizationListener.Visualization;
        
        if (visualization != null)
        {
            AddChild(visualization);
        }
        
        if (effectInput.Effect.RequiresPriority)
        {
            _priorityQueue.Enqueue(queuedEffect);
        }
        else
        {
            _queue.Enqueue(queuedEffect);
        }
    }

    private bool NextEffectReady(Queue<QueuedEffect> queue)
    {
        if (queue.Count == 0)
        {
            return false;
        }

        QueuedEffect nextQueuedEffect = queue.Peek();
        return nextQueuedEffect.VisualizationListener.ReadyForExecute;
    }

    private Unit DetermineTarget(EffectInput effectInput, EffectResult parentEffectResult)
    {
        switch (effectInput.Effect.TargetingStyle) 
        {
            case EffectTargetingStyle.MoveTarget:
                return _lastMoveTarget;
            case EffectTargetingStyle.Self:
                return effectInput.Sender;
            case EffectTargetingStyle.ParentTarget:
                return parentEffectResult.Target;
            case EffectTargetingStyle.ParentSender:
                return parentEffectResult.Sender;
            case EffectTargetingStyle.DeterminedPreOffer:
                return effectInput.Target;
            default:
                return null;
        }
    }

    public bool IsEmpty()
    {
        int totalCount = _queue.Count + _priorityQueue.Count;
        return (totalCount == 0);
    }

    //
    // Effect processing
    //

    [Signal]
    public delegate void OfferEffectResultEventHandler(EffectResult result);

    public void ProcessEffect(EffectInput effectInput)
    {
        Effect effect = effectInput.Effect;
        Unit user = effectInput.Sender;
        Unit target = effectInput.Target;

        EffectResult result = effect.GetEffectResult(user, target);

        if (result == null)
        {
            return;
        }

        if (result.ChildEffects != null)
        {
            foreach (Effect childEffect in result.ChildEffects)
            {

            }
        }

        EmitSignal(SignalName.OfferEffectResult, result);
        _effectHistory.Push(result);
    }
}
