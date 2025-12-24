using Godot;
using System;
using System.Collections;
using System.Threading.Tasks;

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
	private const float ITEM_OUT_OFFSET = -1.0f;
	private float _itemRestY; // rest
	private const float RECOVERY_DELAY_MS = 2000f; // 2 seconds delay


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
	[Export] AudioStreamPlayer3D sound; // voice
	[Export] Timer staminaTimer; // how fast stamina is updated

	// item scenes
	[Export] PackedScene lamp;
	[Export] PackedScene pickaxe;
	[Export] PackedScene flare;
	[Export] PackedScene ghook;

	// bobbing point
	private float bobTime = 0f;

	// rotation lerping
	float targetRotationZ = 0;
	float currentRotationZ = 0;
	
	bool leaningcheck = false;

	// crawl/crouch lerping
	bool finishedLowering = false;
	bool HeightAdjustmentsRunning = false;

	bool itemsetting = false;

	float targetFOV = 70; 	// fov lerping


	// Item States
	public enum Items
	{
		lamp, // 1
		pickaxe, // 2
		flare, // 3
		ghook, // 4
		map, // 5
		empty,
		all,
		none
	}

	public Items currentItem = Items.empty;

	private async Task SetItem(Items selectedItem, PackedScene item)
	{
		if (!HasItem(selectedItem)) { 
			sound.Play();
			return;
		}

		if (itemsetting) return;
		itemsetting = true;

		try
		{
			if (currentItem != Items.empty)
			{
				await ItemSwitchTween(true); // out
				RemoveItem();
			}

			if (selectedItem == currentItem)
			{
				currentItem = Items.empty;
				return;
			}
			
			currentItem = selectedItem;

			var itemInstance = item.Instantiate();
			if (itemInstance is not Node3D itemReference) return;
			
			handeffects.AddChild(itemReference);
			await ItemSwitchTween(false);
		}
		finally
		{
			itemsetting = false;
		}
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
	
	// 1) lamp | 2) pickaxe | 3) flare | 4) grapple hook | 5) map
	public static bool[] availableItems = {false, false, false, false, false};
	public static void equipItem(Items item, bool give = false) {
		switch (item) 
		{
			case Items.lamp:
				availableItems[0] = give;
				break;
			case Items.pickaxe:
				availableItems[1] = give;
				break;
			case Items.flare:
				availableItems[2] = give;
				break;
			case Items.ghook:
				availableItems[3] = give;
				break;
			case Items.map:
				availableItems[4] = give;
				break;
			case Items.all:
				for (int i = 0; i < availableItems.Length; i++) { availableItems[i] = give; }
				break;
			default:
				return;
		}
	}
	void PrintItems() {

		GD.Print(
			"1) lamp : " + (availableItems[0] ? "available" : "NOT available") + "\n" + 
			"2) pickaxe : " + (availableItems[1] ? "available" : "NOT available") + "\n" + 
			"3) flare : " + (availableItems[2] ? "available" : "NOT available") + "\n" + 
			"4) grapple-hook : " + (availableItems[3] ? "available" : "NOT available") + "\n" + 
			"5) map : " + (availableItems[4] ? "available" : "NOT available") + "\n"

		);

	}
	public bool HasItem(Items item) {

		switch (item) 
		{
			case Items.lamp:
				return availableItems[0];
			case Items.pickaxe:
				return availableItems[1];
			case Items.flare:
				return availableItems[2];
			case Items.ghook:
				return availableItems[3];
			case Items.map:
				return availableItems[4];
			default:
				return false;
		}

	}

	
	// Movement States
	public enum MovementState
	{
		idle,
		running,
		crouching,
		crawling,
		walking
	}

	public MovementState currentState = MovementState.idle;

	private bool IsNeutralState() {
		// bool return if player is idle or walking.
		return currentState == MovementState.idle || currentState == MovementState.walking;
	}
	void SetState(MovementState state)
	{
		if (IsNeutralState()) currentState = state;
	}
	void SetDefaultState() { currentState = MovementState.idle; }

	private ulong lastStaminaDrainTime = 0; // timer for draining

	void SprintDrainStamina() {
		if (currentState == MovementState.running) {
			PlayerData.stamina -= 5;
			lastStaminaDrainTime = Time.GetTicksMsec();
		}
		
		if ( !HasStamina(5) ) oosSprint();
	}

	void oosSprint() {
		// OUT OF STAMINA so cancel sprinting
		SetDefaultState();
		SPEED = DEFAULT_SPEED;
		targetFOV = FOV_DEFAULT;
	
	}

	void JumpDrainStamina() { 
		PlayerData.stamina -= 15; 
		lastStaminaDrainTime = Time.GetTicksMsec();
	}
	
	bool HasStamina(int min) 
	{ 
		// min is the minimum required amount when using this function to check for stamina level
		return PlayerData.stamina >= min;
	}

	void RecoverStamina() {
		if (Time.GetTicksMsec() - lastStaminaDrainTime > RECOVERY_DELAY_MS) {

			if (IsNeutralState()) {
				if (PlayerData.stamina > 95 && PlayerData.stamina < 100) PlayerData.stamina += (100 - PlayerData.stamina);
				if (PlayerData.stamina < 100) PlayerData.stamina += 5;
			}

		}
	}

	void CalculateStamina() {
		SprintDrainStamina();
		RecoverStamina();
		GD.Print("Stamina Level -> " + PlayerData.stamina);
	}


	public override void _Ready()
	{
		base._Ready();
		Input.MouseMode = Input.MouseModeEnum.Captured;
		_itemRestY = handeffects.Transform.Origin.Y;
		staminaTimer.Timeout += CalculateStamina;		
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
		_HandleFOV();
		_HandleInput();
	}

	public override void _PhysicsProcess(double delta)
	{
		_HandleMovement(delta);
		_HandleLeaning(delta);
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

	private async Task _HandleInput()
	{
		// close window
		if (Input.IsActionJustPressed("escape")) GetTree().Quit();

		// press o, this is meant to execute a function for testing
		if (Input.IsActionJustPressed("debug print"))
		{
			PrintItems();
		}

		// items
		if ( Input.IsActionJustPressed("itemone") ) await SetItem(Items.lamp, lamp);
		if ( Input.IsActionJustPressed("itemtwo") ) await SetItem(Items.pickaxe, pickaxe);
		if ( Input.IsActionJustPressed("itemthree") ) await SetItem(Items.flare, flare);
		if ( Input.IsActionJustPressed("itemfour") ) await SetItem(Items.ghook, ghook);

		// crouching
		if (Input.IsActionJustPressed("crouch"))
		{

			if (IsNeutralState())
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
			if (IsNeutralState())
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
		if (Input.IsActionPressed("sprint") && IsNeutralState() && HasStamina(5))
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
		if (Input.IsActionJustPressed("jump") && IsOnFloor() && HasStamina(15))
		{
			Velocity = new Vector3(Velocity.X, JUMP_VELOCITY, Velocity.Z);
			JumpDrainStamina();
		}


	}

	private void _HandleMovement(double delta)
	{
		Vector3 velocity = Velocity;

		if (!IsOnFloor()) // if not on floor add gravity to player.
		{
			velocity += GetGravity() * (float)delta;
			PlayerData.IsInAir = true;
		} 
		else 
		{
			PlayerData.IsInAir = false;
		}

		// Get the input direction and handle the movement/deceleration.
		Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward");
		Vector3 direction = (head.GlobalTransform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		if (direction != Vector3.Zero)
		{
			if (currentState == MovementState.idle) SetState(MovementState.walking);
			velocity.X = direction.X * SPEED;
			velocity.Z = direction.Z * SPEED;
		}
		else
		{
			if (currentState == MovementState.walking) SetDefaultState();
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

		PlayerData.camera_position = camera.GlobalPosition; 
		// direction looking
		PlayerData.look_direction = camera.GlobalTransform.Basis.Z * -1.0f;

		// looking in a x,y axis
		PlayerData.rot_horizontal = Player.Rotation.Y;
		PlayerData.rot_vertical = camera.Rotation.X;
		
		string state = currentState.ToString();
		string item = currentItem.ToString();

		PlayerData.CurrentState = state;
		PlayerData.CurrentItem = item;
		PlayerData.hotbarinventory = availableItems;
	}

	private async Task LerpHeadHeight(float offset)
	{
		if (HeightAdjustmentsRunning) return;

		HeightAdjustmentsRunning = true;

		var transform = head.Transform;

		float startY = transform.Origin.Y;
		float targetY = startY + offset;

		float duration = 0.15f;
		float elapsed = 0f;

		while (elapsed < duration)
		{
			if (finishedLowering) break;

			elapsed += (float)GetProcessDeltaTime();
			float t = elapsed / duration;

			transform.Origin.Y = Mathf.Lerp(startY, targetY, t);
			head.Transform = transform;

			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		}

		transform.Origin.Y = targetY;
		head.Transform = transform;

		HeightAdjustmentsRunning = false;
	}

	private async Task ResetHeadHeight()
	{
		if (HeightAdjustmentsRunning) return;

		HeightAdjustmentsRunning = true;

		var transform = head.Transform;
		float startY = transform.Origin.Y;
		float targetY = 0f;

		float duration = 0.12f;
		float elapsed = 0f;

		while (elapsed < duration)
		{
			if (!finishedLowering) break;

			elapsed += (float)GetProcessDeltaTime();
			float t = elapsed / duration;

			transform.Origin.Y = Mathf.Lerp(startY, targetY, t);
			head.Transform = transform;

			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		}

		transform.Origin.Y = 0f;
		head.Transform = transform;

		HeightAdjustmentsRunning = false;
	}

	private async Task ItemSwitchTween(bool moveOut)
	{
		var transform = handeffects.Transform;

		float startY = transform.Origin.Y;
		float targetY = moveOut ? (_itemRestY + ITEM_OUT_OFFSET) : _itemRestY;

		float duration = 0.15f;
		float elapsed = 0f;

		while (elapsed < duration)
		{
			elapsed += (float)GetProcessDeltaTime();
			float t = elapsed / duration;

			transform.Origin.Y = Mathf.Lerp(startY, targetY, t);
			handeffects.Transform = transform;

			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		}

		transform.Origin.Y = targetY;
		handeffects.Transform = transform;
	}

}
