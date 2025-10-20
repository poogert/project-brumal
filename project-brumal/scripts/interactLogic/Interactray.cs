using Godot;
using System;

public partial class Interactray : RayCast3D
{


	public override void _Ready()
	{

	}

	public override void _Process(double delta)
	{

		if (IsColliding() == true)
		{

			Node nodeobject = GetCollider() as Node;
			
			if (nodeobject.GetParent() is Interactable interactable)
			{
				
				if (Input.IsActionJustPressed("interact"))
				{
					interactable.Interact();
				}

			}
		}
	}

}
