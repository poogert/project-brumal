/*
using Godot;

public partial class PlayerStateController : Node
{
	public enum MovementState
	{
		idle,
		running,
		crouching,
		crawling,
		walking,
		jumping
	}

	public enum Items
	{
		lamp,
		pickaxe,
		flare,
		ghook,
		map,
		empty
	}

	public MovementState CurrentState { get; private set; } = MovementState.idle;
	public Items CurrentItem { get; private set; } = Items.empty;

	public void SetState(MovementState newState)
	{
		if (CanTransitionTo(newState))
		{
			CurrentState = newState;
			// Optional: Emit signal for listeners
			EmitSignal("StateChanged", (int)CurrentState);
		}
	}

	public void SetItem(Items newItem)
	{
		CurrentItem = newItem;
		// Optional: Emit signal
		EmitSignal("ItemChanged", (int)CurrentItem);
	}

	private bool CanTransitionTo(MovementState newState)
	{
		// Example: Prevent sprinting while crouching
		if (newState == MovementState.running && CurrentState == MovementState.crouching) return false;
		return true;
	}

	// Signals for global notifications
	[Signal] public delegate void StateChangedEventHandler(int state);
	[Signal] public delegate void ItemChangedEventHandler(int item);
}
*/
