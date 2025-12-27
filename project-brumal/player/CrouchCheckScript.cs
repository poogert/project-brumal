using Godot;
using System;

public partial class CrouchCheckScript : RayCast3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		ForceRaycastUpdate();
		PlayerData.IsBlocking = IsColliding();
	}
}
