using Godot;
using System;

public partial class FlareScript : Node3D
{

	[Export] PackedScene throwable;

	private Node player;
	Node3D playerNode3D;
	CharacterBody3D playerBody;
	private Node3D camera;

	private Vector3 playerposition;

	public override void _Ready() {}

	public override void _Process(double delta)
	{
		if ( Input.IsActionJustPressed("leftclick") && PlayerData.flare_count > 0 )
		{

			Node flare = throwable.Instantiate(); 
			Node3D flare3D = flare as Node3D;

			// spawn in world
			GetTree().Root.AddChild(flare3D);

			// variable shortening
			Vector3 spawn = PlayerData.player_position;
			Vector3 direction = PlayerData.look_direction.Normalized();;
			
			// height and spacing from player offset
			spawn.Y += 0.5f; 
			spawn += direction * .7f; 

			// spawn position
			flare3D.GlobalPosition = spawn; 
			flare3D.LookAt(spawn + direction, Vector3.Up);
			
			// the throwable item currently has rigidbody as child
			RigidBody3D rigidBody3D = flare3D.GetNode<RigidBody3D>("RigidBody3D");
			rigidBody3D.ApplyImpulse(direction * 16.0f);

			PlayerData.flare_count -= 1;
			GD.Print("flares : " + PlayerData.flare_count);

		}
	}
}
