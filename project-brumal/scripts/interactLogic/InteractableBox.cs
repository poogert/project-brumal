using Godot;
using System;

public partial class InteractableBox : Node3D, Interactable
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
			
	}

	public void Touching(){
		GD.Print("XRAY");	
	}

	public void Interact()
	{
		
		QueueFree();
		
	}
}
