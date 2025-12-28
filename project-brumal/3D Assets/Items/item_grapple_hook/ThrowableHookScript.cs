using Godot;
using System;
using System.IO;

public partial class ThrowableHookScript : RigidBody3D
{
	[Export] PackedScene rope;
	[Export] Node3D endRef;
	
	bool hit = false;

	public override void _Ready()
	{
		BodyEntered += HitPoint;
	}

	void HitPoint(Node body)
	{   
		if (body is CharacterBody3D || hit) return;

		float distance = GlobalPosition.DistanceTo(PlayerData.player_position);
		
		//GD.Print("Distance to player: "  + distance);

		var ropeInstance = rope.Instantiate<Path3D>();
		
		GetTree().Root.AddChild(ropeInstance);

		ropeInstance.GlobalPosition = endRef.GlobalPosition;

		ropeInstance.Curve.ClearPoints();
		ropeInstance.Curve.AddPoint(Vector3.Zero);

		//float offset = 1.5f;
		//Vector3 spawnGlobal = PlayerData.player_position + (PlayerData.look_direction.Normalized() * offset);
		
		Vector3 endLocal = ropeInstance.ToLocal(PlayerData.player_position);
		ropeInstance.Curve.AddPoint(endLocal);

		ropeInstance.Call("generate");

		SetDeferred(PropertyName.Freeze, true);
		LinearVelocity = Vector3.Zero;
		AngularVelocity = Vector3.Zero;
		hit = true;
	}
}
