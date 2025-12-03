using Godot;
using Microsoft.VisualBasic;
using System;
using System.Collections;
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

	public enum Items
	{
		flashlight, // 1
		pickaxe, // 2
		flare, // 3
		ghook, // 4
		map, // 5
		empty

	}

	// default enum states
	public MovementState currentState = MovementState.idle; // default state
	public Items currentItem = Items.flashlight;
	
	bool leaningcheck = false;

	// exports
	[Export] public float Speed = 3.0f;
	[Export] public float JumpVelocity = 4.5f;
	[Export] public float sensitivity = 0.003f;
	[Export] CollisionShape3D StandCol; // stand collision root reference
	[Export] CollisionShape3D CrouchCol; // crouch collision root reference
	[Export] Node3D Player; // Scene root reference
	[Export] Node3D head; // head reference
	[Export] Camera3D camera; // camera reference
	[Export] Node3D headeffects;

	// bobbing point
	private float bobTime = 0f;

	// rotation lerping
	float targetRotationZ = 0;
	float currentRotationZ = 0;
	const float rotationAmount = 35;

	// crawl/crouch lerping
	bool finishedLowering = false;
	bool HeightAdjustmentsRunning = false;

	// fov lerping
	float targetFOV = 70;

	void SetItem(Items selecteditem)
	{

	}
	
	void SetState(MovementState state)
	{
		if (currentState == MovementState.idle)
		{
			currentState = state;
		}

		
	}

	void SetDefaultState()
	{
		currentState = MovementState.idle;
	}

	public override void _Ready()
	{
		base._Ready();

		Input.MouseMode = Input.MouseModeEnum.Captured;
		
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		base._UnhandledInput(@event);

		if (@event is InputEventMouseMotion mouseMotion)
		{
			
			camera.RotateX(-mouseMotion.Relative.Y * sensitivity);
			Player.RotateY(-mouseMotion.Relative.X * sensitivity);
			
			// Clamps head_Pivot(head) X so it can only rotate 180 degrees in total
			camera.RotationDegrees = new Vector3(Mathf.Clamp(camera.RotationDegrees.X, -90, 90), camera.RotationDegrees.Y, camera.RotationDegrees.Z);
			
		}


	}

	public override void _Process(double delta) { base._Process(delta); }

	public override void _PhysicsProcess(double delta)
	{
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

		// takes the characterbody3D's velocity
		Vector3 velocity = this.Velocity;

		if (targetFOV != camera.Fov) // camera lerp
		{

			camera.Fov = Mathf.Lerp(camera.Fov, targetFOV, .1f);

		}

		// if not on floor add gravity to player.
		if (!IsOnFloor())
		{

			velocity += GetGravity() * (float)delta;

		}


		// press p, this is meant for printing stats
		if (Input.IsActionJustPressed("debug print"))
		{
			GD.Print("--- Debugging Statistics ---\n");



			GD.Print("\n");
			GD.Print("----------------------------\n");
		}

		// press o, this is meant to execute a function for testing
		if (Input.IsActionJustPressed("debug print"))
		{

		}


		// Action -> crouching ---------------------------------------------------------------
		if (Input.IsActionJustPressed("crouch"))
		{

			if (currentState == MovementState.idle)
			{

				finishedLowering = false;
				SetState(MovementState.crouching);

				StandCol.Disabled = true;
				Speed = 2f;

				LerpHeadHeight(-0.6f);

			}
			else
			{

				finishedLowering = true;
				SetDefaultState();

				StandCol.Disabled = false;
				Speed = 3f;

				ResetHeadHeight();

			}

		}
		// ------------------------------------------------------------------------------------


		// Action -> crawling ---------------------------------------------------------------

		if (Input.IsActionJustPressed("crawl"))
		{
			if (currentState == MovementState.idle)
			{

				finishedLowering = false;
				SetState(MovementState.crawling);

				StandCol.Disabled = true;
				Speed = 1f;

				LerpHeadHeight(-1.2f);

			}
			else
			{
				finishedLowering = true;

				SetDefaultState();
				StandCol.Disabled = false;
				Speed = 3f;

				ResetHeadHeight();
			}

		}

		// ------------------------------------------------------------------------------------

		// Action -> sprinting ---------------------------------------------------------------

		if (Input.IsActionPressed("sprint") && currentState == MovementState.idle)
		{
			//float CurrentFOV = camera.Fov;
			SetState(MovementState.running);
			Speed = 6f;
			targetFOV = 85f;

		}
		if (Input.IsActionJustReleased("sprint") && currentState == MovementState.running)
		{   
			//float CurrentFOV = camera.Fov;
			SetDefaultState();
			Speed = 3f;
			targetFOV = 70f;
		}

		// ------------------------------------------------------------------------------------


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


		// Action -> jumping ------------------------------------------------------------------
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
			//GD.Print("IsjumpingWorking");

		}

		// ------------------------------------------------------------------------------------


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


		// speed physics
		Velocity = velocity;
		MoveAndSlide();

		// head bobbing ------------------------------------------------------
		float actualSpeed = new Vector2(velocity.X, velocity.Z).Length();

		if (actualSpeed > 0.01f && IsOnFloor())
		{
			bobTime += (float)delta * actualSpeed * 5f;  // frequency multiplier
			float bob = Mathf.Sin(bobTime) * 0.06f; // amplitude scales with speed

			var het = headeffects.Transform;
			het.Origin.Y = bob;
			headeffects.Transform = het;
		}
		else
		{
			bobTime = 0f; // reset phase when standing still
		}
		// ------------------------------------------------------------------------------------
		GD.Print("current state:" + currentState);
	}

	private async void LerpHeadHeight(float offset)
	{
		var ht = head.Transform; // set head tranform variable

		/*
		when this function is ran. we want to save the starting y value
		otherwise y will always be updated and lerp will never reach the intended target
		causing lerp to move you down infinitely
		*/
		float start = ht.Origin.Y;
		float target = start + offset;

		/* 
		lerp will infinitley go to target, once we get close enough 
		we snap and we check using while loop
		*/
		while (Mathf.Abs(ht.Origin.Y - target) > 0.01f)
		{
			HeightAdjustmentsRunning = true;
			if (finishedLowering)
			{
				HeightAdjustmentsRunning = false;
				return;
			}

			// lerp Y to target
			ht.Origin.Y = Mathf.Lerp(ht.Origin.Y, target, 7f * (float)GetProcessDeltaTime());

			// move entire head
			head.Transform = ht;

			// without this the while loop instantly processes in the span of a frame
			// if this runs, the while loop does one iteration per frame
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

			// adjust ht for next iteration
			ht = head.Transform;
		}
		// edit y, then snap
		ht.Origin.Y = target;
		head.Transform = ht;
		HeightAdjustmentsRunning = false;

	}

	private async void ResetHeadHeight()
	{
		var ht = head.Transform;


		while (Mathf.Abs(ht.Origin.Y) > 0.01f)
		{
			HeightAdjustmentsRunning = true;
			if (!finishedLowering)
			{
				ht.Origin.Y = 0;
				head.Transform = ht;
				HeightAdjustmentsRunning = false;
				return;
			}

			ht.Origin.Y = Mathf.Lerp(ht.Origin.Y, 0f, 10f * (float)GetProcessDeltaTime());
			head.Transform = ht;

			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			ht = head.Transform;
		}

		ht.Origin.Y = 0;
		head.Transform = ht;
		HeightAdjustmentsRunning = false;
	}



}
