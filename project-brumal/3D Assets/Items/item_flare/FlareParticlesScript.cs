using Godot;
using System;

public partial class FlareParticlesScript : Node3D
{
	[Export] GpuParticles3D flames;
	[Export] GpuParticles3D smoke;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
