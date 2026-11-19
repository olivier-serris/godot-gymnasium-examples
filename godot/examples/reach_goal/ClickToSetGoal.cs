#nullable enable
using Godot;

namespace ReachGoal;

/// <summary>
/// Left click sets the dot's goal; draws it until reached.
/// </summary>
public partial class ClickToSetGoal : Node2D
{
    [Export] public DotAgent? Agent { get; set; }

    public override void _UnhandledInput(InputEvent e)
    {
        if (Agent != null && e is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true })
            Agent.Goal = GetGlobalMousePosition();
    }

    public override void _Process(double delta) => QueueRedraw();

    public override void _Draw()
    {
        if (Agent == null || Agent.IsGoalReached) return;
        DrawCircle(ToLocal(Agent.Goal), Agent.GoalRadius, new Color(0.2f, 0.8f, 0.3f), false);
    }
}
