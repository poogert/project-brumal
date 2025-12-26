using Godot;
using System;

public partial class BorisLookAt : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
		Vector3 PlayerPos = PlayerData.camera_position;
		this.GlobalPosition = PlayerPos;
		
		//this.transform = Player.Position

	}
}
