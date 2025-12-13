using Godot;

public partial class playerone : CharacterBody3D
{
	private const float DEFAULT_SPEED = 3.0f;
	private const float CROUCH_SPEED = 2.0f;
	private const float CRAWL_SPEED = 1.0f;
	private const float SPRINT_SPEED = 6.0f;
	private const float HEAD_CROUCH_OFFSET = -0.6f;
	private const float HEAD_CRAWL_OFFSET = -1.2f;
	private const float FOV_DEFAULT = 70f;
	private const float FOV_SPRINT = 85f;
	private const float ROTATION_AMOUNT = 35f;


	public enum MovementState
	{
		idle,
		running,
		crouching,
		crawling

	}

	public enum Items
	{
		lamp, // 1
		pickaxe, // 2
		flare, // 3
		ghook, // 4
		map, // 5
		empty

	}

	// default enum states
	public MovementState currentState = MovementState.idle; // default state
	
	public Items currentItem = Items.empty;
	


	// exports
	[Export] public float SPEED = 3.0f;
	[Export] public float JUMP_VELOCITY = 4.5f;
	[Export] public float SENSITIVITY = 0.003f;
	[Export] public CollisionShape3D StandCol; // stand collision root reference
	[Export] public CollisionShape3D CrouchCol; // crouch collision root reference
	[Export] public Node3D Player; // Scene root reference
	[Export] public Node3D head; // head reference
	[Export] public Camera3D camera; // camera reference
	[Export] public Node3D headeffects; // HEAD
	[Export] public Node3D handeffects; // HAND


	// item scenes
	[Export] PackedScene lamp;
	[Export] PackedScene pickaxe;
	[Export] PackedScene flare;

	// bobbing point
	private float bobTime = 0f;

	// rotation lerping
	float targetRotationZ = 0;
	float currentRotationZ = 0;
	
	bool leaningcheck = false;

	// crawl/crouch lerping
	bool finishedLowering = false;
	bool HeightAdjustmentsRunning = false;


	float targetFOV = 70; 	// fov lerping

	// ENUM FUNCTIONS
	void SetItem(Items selecteditem, PackedScene item)
	{

		if (selecteditem == currentItem)
		{
			currentItem = Items.empty;
			RemoveItem();

			return;
		} 

		RemoveItem();
		currentItem = selecteditem;
		
		Node itemInstance = item.Instantiate();
		Node3D itemReference = itemInstance as Node3D; 
		
		if (itemReference == null) 
		{ 
			GD.Print("item ref null boah");
			return;
		}

		handeffects.AddChild(itemReference);
	}

	void RemoveItem()
	{
		
		if (handeffects.GetChildCount() > 0)
		{

			Node child = handeffects.GetChild(0);
			handeffects.RemoveChild(child);

			child.QueueFree();

		}
		else
		{
			return;
		}

	}

	void SetState(MovementState state)
	{
		if (currentState == MovementState.idle) currentState = state;
	}

	void SetDefaultState() { currentState = MovementState.idle; }


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
			
			camera.RotateX(-mouseMotion.Relative.Y * SENSITIVITY);
			Player.RotateY(-mouseMotion.Relative.X * SENSITIVITY);
			
			// Clamps head_Pivot(head) X so it can only rotate 180 degrees in total
			camera.RotationDegrees = new Vector3(Mathf.Clamp(camera.RotationDegrees.X, -90, 90), camera.RotationDegrees.Y, camera.RotationDegrees.Z);
			
		}
	}

	public override void _Process(double delta) 
	{ 
		base._Process(delta); 
	}

	public override void _PhysicsProcess(double delta)
	{
		_HandleLeaning(delta);
		_HandleFOV();
		_HandleInput();
		_HandleMovement(delta);
		_HandleBobbing(delta);
		updatePlayerInfo();
	}


	private void _HandleLeaning(double delta)
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
	}

	private void _HandleFOV()
	{
		if (targetFOV != camera.Fov) // camera lerp
		{
			camera.Fov = Mathf.Lerp(camera.Fov, targetFOV, .1f);
		}
	}

	private void _HandleInput()
	{
		// close window
		if (Input.IsActionJustPressed("escape")) GetTree().Quit();

		// press o, this is meant to execute a function for testing
		if (Input.IsActionJustPressed("debug print")) {}

		// items
		if ( Input.IsActionJustPressed("itemone") ) SetItem(Items.lamp, lamp);
		if ( Input.IsActionJustPressed("itemtwo") ) SetItem(Items.pickaxe, pickaxe);
		if ( Input.IsActionJustPressed("itemthree") ) SetItem(Items.flare, flare);

		// crouching
		if (Input.IsActionJustPressed("crouch"))
		{

			if (currentState == MovementState.idle)
			{
				finishedLowering = false;
				SetState(MovementState.crouching);
				StandCol.Disabled = true;
				SPEED = CROUCH_SPEED;

				LerpHeadHeight(HEAD_CROUCH_OFFSET);
			}
			else
			{
				finishedLowering = true;
				SetDefaultState();
				StandCol.Disabled = false;
				SPEED = DEFAULT_SPEED;

				ResetHeadHeight();
			}

		}


		// Action -> crawling ---------------------------------------------------------------
		if (Input.IsActionJustPressed("crawl"))
		{
			if (currentState == MovementState.idle)
			{
				finishedLowering = false;
				SetState(MovementState.crawling);
				StandCol.Disabled = true;
				SPEED = CRAWL_SPEED;

				LerpHeadHeight(HEAD_CRAWL_OFFSET);
			}
			else
			{
				finishedLowering = true;
				SetDefaultState();
				StandCol.Disabled = false;
				SPEED = DEFAULT_SPEED;

				ResetHeadHeight();
			}

		}


		// Action -> sprinting ---------------------------------------------------------------
		if (Input.IsActionPressed("sprint") && currentState == MovementState.idle)
		{
			//float CurrentFOV = camera.Fov;
			SetState(MovementState.running);
			SPEED = SPRINT_SPEED;
			targetFOV = FOV_SPRINT;

		}
		
		if (Input.IsActionJustReleased("sprint") && currentState == MovementState.running)
		{   
			//float CurrentFOV = camera.Fov;
			SetDefaultState();
			SPEED = DEFAULT_SPEED;
			targetFOV = FOV_DEFAULT;
		}


		// Action -> leaning ---------------------------------------------------------------

		if (Input.IsActionPressed("LeanLeft"))
		{
			leaningcheck = true;
			targetRotationZ = ROTATION_AMOUNT;
		}
		if (Input.IsActionJustReleased("LeanLeft"))
		{

			leaningcheck = false;
			targetRotationZ = 0f;
		}
		if (Input.IsActionPressed("LeanRight"))
		{
			leaningcheck = true;
			targetRotationZ = -ROTATION_AMOUNT;
		}
		if (Input.IsActionJustReleased("LeanRight"))
		{
			leaningcheck = false;
			targetRotationZ = 0f;
		}


		// Action -> jumping ------------------------------------------------------------------
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			Velocity = new Vector3(Velocity.X, JUMP_VELOCITY, Velocity.Z);
		}

	}

	private void _HandleMovement(double delta)
	{
		Vector3 velocity = Velocity;

		if (!IsOnFloor()) // if not on floor add gravity to player.
		{
			velocity += GetGravity() * (float)delta;
		}

		// Get the input direction and handle the movement/deceleration.
		Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward");
		Vector3 direction = (head.GlobalTransform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * SPEED;
			velocity.Z = direction.Z * SPEED;
		}
		else
		{
			velocity.X = 0;
			velocity.Z = 0;
		}

		// speed physics
		Velocity = velocity;
		MoveAndSlide();
	}

	private void _HandleBobbing(double delta)
	{
		Vector3 velocity = Velocity;

		// head/hand bobbing ------------------------------------------------------
		float actualSpeed = new Vector2(velocity.X, velocity.Z).Length();

		if (actualSpeed > 0.01f && IsOnFloor())
		{
			bobTime += (float)delta * actualSpeed * 5f;  // frequency multiplier
			float bob = Mathf.Sin(bobTime) * 0.06f; // amplitude scales with speed

			
			const float hand_offset = Mathf.Pi/ 4f; 

			float bobHand = Mathf.Sin(bobTime + hand_offset) * 0.06f;

			// head moving
			var het = headeffects.Transform;
			het.Origin.Y = bob;
			headeffects.Transform = het;

			// hand moving
			var hand = handeffects.Transform;
			hand.Origin.Y = bobHand;
			handeffects.Transform = hand;
		
		}
		else
		{
			bobTime = 0f; // reset phase when standing still
		}

	}

	private void updatePlayerInfo()
	{
		// position in map
		PlayerData.player_position = GlobalPosition; 

		// direction looking
		PlayerData.look_direction = camera.GlobalTransform.Basis.Z * -1.0f;

		// looking in a x,y axis
		PlayerData.rot_horizontal = Player.Rotation.Y;
		PlayerData.rot_vertical = camera.Rotation.X;

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

	private async void HandEffectsLerp()
	{
		var he = headeffects.Transform;

		float originalY = he.Origin.Y; // save og point

		// -1 below normal position
		float target = originalY - 5;
		he.Origin.Y = target;

		handeffects.Transform = he; // starting spot


		while (Mathf.Abs(he.Origin.Y - target) > 0.01f)
		{

			he.Origin.Y = Mathf.Lerp(he.Origin.Y, target, 7f * (float)GetProcessDeltaTime());
			handeffects.Transform = he;

			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			he = handeffects.Transform;
		}

		// snap back
		he.Origin.Y = target;
		handeffects.Transform = he;
	}

}
