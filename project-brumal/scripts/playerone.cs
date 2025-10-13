using Godot;
using System;
using System.Net.Http;
using System.Runtime.CompilerServices;

public partial class playerone : CharacterBody3D
{

	public enum MovementState
	{
		idle,
		//walk,
		running,
		crouching,
		crawling,
		jumping,
		leaning

	}


	[Export] public float Speed = 3.0f;
	[Export] public float JumpVelocity = 4.5f;
	[Export] public float sensitivity = 0.003f;

	public MovementState currentState = MovementState.idle; // default state

	CollisionShape3D StandCol; // stand collision root reference
	CollisionShape3D CrouchCol; // crouch collision root reference
	Node3D Player; // Scene root reference
	Node3D head; // head reference
	Camera3D camera; // camera reference



	void SetState(MovementState state)
	{
		if (
			(state == MovementState.crouching && currentState == MovementState.crawling) ||
			(state == MovementState.crawling && currentState == MovementState.crouching) ||
			(state == MovementState.running && currentState == MovementState.crouching) ||
			(state == MovementState.running && currentState == MovementState.crawling)
			)
		{
			// basically if you change state, and it collides with these things ^^ then nothing will happen.
			return;
		}

		currentState = state;
	}

	void SetDefaultState()
	{
		currentState = MovementState.idle;
	}

	public override void _Ready()
	{

		base._Ready();

		Input.MouseMode = Input.MouseModeEnum.Captured;

		StandCol = GetNode<CollisionShape3D>("StandingCollision");  // initialize Standing collision
		CrouchCol = GetNode<CollisionShape3D>("CrouchedCollision"); // initialize crouch collision

		Player = GetNode<Node3D>("."); // initialize player node
		head = GetNode<Node3D>("head_pivot"); // initialize head node
		camera = GetNode<Camera3D>("head_pivot/head_camera"); // initialize camera node

	}

	public override void _UnhandledInput(InputEvent @event)
	{
		base._UnhandledInput(@event);

		if (@event is InputEventMouseMotion mouseMotion)
		{

			camera.RotateX(-mouseMotion.Relative.Y * sensitivity);
			Player.RotateY(-mouseMotion.Relative.X * sensitivity);

			camera.RotationDegrees = new Vector3(Mathf.Clamp(camera.RotationDegrees.X, -89, 89),camera.RotationDegrees.Y,camera.RotationDegrees.Z);
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


		// Action -> crouch ---------------------------------------------------------------

		if (Input.IsActionJustPressed("crouch"))
		{
			SetState(MovementState.crouching);
			StandCol.Disabled = true;
			// basically makes player overall hitbox smaller
			Speed = Speed / 2.0f;
			// makes speed half 
			head.Transform = head.Transform.Translated(new Vector3(0, -0.3f, 0));
			// moves head pivot down thus moving camera down 
		}
		if (Input.IsActionJustReleased("crouch"))
		{
			SetDefaultState();
			StandCol.Disabled = false;
			// returns overall hitbox to normal
			Speed = Speed * 2.0f;
			// makes speed normal
			head.Transform = head.Transform.Translated(new Vector3(0, 0.3f, 0));
			// moves head pivot up again
		}
		// ------------------------------------------------------------------------------------


		// Action -> crawl ---------------------------------------------------------------

		if (Input.IsActionJustPressed("crawl"))
		{
			SetState(MovementState.crawling);
			StandCol.Disabled = true;
			Speed /= 3.0f;
			head.Transform = head.Transform.Translated(new Vector3(0, -0.5f, 0));
		}
		if (Input.IsActionJustReleased("crawl"))
		{
			SetDefaultState();
			StandCol.Disabled = false;
			Speed *= 3.0f;
			head.Transform = head.Transform.Translated(new Vector3(0, 0.8f, 0));
		}

		// ------------------------------------------------------------------------------------


		// Action -> leaning ---------------------------------------------------------------
		
		if (Input.IsActionJustPressed("LeanLeft"))
		{
			SetState(MovementState.leaning);
			head.RotateZ(-30f);
			// leans left
		}
		if (Input.IsActionJustReleased("LeanLeft"))
		{
			SetDefaultState();
			head.RotateZ(30f);
			// leans it back to normal
		}
		
		if(Input.IsActionJustPressed("LeanRight")){
			SetState(MovementState.leaning);
			head.RotateZ(30f);
			// leans right
		}
		if (Input.IsActionJustReleased("LeanRight"))
		{
			SetDefaultState();
			head.RotateZ(-30f);
			// leans it back to normal
		}

		// ------------------------------------------------------------------------------------


		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;

		}





		// close window
		if (Input.IsActionJustPressed("escape"))
		{
			//Input.MouseMode = MouseMode.MOUSE_MODE_VISIBLE;
			GetTree().Quit();

		}


		// Get the input direction and handle the movement/deceleration.
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
