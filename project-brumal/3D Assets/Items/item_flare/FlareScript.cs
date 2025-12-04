using Godot;
using System;
/*
public partial class FlareScript : Node3D
{

	[Export] PackedScene throwable;

	private CharacterBody3D player;
    private Node3D camera;
	
	public int flareCount = 5;
	public override void _Ready()
    {
        player = GetNode<CharacterBody3D>("World/Player");
		camera = GetNode<Node3D>("World/Player/CharacterBody3D/head_pivot/head_adjustments/head_camera");
    }

	public override void _Process(double delta)
    {
		if ( Input.IsActionJustPressed("leftclick") && flareCount > 0 )
		{
			Node flare = throwable.Instantiate(); 
			GetTree().Root.AddChild(flare);

			// need a reference to the player character within the world scene
			Vector3 spawn = player.GlobalTransform.Origin; 

			// once player reference made, grab camera
			Vector3 throwDirection = camera.GlobalTransform.Basis.Z.Normalized() * -1.0f;

			// the throwable item currently has rigidbody as child
			RigidBody3D rigidBody3D = flare.GetNode<RigidBody3D>("RigidBody3D");
			rigidBody3D.ApplyImpulse(throwDirection * 25.0f);

			flareCount -= 1;
		}
    }
}
*/