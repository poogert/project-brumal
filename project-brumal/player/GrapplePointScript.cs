using Godot;
using System;

public partial class GrapplePointScript : RayCast3D
{
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (IsColliding() == true)
		{
			PlayerData.ray = (true, GetCollisionPoint());
		} 
		else
		{
			PlayerData.ray = (false, null);	
		}
	}
	
}
