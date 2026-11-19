#nullable enable
using System;
using Godot;

using GodotGymnasium;

namespace ReachGoal;

/// <summary>
/// Controls an agent with the arrow keys (for agents whose action is a 2D direction).
/// Disabled during training.
/// </summary>
public partial class KeyboardController : Node
{
    [Export] public ControllableAgent Agent { get; set; } = null!;
    [Export] public StringName Left { get; set; } = "ui_left";
    [Export] public StringName Right { get; set; } = "ui_right";
    [Export] public StringName Up { get; set; } = "ui_up";
    [Export] public StringName Down { get; set; } = "ui_down";

    public override void _Ready()
    {
        // Run before the agent's own _PhysicsProcess so the action is used in the same frame.
        ProcessPhysicsPriority = int.MinValue;
        if (GetTree().CurrentScene is GymBridge)
        {
            SetPhysicsProcess(false); // training: actions come from Python
            return;
        }
        if (Agent is not { ActionType: ActionType.Box, ActionSize: 2 })
        {
            GD.PushError($"[KeyboardController] {Name}: Agent must be set and have a Box action of size 2");
            SetPhysicsProcess(false);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        var input = Input.GetVector(Left, Right, Up, Down);
        Agent.ApplyAction([input.X, input.Y]);
    }
}
