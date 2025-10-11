using Godot;
using System;
using System.Net.Http;
using System.Runtime.CompilerServices;

public partial class playerone : CharacterBody3D
{
	[Export] public float Speed = 3.0f;
	[Export] public float JumpVelocity = 4.5f;
	[Export] public float sensitivity = 0.003f;
	
	Node3D Player; // Scene root reference
	Node3D head; // head reference
	Camera3D camera; // camera reference

	public override void _Ready()
	{

		base._Ready();
		Input.MouseMode = Input.MouseModeEnum.Captured;
		Player = GetNode<Node3D>("."); // initialize player node
		head = GetNode<Node3D>("head_pivot"); // initialize head node
		camera = GetNode<Camera3D>("head_pivot/head_camera"); // initialize camera node
		
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		base._UnhandledInput(@event);

		if (@event is InputEventMouseMotion mouseMotion)
		{

			head.RotateX(-mouseMotion.Relative.Y * sensitivity);
			Player.RotateY(-mouseMotion.Relative.X * sensitivity);
			
			head.RotationDegrees = new Vector3(Mathf.Clamp(head.RotationDegrees.X, -90, 90),head.RotationDegrees.Y,head.RotationDegrees.Z);
			// ^^ Clamps head_Pivot(head) X so it can only rotate 180 degrees in total
		}
   
   
	}



	public override void _PhysicsProcess(double delta)
	{

		// takes the characterbody3D's velocity
		Vector3 velocity = this.Velocity;
		
		// if not on floor add gravity to player.
		if (!IsOnFloor())
		{

			velocity += GetGravity() * (float)delta;

		}

		// if space pressed, add jump velocity
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{

			velocity.Y = JumpVelocity;

		}

		// close window
		if ( Input.IsActionJustPressed("escape") )
		{
			//Input.MouseMode = MouseMode.MOUSE_MODE_VISIBLE;
			GetTree().Quit();

		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward");
		Vector3 direction = (head.GlobalTransform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		if (direction != Vector3.Zero)
		{

			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;

		}
		else
		{

			velocity.X = 0;
			// Mathf.MoveToward(Velocity.X, 0, Speed); 
			// supposedly for slowing player down if no input
			// we dont want that cuz it simulates slipping/sliding instead of instant stop after no input.
			velocity.Z = 0;
			// Mathf.MoveToward(Velocity.Z, 0, Speed);

		}

		Velocity = velocity.Normalized() * Speed;
		MoveAndSlide();


	}
}
