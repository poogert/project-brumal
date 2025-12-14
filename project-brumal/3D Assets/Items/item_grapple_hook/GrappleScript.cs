using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

public partial class GrappleScript : Node
{
	[Export] PackedScene placed_hook;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if ( Input.IsActionJustPressed("leftclick") ) {
			
			if (PlayerData.ray.colliding) 
			{
				
				Vector3 position = PlayerData.ray.point ?? Vector3.Zero;
				if (position == Vector3.Zero) return;

				GD.Print(position);
				
				Node hook = placed_hook.Instantiate();
				Node3D hook3D = hook as Node3D;

				GetTree().Root.AddChild(hook3D);

				Vector3 direction = PlayerData.look_direction.Normalized();

				hook3D.GlobalPosition = position;
				hook3D.LookAt(position + direction, Vector3.Up);
			}
			
		} 
	}
}
