using Godot;
using System;

public partial class LeveLoadScript : Area3D
{

	public PackedScene level;

	public override void _Ready()
	{
		BodyEntered += OnEntered; // signal to method connection

		var parentnode = GetParent(); // parent
		var parent = parentnode as SetLevelField; // accessible field from parent

		if (parent != null) level = parent.next_level; // set level
	}
	
	public void OnEntered(Node3D body)
	{
		if (body is CharacterBody3D) GetTree().ChangeSceneToFile(level.ResourcePath);	
	}


}
