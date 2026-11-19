#nullable enable
using Godot;

using GodotGymnasium;

namespace ReachGoal;

/// <summary>
/// Training scene for DotAgent: random start and goal in a square arena, reward 1 when the
/// dot reaches the goal.
/// </summary>
public partial class ReachGoalEnv : GymEnv
{
    float ArenaHalfSize => Dot.ViewRange / 2; // px: the arena is what the dot sees

    DotAgent Dot => (DotAgent)Agent;
    Node2D _canvas = null!;

    public override void _Ready()
    {
        _canvas = GetNode<Node2D>("Canvas");
        _canvas.Draw += OnDraw;
        ProcessPhysicsPriority = 1; // after the dot moved: clamp it to the arena
        Reset();
    }

    public override void Reset()
    {
        var start = RandomPoint();
        Vector2 goal;
        do goal = RandomPoint();
        while (start.DistanceTo(goal) < 2 * Dot.GoalRadius); // never start on the goal
        Dot.Body.GlobalPosition = _canvas.ToGlobal(start);
        Dot.Goal = _canvas.ToGlobal(goal);
        _canvas.QueueRedraw();
    }

    Vector2 RandomPoint() =>
        new(Rng.RandfRange(-ArenaHalfSize, ArenaHalfSize), Rng.RandfRange(-ArenaHalfSize, ArenaHalfSize));

    public override void _PhysicsProcess(double delta)
    {
        var limit = new Vector2(ArenaHalfSize, ArenaHalfSize);
        var local = _canvas.ToLocal(Dot.Body.GlobalPosition).Clamp(-limit, limit);
        Dot.Body.GlobalPosition = _canvas.ToGlobal(local);

        // Run alone (F6): new goal once reached.
        if (GetTree().CurrentScene == this && IsTerminated()) Reset();
    }

    public override float GetReward() => Dot.IsGoalReached ? 1f : 0f;

    public override bool IsTerminated() => Dot.IsGoalReached;

    void OnDraw()
    {
        var arena = new Rect2(-ArenaHalfSize, -ArenaHalfSize, 2 * ArenaHalfSize, 2 * ArenaHalfSize);
        _canvas.DrawRect(arena, Colors.White, filled: false, width: 2f);
        _canvas.DrawCircle(_canvas.ToLocal(Dot.Goal), Dot.GoalRadius, new Color(0.2f, 0.8f, 0.3f), false);
    }
}
