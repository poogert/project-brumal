using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

public partial class GrappleScript : Node
{
	[Export] PackedScene placed_hook;
	[Export] Node3D hook_holding;
	private Node3D currentHook;


	public override void _Ready()
	{
	}
	public override void _Process(double delta)
	{
		if (Input.IsActionJustReleased("leftclick"))
		{
			if (!PlayerData.ray.colliding) return;

			Vector3 position = PlayerData.ray.point ?? Vector3.Zero;
			if (position == Vector3.Zero) return;

			// Remove previous hook if it exists
			if (currentHook != null && IsInstanceValid(currentHook))
			{
				currentHook.QueueFree();
				currentHook = null;
			}

			Node hook = placed_hook.Instantiate();
			currentHook = hook as Node3D;

			GetTree().Root.AddChild(currentHook);

			Vector3 playerPos = PlayerData.player_position;
			Vector3 direction = PlayerData.look_direction.Normalized();
			currentHook.LookAt(direction, Vector3.Up);

			TweenHookToTarget(currentHook, playerPos, position);
		}

	}

	private void TweenHookToTarget(Node3D hook, Vector3 startPos, Vector3 targetPos)
	{
		// Safety
		if (hook == null)
			return;

		// Ensure hook starts at player
		hook.GlobalPosition = startPos;

		float distance = getGrappleDistance(startPos, targetPos);

		float time = NormalizeDistance(distance) / 2;

		Tween tween = hook.CreateTween();

		tween.TweenProperty(
			hook,
			"global_position",
			targetPos,
			time
		).SetTrans(Tween.TransitionType.Quad)
		.SetEase(Tween.EaseType.Out);
	}

	private float getGrappleDistance(Vector3 playerPos, Vector3 hookPos)
	{
		return playerPos.DistanceTo(hookPos);
	}

	private float NormalizeDistance(float distance)
	{
		return Mathf.Clamp(distance / 10, 0f, 1f);
	}
}
