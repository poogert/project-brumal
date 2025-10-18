using Godot;
using System;

public partial class Interactray : RayCast3D
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

			Node nodeobject = GetCollider() as Node3D;
			
			if (nodeobject.GetParent() is Interactable interactable)
			{
				interactable.Touching();
				if (Input.IsActionJustPressed("interact"))
				{
					interactable.Interact();
				}

			}
			
		}

	}
}
