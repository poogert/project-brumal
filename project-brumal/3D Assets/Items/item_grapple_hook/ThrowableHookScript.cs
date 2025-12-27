using Godot;
using System;

public partial class ThrowableHookScript : RigidBody3D
{
	
	public override void _Ready()
	{
		this.BodyEntered += HitPoint;
		GD.Print("ready");
	}

	void HitPoint(Node body)
	{	
		if (body is CharacterBody3D) return;

		GD.Print("hit");

		SetDeferred(PropertyName.Freeze, true);
		LinearVelocity = Vector3.Zero;
		AngularVelocity = Vector3.Zero;
	}
}
