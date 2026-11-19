#nullable enable
using System;
using Godot;

using GodotGymnasium;

namespace ReachGoal;

/// <summary>
/// Lets a neural network move its parent Node2D (the dot) toward Goal.
/// Observation: vector from the dot to the goal. Action: direction to move in.
/// </summary>
public partial class DotAgent : ControllableAgent
{
    [Export, AgentParameter] public float Speed { get; set; } = 200f;      // px per second
    [Export, AgentParameter] public float GoalRadius { get; set; } = 20f;  // px
    /// px: the goal is seen up to this distance (observed as 1), farther goals are clipped.
    /// Training: also the arena's width.
    [Export, AgentParameter] public float ViewRange { get; set; } = 400f;
    const float DrawRadius = 8f;                           // px, drawing only

    public override ActionType ActionType => ActionType.Box;
    public override int ActionSize => 2;

    /// The controlled dot.
    public Node2D Body => _body ?? throw new InvalidOperationException("DotAgent is not ready");

    /// Global position (px).
    public Vector2 Goal { get; set; }

    /// Goal - dot position (px).
    public Vector2 RelativeGoal
    {
        get => Goal - Body.GlobalPosition;
        set => Goal = Body.GlobalPosition + value;
    }

    public bool IsGoalReached => RelativeGoal.Length() < GoalRadius;

    Node2D? _body;
    Vector2 _velocity;

    public override void _Ready()
    {
        _body = GetParent<Node2D>();
        _body.Draw += OnDraw;
        Goal = _body.GlobalPosition;
    }

    public override void ApplyAction(ReadOnlySpan<float> action)
    {
        _velocity = new Vector2(action[0], action[1]) * Speed;
    }

    public override void _PhysicsProcess(double delta)
    {
        Body.GlobalPosition += _velocity * (float)delta;
        Body.QueueRedraw();
    }

    public override float[] GetObservation()
    {
        var rel = RelativeGoal.LimitLength(ViewRange) / ViewRange;
        return new[] { rel.X, rel.Y };
    }

    void OnDraw() => Body.DrawCircle(Vector2.Zero, DrawRadius, new Color(0.3f, 0.5f, 1f));
}
