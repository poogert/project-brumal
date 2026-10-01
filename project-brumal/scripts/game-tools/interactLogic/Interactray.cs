using Godot;
using System;

public partial class Interactray : RayCast3D
{


	public override void _Ready()
	{

	}

	public override void _Process(double delta)
	{

		if ( !IsColliding() ) return;
		if (!Input.IsActionJustPressed("interact")) return;


		Node node_target = GetCollider() as Node;

		Interactable cs_script = findInteractable(node_target);
		
		

		if (cs_script != null)
		{
			cs_script.Interact();
			return;
		}

		Node gd_script = findGDInteractable(node_target);
		
		if (gd_script != null)
		{
			GD.Print("GDSCRIPT FOUND");
			gd_script.Call("interact");
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

	private Node findGDInteractable(Node target) 
	{
		if (target == null) return null;

		if (target.HasMethod("interact")) return target;

		return findGDInteractable(target.GetParent());
	}

}
