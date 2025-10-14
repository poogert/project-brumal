using Godot;
using System;
using System.Net.Http;
using System.Runtime.CompilerServices;

public partial class playerone : CharacterBody3D
{

	public enum MovementState
	{
		idle,
		running,
		crouching,
		crawling

	}


	[Export] public float Speed = 3.0f;
	[Export] public float JumpVelocity = 4.5f;
	[Export] public float sensitivity = 0.003f;

	public MovementState currentState = MovementState.idle; // default state
	bool leaningcheck = false;

	CollisionShape3D StandCol; // stand collision root reference
	CollisionShape3D CrouchCol; // crouch collision root reference
	Node3D Player; // Scene root reference
	Node3D head; // head reference
	Camera3D camera; // camera reference


// rotation lerping
	float targetRotationZ = 0;
	float currentRotationZ = 0;
	const float rotationAmount = 35;

	// crouch crawl lerping, not yet implemented
	float targetHeight = 0;
	float currentHeight = 0;

	// 
	float targetFOV = 70;


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


	public override void _Process(double delta)
	{
		base._Process(delta);

	}



	public override void _PhysicsProcess(double delta)
	{
		//GD.Print(IsOnFloor());
		GD.Print(camera.Fov);
		//GD.Print(Velocity.Y);

		if (leaningcheck) // if leaning, lerp to target
		{
			// lerping
			currentRotationZ = Mathf.Lerp(currentRotationZ, targetRotationZ, 9f * (float)delta);

			// using basis to rotate ONLY Z, not x, y.
			Basis basis = Basis.FromEuler(new Vector3(0, 0, Mathf.DegToRad(currentRotationZ)));
			head.Transform = new Transform3D(basis, head.Transform.Origin);

		}
		else // if not leaning lerp to 0 degrees
		{

			currentRotationZ = Mathf.Lerp(currentRotationZ, 0, 9f * (float)delta);

			Basis basis = Basis.FromEuler(new Vector3(0, 0, Mathf.DegToRad(currentRotationZ)));
			head.Transform = new Transform3D(basis, head.Transform.Origin);

		}


		// NOT YET IMPLEMENTED!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		if (currentState == MovementState.crouching || currentState == MovementState.crawling)
		{
			// interp from regular height to crouch/crawl height
		}
		else
		{
			// interp back to norm	
		}



		// takes the characterbody3D's velocity
		Vector3 velocity = this.Velocity;


		// if not on floor add gravity to player.
		if (!IsOnFloor())
		{
			//velocity.Y -= .05f;
			velocity += GetGravity() * (float)delta;
			GD.Print("IsOnFloorGravityWorking");
		}


		// Action -> crouch ---------------------------------------------------------------

		if (Input.IsActionJustPressed("crouch"))
		{
			SetState(MovementState.crouching);
			StandCol.Disabled = true;
			// basically makes player overall hitbox smaller
			Speed = 2f;
			// makes speed half 
			head.Transform = head.Transform.Translated(new Vector3(0, -0.6f, 0));
			// moves head pivot down thus moving camera down 
		}
		if (Input.IsActionJustReleased("crouch"))
		{
			SetDefaultState();
			StandCol.Disabled = false;
			// returns overall hitbox to normal
			Speed = 3f;
			// makes speed normal
			head.Transform = head.Transform.Translated(new Vector3(0, 0.6f, 0));
			// moves head pivot up again
		}
		// ------------------------------------------------------------------------------------


		// Action -> crawl ---------------------------------------------------------------

		if (Input.IsActionJustPressed("crawl"))
		{
			SetState(MovementState.crawling);
			StandCol.Disabled = true;
			Speed = 1f;
			head.Transform = head.Transform.Translated(new Vector3(0, -1.2f, 0));
		}
		if (Input.IsActionJustReleased("crawl"))
		{
			SetDefaultState();
			StandCol.Disabled = false;
			Speed = 3f;
			head.Transform = head.Transform.Translated(new Vector3(0, 1.2f, 0));
		}

		// ------------------------------------------------------------------------------------

		// Action -> sprinting ---------------------------------------------------------------

		if (Input.IsActionPressed("sprint"))
		{	
			//float CurrentFOV = camera.Fov;
			SetState(MovementState.running);
			Speed = 6f;
			targetFOV = 85f;

		}
		if (Input.IsActionJustReleased("sprint"))
		{	//float CurrentFOV = camera.Fov;
			SetDefaultState();
			Speed = 3f;
			targetFOV = 70f;
		}

		// ------------------------------------------------------------------------------------
		
		if (targetFOV != camera.Fov){
			camera.Fov = Mathf.Lerp(camera.Fov,targetFOV, .1f);
		}

		// Action -> leaning ---------------------------------------------------------------

		if (Input.IsActionPressed("LeanLeft"))
		{
			//SetState(MovementState.leaning);
			leaningcheck = true;
			targetRotationZ = rotationAmount;
		}
		if (Input.IsActionJustReleased("LeanLeft"))
		{
			//SetDefaultState();
			leaningcheck = false;
			targetRotationZ = 0f;
		}

		if (Input.IsActionPressed("LeanRight"))
		{
			//SetState(MovementState.leaning);
			leaningcheck = true;
			targetRotationZ = -rotationAmount;
		}
		if (Input.IsActionJustReleased("LeanRight"))
		{
			//SetDefaultState();
			leaningcheck = false;
			targetRotationZ = 0f;
		}

		// ------------------------------------------------------------------------------------


		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
			//GD.Print("IsjumpingWorking");

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

		Velocity = velocity;
		// this had a normalize and *speed along with it but i removed it
		MoveAndSlide();


	}
}
