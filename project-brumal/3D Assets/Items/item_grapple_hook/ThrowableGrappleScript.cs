using Godot;

public partial class ThrowableGrappleScript : Node
{
	[Export] PackedScene throwable;

	public override void _Ready() {}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustReleased("leftclick"))
		{
			
			Node hook = throwable.Instantiate(); 
			Node3D hook3D = hook as Node3D;

			// spawn in world
			GetTree().Root.AddChild(hook3D);

			// variable shortening
			Vector3 spawn = PlayerData.player_position;
			Vector3 direction = PlayerData.look_direction.Normalized();;
			
			// height and spacing from player offset
			spawn.Y += 0.5f; 
			spawn += direction * 1f; 

			// spawn position
			hook3D.GlobalPosition = spawn; 
			hook3D.LookAt(spawn + direction, Vector3.Up);
			
			// the throwable item currently has rigidbody as child
			RigidBody3D rigidBody3D = hook3D.GetNode<RigidBody3D>("RigidBody3D");
			rigidBody3D.ApplyImpulse(direction * 5.0f);

		}

	}

}