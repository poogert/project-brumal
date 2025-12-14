using Godot;
using System;

public partial class TestLevelScript : Node3D
{
	[Export] AudioStreamPlayer3D sound;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sound.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
