# godot-gymnasium-examples

Examples for [godot-gymnasium](https://github.com/olivier-serris/godot-gymnasium), which is a git submodule here, in
`godot/addons/godot_gymnasium/`.

```bash
git clone --recursive https://github.com/olivier-serris/godot-gymnasium-examples
cd godot-gymnasium-examples
export GODOT4=/path/to/Godot_v4.x-stable_mono_linux.x86_64
```

## Reach a goal

A dot must move to a goal.

- [DotAgent](godot/examples/reach_goal/DotAgent.cs) (`ControllableAgent`): observes the goal, applies the actions.
- [ReachGoalEnv](godot/examples/reach_goal/ReachGoalEnv.cs) (`GymEnv`): reset, reward, end of episode.
  Open `reach_goal.tscn` and press **F6** to move the dot with the arrow keys.
- `reach_goal_play.tscn`: the game, the dot driven by an `NNController` running the trained `.onnx`.

1. Build the C# project (once, and after every C# change):

   ```bash
   $GODOT4 --headless --path godot --build-solutions --quit
   ```

2. Train:

   ```bash
   uv run godot/addons/godot_gymnasium/python/train.py --project godot --env-scene res://examples/reach_goal/reach_goal.tscn
   ```

   When training ends, the run folder `runs/reach_goal_<date>_<time>/` holds the trained policy `policy.onnx`.

3. Watch the training (optional, in another terminal): `uv run tensorboard --logdir runs`

4. Play: open `reach_goal_play.tscn` in the editor, set the `ModelPath` of its `NNController` to the full path of
   that `policy.onnx`, and press **F6**. Left click sets the dot's goal.
   To ship the game: copy the `.onnx` into the Godot project, set `ModelPath` to its `res://` path, and in
   **Project → Export…**, tab **Resources**, type `*.onnx` in "Filters to export non-resource files/folders"
   (otherwise Godot leaves the `.onnx` out of the exported game).
