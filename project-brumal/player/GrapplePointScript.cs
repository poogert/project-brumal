using Godot;
using System;

public partial class GrapplePointScript : RayCast3D
{
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{	
		if (IsColliding() == true)
		{
			PlayerData.ray = (true, GetCollisionPoint());
			PlayerData.collider = GetCollider() as Node;
		} 
		else
		{
			PlayerData.ray = (false, null);	
		}
	}
	
}
