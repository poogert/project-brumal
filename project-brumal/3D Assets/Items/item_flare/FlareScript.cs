using Godot;
using System;

public partial class FlareScript : Node3D
{

	[Export] PackedScene throwable;

	private Node player;
	Node3D playerNode3D;
	CharacterBody3D playerBody;
	private Node3D camera;

	
	
	public int flareCount = 200;
	public override void _Ready()
	{
	
	}

	public override void _Process(double delta)
	{
		if ( Input.IsActionJustPressed("leftclick") && flareCount > 0 )
		{


			player = GetTree().GetFirstNodeInGroup("Player");
			playerNode3D = player as Node3D;
			playerBody = playerNode3D as CharacterBody3D;

			camera = player.GetNode<Node3D>("CharacterBody3D/head_pivot");

			/* 
				left off here -> issues 
					player position static
					when throwing the flare it doesnt detect y axis only z/x 
			*/ 
			GD.Print("throwing!");

			GD.Print("player position : " + playerNode3D.GlobalPosition);

			Node flare = throwable.Instantiate(); 
			Node3D flare3D = flare as Node3D;

			GetTree().Root.AddChild(flare);

			Vector3 spawnPosition = playerNode3D.GlobalPosition;
			Vector3 throwDirection = camera.GlobalTransform.Basis.Z.Normalized() * -1.0f;

			spawnPosition.Y += 1.5f; 
			spawnPosition += throwDirection * 0.5f; 

			flare3D.GlobalPosition = spawnPosition; 

			// the throwable item currently has rigidbody as child
			RigidBody3D rigidBody3D = flare3D.GetNode<RigidBody3D>("RigidBody3D");
			rigidBody3D.ApplyImpulse(throwDirection * 16.0f);

			flareCount -= 1;
			GD.Print("flares : " + flareCount);

		}
	}
}
