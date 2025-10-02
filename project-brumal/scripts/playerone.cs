using Godot;
using System;
using System.Net.Http;
using System.Runtime.CompilerServices;

public partial class playerone : CharacterBody3D
{
	public const float Speed = 3.0f;
	public const float JumpVelocity = 4.5f;
	public const float sensitivity = 0.003f;


	Node3D head; // head reference
	Camera3D camera; // camera reference

	public override void _Ready()
	{

		base._Ready();
		Input.MouseMode = Input.MouseModeEnum.Captured;

		head = GetNode<Node3D>("head_pivot"); // initialize head node
		camera = GetNode<Camera3D>("head_pivot/head_camera"); // initialize camera node
		
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		base._UnhandledInput(@event);

		if (@event is InputEventMouseMotion mouseMotion)
		{

			head.RotateY(-mouseMotion.Relative.X * sensitivity);
			camera.RotateX(-mouseMotion.Relative.Y * sensitivity);

			var camerasRotationValue = camera.Rotation; // get camera rotation
			/* explaination in pseudocode:
				Mathf.Clamp(
					value to limit,
					minimum value (-90) which is straight down,
					maximum value (90) straight up
				);
			*/
			camerasRotationValue.X = Mathf.Clamp( // set rotation x up/down
				camerasRotationValue.X, // get value of x
				-Mathf.DegToRad(80), // min value (radians not degrees)
				Mathf.DegToRad(80) // max value
			);

			camera.Rotation = camerasRotationValue; // now value is locked.
			
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

		Velocity = velocity;
		MoveAndSlide();


	}
}
