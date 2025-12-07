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
			Interactable interactable = findInteractable(nodeobject);

			if (interactable != null)
			{
				if (Input.IsActionJustPressed("interact")) interactable.Interact();
			}

		}
	}

	// recursion to find an interactable part of the the node
	// edge case to fix later : if the node adjacent to the scan has an interactable script
	// this will fail...
	private Interactable findInteractable(Node target) 
	{
		if (target == null) return null;

		if (target is Interactable interactable) return interactable;

		return findInteractable(target.GetParent());
	}

}
